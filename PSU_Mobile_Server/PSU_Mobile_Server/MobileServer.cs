using Common;
using PSU_Mobile_Server.Controllers;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PSU_Mobile_Server
{
	internal class MobileServer
	{
		private static long _requestCount = 0;

		private readonly HttpListener _listener;
		private readonly CancellationTokenSource _cancellationTokenSource;

		private MobileServer()
		{
			_listener = new HttpListener();
			_cancellationTokenSource = new CancellationTokenSource();
			ShutdownProcessor.Initialize(_cancellationTokenSource);
		}

		public static Lazy<MobileServer> Instance { get; } = new Lazy<MobileServer>(new MobileServer());

		public Task ServerProcessingTask { get; private set; }

		public void Start(int[] ports)
		{
			Console.WriteLine("Starting a server...");

			var addresses = Dns.GetHostAddresses(Dns.GetHostName()).Where(a => a.AddressFamily == AddressFamily.InterNetwork).ToList();
			addresses.Add(IPAddress.Loopback);
			foreach (var port in ports)
			{
				addresses.ForEach(ipAddress =>
					_listener.Prefixes.Add($"http://{ipAddress}:{port}/"));
			}

			_listener.Start();
			ServerProcessingTask = HandleIncomingConnections(_cancellationTokenSource.Token);
			Console.WriteLine("Server started");
		}

		public void StopServer()
		{
			Console.WriteLine("Stopping server...");
			_listener.Close();
			Auth.Instance.Value.Save();
			Console.WriteLine("Server stopped");
		}

		private async Task HandleIncomingConnections(CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				HttpListenerContext context;
				try
				{
					context = await _listener.GetContextAsync();
				}
				catch (Exception e)
				{
					//Костыль. Считаем, что ситуация нормальная и соответсвует команде выключения сервера
					//TODO: возможно, есть ещё какие-то ситуации, на которые нужно адекватно реагировать
					return;
				}

				var task = new Task(() => ProcessConnection(context, token), TaskCreationOptions.LongRunning);
				task.Start();
			}
		}

		private async void ProcessConnection(HttpListenerContext context, CancellationToken token)
		{
			try
			{
				await ProcessConnectionAsync(context);
				token.ThrowIfCancellationRequested();
			}
			catch (OperationCanceledException)
			{
				StopServer();
			}
			catch (Exception ex)
			{
				LogManager.WriteError(ex, "Error while processing connection");
			}
		}

		private static async Task ProcessConnectionAsync(HttpListenerContext context)
		{
			var contextRequest = context.Request;
			var contextResponse = context.Response;

			var response = Stream.Null;

			Console.WriteLine($"Request #{++_requestCount}");
			Console.WriteLine($"Client IP: {contextRequest.RemoteEndPoint}");
			Console.WriteLine($"Client UserAgent: {contextRequest.UserAgent}");

			var postHttpMethod = HttpMethod.Post.Method;
			if (!contextRequest.HttpMethod.Equals(postHttpMethod))
			{
				Console.WriteLine($"Http method wasn't {postHttpMethod}");
			}
			else
			{
				if (!contextRequest.Headers.AllKeys.Contains(CommonConstants.RequestInfoHeaderName))
				{
					Console.WriteLine($"Headers hasn't {CommonConstants.RequestInfoHeaderName}");
				}
				else
				{
					var rawRequestInfo = contextRequest.Headers[CommonConstants.RequestInfoHeaderName];
					HttpStatusCode code;
					(code, response) = ProcessRequest(contextRequest.InputStream, rawRequestInfo);
					contextResponse.StatusCode = (int)code;
				}
			}

			contextResponse.ContentType = "application/json";
			contextResponse.ContentEncoding = CommonConstants.StandardEncoding;

			await using var encryptedData = await CryptHelper.EncryptAndBase64(CryptHelper.MasterPass, response);
			contextResponse.ContentLength64 = encryptedData.Length;
			await encryptedData.CopyToAsync(contextResponse.OutputStream);

			contextResponse.Close();
			Console.WriteLine();
		}

		private static (HttpStatusCode, Stream) ProcessRequest(Stream inputStream, string rawRequestInfo)
		{
			try
			{
				var request = GetRequest(rawRequestInfo);
				if (string.IsNullOrEmpty(request?.UserInfo?.UserName) || string.IsNullOrEmpty(request.ApiMethod))
				{
					Console.WriteLine("Bad request");
					return (HttpStatusCode.BadRequest, Stream.Null);
				}

				if (!Auth.Instance.Value.IsAuthorized(request.UserInfo))
				{
					Console.WriteLine($"Unauthorized user \"{request.UserInfo.UserName}\"");
					return (HttpStatusCode.Unauthorized, Stream.Null);
				}

				var apiAction = request.ApiMethod;
				var isMethodImplemented = ApiControllersInitializer.Instance.
					RequestToControllersDictionary.TryGetValue(apiAction.ToLowerInvariant(), out var processor);
				if (!isMethodImplemented)
				{
					Console.WriteLine($"Unsupported method {apiAction}");
					return (HttpStatusCode.NotImplemented, Stream.Null);
				}

				if (!Auth.Instance.Value.HasUserPermission(request.UserInfo, apiAction))
				{
					Console.WriteLine($"User \"{request.UserInfo.UserName}\" not permitted to \"{apiAction}\"");
					return (HttpStatusCode.Forbidden, Stream.Null);
				}

				Console.WriteLine($"User: \"{request.UserInfo.UserName}\"{Environment.NewLine}Method: \"{apiAction}\"");

				var decryptedContent = CryptHelper.Decrypt(CryptHelper.MasterPass, inputStream).Result;
				return processor.ProcessRequest(request.ContentInfo, decryptedContent);
			}
			catch (Exception e)
			{
				LogManager.WriteError(e, "Error while processing request");
				return (HttpStatusCode.InternalServerError, Stream.Null);
			}
		}

		private static Request GetRequest(string rawRequestInfo)
		{
			try
			{
				var result = CryptHelper.DecryptBased64(CryptHelper.MasterPass, rawRequestInfo).Result;
				return JsonSerializer.Deserialize<Request>(result);
			}
			catch
			{
				return null;
			}
		}
	}
}

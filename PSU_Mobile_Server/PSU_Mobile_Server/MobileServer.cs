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

			var response = string.Empty;

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
				contextResponse.StatusCode = (int)ProcessRequest(contextRequest.InputStream, out response);
			}

			contextResponse.ContentType = "application/json";
			contextResponse.ContentEncoding = CommonConstants.StandardEncoding;

			var encryptedData = await CryptHelper.EncryptAndBase64(CryptHelper.MasterPass, response);
			var data = CommonConstants.StandardEncoding.GetBytes(encryptedData);
			contextResponse.ContentLength64 = data.LongLength;
			await contextResponse.OutputStream.WriteAsync(data.AsMemory(0, data.Length));

			contextResponse.Close();

			Console.WriteLine();
		}

		private static HttpStatusCode ProcessRequest(Stream inputStream, out string response)
		{
			response = string.Empty;

			try
			{
				var request = GetRequest(inputStream);
				if (request == null)
				{
					Console.WriteLine("Bad request");
					return HttpStatusCode.BadRequest;
				}

				if (!Auth.Instance.Value.IsAuthorized(request.UserInfo))
				{
					Console.WriteLine($"Unauthorized user \"{request.UserInfo.UserName}\"");
					return HttpStatusCode.Unauthorized;
				}

				var apiAction = request.ApiMethod;
				var isMethodImplemented = ApiControllersInitializer.Instance.
					RequestToControllersDictionary.TryGetValue(apiAction.ToLowerInvariant(), out var processor);
				if (!isMethodImplemented)
				{
					Console.WriteLine($"Unsupported method {apiAction}");
					return HttpStatusCode.NotImplemented;
				}

				if (!Auth.Instance.Value.HasUserPermission(request.UserInfo, apiAction))
				{
					Console.WriteLine($"User \"{request.UserInfo.UserName}\" not permitted to \"{apiAction}\"");
					return HttpStatusCode.Forbidden;
				}

				Console.WriteLine($"User: \"{request.UserInfo.UserName}\"{Environment.NewLine}Method: \"{apiAction}\"");
				processor.ProcessRequest(request.RequestContent);
				response = processor.Response;
				return processor.StatusCode;
			}
			catch (Exception e)
			{
				LogManager.WriteError(e, "Error while processing request");
				return HttpStatusCode.InternalServerError;
			}
		}

		private static Request GetRequest(Stream inputStream)
		{
			byte[] buffer;
			using (var ms = new MemoryStream())
			{
				inputStream.CopyToAsync(ms);
				ms.Position = 0;
				buffer = ms.ToArray();
			}

			var encryptedRequest = CommonConstants.StandardEncoding.GetString(buffer);
			var request = CryptHelper.DecryptBased64(CryptHelper.MasterPass, encryptedRequest).Result;
			var deserializedRequest = JsonSerializer.Deserialize<Request>(request);
			return deserializedRequest;
		}
	}
}

using Common;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace PSU_Mobile_Server
{
	internal class MobileServer
	{
		private readonly HttpListener _listener;
		private static int _requestCount = 0;

		private readonly CancellationTokenSource _cancellationTokenSource;

		public MobileServer()
		{
			_listener = new HttpListener();
			_cancellationTokenSource = new CancellationTokenSource();
		}

		public void Start()
		{
			Console.WriteLine("Starting a server...");
			_listener.Prefixes.Add(CommonConstants.Uri);
			_listener.Start();
			Console.WriteLine("Server started");
			var listenTask = HandleIncomingConnections();
			listenTask.GetAwaiter().GetResult();

			_listener.Close();
			Console.WriteLine("Server was stopped");
		}

		public async Task HandleIncomingConnections()
		{
			var token = _cancellationTokenSource.Token;
			while (!token.IsCancellationRequested)
			{
				// Will wait here until we hear from a connection
				var context = await _listener.GetContextAsync();

				var task = new Task(() => ProcessConnection(context, token), token, TaskCreationOptions.LongRunning);
				task.Start();
			}
		}

		private async void ProcessConnection(HttpListenerContext context, CancellationToken token)
		{
			try
			{
				var contextRequest = context.Request;
				var contextResponse = context.Response;

				var response = string.Empty;

				// Print out some info about the request
				Console.WriteLine("Request #: {0}", ++_requestCount);
				Console.WriteLine(contextRequest.Url.ToString());
				Console.WriteLine(contextRequest.HttpMethod);
				Console.WriteLine(contextRequest.UserHostName);
				Console.WriteLine(contextRequest.UserAgent);
				Console.WriteLine();

				// If `shutdown` url requested w/ POST, then shutdown the server after serving the page
				if (contextRequest.HttpMethod == "POST")
				{
					var apiAction = contextRequest.Headers["Method"];

					//TODO: тупо костылейшн. Обязательна авторизация перед такими запросами! Но у нас её пока нет, потому так
					if (apiAction == "shutdown")
					{
						Console.WriteLine("Shutdown requested");
						_cancellationTokenSource.Cancel();
					}
					contextResponse.StatusCode = ProcessRequest(apiAction, out response);
				}

				// Write the response info
				var data = CommonConstants.StandardEncoding.GetBytes(response);
				contextResponse.ContentType = "text/html";
				contextResponse.ContentEncoding = CommonConstants.StandardEncoding;
				contextResponse.ContentLength64 = data.LongLength;

				// Write out to the response stream (asynchronously), then close it
				await contextResponse.OutputStream.WriteAsync(data.AsMemory(0, data.Length), token);
				contextResponse.Close();
			}
			catch (Exception ex)
			{
				LogManager.WriteError(ex, "Error while processing connection");
			}
		}

		private static int ProcessRequest(string apiAction, out string response)
		{
			var isMethodImplemented =
				ApiControllersInitializer.Instance.RequestToControllersDictionary.TryGetValue(apiAction,
					out var processor);
			if (!isMethodImplemented)
			{
				Console.WriteLine($"Unsupported method {apiAction}");
				response = $"Unsupported method {apiAction}";
				return 501;
			}

			processor.ProcessRequest();
			response = processor.Response;
			return processor.StatusCode;
		}
	}
}

using Common;
using System;

namespace PSU_Mobile_Server
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			MobileServer server = null;
			AppDomain.CurrentDomain.ProcessExit += OnProcessExiting;
			try
			{
				server = MobileServer.Instance.Value;
				server.Start(new[] { CommonConstants.Port });
				server.ServerProcessingTask.GetAwaiter().GetResult();
			}
			catch (Exception ex)
			{
				LogManager.WriteError(ex, "Server crashed");
				server?.StopServer();

				Console.WriteLine("Press any key to exit...");
				Console.ReadKey();
			}
		}

		private static void OnProcessExiting(object sender, EventArgs e)
		{
			try
			{
				Auth.Instance.Value.Save();
			}
			catch (Exception ex)
			{
				LogManager.WriteError(ex, "Error while closing server");
			}
		}
	}
}

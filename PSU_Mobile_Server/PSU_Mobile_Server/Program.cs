using Common;
using System;
using System.Diagnostics;
using System.Reflection;

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
			Auth.Instance.Value.Save();
		}
	}
}

using System.Threading;

namespace PSU_Mobile_Server.Controllers
{
	internal class ShutdownProcessor : BaseApiController
	{
		private static CancellationTokenSource _shutdownTokenSource;

		public ShutdownProcessor() : base("Shutdown") { }

		public static void Initialize(CancellationTokenSource shutdownTokenSource)
		{
			_shutdownTokenSource = shutdownTokenSource;
		}

		public override void ProcessRequest(string requestContent)
		{
			Response = "Server stopped";
			_shutdownTokenSource.Cancel();
			base.ProcessRequest(requestContent);
		}
	}
}
using System.IO;
using System.Net;
using System.Threading;
using Common;

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

		public override (HttpStatusCode, Stream) ProcessRequest(byte[] contentInfo, Stream requestContent)
		{
			var response = new MemoryStream(CommonConstants.StandardEncoding.GetBytes("Server stopped"));
			_shutdownTokenSource.Cancel();
			return (HttpStatusCode.OK, response);
		}
	}
}
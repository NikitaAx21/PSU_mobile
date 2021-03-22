using System.Net;
using System.Text;

namespace Common
{
	public static class CommonConstants
	{
		public const int Port = 8888;
		public static readonly string Uri = $"http://{IPAddress.Loopback}:{CommonConstants.Port}/";
		public static Encoding StandardEncoding = Encoding.UTF8;
		public const string PassSalt = "xt5g6%HJ%tjy";
		public const string SuperPass = "SuperMegaPa$$w0rd";
		public const string SuperUser = "root";
		public const string RequestInfoHeaderName = "X-Request";
	}
}

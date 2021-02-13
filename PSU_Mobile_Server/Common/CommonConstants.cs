using System.Text;

namespace Common
{
	public static class CommonConstants
	{
		public const int Port = 8888;
		public static readonly string Uri = $"http://localhost:{CommonConstants.Port}/";
		public static Encoding StandardEncoding = Encoding.UTF8;
	}
}

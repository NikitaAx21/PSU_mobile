using System.IO;
using System.Net;
using Common;

namespace PSU_Mobile_Server.Controllers
{
	internal abstract class BaseApiController
	{
		protected BaseApiController(string requestName)
		{
			RequestName = requestName;
		}

		public string RequestName { get; }

		public virtual (HttpStatusCode, Stream) ProcessRequest(byte[] contentInfo, Stream requestContent)
		{
			var response = new MemoryStream(CommonConstants.StandardEncoding.GetBytes("С запросом все круто!"));
			return (HttpStatusCode.OK, response);
		}
	}
}

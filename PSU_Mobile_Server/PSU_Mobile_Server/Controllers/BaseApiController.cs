using System.Net;

namespace PSU_Mobile_Server.Controllers
{
	internal abstract class BaseApiController
	{
		protected BaseApiController(string requestName)
		{
			RequestName = requestName;
			StatusCode = HttpStatusCode.OK;
			Response = "С запросом все круто!";
		}

		public string RequestName { get; }
		public string Response { get; protected set; }
		public HttpStatusCode StatusCode { get; protected set; }

		public virtual void ProcessRequest(string requestContent)
		{

		}
	}
}

namespace PSU_Mobile_Server.Controllers
{
	internal abstract class BaseApiController
	{
		protected BaseApiController(string requestName)
		{
			RequestName = requestName;
			StatusCode = 200;
			Response = "С запросом все круто!";
		}

		public string RequestName { get; }
		public string Response { get; protected set; }
		public int StatusCode { get; protected set; }

		public virtual void ProcessRequest(string requestContent)
		{

		}
	}
}

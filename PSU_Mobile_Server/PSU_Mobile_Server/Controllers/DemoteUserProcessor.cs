using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Text.Json;
using Common;
namespace PSU_Mobile_Server.Controllers
{
	internal class DemoteUserProcessor : BaseApiController
	{
		public DemoteUserProcessor() : base("Demote")
		{

		}

		public override (HttpStatusCode, Stream) ProcessRequest(byte[] contentInfo, Stream requestContent)
		{
			try
			{
				var userID = JsonSerializer.DeserializeAsync<User>(requestContent).Result.ID;
				var isUserPromoted = Auth.Instance.Value.TryToDemote(userID);

				var statusCode = isUserPromoted ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
				return (statusCode, Stream.Null);
			}
			catch (Exception)
			{
				return (HttpStatusCode.InternalServerError, Stream.Null);
			}

			//return base.ProcessRequest(contentInfo, requestContent);
		}
	}
}

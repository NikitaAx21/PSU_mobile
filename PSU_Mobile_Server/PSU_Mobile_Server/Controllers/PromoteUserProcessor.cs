using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Text.Json;
using Common;

namespace PSU_Mobile_Server.Controllers
{
	internal class PromoteUserProcessor : BaseApiController
	{
		public PromoteUserProcessor() : base("Promote")
		{

		}

		public override (HttpStatusCode, Stream) ProcessRequest(byte[] contentInfo, Stream requestContent)
		{
			try
			{
				var userID = JsonSerializer.DeserializeAsync<User>(requestContent).Result.ID;
				Console.WriteLine($" name {userID}");

				var isUserPromoted = Auth.Instance.Value.TryToPromote(userID);

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

using System;
using System.IO;
using System.Net;
using System.Text.Json;
using Common;

namespace PSU_Mobile_Server.Controllers
{
 	internal class UpdUserProcessor : BaseApiController
	{
		public UpdUserProcessor() : base("UpdUser")
		{

		}

		public override (HttpStatusCode, Stream) ProcessRequest(byte[] contentInfo, Stream requestContent)
		{
			try
			{
				var userInfo = JsonSerializer.DeserializeAsync<User>(requestContent).Result;
				var isUserUpdated = Auth.Instance.Value.TryUpdUser(userInfo);


				var statusCode = isUserUpdated ? HttpStatusCode.Created : HttpStatusCode.InternalServerError;
				return (statusCode, Stream.Null);
			}
			catch (Exception)
			{
				return (HttpStatusCode.InternalServerError, Stream.Null);
			}
		}
	}



}

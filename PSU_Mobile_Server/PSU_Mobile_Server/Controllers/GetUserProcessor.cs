using System;
using System.IO;
using System.Net;
using System.Text.Json;
using Common;
using System.Text;

namespace PSU_Mobile_Server.Controllers
{

	internal class GetUserProcessor : BaseApiController
	{
		public GetUserProcessor() : base("GetUser")
		{

		}

		public override (HttpStatusCode, Stream) ProcessRequest(byte[] contentInfo, Stream requestContent)
		{
			try
			{
				var userInfo = JsonSerializer.DeserializeAsync<User>(requestContent).Result.ID;

				var tempuser=new User();
				var isUserObtained = Auth.Instance.Value.TryGetUser(userInfo, out tempuser);
				var statusCode = isUserObtained ? HttpStatusCode.Created : HttpStatusCode.InternalServerError;


				var serializedResp = new MemoryStream(CommonConstants.StandardEncoding.GetBytes(JsonSerializer.Serialize(tempuser)));

				return (statusCode, serializedResp/**/);
			}
			catch (Exception)
			{
				return (HttpStatusCode.InternalServerError, Stream.Null);
			}
		}
	}


}

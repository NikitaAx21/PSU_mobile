using System;
using System.IO;
using System.Net;
using System.Text.Json;
using Common;

namespace PSU_Mobile_Server.Controllers
{
	internal class CreateUserProcessor : BaseApiController
	{
		public CreateUserProcessor() : base("CreateUser")
		{

		}

		public override (HttpStatusCode, Stream) ProcessRequest(byte[] contentInfo, Stream requestContent)
		{
			try
			{
				var userInfo = JsonSerializer.DeserializeAsync<UserInfo>(requestContent).Result;
				var isUserCreated = Auth.Instance.Value.TryAddUser(userInfo);
				var statusCode = isUserCreated ? HttpStatusCode.Created : HttpStatusCode.InternalServerError;
				return (statusCode, Stream.Null);
			}
			catch (Exception)
			{
				return (HttpStatusCode.InternalServerError, Stream.Null);
			}
		}
	}
}

using System;
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

		public override void ProcessRequest(string requestContent)
		{
			try
			{
				var userInfo = JsonSerializer.Deserialize<UserInfo>(requestContent);
				var isUserCreated = Auth.Instance.Value.TryAddUser(userInfo);
				StatusCode = isUserCreated ? HttpStatusCode.Created : HttpStatusCode.InternalServerError;
				base.ProcessRequest(requestContent);
			}
			catch (Exception ex)
			{
				StatusCode = HttpStatusCode.InternalServerError;
			}
		}
	}
}

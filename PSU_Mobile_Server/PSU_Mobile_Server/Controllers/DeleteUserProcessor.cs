using System;
using System.IO;
using System.Net;
using System.Text.Json;
using Common;

namespace PSU_Mobile_Server.Controllers
{
   	internal class DeleteUserProcessor : BaseApiController
	{
		public DeleteUserProcessor() : base("DelUser")
		{

		}

		public override (HttpStatusCode, Stream) ProcessRequest(byte[] contentInfo, Stream requestContent)
		{
			try
			{
				var userID = JsonSerializer.DeserializeAsync<User>(requestContent).Result.ID;// ID !!

				var isUserdeleted = Auth.Instance.Value.TryDeleteUser(userID);

				var statusCode = isUserdeleted ? HttpStatusCode.Created/*?*/ : HttpStatusCode.InternalServerError;
				return (statusCode, Stream.Null);// ответ ??
			}
			catch (Exception)
			{
				return (HttpStatusCode.InternalServerError, Stream.Null);
			}
		}
	}






}

using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Text.Json;
using Common;

namespace PSU_Mobile_Server.Controllers
{
	internal class DeleteGroupProcessor : BaseApiController
	{
		public DeleteGroupProcessor() : base("DelGroup")
		{

		}

		public override (HttpStatusCode, Stream) ProcessRequest(byte[] contentInfo, Stream requestContent)
		{
			try
			{
				var groupID = JsonSerializer.DeserializeAsync<Group>(requestContent).Result.ID;// ID !!
				var isGroupExisted = Auth.Instance.Value.TryDeleteGroup(groupID);

				var statusCode = isGroupExisted ? HttpStatusCode.Created/*?*/ : HttpStatusCode.InternalServerError;
				return (statusCode, Stream.Null);// ответ ??
			}
			catch (Exception)
			{
				return (HttpStatusCode.InternalServerError, Stream.Null);
			}

			//return base.ProcessRequest(contentInfo, requestContent);
		}
	}







}

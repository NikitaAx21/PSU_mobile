using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Text.Json;
using Common;

namespace PSU_Mobile_Server.Controllers//===========----------------
{
	internal class DeleteFileProcessor : BaseApiController
	{
		public DeleteFileProcessor() : base("AddGroup")
		{

		}

		public override (HttpStatusCode, Stream) ProcessRequest(byte[] contentInfo, Stream requestContent)
		{
			try
			{
				var groupInfo = JsonSerializer.DeserializeAsync<Group>(requestContent).Result;
				var isGroupCreated = Auth.Instance.Value.TryAddGroup(groupInfo);

				var statusCode = isGroupCreated ? HttpStatusCode.Created : HttpStatusCode.InternalServerError;
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

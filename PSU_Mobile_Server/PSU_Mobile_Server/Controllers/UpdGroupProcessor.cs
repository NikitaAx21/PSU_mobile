using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Text.Json;
using Common;

namespace PSU_Mobile_Server.Controllers
{
	internal class UpdGroupProcessor : BaseApiController
	{
		public UpdGroupProcessor() : base("UpdGroup")
		{

		}

		public override (HttpStatusCode, Stream) ProcessRequest(byte[] contentInfo, Stream requestContent)
		{
			try
			{
				var groupInfo = JsonSerializer.DeserializeAsync<Group>(requestContent).Result;
				var isGroupUpdated = Auth.Instance.Value.TryUpdGroup(groupInfo);


				var statusCode = isGroupUpdated ? HttpStatusCode.Created : HttpStatusCode.InternalServerError;
				return (statusCode, Stream.Null);
			}
			catch (Exception)
			{
				return (HttpStatusCode.InternalServerError, Stream.Null);
			}
		}
	}




}

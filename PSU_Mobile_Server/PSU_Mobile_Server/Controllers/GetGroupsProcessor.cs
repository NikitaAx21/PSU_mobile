using System;
using System.IO;
using System.Net;
using System.Text.Json;
using Common;
using System.Text;


namespace PSU_Mobile_Server.Controllers
{
	internal class GetGroupProcessor : BaseApiController
	{
		public GetGroupProcessor() : base("GetGroup")
		{

		}

		public override (HttpStatusCode, Stream) ProcessRequest(byte[] contentInfo, Stream requestContent)
		{
			try
			{
				var groupInfo = JsonSerializer.DeserializeAsync<Group>(requestContent).Result.ID;

				var isGroupObtained = Auth.Instance.Value.TryGetGroup(groupInfo, out var tempgroup);
				var statusCode = isGroupObtained ? HttpStatusCode.Created : HttpStatusCode.InternalServerError;


				var serializedResp = new MemoryStream(CommonConstants.StandardEncoding.GetBytes(JsonSerializer.Serialize(tempgroup)));

				return (statusCode, serializedResp/**/);
			}
			catch (Exception)
			{
				return (HttpStatusCode.InternalServerError, Stream.Null);
			}
		}
	}


}
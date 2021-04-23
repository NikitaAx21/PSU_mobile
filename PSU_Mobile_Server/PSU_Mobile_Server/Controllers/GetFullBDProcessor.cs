using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Text.Json;
using Common;


namespace PSU_Mobile_Server.Controllers
{
 	internal class GetFullBDProcessor : BaseApiController
	{
		public GetFullBDProcessor() : base("GetFullBD")
		{

		}

		public override (HttpStatusCode, Stream) ProcessRequest(byte[] contentInfo, Stream requestContent)
		{
			try
			{
				var username = JsonSerializer.DeserializeAsync<User>(requestContent).Result.UserName;

				Console.WriteLine($"requestContent username: {username}");


				var isAllBDObtained = Auth.Instance.Value.TryGetFullBD(username, out var tempBD);// вылет, требуется дебаг

				Console.WriteLine("isAllBDObtained");
				Console.WriteLine(isAllBDObtained);

				var statusCode = isAllBDObtained ? HttpStatusCode.Created : HttpStatusCode.InternalServerError;


				var serializedResp = new MemoryStream(CommonConstants.StandardEncoding.GetBytes(JsonSerializer.Serialize(tempBD)));

				return (statusCode, serializedResp/**/);
			}
			catch (Exception)
			{
				return (HttpStatusCode.InternalServerError, Stream.Null);
			}
		}
	}

}

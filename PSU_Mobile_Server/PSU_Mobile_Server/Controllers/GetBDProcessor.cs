using System;
using System.IO;
using System.Net;
using System.Text.Json;
using Common;
using System.Text;


namespace PSU_Mobile_Server.Controllers
{
	internal class GetBDProcessor : BaseApiController
	{
		public GetBDProcessor() : base("GetBD")
		{

		}

		public override (HttpStatusCode, Stream) ProcessRequest(byte[] contentInfo, Stream requestContent)
		{
			try
			{
				var username = JsonSerializer.DeserializeAsync<User>(requestContent).Result.UserName;

				var isBDObtained = false;//Auth.Instance.Value.TryGetBD(username, out var tempBD);// вылет, требуется дебаг

				DataBase asd = new DataBase();
				try
				{
					isBDObtained = Auth.Instance.Value.TryGetBD(username, out var tempBD);// вылет, требуется дебаг
					asd = tempBD;
				}
				catch (Exception)
				{

					Console.WriteLine("Exception");

					return (HttpStatusCode.Created, Stream.Null);//вылет здесь
				}

				Console.WriteLine("isBDObtained");
				Console.WriteLine(isBDObtained);

				var statusCode = isBDObtained ? HttpStatusCode.Created : HttpStatusCode.InternalServerError;


				var serializedResp = new MemoryStream(CommonConstants.StandardEncoding.GetBytes(JsonSerializer.Serialize(asd/*tempBD*/)));

				return (statusCode, serializedResp/**/);
			}
			catch (Exception)
			{
				return (HttpStatusCode.InternalServerError, Stream.Null);
			}
		}
	}







}

using Common;
using System;
using System.IO;
using System.Net;

namespace PSU_Mobile_Server.Controllers
{
	//TODO (Никита): этот процессор - пример. в релизе, наверное, нужны более конкретные запросы
	internal class SaveFileProcessor : BaseApiController
	{
		public SaveFileProcessor() : base("SaveFile")
		{

		}

		public override (HttpStatusCode, Stream) ProcessRequest(byte[] contentInfo, Stream requestContent)
		{
			try
			{
				//TODO (Никита): просто костыльный путь для сохранения
				Directory.CreateDirectory(".//server/");
				var fileName = CommonConstants.StandardEncoding.GetString(contentInfo);
				using var writeStream = File.OpenWrite($".//server/{fileName}");
				requestContent.CopyTo(writeStream);
				return (HttpStatusCode.Created, Stream.Null);
			}
			catch (Exception)
			{
				return (HttpStatusCode.InternalServerError, Stream.Null);
			}
		}
	}
}

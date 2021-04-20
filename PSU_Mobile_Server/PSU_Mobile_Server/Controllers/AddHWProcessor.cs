using System;
using System.IO;
using System.Net;
using System.Text.Json;
using Common;


namespace PSU_Mobile_Server.Controllers
{
	internal class AddHWProcessor : BaseApiController
	{
		public AddHWProcessor() : base("AddHWFile")
		{

		}

		public override (HttpStatusCode, Stream) ProcessRequest(byte[] contentInfo, Stream requestContent)
		{
			try
			{
				var paramInfo = JsonSerializer.Deserialize<FileProcessorInfo>(contentInfo);

				string newPath;
				var isFileInfoCorrect = Auth.Instance.Value.TryGetHWFilePath(paramInfo, out newPath);

				if (isFileInfoCorrect)
				{
					Directory.CreateDirectory($".//server/{newPath}");//
					var fileName = paramInfo.filename;
					using var writeStream = File.OpenWrite($".//server/{newPath}");
					requestContent.CopyTo(writeStream);
				}


				var statusCode = isFileInfoCorrect ? HttpStatusCode.Created : HttpStatusCode.InternalServerError;

				return (statusCode, Stream.Null);
			}
			catch (Exception)
			{
				return (HttpStatusCode.InternalServerError, Stream.Null);
			}
		}
	}



}

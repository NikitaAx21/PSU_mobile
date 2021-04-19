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



				//contentInfo => id's ? 
				//Guid userID = new Guid(contentInfo);//
				//Guid groupID = new Guid(contentInfo);//
				//Guid lessonID = new Guid(contentInfo);//

				string newPath;
				var isFileInfoCorrect = Auth.Instance.Value.TryGetHWFilePath(paramInfo.userID, paramInfo.groupID, paramInfo.lessonID, out newPath);

				if (isFileInfoCorrect)
				{
					Directory.CreateDirectory($".//server/{newPath}/");//
					var fileName = paramInfo.filename;
					using var writeStream = File.OpenWrite($".//server/{newPath}/{fileName}");
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

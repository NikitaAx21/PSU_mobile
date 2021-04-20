using System;
using System.IO;
using System.Net;
using System.Text.Json;
using Common;


namespace PSU_Mobile_Server.Controllers
{

	internal class GetFileProcessor : BaseApiController
	{
		public GetFileProcessor() : base("GetFile")
		{

		}

		public override (HttpStatusCode, Stream) ProcessRequest(byte[] contentInfo, Stream requestContent)
		{
			try
			{

				var paramInfo = JsonSerializer.DeserializeAsync<FileProcessorInfo>(requestContent).Result;


				var isFileInfoCorrect = Auth.Instance.Value.FileExistanceCheck(paramInfo/*, out var fileStream*/);

				var reqContent = Stream.Null;//??


				if (isFileInfoCorrect && File.Exists(paramInfo.filename))
				{

					reqContent = File.OpenRead($".//server/{paramInfo.filename}");


				}


				var statusCode = isFileInfoCorrect ? HttpStatusCode.Created : HttpStatusCode.InternalServerError;

				return (statusCode, reqContent/*Stream.Null*/);
			}
			catch (Exception)
			{
				return (HttpStatusCode.InternalServerError, Stream.Null);
			}
		}
	}






}

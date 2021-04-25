using System;
using System.IO;
using System.Net;
using System.Text.Json;
using Common;


namespace PSU_Mobile_Server.Controllers
{

	internal class AddCommFileProcessor : BaseApiController
	{
		public AddCommFileProcessor() : base("AddCommFile")
		{

		}

		public override (HttpStatusCode, Stream) ProcessRequest(byte[] contentInfo, Stream requestContent)
		{
			try
			{
				var asd = CryptHelper.Decrypt(CryptHelper.MasterPass, contentInfo/*string.Empty*/).Result;

				var paramInfo = JsonSerializer.Deserialize<FileProcessorInfo>(asd);

				string newPath;
				var isFileInfoCorrect = Auth.Instance.Value.TryGetCommFilePath(paramInfo, out newPath);

				if (isFileInfoCorrect)
				{
					Directory.CreateDirectory($".//server/{newPath}");//
					using var writeStream = File.OpenWrite($".//server/{newPath}{paramInfo.filename}");
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

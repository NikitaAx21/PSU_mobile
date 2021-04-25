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
				Console.WriteLine($" WTF ");

				var asd = CryptHelper.Decrypt(CryptHelper.MasterPass, contentInfo/*string.Empty*/).Result;


				Console.WriteLine($" AddCommFile {asd} ");


				var paramInfo = JsonSerializer.Deserialize<FileProcessorInfo>(asd);


				string newPath;
				var isFileInfoCorrect = Auth.Instance.Value.TryGetHWFilePath(paramInfo, out newPath);


				Console.WriteLine($" AddCommFile {isFileInfoCorrect} ");



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

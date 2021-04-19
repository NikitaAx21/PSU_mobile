using System;
using System.IO;
using System.Net;
using System.Text.Json;
using Common;


namespace PSU_Mobile_Server.Controllers
{
	internal class AddFileProcessor : BaseApiController
	{
		public AddFileProcessor() : base("AddFile")
		{

		}

		public override (HttpStatusCode, Stream) ProcessRequest(byte[] contentInfo, Stream requestContent)
		{
			try
			{
				var fileInfo = JsonSerializer.DeserializeAsync<UserInfo>(requestContent).Result;// ID ??

				var isFileCreated = Auth.Instance.Value.TryAddfile(fileInfo);
				var statusCode = isFileCreated ? HttpStatusCode.Created : HttpStatusCode.InternalServerError;
				return (statusCode, Stream.Null);
			}
			catch (Exception)
			{
				return (HttpStatusCode.InternalServerError, Stream.Null);
			}
		}
	}













}

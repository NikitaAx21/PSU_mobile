﻿using System;
using System.IO;
using System.Net;
using System.Text.Json;
using Common;

namespace PSU_Mobile_Server.Controllers
{
	internal class CreateUserProcessor : BaseApiController
	{
		public CreateUserProcessor() : base("CreateUser")//студент с дефолтом  // доп права от админа на создание преподов
		{

		}

		public override (HttpStatusCode, Stream) ProcessRequest(byte[] contentInfo, Stream requestContent)
		{
			try
			{
				var userInfo = JsonSerializer.DeserializeAsync<User>(requestContent).Result;

				var isUserCreated = Auth.Instance.Value.TryAddUser(userInfo);
				var statusCode = isUserCreated ? HttpStatusCode.Created : HttpStatusCode.InternalServerError;
				return (statusCode, Stream.Null);
			}
			catch (Exception)
			{
				return (HttpStatusCode.InternalServerError, Stream.Null);
			}
		}
	}
}

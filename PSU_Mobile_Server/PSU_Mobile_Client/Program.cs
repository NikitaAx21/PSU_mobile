using Common;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace PSU_Mobile_Client
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			try
			{
				var handler = new HttpClientHandler();
				var client = new HttpClient(handler);

				var ip = IPAddress.Loopback;
				var baseAddress = new Uri($"http://{ip}:{CommonConstants.Port}/");

				while (true)
				{
					Work(client, baseAddress);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteError(ex, "Client crashed!");
			}
		}

		private static void Work(HttpClient client, Uri baseAddress)
		{
			var newUserInfo = new UserInfo
			{
				UserName = $"User_{new Random().Next()}",
				PasswordHash = AuthHelper.HashPassword("SomePass"),
			};

			var userInfo = new UserInfo
			{
				PasswordHash = AuthHelper.HashPassword(CommonConstants.SuperPass),
				UserName = CommonConstants.SuperUser
			};

			using var reqContent = new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(newUserInfo)));

			//using var reqContent = File.OpenRead(@"D:\metanit.zip");
			//var content = encryptedFile;

			//var fileName = $"metanit_{new Random().Next()}.zip";
			var req = new Request
			{
				UserInfo = userInfo,
				ApiMethod = "CreateUser",
				//Method = "SaveFile",
				ContentInfo = CryptHelper.Encrypt(CryptHelper.MasterPass, string.Empty).Result
				//ContentInfo = CommonConstants.StandardEncoding.GetBytes(fileName)
			};
			var serializedReq = JsonSerializer.Serialize(req);
			var encryptedReq = CryptHelper.EncryptAndBase64(CryptHelper.MasterPass, serializedReq).Result;
			var encryptedRequest = CryptHelper.Encrypt(CryptHelper.MasterPass, reqContent).Result;

			var cont = new StreamContent(encryptedRequest);
			cont.Headers.Add(CommonConstants.RequestInfoHeaderName, encryptedReq);
			var response = client.PostAsync(baseAddress, cont).Result;
			var result = CryptHelper.Decrypt(CryptHelper.MasterPass, response.Content.ReadAsByteArrayAsync().Result).Result;
			Console.WriteLine($"StatusCode: {response.StatusCode}");
			Console.WriteLine($"Response: {result}");

			return;
			while (true)
			{
				//TODO просто костыль, чтоб не закрывалось окно и не терялся контекст
			}
		}
	}
}
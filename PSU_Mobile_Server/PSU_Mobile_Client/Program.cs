using Common;
using System;
using System.Net;
using System.Net.Http;
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
			var req = new Request
			{
				UserInfo = userInfo,
				ApiMethod = "CreateUser",
				RequestContent = JsonSerializer.Serialize(newUserInfo)
			};

			var requestString = JsonSerializer.Serialize(req);
			var encryptedRequest = CryptHelper.EncryptAndBase64(CryptHelper.MasterPass, requestString).Result;

			var response = client.PostAsync(baseAddress, new StringContent(encryptedRequest)).Result;
			var encryptedResult = CommonConstants.StandardEncoding.GetString(response.Content.ReadAsByteArrayAsync().Result);
			var result = CryptHelper.DecryptBased64(CryptHelper.MasterPass, encryptedResult).Result;
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
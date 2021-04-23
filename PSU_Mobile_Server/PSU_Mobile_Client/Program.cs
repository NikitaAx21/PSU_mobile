using Common;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;


namespace PSU_Mobile_Client
{
	internal class Program
	{



		private static DataBase BD;

		private static void Main(string[] args)
		{
			try
			{
				var handler = new HttpClientHandler();
				var client = new HttpClient(handler);

				var ip = IPAddress.Loopback;
				var baseAddress = new Uri($"http://{ip}:{CommonConstants.Port}/");

				Work(client, baseAddress, "GetFullBD");


				//while (true)
				//for(int i=0; i<50;i++)
				//{
				Work(client, baseAddress, "AddGroup");//"AddLesson"); 

				//}
			}
			catch (Exception ex)
			{
				LogManager.WriteError(ex, "Client crashed!");
			}
		}

		private static void Work(HttpClient client, Uri baseAddress, string Method)
		{
			//var newUserInfo = new User//Info
			//{
			//	UserName = $"User_{new Random().Next()}",
			//	PasswordHash = AuthHelper.HashPassword("SomePass"),
			//};

			var userInfo = new User//Info
			{
				PasswordHash = AuthHelper.HashPassword(CommonConstants.SuperPass),
				UserName = CommonConstants.SuperUser
			};

			


			using var reqContent = new MemoryStream(Encoding.UTF8.GetBytes(getreqContent(Method, out string info)));// сериализация тела запроса для метода


			//using var reqContent = File.OpenRead(@"D:\metanit.zip");
			//var content = encryptedFile;

			//var fileName = $"metanit_{new Random().Next()}.zip";


			//if (Method== "AddCommFile")
			//{
			//	ContentInfo=...


			//}


			var req = new Request
			{
				UserInfo = userInfo,
				ApiMethod = Method,//"CreateUser",
				//Method = "SaveFile",



				ContentInfo = CryptHelper.Encrypt(CryptHelper.MasterPass, info/*string.Empty*/).Result
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

			if (Method== "GetFullBD" || Method=="GetBD")
			{
				var result1 = CryptHelper.Decrypt(CryptHelper.MasterPass, response.Content.ReadAsStream()).Result;

				var lifo = JsonSerializer.DeserializeAsync<DataBase>(result1).Result;

				BD = lifo;

				return;

			}


			while (true)
			{
					//TODO просто костыль, чтоб не закрывалось окно и не терялся контекст
			}
		}



		//private static T Work2<T>() where T:class //былоб неплохо....
		//{

		//	T v = new T();
		//	return v;
		//}	





		private static string getreqContent(string Method, out string info)
		{

			info = string.Empty;

			if (Method == "CreateUser")//+
			{
				string rndname = $"User_{new Random().Next()}";

				var newUserInfo = new User//Info
				{
					UserName = rndname,
					PasswordHash = "SomePass",//AuthHelper.HashPassword("SomePass"),

					Name = rndname.Substring(0,7),
					Surname = rndname.Substring(10,3)

				};

				return JsonSerializer.Serialize(newUserInfo);
			}



			if (Method == "GetBD")//----
			{
				var newUserInfo = new User//Info
				{
					UserName = CommonConstants.SuperUser
				};

				return JsonSerializer.Serialize(newUserInfo);
			}


			if (Method == "GetFullBD")//+
			{
				var newUserInfo = new User
				{
					UserName = CommonConstants.SuperUser
				};

				return JsonSerializer.Serialize(newUserInfo);
			}


			if (Method == "AddGroup")//+
			{
				string rndname = $"Group_{new Random().Next()}";

				var newGroupInfo = new Group//Info
				{
					GroupName = rndname
				};

				return JsonSerializer.Serialize(newGroupInfo);
			}

			if (Method == "AddLesson")//-----------------??
			{

				info = JsonSerializer.Serialize(BD.Groups[0].ID);

				string rndname = $"Lesson_{new Random().Next()}";

				var newLessonInfo = new Lesson//Info
				{
					TopicName = rndname,

					TestFlag = false,

					Date = DateTime.Now,

				};

				return JsonSerializer.Serialize(newLessonInfo);
			}






			//if (Method == "AddCommFile")
			//{

			//	var FileProcessorInfo = new info
			//	{
			//		userID = BD.Users[1],
			//		groupID= BD.Groups[1],

			//		filename="xxx"

			//	};

			//	return JsonSerializer.Serialize(info);
			//}

			var defaultUserInfo = new User
			{
				UserName = $"User_{new Random().Next()}",
				PasswordHash = AuthHelper.HashPassword("SomePass"),
			};

			return JsonSerializer.Serialize(defaultUserInfo);
		}
	}
}
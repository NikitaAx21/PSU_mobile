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


    //            for (int i = 0; i < 10; i++)
    //            {
    //                Work(client, baseAddress, "CreateUser");
    //            }

    //            Work(client, baseAddress, "AddGroup");
    //            Work(client, baseAddress, "AddGroup");

    //            Work(client, baseAddress, "GetFullBD");


    //            Work(client, baseAddress, "AddLesson");

                //Work(client, baseAddress, "GetFullBD");


                Work(client, baseAddress, "UpdGroup");//"AddLesson"); 


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

			var userInfo = new User
			{
				PasswordHash = AuthHelper.HashPassword(CommonConstants.SuperPass),
				UserName = CommonConstants.SuperUser
			};







			using var reqContent1 = File.OpenRead(@"D:\CameraA.zip");

			//using var encryptedFile = CryptHelper.Encrypt(CryptHelper.MasterPass, reqContent1).Result;

			var content = reqContent1;//encryptedFile;

			//var fileName = $"metanit_{new Random().Next()}.zip";





			using var reqContent = new MemoryStream(Encoding.UTF8.GetBytes(getreqContent(Method, out string info)));// сериализация тела запроса для метода





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


			Stream encryptedRequest = null;

			if (Method != "AddCommFile" && Method != "AddHWFile")
			{
				encryptedRequest = CryptHelper.Encrypt(CryptHelper.MasterPass, reqContent).Result;
			}
			else
			{
				encryptedRequest = CryptHelper.Encrypt(CryptHelper.MasterPass, content).Result;
			}



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

			if (!(Method == "UpdGroup"))
				return;////////

			Console.WriteLine($" ========================= ");


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
					Surname = rndname.Substring(7,3)

				};

				Console.WriteLine($" na me {newUserInfo.Name}");
				Console.WriteLine($" Surname {newUserInfo.Surname}");


				return JsonSerializer.Serialize(newUserInfo);
			}



			if (Method == "GetBD")//+ запрос от рута тут же
			{
				var newUserInfo = new User//Info
				{
					UserName = CommonConstants.SuperUser//BD.Users[1].UserName//CommonConstants.SuperUser
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


			//Удаляет вторую группу если есть
			if (Method == "DelGroup")//+
			{
				Guid gelguid = Guid.Empty;//

				if (BD.Groups.Count > 1)
				{
					gelguid = BD.Groups[1].ID;
				}

				var newGroupInfo = new Group
				{
					ID = gelguid,
				};

				return JsonSerializer.Serialize(newGroupInfo);
			}


			if (Method == "AddLesson")//+                               
			{

				info = BD.Groups[0].ID.ToString();

				string rndname = $"Lesson_{new Random().Next()}";

				var newLessonInfo = new Lesson//Info
				{
					TopicName = rndname,

					TestFlag = false,

					Date = DateTime.Now,

				};

				return JsonSerializer.Serialize(newLessonInfo);
			}



			//Удаляет последнее занятие в первой группе
			if (Method == "DelLesson")// +                         
			{
				info = BD.Groups[0].ID.ToString();

				var newLessonInfo = new Lesson
				{
					ID= BD.Groups[0].Lessons[BD.Groups[0].Lessons.Count-1].ID,
				};

				return JsonSerializer.Serialize(newLessonInfo);
			}



			//Перевод второго юзера в учителя
			if (Method == "Promote")//+
			{
				Guid gelguid = Guid.Empty;//

				//if (BD.Users.Count > 1)
				//{
					gelguid = BD.Users[1].ID;
				//}

				var newUserInfo = new User
				{
					ID = gelguid,
				};

				return JsonSerializer.Serialize(newUserInfo);
			}

			//Перевод второго юзера в простого юзера
			if (Method == "Demote")//+
			{
				Guid gelguid = Guid.Empty;//

				//if (BD.Users.Count > 1)
				//{
					gelguid = BD.Users[1].ID;
				//}

				var newUserInfo = new User
				{
					ID = gelguid,
				};

				return JsonSerializer.Serialize(newUserInfo);
			}



			//Добавление в первую группу и первый урок
			if (Method == "AddCommFile")//+
			{

                var FileInfo = new FileProcessorInfo
				{
                    userID = BD.Users[0].ID,
                    groupID = BD.Groups[0].ID,
					lessonID = BD.Groups[0].Lessons[0].ID,

					filename =  $"metanit_{new Random().Next()}.zip",

				};

				info = JsonSerializer.Serialize(FileInfo);

				return JsonSerializer.Serialize(info);
            }





			//Добавление в первую группу и первый урок
			if (Method == "AddHWFile")//+
			{
				var FileInfo = new FileProcessorInfo
				{
					userID = BD.Users[0].ID,
					groupID = BD.Groups[0].ID,
					lessonID = BD.Groups[0].Lessons[0].ID,

					filename = $"metanit_{new Random().Next()}.zip",
				};

				info = JsonSerializer.Serialize(FileInfo);

				return JsonSerializer.Serialize(info);
			}



			//меняет имя второму пользователю и даёт управление над первой группой
			if (Method == "UpdUser")//+
			{

				string rndname = $"upd_User_{new Random().Next()}";


				List<Guid> ruledGroups1 = new List<Guid>();
				ruledGroups1.Add(BD.Groups[0].ID);

				var newUserInfo = new User//Info
				{
					ID = BD.Users[1].ID,
					// UserName = rndname,
					//PasswordHash = "SomePass",//AuthHelper.HashPassword("SomePass"),

					Name = rndname.Substring(0, 7),
					Surname = rndname.Substring(7, 3),

					ruledGroups= ruledGroups1,
				};
				return JsonSerializer.Serialize(newUserInfo);
			}

			//меняет имя группы и добавляет третьего пользователя в ней
			if (Method == "UpdGroup")//+
			{
				string rndname = $"newGroup_{new Random().Next()}";

				List<string> usersList = new List<string>();
				usersList.Add(BD.Users[2].UserName);

				var newGroupInfo = new Group//Info
				{
					ID = BD.Groups[0].ID,
					GroupName = rndname,
					UserLogins=usersList
				};

				return JsonSerializer.Serialize(newGroupInfo);
			}








			var defaultUserInfo = new User
			{
				UserName = $"User_{new Random().Next()}",
				PasswordHash = AuthHelper.HashPassword("SomePass"),
			};

			return JsonSerializer.Serialize(defaultUserInfo);
		}
	}
}
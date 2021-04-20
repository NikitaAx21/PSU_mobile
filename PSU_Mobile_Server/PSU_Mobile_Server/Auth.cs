using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;

namespace PSU_Mobile_Server
{
	internal class Auth
	{
		private readonly string _filePath = $"{AppDomain.CurrentDomain.BaseDirectory}//DB.dat";
		private const string _pass = "5T5WQ}n~djZTBf#ZSAK3DGKhz";

		private DataBase BD;



		private static readonly object Lock = new object();

		private Timer _timer;

		private Auth()
		{
			var database = Load();
			_timer = new Timer(TimeSpan.FromMinutes(1).TotalMilliseconds)
			{
				AutoReset = true,
				Enabled = true
			};
			_timer.Elapsed += (s, e) => Save();

			if (database != null)
			{
				if (database.Users != null && database.Users.Any())
				{
					BD = database;
					return;
				}

				BD.Users = new List<User>();
			}
			else
			{
				BD = new DataBase
				{
					Users = new List<User>(),
					Groups =new List<Group>(),
					Files = new List<string>()
				};
			}

			var superUser = new User
			{
				PasswordHash = AuthHelper.HashPassword(CommonConstants.SuperPass),
				UserName = CommonConstants.SuperUser,
				PermittedCommands = ApiControllersInitializer.Instance.RequestToControllersDictionary.Keys.ToList()
			};
			BD.Users.Add(superUser);
			Task.Run(Save);
		}

		public static Lazy<Auth> Instance { get; } = new Lazy<Auth>(new Auth());

		public bool IsAuthorized(User user)
		{
			lock (Lock)
			{
				var usersWithSameLogin = BD.Users.Where(u => u.UserName == user.UserName);
				if (usersWithSameLogin.Count() != 1)
					return false;

				var userProfile = usersWithSameLogin.First();
				if (userProfile.PasswordHash != user.PasswordHash)
					return false;

				return true;
			}
		}

		public bool HasUserPermission(User user, string apiAction)// проверка ?
		{
			lock (Lock)
			{
				var usersWithSameLogin = BD.Users.Where(u => u.UserName == user.UserName);
				if (usersWithSameLogin.Count() != 1)
					return false;

				var userProfile = usersWithSameLogin.First();
				if (userProfile.PasswordHash != user.PasswordHash)
					return false;

				return userProfile.PermittedCommands.Select(p => p.ToLowerInvariant()).
					Contains(apiAction.ToLowerInvariant());
			}
		}


		//==============================================================
		public bool TryAddUser(User newUser)
		{
			lock (Lock)
			{
				if (BD.Users.Select(u => u.UserName).Contains(newUser.UserName))
					return false;

				if (string.IsNullOrEmpty(newUser.PasswordHash))
					return false;

				BD.Users.Add(new User
				{
					UserName = newUser.UserName,
					PasswordHash = newUser.PasswordHash,

					Name = newUser.Name,
					Surname = newUser.Surname,

					// Надо ли
					PermittedCommands = newUser.PermittedCommands,//??

					ruledGroups = newUser.ruledGroups//??
				});

				return true;
			}
		}

		public bool TryUpdUser(User user)
		{
			lock (Lock)
			{

				var userForUpd = BD.Users.FirstOrDefault(u => u.ID == user.ID);

				if (userForUpd == null)
					return false;

				userForUpd.Name = user.Name;
				userForUpd.Surname = user.Surname;
				userForUpd.PermittedCommands = user.PermittedCommands;
				userForUpd.ruledGroups = user.ruledGroups;
				userForUpd.Name = user.Name;

				return true;
			}
		}

		public bool TryGetUser(Guid ID,out User user)
		{
			lock (Lock)
			{
				user = null;

				var userForDeleting = BD.Users.FirstOrDefault(u => u.ID == ID);
				if (userForDeleting == null)
					return false;

				user = userForDeleting;
				return true;
			}
		}

		public bool TryDeleteUser(Guid ID)//
		{
			lock (Lock)
			{
				var userForDeleting = BD.Users.FirstOrDefault(u => u.ID == ID);
				if (userForDeleting == null)
					return false;

				BD.Users.Remove(userForDeleting);
				return true;
			}
		}

		//==============================================================

		public bool TryAddGroup(Group newGroup)
		{
			lock (Lock)
			{
				if (BD.Groups.Select(u => u.GroupName).Contains(newGroup.GroupName))
					return false;


				if (BD.Groups.Select(u => u.ID).Contains(newGroup.ID))
					return false;


				BD.Groups.Add(new Group
				{
					GroupName = newGroup.GroupName,
					Lessons = newGroup.Lessons,
					UserLogins = newGroup.UserLogins,
					CommonFilesLinks = newGroup.CommonFilesLinks
				});

				return true;
			}
		}

		public bool TryUpdGroup(Group group)
		{
			lock (Lock)
			{
				if (!BD.Groups.Select(u => u.ID).Contains(group.ID))
					return false;

				var groupForUpd = BD.Groups.FirstOrDefault(u => u.ID == group.ID);

				if (groupForUpd == null)
					return false;
				
				groupForUpd.GroupName = group.GroupName;
				groupForUpd.Lessons = group.Lessons;//??
				groupForUpd.UserLogins = group.UserLogins;
				groupForUpd.CommonFilesLinks = group.CommonFilesLinks;

				return true;
			}
		}

		public bool TryGetGroup(Guid ID, out Group group)//
		{
			lock (Lock)
			{
				group = new Group();

				var newgroup = BD.Groups.FirstOrDefault(u => u.ID == ID);
				if (newgroup == null)
					return false;

				group = newgroup;
				return true;
			}
		}

		public bool TryDeleteGroup(Guid ID)//??
		{
			lock (Lock)
			{
				var userForDeleting = BD.Groups.FirstOrDefault(u => u.ID == ID);
				if (userForDeleting == null)
					return false;

				BD.Groups.Remove(userForDeleting);
				return true;
			}
		}

		//==============================================================



		public bool TryAddLesson(Guid currentGroup, Lesson newLesson)
		{
			lock (Lock)
			{
				var groupForLesson = BD.Groups.FirstOrDefault(u => u.ID == currentGroup);

				if (groupForLesson == null)
					return false;

				if (groupForLesson.Lessons.Select(u => u.TopicName/**/).Contains(newLesson.TopicName))
					return false;


				groupForLesson.Lessons.Add(new Lesson
				{
					TopicName = newLesson.TopicName,
					TestFlag = newLesson.TestFlag,
					Date = newLesson.Date,
					LessonFilesLinks = newLesson.LessonFilesLinks,
					HomeWorkFilesLinks = newLesson.HomeWorkFilesLinks,// ??
					Presence = newLesson.Presence
				});

				return true;
			}
		}

		public bool TryUpdLesson(Guid currentGroup, Lesson newLesson)
		{
			lock (Lock)
			{
				var groupForLesson = BD.Groups.FirstOrDefault(u => u.ID == currentGroup);

				if (groupForLesson == null)
					return false;


				var lessonForUpd = groupForLesson.Lessons.FirstOrDefault(u => u.ID == currentGroup);

				if (lessonForUpd == null)
					return false;

				lessonForUpd.TopicName = newLesson.TopicName;
				lessonForUpd.TestFlag = newLesson.TestFlag;
				lessonForUpd.Date = newLesson.Date;
				lessonForUpd.LessonFilesLinks = newLesson.LessonFilesLinks;//??
				lessonForUpd.HomeWorkFilesLinks = newLesson.HomeWorkFilesLinks;//??
				lessonForUpd.Presence = newLesson.Presence;

				return true;
			}
		}

		public bool TryGetLesson(Guid gID,Guid lID, out Lesson newLesson)//
		{
			lock (Lock)
			{
				newLesson = new Lesson();

				var groupForLesson = BD.Groups.FirstOrDefault(u => u.ID == gID);

				if (groupForLesson == null)
					return false;


				var lessonToGet = groupForLesson.Lessons.FirstOrDefault(u => u.ID == lID);

				if (lessonToGet == null)
					return false;


				newLesson = lessonToGet;
				return true;
			}
		}

		public bool TryDeleteLesson(Guid currentGroup, Guid ID)//??
		{
			lock (Lock)
			{
				var groupForLesson = BD.Groups.FirstOrDefault(u => u.ID == currentGroup);

				if (groupForLesson == null)
					return false;


				var lessonForDeleting = groupForLesson.Lessons.FirstOrDefault(u => u.ID == ID);

				if (lessonForDeleting == null)
					return false;


				groupForLesson.Lessons.Remove(lessonForDeleting);
				return true;
			}
		}

		//==============================================================


		public bool TryGetCommFilePath(FileProcessorInfo paramInfo, out string newPath)//
		{
			lock (Lock)//второй лок
			{
				newPath="";



				var user = BD.Users.FirstOrDefault(u => u.ID == paramInfo.userID);

				if (user == null)
					return false;



				if (!user.ruledGroups.Contains(paramInfo.groupID))
					return false;




				var group = BD.Groups.FirstOrDefault(u => u.ID == paramInfo.groupID);

				if (group == null)
					return false;

				newPath = group.GroupName+"/";//??


				var lesson = group.Lessons.FirstOrDefault(u => u.ID == paramInfo.lessonID);

				if (paramInfo.lessonID != Guid.Empty)//??
				{
					if (lesson != null)
						newPath += lesson.TopicName + "/";
				}

				newPath += "/common/"+ paramInfo.filename;


				if (paramInfo.lessonID != Guid.Empty)//??
				{
					if (lesson.LessonFilesLinks.Contains(newPath))
						return false;

					lesson.LessonFilesLinks.Add(newPath);
				}
				else
				{
					if (group.CommonFilesLinks.Contains(newPath))
						return false;

					group.CommonFilesLinks.Add(newPath);
				}

				return true;
			}
		}


		public bool TryGetHWFilePath(FileProcessorInfo paramInfo, out string newPath)//
		{
			lock (Lock)//второй лок
			{
				newPath = "";

				var user = BD.Users.FirstOrDefault(u => u.ID == paramInfo.userID);

				if (user == null)
					return false;


				var group = BD.Groups.FirstOrDefault(u => u.ID == paramInfo.groupID);

				if (group == null)
					return false;


				var lesson = group.Lessons.FirstOrDefault(u => u.ID == paramInfo.lessonID);

				if (lesson == null)
					return false;



				newPath += group.GroupName + "/"+lesson.TopicName +"/"+ user.UserName + "/common/";

				newPath += paramInfo.filename;


				if (lesson.LessonFilesLinks.Contains(newPath))
					return false;


				/*перенос в процессор?	добавление в группы и занятия*/
				lesson.LessonFilesLinks.Add(newPath);

				return true;
			}
		}

		



		///<summary>Проверка "логического" наличия файла</summary>
		public bool FileExistanceCheck(FileProcessorInfo paramInfo/*, out Stream fileStream*/)//
		{
			lock (Lock)//
			{
				//paramInfo.fileName = путь


				var user = BD.Users.FirstOrDefault(u => u.ID == paramInfo.userID);

				if (user == null)
					return false;


				var group = BD.Groups.FirstOrDefault(u => u.ID == paramInfo.groupID);

				if (group == null)
					return false;


				var lesson = group.Lessons.FirstOrDefault(u => u.ID == paramInfo.lessonID);

				if (lesson == null)
					return false;


				if (paramInfo.filename.Contains("/common/") && paramInfo.filename.Contains(group.GroupName))//GroupName
				{
					
					if (group.CommonFilesLinks.Contains(paramInfo.filename))
						return false;
					
					//fileStream чёт присвоить??

					return true;
				}



				if ((paramInfo.filename.Contains(user.UserName) || paramInfo.filename.Contains("/common/"))&& paramInfo.filename.Contains(lesson.TopicName) && paramInfo.filename.Contains(group.GroupName))//
				{

					if (lesson.LessonFilesLinks.Contains(paramInfo.filename))
						return false;


					//fileStream чёт присвоить??

					return true;
				}

				return false;
			}

		}









		//public bool TryAddCommFile(FileProcessorInfo paramInfo, out string newPath)//
		//{
		//	lock (Lock)
		//	{
		//		if (!TryGetFilePath(paramInfo, out var filePath))
		//			return false;

		//		newPath = filePath;


		//		var user = BD.Users.FirstOrDefault(u => u.ID == paramInfo.userID);

		//		if (user == null)
		//			return false;



		//		if (!user.ruledGroups.Contains(paramInfo.groupID))
		//			return false;


		//		var group = BD.Groups.FirstOrDefault(u => u.ID == paramInfo.groupID);

		//		if (group == null)
		//			return false;

		//		var lesson = group.Lessons.FirstOrDefault(u => u.ID == paramInfo.lessonID);


		//		if (lessonID != Guid.Empty)//??
		//		{
		//			if (lesson.LessonFilesLinks.Contains(newPath))
		//				return false;

		//			lesson.LessonFilesLinks.add(newPath);
		//		}
		//		else
		//		{
		//			if (group.CommonFilesLinks.Contains(newPath))
		//				return false;

		//			group.CommonFilesLinks.add(newPath);
		//		}

		//		return true;
		//	}




		//}





		//==============================================================


		private DataBase Load()
		{
			try
			{
				byte[] buffer;
				using (var ms = new MemoryStream())
				using (var f = new FileStream(_filePath,
					FileMode.OpenOrCreate))
				{
					f.CopyTo(ms);
					ms.Position = 0;
					buffer = ms.ToArray();
				}

				var decryptedFile = CryptHelper.Decrypt(_pass, buffer).Result;
				var database = JsonSerializer.Deserialize<DataBase>(decryptedFile);

				return database;
			}
			catch (Exception e)
			{
				LogManager.WriteError(e, "BD loading failed");
				return null;
			}
		}

		public void Save()
		{
			try
			{
				using var f = new FileStream(_filePath, FileMode.Create);
				string buffer;
				lock (Lock)
				{
					buffer = JsonSerializer.Serialize(BD);
				}

				var encryptedFile = CryptHelper.Encrypt(_pass, buffer).Result;
				f.Write(encryptedFile);
			}
			catch (Exception e)
			{
				LogManager.WriteError(e, "BD saving failed");
			}
		}
	}
}

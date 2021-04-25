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
					Groups =new List<Group>()//,
					//Files = new List<string>()
				};
			}

			List<string> tmpList = ApiControllersInitializer.Instance.RequestToControllersDictionary.Keys.ToList();
			var superUser = new User
			{
				PasswordHash = AuthHelper.HashPassword(CommonConstants.SuperPass),
				UserName = CommonConstants.SuperUser,
				PermittedCommands = tmpList// ApiControllersInitializer.Instance.RequestToControllersDictionary.Keys.ToList()
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

		public bool TryGetBD(string UserName, out DataBase tmpDB)//
		{
			lock (Lock)
			{
				Console.WriteLine("TryGetBD");
				tmpDB = new DataBase();


				var thisUser = BD.Users.FirstOrDefault(u => u.UserName == UserName);
				if (thisUser == null)
					return false;

				Console.WriteLine("know thisUser");

				//============================
				if (thisUser.UserName != CommonConstants.SuperUser)
				{

					List<Group> tmpGroupsList = new List<Group>();
					List<User> tmpUserList = new List<User>();
					tmpUserList.Add(thisUser);
					Console.WriteLine(tmpUserList.Count);


					Console.WriteLine($"  thisUser.ruledGroups  {thisUser.ruledGroups.Count}");
					foreach (Guid groupID in thisUser.ruledGroups)
					{
						var newgroup = BD.Groups.FirstOrDefault(u => u.ID == groupID);
						if (newgroup == null)
							continue;//return false; ??

						Console.WriteLine($"newgroup  {newgroup.GroupName}");

						tmpGroupsList.Add(newgroup);
					}

					//=============================

					if (thisUser.ruledGroups.Count == 0)// если не руководит, то участвует
					{
						foreach (Group group in BD.Groups)
						{
							var foundname = group.UserLogins.FirstOrDefault(u => u == UserName);
							if ((foundname == null) || (foundname != UserName))
								continue;


							Console.WriteLine($"group  {group.GroupName}");

							tmpGroupsList.Add(group);
						}
					}

					tmpDB.Groups = tmpGroupsList;

					Console.WriteLine("Groups done");

					//=============================

					foreach (Group group in tmpDB.Groups)// добавлять только нужную инфу о юзерах??
					{
						foreach (string userlogin in group.UserLogins)
						{

							Console.WriteLine($"userlogin  {userlogin}");


							var tmpuser = BD.Users.FirstOrDefault(u => u.UserName == userlogin);
							if ((tmpuser == null))
								continue;


							// обнулять PasswordHash PermittedCommands ruledGroups

							var userInfo = new User
							{
								ID = tmpuser.ID,
								UserName = tmpuser.UserName,
								Name = tmpuser.Name,
								Surname = tmpuser.Surname
							};


							tmpUserList.Add(userInfo);
						}
					}

					tmpDB.Users = tmpUserList;
				}
				else
				{

					tmpDB = new DataBase()
					{
						Users = BD.Users,
						Groups = BD.Groups
					};
				}

				return true;
			}
		}


//==============================================================

		public bool TryGetFullBD(string UserName, out DataBase tmpDB)//
		{
			lock (Lock)
			{
				tmpDB = new DataBase();
				var thisUser = BD.Users.FirstOrDefault(u => u.UserName == UserName);
				if (thisUser == null)
					return false;

				/*
				 
					Доп проверка рута?
				 
				 */

				tmpDB = new DataBase()
				{
					Users = BD.Users,
					Groups = BD.Groups
				};	
				return true;
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
					PasswordHash = AuthHelper.HashPassword(newUser.PasswordHash),//newUser.PasswordHash,

					Name = newUser.Name,
					Surname = newUser.Surname,
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

				if(user.PasswordHash!=null)
					userForUpd.PasswordHash = AuthHelper.HashPassword(user.PasswordHash);// хэш ли?

				if (user.Name != null)
					userForUpd.Name = user.Name;

				if (user.Surname != null)
					userForUpd.Surname = user.Surname;

				if (user.ruledGroups.Count!=0)
					userForUpd.ruledGroups = user.ruledGroups;

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



		public bool TryToPromote(Guid ID)
		{
			lock (Lock)
			{
				if (ID==Guid.Empty)
					return false;

				var userForPromoting= BD.Users.FirstOrDefault(u => u.ID == ID);
				if (userForPromoting == null)
					return false;

				if(userForPromoting.PermittedCommands.Contains("AddCommFile"))
					return false;

				userForPromoting.PermittedCommands = AddTeacherPermissions();

				return true;
			}
		}








		public bool TryToDemote(Guid ID)
		{
			lock (Lock)
			{
				if (ID == Guid.Empty)
					return false;

				var userForDemoting = BD.Users.FirstOrDefault(u => u.ID == ID);
				if (userForDemoting == null)
					return false;

				if (!userForDemoting.PermittedCommands.Contains("AddCommFile"))
					return false;

				userForDemoting.PermittedCommands = AddBasicPermissions();

				return true;
			}
		}




		public List<string> AddBasicPermissions()
		{
			lock (Lock)
			{
				List<string> Commands = new List<string>();

				Commands.Add("GetBD");

				Commands.Add("AddHWFile");

				Commands.Add("GetFile");

				return Commands;
			}
		}



		public List<string> AddTeacherPermissions()
		{
			lock (Lock)
			{
				List<string> Commands = new List<string>();

				Commands.AddRange(AddBasicPermissions());

				Commands.Add("AddCommFile");

				Commands.Add("AddLesson");

				Commands.Add("DelLesson");

				Commands.Add("GetGroup");//--

				Commands.Add("GetLesson");//--

				Commands.Add("UpdGroup");

				Commands.Add("UpdLesson");


				return Commands;
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

				if(group.GroupName!=null)
					groupForUpd.GroupName = group.GroupName;

				if (group.UserLogins.Count != 0)
					groupForUpd.UserLogins = group.UserLogins;

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
				Console.WriteLine($" groupForLesson {groupForLesson.GroupName} ");

				Console.WriteLine($" newLesson TopicName {newLesson.TopicName} ");

				if (groupForLesson == null)
					return false;

				if (groupForLesson.Lessons.Select(u => u.TopicName/**/).Contains(newLesson.TopicName))
					return false;


				groupForLesson.Lessons.Add(new Lesson
				{
					TopicName = newLesson.TopicName,
					TestFlag = newLesson.TestFlag,
					Date = newLesson.Date,
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
				//lessonForUpd.LessonFilesLinks = newLesson.LessonFilesLinks;//??
				//lessonForUpd.HomeWorkFilesLinks = newLesson.HomeWorkFilesLinks;//??
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
			lock (Lock)
			{
				newPath="";



				var user = BD.Users.FirstOrDefault(u => u.ID == paramInfo.userID);

				if (user == null)
					return false;



				if ((!user.ruledGroups.Contains(paramInfo.groupID)) && (user.UserName!=CommonConstants.SuperUser))
					return false;




				var group = BD.Groups.FirstOrDefault(u => u.ID == paramInfo.groupID);

				if (group == null)
					return false;

				newPath = group.GroupName + "/";//??


				var lesson = group.Lessons.FirstOrDefault(u => u.ID == paramInfo.lessonID);

				if (paramInfo.lessonID != Guid.Empty)//??
				{
					if (lesson != null)
						newPath += lesson.TopicName + "/";
				}

				newPath += "common/";


				if (paramInfo.lessonID != Guid.Empty)//??
				{
					if (lesson.LessonFilesLinks.Contains(newPath + paramInfo.filename))
						return false;

					lesson.LessonFilesLinks.Add(newPath + paramInfo.filename);
				}
				else
				{
					if (group.CommonFilesLinks.Contains(newPath+ paramInfo.filename))
						return false;

					group.CommonFilesLinks.Add(newPath + paramInfo.filename);
				}

				return true;
			}
		}


		public bool TryGetHWFilePath(FileProcessorInfo paramInfo, out string newPath)//проверка на принадлежность группе...
		{
			lock (Lock)
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


				newPath += group.GroupName + "/"+lesson.TopicName +"/"+ user.UserName + "/";


				if (lesson.LessonFilesLinks.Contains(newPath+ paramInfo.filename))
					return false;


				/*перенос в процессор?	добавление в группы и занятия*/
				lesson.LessonFilesLinks.Add(newPath+ paramInfo.filename);

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
				using var f = new FileStream(_filePath, FileMode.OpenOrCreate);
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

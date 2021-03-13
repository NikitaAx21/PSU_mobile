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
		private readonly string _filePath = $"{AppDomain.CurrentDomain.BaseDirectory}//users.dat";
		private const string _pass = "5T5WQ}n~djZTBf#ZSAK3DGKhz";
		private List<User> _users;
		private static readonly object Lock = new object();

		private Timer _timer;

		private Auth()
		{
			var users = Load();
			_timer = new Timer(TimeSpan.FromMinutes(1).TotalMilliseconds)
			{
				AutoReset = true,
				Enabled = true
			};
			_timer.Elapsed += (s, e) => Save();

			if (users != null && users.Any())
			{
				_users = users;
				return;
			}

			var superUser = new User
			{
				PasswordHash = AuthHelper.HashPassword(CommonConstants.SuperPass),
				UserName = CommonConstants.SuperUser,
				PermittedCommands = ApiControllersInitializer.Instance.RequestToControllersDictionary.Keys.ToList()
			};
			_users = new List<User> { superUser };
			Task.Run(Save);
		}

		public static Lazy<Auth> Instance { get; } = new Lazy<Auth>(new Auth());

		public bool IsAuthorized(UserInfo user)
		{
			lock (Lock)
			{
				var usersWithSameLogin = _users.Where(u => u.UserName == user.UserName).ToList();
				if (usersWithSameLogin.Count != 1)
					return false;

				var userProfile = usersWithSameLogin.First();
				if (userProfile.PasswordHash != user.PasswordHash)
					return false;

				return true;
			}
		}

		public bool HasUserPermission(UserInfo user, string apiAction)
		{
			lock (Lock)
			{
				var usersWithSameLogin = _users.Where(u => u.UserName == user.UserName).ToList();
				if (usersWithSameLogin.Count != 1)
					return false;

				var userProfile = usersWithSameLogin.First();
				if (userProfile.PasswordHash != user.PasswordHash)
					return false;

				return userProfile.PermittedCommands.Select(p => p.ToLowerInvariant()).
					Contains(apiAction.ToLowerInvariant());
			}
		}

		public bool TryAddUser(UserInfo newUser)
		{
			lock (Lock)
			{
				if (_users.Select(u => u.UserName).Contains(newUser.UserName))
					return false;

				if (string.IsNullOrEmpty(newUser.PasswordHash))
					return false;

				_users.Add(new User
				{
					UserName = newUser.UserName,
					PasswordHash = newUser.PasswordHash
				});

				return true;
			}
		}

		public bool TryDeleteUser(string userName)
		{
			lock (Lock)
			{
				if (_users.Select(u => u.UserName).Contains(userName))
					return false;

				var userForDeleting = _users.FirstOrDefault(u => u.UserName == userName);
				if (userForDeleting == null)
					return false;

				_users.Remove(userForDeleting);
				return true;
			}
		}

		private List<User> Load()
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
				var users = JsonSerializer.Deserialize<List<User>>(decryptedFile);

				return users;
			}
			catch (Exception e)
			{
				LogManager.WriteError(e, "Users loading failed");
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
					buffer = JsonSerializer.Serialize(_users);
				}

				var encryptedFile = CryptHelper.Encrypt(_pass, buffer).Result;
				f.Write(encryptedFile);
			}
			catch (Exception e)
			{
				LogManager.WriteError(e, "Users saving failed");
			}
		}
	}
}

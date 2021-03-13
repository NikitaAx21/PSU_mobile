using System;
using System.Security.Cryptography;
using System.Text;

namespace Common
{
	public static class AuthHelper
	{
		public static string HashPassword(string password)
		{
			var bytes = Encoding.UTF8.GetBytes($"{password}{CommonConstants.PassSalt}");
			var hash = SHA512.HashData(bytes);
			var passwordHash = Convert.ToBase64String(hash);
			return passwordHash;
		}
	}
}

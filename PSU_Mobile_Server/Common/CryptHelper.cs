using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
	public static class CryptHelper
	{
		public static string MasterPass = "0H;#4c,lt`F! ?: 46ZI* J1ZfGXi:d38";

		public static async Task<byte[]> Encrypt(string password, string content)
		{
			var bytes = Encoding.UTF8.GetBytes($"{password}");
			var hash = SHA256.HashData(bytes);

			byte[] encryptedBytes;
			using (var aes = Aes.Create())
			{
				aes.Key = hash;

				var cryptoTransform = aes.CreateEncryptor(aes.Key, aes.IV);
				await using var memoryStream = new MemoryStream();
				await using var cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write);
				await using (var streamWriter = new StreamWriter(cryptoStream))
				{
					await streamWriter.WriteAsync(content);
				}
				var encryptedPacket = aes.IV.ToList();
				encryptedPacket.AddRange(memoryStream.ToArray());
				encryptedBytes = encryptedPacket.ToArray();
			}

			return encryptedBytes;
		}

		public static async Task<string> EncryptAndBase64(string password, string content)
		{
			var encryptedBytes = await Encrypt(password, content);
			var encryptedString = Convert.ToBase64String(encryptedBytes);
			return encryptedString;
		}

		public static async Task<string> Decrypt(string password, byte[] encryptedBytes)
		{
			var bytes = Encoding.UTF8.GetBytes($"{password}");
			var hash = SHA256.HashData(bytes);

			string str;
			using (var aes = Aes.Create())
			{
				aes.IV = encryptedBytes.Take(aes.IV.Length).ToArray();
				encryptedBytes = encryptedBytes.Skip(aes.IV.Length).ToArray();
				aes.Key = hash;

				var cryptoTransform = aes.CreateDecryptor(aes.Key, aes.IV);
				await using var memoryStream = new MemoryStream(encryptedBytes);
				await using var cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Read);
				using var streamReader = new StreamReader(cryptoStream);
				str = await streamReader.ReadToEndAsync();
			}
			return str;
		}

		public static async Task<string> DecryptBased64(string password, string encryptedString)
		{
			var encryptedBytes = Convert.FromBase64String(encryptedString);
			var decryptedString = await Decrypt(password, encryptedBytes);
			return decryptedString;
		}
	}
}

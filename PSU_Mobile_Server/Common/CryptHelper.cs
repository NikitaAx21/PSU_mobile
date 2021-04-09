using System;
using System.Buffers.Text;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
	public static class CryptHelper
	{
		public const string MasterPass = "0H;#4c,lt`F! ?: 46ZI* J1ZfGXi:d38";

		public static async Task<byte[]> Encrypt(string password, string content)
		{
			var bytes = Encoding.UTF8.GetBytes($"{password}");
			var hash = SHA256.HashData(bytes);

			byte[] encryptedBytes;
			using (var aes = Aes.Create())
			{
				aes.Key = hash;
				aes.Mode = CipherMode.CBC;
				aes.Padding = PaddingMode.PKCS7;

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

		public static async Task<Stream> Encrypt(string password, Stream content)
		{
			var bytes = Encoding.UTF8.GetBytes($"{password}");
			var hash = SHA256.HashData(bytes);

			var resultStream = new MemoryStream();
			using (var aes = Aes.Create())
			{
				aes.Key = hash;
				aes.Mode = CipherMode.CBC;
				aes.Padding = PaddingMode.PKCS7;

				using var cryptoTransform = aes.CreateEncryptor(aes.Key, aes.IV);
				await using var encryptedStream = new MemoryStream();
				await using var cryptoStream = new CryptoStream(encryptedStream, cryptoTransform, CryptoStreamMode.Write);
				await content.CopyToAsync(cryptoStream);
				await content.DisposeAsync();
				await cryptoStream.FlushFinalBlockAsync();
				encryptedStream.Position = 0;

				await resultStream.WriteAsync(aes.IV);
				await encryptedStream.CopyToAsync(resultStream);
			}

			resultStream.Position = 0;
			return resultStream;
		}

		public static async Task<string> EncryptAndBase64(string password, string content)
		{
			var encryptedBytes = await Encrypt(password, content);
			var encryptedString = Convert.ToBase64String(encryptedBytes);
			return encryptedString;
		}

		public static async Task<Stream> EncryptAndBase64(string password, Stream content)
		{
			await using var encryptedBytes = await Encrypt(password, content);
			using var transformer = new ToBase64Transform();
			var destination = new MemoryStream();
			await using var ms = new MemoryStream();
			await using var cryptoStream = new CryptoStream(ms, transformer, CryptoStreamMode.Write);
			await encryptedBytes.CopyToAsync(cryptoStream);
			await cryptoStream.FlushFinalBlockAsync();

			ms.Position = 0;
			await ms.CopyToAsync(destination);
			destination.Position = 0;
			return destination;
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
				aes.Mode = CipherMode.CBC;
				aes.Padding = PaddingMode.PKCS7;

				var cryptoTransform = aes.CreateDecryptor(aes.Key, aes.IV);
				await using var memoryStream = new MemoryStream(encryptedBytes);
				await using var cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Read);
				using var streamReader = new StreamReader(cryptoStream);
				str = await streamReader.ReadToEndAsync();
			}
			return str;
		}

		public static async Task<Stream> Decrypt(string password, Stream inputStream)
		{
			var passwordBytes = Encoding.UTF8.GetBytes($"{password}");
			var hash = SHA256.HashData(passwordBytes);

			var destination = new MemoryStream();
			using (var aes = Aes.Create())
			{
				var iv = new byte[aes.IV.Length];
				await inputStream.ReadAsync(iv.AsMemory(0, aes.IV.Length));
				aes.IV = iv;
				aes.Key = hash;
				aes.Mode = CipherMode.CBC;
				aes.Padding = PaddingMode.PKCS7;

				await using var encryptedStream = new MemoryStream();
				await inputStream.CopyToAsync(encryptedStream);
				await inputStream.DisposeAsync();
				encryptedStream.Position = 0;

				using var cryptoTransform = aes.CreateDecryptor(aes.Key, aes.IV);
				await using var cryptoStream = new CryptoStream(encryptedStream, cryptoTransform, CryptoStreamMode.Read);
				await cryptoStream.CopyToAsync(destination);
			}

			destination.Position = 0;
			return destination;
		}

		public static async Task<string> DecryptBased64(string password, string encryptedString)
		{
			var encryptedBytes = Convert.FromBase64String(encryptedString);
			var decryptedString = await Decrypt(password, encryptedBytes);
			return decryptedString;
		}

		public static async Task<Stream> DecryptBased64(string password, Stream inputStream)
		{
			await using var based64Stream = new MemoryStream();
			await inputStream.CopyToAsync(based64Stream);
			await inputStream.DisposeAsync();
			based64Stream.Position = 0;

			using var transformer = new FromBase64Transform();
			await using var encryptedStream = new MemoryStream();
			await using var cryptoStream = new CryptoStream(based64Stream, transformer, CryptoStreamMode.Read);
			await cryptoStream.CopyToAsync(encryptedStream);
			encryptedStream.Position = 0;

			var resultStream = await Decrypt(password, encryptedStream);
			return resultStream;
		}
	}
}

using System;
using System.IO;
using System.Text;

namespace Common
{
	public static class LogManager
	{
		private const string Path = @".\Log";
		private static readonly DateTime ProgramsStartDateTimeLocal = DateTime.Now;

		public static async void WriteError(Exception e, string additionInfo)
		{
			var fileName = $"ERROR_{ProgramsStartDateTimeLocal:yyyy-MM-dd_hh-mm-ss}.txt";
			var errorString = $"ERROR\n{additionInfo}\n{e?.Message}\nStackTrace: {e?.StackTrace}";

			Console.Write(errorString);
			try
			{
				Directory.CreateDirectory(Path);
			}
			catch
			{
				//Ignore;
			}

			try
			{
				await using var s = new FileStream($"{Path}\\{fileName}", FileMode.Append);
				var errorBytes = Encoding.UTF8.GetBytes(errorString);
				var buffer = errorBytes.AsMemory(0, errorBytes.Length);
				await s.WriteAsync(buffer);
			}
			catch
			{
				//Ignore;
			}
		}
	}
}
using Common;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;

namespace PSU_Mobile_Client
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			try
			{
				Work();
			}
			catch (Exception ex)
			{
				LogManager.WriteError(ex, "Client crashed!");
			}
		}

		private static void Work()
		{
			var handler = new HttpClientHandler();
			var baseAddress = new Uri(CommonConstants.Uri);
			var client = new HttpClient(handler);

			var request = new StringContent("Group1", CommonConstants.StandardEncoding);
			request.Headers.Add("Method", "AddGroup");
			var response = client.PostAsync(baseAddress, request).Result;
			var result = CommonConstants.StandardEncoding.GetString(response.Content.ReadAsByteArrayAsync().Result);
			Console.WriteLine(result);
			
			while (true)
			{
				//TODO просто костыль, чтоб не закрывалось окно и не терялся контекст
			}
		}
	}
}

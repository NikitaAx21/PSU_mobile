namespace PSU_Mobile_Server
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var server = new MobileServer();
			server.Start();
		}
	}
}

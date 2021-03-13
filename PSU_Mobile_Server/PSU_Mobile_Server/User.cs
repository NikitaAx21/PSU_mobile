using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PSU_Mobile_Server
{
	public class User
	{
		[JsonPropertyName("Login")]
		public string UserName { get; set; }

		[JsonPropertyName("PassHash")]
		public string PasswordHash { get; set; }

		[JsonPropertyName("Permissions")]
		public List<string> PermittedCommands { get; set; }
	}
}

using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;

namespace Common
{
	public class User
	{

		public User()
		{
			ID = Guid.NewGuid();

			ruledGroups = new List<Guid>();

			PermittedCommands = new List<string>();

			PermittedCommands.Add("GetBD");

			PermittedCommands.Add("AddHWFile");

			PermittedCommands.Add("GetFile");
		}

		[JsonPropertyName("ID")]
		public Guid ID { get; set; }


		[JsonPropertyName("Name")]
		public string Name { get; set; }

		[JsonPropertyName("Surname")]
		public string Surname { get; set; }



		[JsonPropertyName("Login")]
		public string UserName { get; set; }

		[JsonPropertyName("PassHash")]
		public string PasswordHash { get; set; }



		[JsonPropertyName("Permissions")]
		public List<string> PermittedCommands { get; set; }

		[JsonPropertyName("ruledGroups")]// ??
		public List<Guid> ruledGroups { get; set; }

	}
}

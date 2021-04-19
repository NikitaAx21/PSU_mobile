using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PSU_Mobile_Server
{
	public class User1//закинут в Common для работы с тестовым клиентом
	{

		public User1()
		{
			ID = Guid.NewGuid();
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
		public List<Guid/*string*/> ruledGroups { get; set; }

	}
}

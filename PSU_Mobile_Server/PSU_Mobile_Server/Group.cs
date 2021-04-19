using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PSU_Mobile_Server
{
    public class Group
    {
		public Group()
		{
			ID = Guid.NewGuid();
		}


		[JsonPropertyName("ID")]
		public Guid ID { get; set; }


		[JsonPropertyName("Name")]
		public string GroupName { get; set; }

		[JsonPropertyName("Lessons")]
		public List<Lesson> Lessons { get; set; }

		[JsonPropertyName("UserLogins")]
		public List<string> UserLogins { get; set; }

		[JsonPropertyName("CommonFilesLinks")]// ?? список ссылок на общие материалы
		public List<string> CommonFilesLinks { get; set; }

	}
}

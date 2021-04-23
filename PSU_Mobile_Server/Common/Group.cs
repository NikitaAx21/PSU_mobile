using System.Collections.Generic;
using System.Text.Json.Serialization;

using System;


namespace Common
{
	public class Group//Добавить преподавателя?
	{
		public Group()
		{
			ID = Guid.NewGuid();

			Lessons = new List<Lesson>();

			UserLogins = new List<string>();

			CommonFilesLinks = new List<string>();
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

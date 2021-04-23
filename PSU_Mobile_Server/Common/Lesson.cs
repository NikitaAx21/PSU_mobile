using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;


namespace Common
{
	public class Lesson
    {

		public Lesson()
		{
			ID = Guid.NewGuid();

			TestFlag = false;

			LessonFilesLinks = new List<string>();

			HomeWorkFilesLinks = new List<string>();

			Presence = new Dictionary<Guid/*пользователи*/, int?>();
		}


		[JsonPropertyName("ID")]
		public Guid ID { get; set; }

		[JsonPropertyName("Name")]
		public string TopicName { get; set; }


		[JsonPropertyName("TestFlag")]
		public bool TestFlag { get; set; }// ?? является ли занятие контрольной точкой

		[JsonPropertyName("Date")]
		public DateTime Date { get; set; }


		[JsonPropertyName("LessonFilesLinks")]// ?? список ссылок на материалы данного занятия
		public List<string> LessonFilesLinks { get; set; }


		[JsonPropertyName("HomeWorkFilesLinks")]// ?? список ссылок на  домашние задания/готовые работы
		public List<string> HomeWorkFilesLinks { get; set; }


		[JsonPropertyName("Presence")]
		public Dictionary<Guid/*пользователи*/, int?> Presence { get; set; }// определение присутствия и оценок

	}
}

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;


namespace PSU_Mobile_Server
{
	public class Lesson
    {

		public Lesson()
		{
			ID = Guid.NewGuid();
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
		public Dictionary<string, int?> Presence { get; set; }// определение присутствия и оценок

	}
}

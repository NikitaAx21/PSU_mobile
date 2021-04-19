using System;
using System.IO;
using System.Net;
using System.Text.Json;
using Common;
using System.Text;

namespace PSU_Mobile_Server.Controllers
{

	internal class GetLessonProcessor : BaseApiController
	{
		public GetLessonProcessor() : base("GetLesson")
		{

		}

		public override (HttpStatusCode, Stream) ProcessRequest(byte[] contentInfo, Stream requestContent)
		{
			try
			{
				Guid currentGroup = new Guid(contentInfo);// !!! группа в которую добавляем

				var lessonInfo = JsonSerializer.DeserializeAsync<Lesson>(requestContent).Result.ID;

				var templesson = new Lesson();
				var isLessonObtained = Auth.Instance.Value.TryGetLesson(currentGroup, lessonInfo, out templesson);
				var statusCode = isLessonObtained ? HttpStatusCode.Created : HttpStatusCode.InternalServerError;


				var serializedResp = new MemoryStream(CommonConstants.StandardEncoding.GetBytes(JsonSerializer.Serialize(templesson)));

				return (statusCode, serializedResp/**/);
			}
			catch (Exception)
			{
				return (HttpStatusCode.InternalServerError, Stream.Null);
			}
		}
	}




}

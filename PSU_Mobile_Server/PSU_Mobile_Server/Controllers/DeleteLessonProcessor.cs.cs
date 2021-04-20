using System;
using System.IO;
using System.Net;
using System.Text.Json;
using Common;

namespace PSU_Mobile_Server.Controllers
{
	internal class DeleteLessonProcessor : BaseApiController
	{
		public DeleteLessonProcessor() : base("DelLesson")
		{

		}

		public override (HttpStatusCode, Stream) ProcessRequest(byte[] contentInfo, Stream requestContent)
		{
			try
			{
				Guid currentGroup = new Guid(contentInfo);// !!! группа в которую добавляем


				var lessonID = JsonSerializer.DeserializeAsync<Lesson>(requestContent).Result.ID;// ID !!

				var isLessonDeleted = Auth.Instance.Value.TryDeleteLesson(currentGroup, lessonID);


				var statusCode = isLessonDeleted ? HttpStatusCode.Created/*?*/ : HttpStatusCode.InternalServerError;
				return (statusCode, Stream.Null);// ответ ??
			}
			catch (Exception)
			{
				return (HttpStatusCode.InternalServerError, Stream.Null);
			}
		}
	}


}

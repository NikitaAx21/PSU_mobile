using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Text.Json;
using Common;

namespace PSU_Mobile_Server.Controllers
{
	internal class UpdLessonProcessor : BaseApiController
	{
		public UpdLessonProcessor() : base("UpdLesson")
		{

		}

		public override (HttpStatusCode, Stream) ProcessRequest(byte[] contentInfo, Stream requestContent)
		{
			try
			{
				Guid currentGroup = new Guid(contentInfo);// !!! группа в которую добавляем

				var lessonInfo = JsonSerializer.DeserializeAsync<Lesson>(requestContent).Result;
				var isLessonUpdated = Auth.Instance.Value.TryUpdLesson(currentGroup,lessonInfo);


				var statusCode = isLessonUpdated ? HttpStatusCode.Created : HttpStatusCode.InternalServerError;
				return (statusCode, Stream.Null);
			}
			catch (Exception)
			{
				return (HttpStatusCode.InternalServerError, Stream.Null);
			}
		}
	}

}

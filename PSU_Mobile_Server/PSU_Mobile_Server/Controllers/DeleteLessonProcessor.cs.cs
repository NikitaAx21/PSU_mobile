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

				var asd = CryptHelper.Decrypt(CryptHelper.MasterPass, contentInfo/*.*/).Result;

				Guid currentGroup = Guid.Parse(asd);




				var lessonID = JsonSerializer.DeserializeAsync<Lesson>(requestContent).Result.ID;// ID !!

				var isLessonDeleted = Auth.Instance.Value.TryDeleteLesson(currentGroup, lessonID);


				var statusCode = isLessonDeleted ? HttpStatusCode.OK/*?*/ : HttpStatusCode.InternalServerError;
				return (statusCode, Stream.Null);
			}
			catch (Exception)
			{
				return (HttpStatusCode.InternalServerError, Stream.Null);
			}
		}
	}


}

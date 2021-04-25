using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Text.Json;
using Common;
using System.Text;

namespace PSU_Mobile_Server.Controllers
{
	internal class AddLessonProcessor : BaseApiController
	{
		public AddLessonProcessor() : base("AddLesson")
		{

		}

		public override (HttpStatusCode, Stream) ProcessRequest(byte[] contentInfo, Stream requestContent)
		{
			try
			{
				var asd = CryptHelper.Decrypt(CryptHelper.MasterPass, contentInfo/*string.Empty*/).Result;

				Guid currentGroup = Guid.Parse(asd);// !!! группа в которую добавляем

				var lessonInfo = JsonSerializer.DeserializeAsync<Lesson>(requestContent).Result;
				var isLessonCreated = Auth.Instance.Value.TryAddLesson(currentGroup,lessonInfo);

				var statusCode = isLessonCreated ? HttpStatusCode.Created : HttpStatusCode.InternalServerError;
				return (statusCode, Stream.Null);
			}
			catch (Exception)
			{
				return (HttpStatusCode.InternalServerError, Stream.Null);
			}

			//return base.ProcessRequest(contentInfo, requestContent);
		}
	}



}

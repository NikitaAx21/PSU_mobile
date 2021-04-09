using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace PSU_Mobile_Server.Controllers
{
	internal class AddGroupProcessor : BaseApiController
	{
		public AddGroupProcessor() : base("AddGroup")
		{

		}

		public override (HttpStatusCode, Stream) ProcessRequest(byte[] contentInfo, Stream requestContent)
		{
			return base.ProcessRequest(contentInfo, requestContent);
		}
	}
}

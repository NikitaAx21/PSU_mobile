using System.Text.Json.Serialization;

namespace Common
{
	public class Request
	{
		[JsonPropertyName("User")]
		public UserInfo UserInfo { get; set; }

		[JsonPropertyName("Content")]
		public string RequestContent { get; set; }
		
		[JsonPropertyName("Method")]
		public string ApiMethod { get; set; }
	}
}

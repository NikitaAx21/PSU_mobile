using System.Text.Json.Serialization;

namespace Common
{
	public class UserInfo
	{
		[JsonPropertyName("Login")]
		public string UserName { get; set; }

		[JsonPropertyName("PassHash")]
		public string PasswordHash { get; set; }
	}
}

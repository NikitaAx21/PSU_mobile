using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Common
{
	public class Group
	{
		[JsonPropertyName("Name")]
		public string GroupName { get; set; }
	}
}

﻿using System.Text.Json.Serialization;

namespace Common
{
	public class Request
	{
		[JsonPropertyName("User")]
		public User/*Info*/ UserInfo { get; set; }

		[JsonPropertyName("ContentInfo")]
		public byte[] ContentInfo { get; set; }
		
		[JsonPropertyName("Method")]
		public string ApiMethod { get; set; }
	}
}

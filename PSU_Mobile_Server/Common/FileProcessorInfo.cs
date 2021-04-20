using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Common
{
    public class FileProcessorInfo
    {

        public FileProcessorInfo()
        {
            lessonID = Guid.Empty;
        }


        [JsonPropertyName("userID")]
        public Guid userID { get; set; }


        [JsonPropertyName("groupID")]
        public Guid groupID { get; set; }


        [JsonPropertyName("lessonID")]
        public Guid lessonID { get; set; }


        [JsonPropertyName("filename")]
        public string filename { get; set; }






    }
}

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PSU_Mobile_Server
{
    public class DataBase
    {
        [JsonPropertyName("Users")]
        public List<User> Users { get; set; }


        [JsonPropertyName("Groups")]
        public List<Group> Groups { get; set; }


        [JsonPropertyName("Files")]
        public List<string> Files { get; set; }// все файлы
    }
}

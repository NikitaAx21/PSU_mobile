using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Common
{
    public class DataBase
    {


        public DataBase()
        {
            Users = new List<User>();

            Groups = new List<Group>();
        }


        [JsonPropertyName("Users")]
        public List<User> Users { get; set; }


        [JsonPropertyName("Groups")]
        public List<Group> Groups { get; set; }


        //[JsonPropertyName("Files")]
        //public List<string> Files { get; set; }// все файлы
    }
}

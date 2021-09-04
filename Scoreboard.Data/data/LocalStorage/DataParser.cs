﻿using Newtonsoft.Json;

namespace Scoreboard.Data.data.LocalStorage
{
    public static class DataParser
    {
        public static T ConstructItemDatabase<T>(string fileContents)
        {
            return JsonConvert.DeserializeObject<T>(fileContents);
        }

        // to serialize (save) database to a file
        public static string SaveListToJsonString<T>(T database)
        {
            return JsonConvert.SerializeObject(database);
        }
    }
}
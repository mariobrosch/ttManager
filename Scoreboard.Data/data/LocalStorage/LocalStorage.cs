﻿using System.Configuration;
using System.IO;
using Scoreboard.Data.enums;

namespace Scoreboard.Data.data.LocalStorage
{
    class LocalStorage
    {
        private static readonly string localStoragePath = ConfigurationManager.AppSettings["localStoragePath"];

        internal static string MakeRequest(HttpMethods method, string table, string key, string filter, string data)
        {

            var filePath = Path.Combine(localStoragePath, table + ".json");
            if (!File.Exists(filePath))
            {
                if (!Directory.Exists(localStoragePath))
                {
                    Directory.CreateDirectory(localStoragePath);
                }
                File.WriteAllText(filePath, "[]");
            }
            var fileContent = File.ReadAllText(filePath);

            switch (method)
            {
                case HttpMethods.GET:
                    return Get.Perform(fileContent, table, key, filter);
                case HttpMethods.POST:
                    string newId;
                    var newPostContent = Create.Perform(fileContent, table, data, out newId);
                    File.WriteAllText(filePath, newPostContent);
                    return newId;
                case HttpMethods.PUT:
                    string isUpdated;
                    var newPutContent = Update.Perform(fileContent, table, key, data, out isUpdated);
                    File.WriteAllText(filePath, newPutContent);
                    return isUpdated;
                case HttpMethods.DELETE:
                    string removeCount;
                    var newDeleteContent = Delete.Perform(fileContent, table, key, filter, out removeCount);
                    File.WriteAllText(filePath, newDeleteContent);
                    return removeCount;
            }
            return "";
        }
    }
}
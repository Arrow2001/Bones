using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Bones.Currency.UserData
{
    public static class DataStorage
    {
        public static void SaveUserAccounts(IEnumerable<UserAccount> accounts, string filepath)
        {
            string json = JsonConvert.SerializeObject(accounts, Formatting.Indented);
            File.WriteAllText(filepath, json);
        }

        public static IEnumerable<UserAccount> LoadUserAccounts(string filepath)
        {
            if (!File.Exists(filepath)) return null;
            string json = File.ReadAllText(filepath);
            return JsonConvert.DeserializeObject<List<UserAccount>>(json);
        }

        public static bool SaveExists(string filePath) => File.Exists(filePath);
    }
}

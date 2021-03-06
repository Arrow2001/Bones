using System.Collections.Generic;
using System.Linq;
using Discord.WebSocket;

namespace Bones.Currency.UserData
{
    public class UserAccounts
    {
        private static List<UserAccount> accounts;
        private static string accountsFile = "Resources/user_data.json";

        static UserAccounts()
        {
            if (DataStorage.SaveExists(accountsFile))
                accounts = DataStorage.LoadUserAccounts(accountsFile).ToList();
            else
            {
                accounts = new List<UserAccount>();
                SaveAccounts();
            }
        }

        public static void SaveAccounts() => DataStorage.SaveUserAccounts(accounts, accountsFile);
        public static UserAccount GetAccount(SocketUser user) => GetOrCreateUserAccount(user.Id);

        private static UserAccount GetOrCreateUserAccount(ulong id)
        {
            var result = from a in accounts
                         where a.UserID == id
                         select a;
            var account = result.FirstOrDefault();
            if (account == null) account = CreateNewAccount(id);
            return account;
        }

        private static UserAccount CreateNewAccount(ulong id)
        {
            var newAccount = new UserAccount
            {
                UserID = id,
                Bones = 0,
                lastFmUsername = "not set",
                favEpisodes = "",
                DailyClaimed = false
            };
            accounts.Add(newAccount);
            SaveAccounts();
            return newAccount;
        }
    }
}

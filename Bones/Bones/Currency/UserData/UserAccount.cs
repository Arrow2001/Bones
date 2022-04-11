using System;
using System.Collections.Generic;
using System.Text;

namespace Bones.Currency.UserData
{
    public class UserAccount
    {
        public ulong UserID { get; set; }
        public int Bones { get; set; }
        public string lastFmUsername { get; set; }
        public string favEpisodes { get; set;}
        public bool DailyClaimed { get; set; }
    }

    
}

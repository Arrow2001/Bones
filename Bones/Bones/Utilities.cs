using System;
using Discord;
using System.IO;
using System.Net;
using System.Drawing;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace Bones
{
    class Utilities
    {
        public static readonly Random getrandom = new Random();
        public static int GetRandomNumber(int min, int max)
        {
            lock (getrandom) { return getrandom.Next(min, max); }
        }
    }
}

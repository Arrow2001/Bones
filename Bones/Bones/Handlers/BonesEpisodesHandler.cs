using Bones.Commands;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Bones.Handlers
{
    class BonesEpisodesHandler
    {
        public static BonesEps BonesEps;

        public static void BoneSetup()
        {
                BonesEps = JsonConvert.DeserializeObject<BonesEps>(System.IO.File.ReadAllText(@"C:\Users\IainN\Desktop\Bones\Bones\Bones\Commands\bones_episodes.json"));
        }

    }
   
    public partial class BonesEps
    {
        [JsonProperty("Episodes")]
        public BonesEpisode[] Episodes { get; set; }
    }

    public class BonesEpisode
    {
        [JsonProperty("title")]
        public string episodeTitle { get; set; }

        [JsonProperty("synopsis")]
        public string synopsis { get; set; }

        [JsonProperty("airdate")]
        public string airdate { get; set; }

        [JsonProperty("director")]
        public string director { get; set; }

        [JsonProperty("image")]
        public string image { get; set; }
    }
}

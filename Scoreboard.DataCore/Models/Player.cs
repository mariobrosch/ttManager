﻿using Newtonsoft.Json;
using Scoreboard.DataCore.Data.Requests;
using System.Drawing;
using System.Linq;

namespace Scoreboard.DataCore.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhotoUrl { get; set; }
        public string Photo { get; set; }
        public string Enabled { get; set; }
        [JsonIgnore]
        public bool IsEnabled
        {
            get
            {
                return Enabled == "1";
            }
            set
            {
                Enabled = value ? "1" : "0";
            }
        }
        public string Removed { get; set; }
        [JsonIgnore]
        public bool IsRemoved
        {
            get
            {
                return Removed == "1";
            }
            set
            {
                Removed = value ? "1" : "0";
            }
        }
        [JsonIgnore]
        public int PlayedGames
        {
            get
            {
                return MatchData.GetForPlayer(Id).Count();
            }
        }
    }
}

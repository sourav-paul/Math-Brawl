using System;
using System.Collections.Generic;

namespace MathBrawlServer
{
    public class Payload
    {
        public string Type { get; set; }  // "connect" / "level" / ""
        public string Client { get; set; } // "dashboard" / "leaderboard" / "player"
        
        public string Status { get; set; } // "lobby" / "playing" / 
        
        public Guid PlayerId { get; set; }
        public string PlayerName { get; set; }
        public Guid OpponentId { get; set; }
        
        public LevelGenerator.Level Level { get; set; }
        public Dashboard Dashboard { get; set; }
        public Dictionary<string, int> Leaderboard { get; set; }
    }

    public class Dashboard
    {
        public bool AllowDivision { get; set; }
        public int MinRange { get; set; }
        public int MaxRange { get; set; }
    }
}
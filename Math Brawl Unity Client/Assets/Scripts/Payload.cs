
using System;
using System.Collections.Generic;
using UnityEngine;

public class Payload
{
    public string Type { get; set; }  // "connect" / "level" / "user-creation"
    public string Client { get; set; } // "dashboard" / "leaderboard" / "player"
        
    public string Status { get; set; } // "room" / "playing" / 
        
    public Guid PlayerId { get; set; }
    public Guid RoomId { get; set; }
    public string PlayerName { get; set; }
    public Guid OpponentId { get; set; }
    public int Score { get; set; }
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
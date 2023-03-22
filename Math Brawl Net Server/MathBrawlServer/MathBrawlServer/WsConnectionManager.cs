using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MathBrawlServer
{
    public class WsConnectionManager
    {
        private ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();

        public ConcurrentDictionary<Guid, ConcurrentBag<Payload>> rooms =
            new ConcurrentDictionary<Guid, ConcurrentBag<Payload>>();

        public string AddSocket(WebSocket socket)
        {
            string ConnID = Guid.NewGuid().ToString();
            _sockets.TryAdd(ConnID, socket);
            Console.WriteLine("WebSocketServerConnectionManager-> AddSocket: WebSocket added with ID: " + ConnID);
            return ConnID;
        }

        public void AddPlayerToRoom(Payload user)
        {
            if (rooms.Count > 0)
            {
                foreach (var room in rooms)
                {
                    if (room.Value.Count == 1)
                    {
                        // add player here
                        room.Value.Add(user);
                        Console.WriteLine("adding player to an existing room!");
                        
                        // assign opponent id to each other
                        room.Value.ElementAt(0).OpponentId = new Guid(room.Value.ElementAt(1).PlayerId.ToString());
                        room.Value.ElementAt(1).OpponentId = new Guid(room.Value.ElementAt(0).PlayerId.ToString());
                        // start a new game between these two player
                        RouteGameStartAsync(room.Value.ToList());
                        
                        break;
                    }
                    else
                    {
                        // create new room and add player there
                        var newRoom = new KeyValuePair<Guid, ConcurrentBag<Payload>> (Guid.NewGuid(), new ConcurrentBag<Payload>(){user});
                        rooms.TryAdd(newRoom.Key, newRoom.Value);
                        Console.WriteLine("adding player to a new  room!");
                        
                        // send this user to the room and keep on waiting
                        
                        
                        break;
                    }
                }
            }
            else
            {
                // create first room and add player there
                var newRoom = new KeyValuePair<Guid, ConcurrentBag<Payload>> (Guid.NewGuid(), new ConcurrentBag<Payload>(){user});
                rooms.TryAdd(newRoom.Key, newRoom.Value);
                Console.WriteLine("adding player to the first  room!");
            }
        }
        
        public async Task RouteGameStartAsync(List<Payload> payloads)
        {
            var Level = LevelGenerator.GenerateIntegers(3);
            
            foreach (var payload in payloads)
            {
                payload.Type = "game-data";
                payload.Status = "playing";
                payload.Level =Level;
            
                if (!string.IsNullOrEmpty(payload.PlayerId.ToString()))
                {
                    Console.WriteLine("Targeted");
                    var sock = GetAllSockets().FirstOrDefault(s => s.Key == payload.PlayerId.ToString());
                    if (sock.Value != null)
                    {
                        if (sock.Value.State == WebSocketState.Open)
                            await sock.Value.SendAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload)), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    else
                    {
                        Console.WriteLine("Invalid Recipient");
                    }
                }
            }
        }
        
        public ConcurrentDictionary<string, WebSocket> GetAllSockets()
        {
            return _sockets;
        }

        public void SendNextLevel(Payload payload)
        {
            foreach (var room in rooms)
            {
                foreach (var player in room.Value)
                {
                    if (player.PlayerId==payload.PlayerId)
                    {
                        player.Score = payload.Score;
                        Console.WriteLine("sending new level to players in a room!");
                        SendScoresToLeaderboard(room.Value.ToList());
                        RouteGameStartAsync(room.Value.ToList());
                        break;
                    }
                }
            }
        }

        public string LeaderboardId = String.Empty;
        
        private async void SendScoresToLeaderboard(List<Payload> payloads)
        {
            foreach (var payload in payloads)
            {
                if (!string.IsNullOrEmpty(payload.PlayerId.ToString()))
                {
                    Console.WriteLine("Targeted");
                    var sock = GetAllSockets().FirstOrDefault(s => s.Key == LeaderboardId);
                    if (sock.Value != null)
                    {
                        KeyValuePair<string, int> score =
                            new KeyValuePair<string, int>(payload.PlayerName, payload.Score);
                        
                        if (sock.Value.State == WebSocketState.Open)
                        {
                            await sock.Value.SendAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(score)), WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                            
                    }
                    else
                    {
                        Console.WriteLine("Invalid Recipient");
                    }
                }
            }
        }
    }
}
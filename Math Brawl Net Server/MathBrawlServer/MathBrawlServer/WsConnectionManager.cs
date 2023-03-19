using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace MathBrawlServer
{
    public class WsConnectionManager
    {
        private ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();

        public ConcurrentDictionary<Guid, ConcurrentBag<Guid>> rooms =
            new ConcurrentDictionary<Guid, ConcurrentBag<Guid>>();

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
                        room.Value.Add(user.PlayerId);
                        Console.WriteLine("adding player to an existing room!");
                        
                        // start a new game between these two player
                        // WsMiddleware.
                        
                        break;
                    }
                    else
                    {
                        // create new room and add player there
                        var newRoom = new KeyValuePair<Guid, ConcurrentBag<Guid>> (Guid.NewGuid(), new ConcurrentBag<Guid>(){user.PlayerId});
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
                var newRoom = new KeyValuePair<Guid, ConcurrentBag<Guid>> (Guid.NewGuid(), new ConcurrentBag<Guid>(){user.PlayerId});
                rooms.TryAdd(newRoom.Key, newRoom.Value);
                Console.WriteLine("adding player to the first  room!");
            }
        }
        
        public ConcurrentDictionary<string, WebSocket> GetAllSockets()
        {
            return _sockets;
        }
    }
}
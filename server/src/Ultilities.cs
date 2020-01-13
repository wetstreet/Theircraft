using System;
using System.Net;
using System.Net.Sockets;
using protocol.cs_theircraft;

namespace ChatRoomServer
{

    public struct Vector3
    {
        public float x;
        public float y;
        public float z;
        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    public static class Ultilities
    {
        public static Vector2Int GetChunk(CSVector3Int position)
        {
            int chunkX = (int)Math.Floor((float)position.x / 16);
            int chunkZ = (int)Math.Floor((float)position.z / 16);
            Vector2Int ret = new Vector2Int
            {
                x = chunkX,
                y = chunkZ
            };
            return ret;
        }

        public static string GetIpv4Address()
        {
            string hostname = Dns.GetHostName();
            IPAddress[] ipadrlist = Dns.GetHostAddresses(hostname);
            foreach (IPAddress ipa in ipadrlist)
            {
                if (ipa.AddressFamily == AddressFamily.InterNetwork)
                    return ipa.ToString();
            }
            return null;
        }

        public static void Print(string content, string sender = null)
        {
            if (sender == null)
            {
                Console.WriteLine($"[{DateTime.Now.ToString("t")}]{content}");
            }
            else
            {
                Console.WriteLine($"[{DateTime.Now.ToString("t")} {sender}]{content}");
            }
        }
    }
}

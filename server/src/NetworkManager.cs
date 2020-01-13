using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Threading;
using System.Net;
using protocol.cs_enum;

namespace ChatRoomServer
{
    public delegate void CallBackFunction(Player player, MemoryStream stream);

    class Package
    {
        public Socket socket;
        public ENUM_CMD type;
        public byte[] data;
    }

    public static class NetworkManager
    {
        static readonly int port = 8848;

        static Queue<Package> _message = new Queue<Package>();

        static readonly Dictionary<ENUM_CMD, CallBackFunction> _callbacks = new Dictionary<ENUM_CMD, CallBackFunction>();
        static BinaryFormatter formatter = new BinaryFormatter();

        public static void Start()
        {
            string ip = Ultilities.GetIpv4Address();
			//string ip = new WebClient().DownloadString("http://icanhazip.com");
			//ip = ip.Substring(0,ip.Length-1);//remove the \n character
            Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            try
            {
                _socket.Bind(endPoint);
            }
            catch (Exception ex)
            {
                Ultilities.Print("bind socket failed");
				Ultilities.Print(ex.ToString());
                return;
            }
            _socket.Listen(0);

            Ultilities.Print($"server started, ip = {ip}, port = {port}");

            Thread sendThread = new Thread(Send);
            sendThread.Start();

            while (true)
            {
                Socket client = _socket.Accept();

                Player player = new Player();
                player.socket = client;

                Ultilities.Print($"{player.socket.RemoteEndPoint} connected");

                Thread receiveThread = new Thread(Receive);
                receiveThread.Start(player);
            }
        }

        public static void Enqueue<T>(Socket socket, ENUM_CMD type, T obj)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(stream, obj);
                Package package = new Package
                {
                    socket = socket,
                    type = type,
                    data = stream.ToArray()
                };
                _message.Enqueue(package);
            }
        }

        static void Send()
        {
            while (true)
            {
                if (_message.Count > 0)
                {
                    Package package = _message.Dequeue();
                    List<byte> bytes = new List<byte>();
                    bytes.AddRange(BitConverter.GetBytes((ushort)package.type));
                    bytes.AddRange(BitConverter.GetBytes((uint)package.data.Length));
                    bytes.AddRange(package.data);
                    try
                    {
                        int num = package.socket.Send(bytes.ToArray());
                        if (package.type != ENUM_CMD.CS_PLAYER_MOVE_NOTIFY)
                            Ultilities.Print($"send mesage, num={num}, length={package.data.Length}, type={package.type}");
                    }
                    catch
                    {
                        Console.WriteLine("send failed");
                        continue;
                    }
                }
            }
        }

        static void PlayerDisconnect(Player player)
        {
            Ultilities.Print($"player {player.name} disconnected");
            player.socket.Close();
            GameLogic.RemovePlayer(player);
        }

        static void Receive(object obj)
        {
            Player player = obj as Player;

            while (true)
            {
                byte[] data = new byte[6];
                int receive = 0;
                try
                {
                    receive = player.socket.Receive(data);
                }
                catch
                {
                    PlayerDisconnect(player);
                    break;
                }

                if (receive > 0)
                {
                    MemoryStream stream = new MemoryStream(data);
                    BinaryReader binary = new BinaryReader(stream, Encoding.UTF8);
                    ENUM_CMD type = (ENUM_CMD)binary.ReadUInt16();
                    uint length = binary.ReadUInt32();

                    data = new byte[length];
                    player.socket.Receive(data);
                    stream = new MemoryStream(data);

                    if (type != ENUM_CMD.CS_HERO_MOVE_REQ)
                        Ultilities.Print($"recieved message, type={type}, length={length}");
                    if (_callbacks.ContainsKey(type))
                    {
                        CallBackFunction func = _callbacks[type];
                        func(player, stream);
                    }
                }
                else
                {
                    PlayerDisconnect(player);
                    break;
                }
            }
        }

        public static void Register(ENUM_CMD type, CallBackFunction func)
        {
            if (!_callbacks.ContainsKey(type))
            {
                _callbacks.Add(type, func);
            }
        }

        public static T Deserialize<T>(MemoryStream stream)
        {
            return ProtoBuf.Serializer.Deserialize<T>(stream);
        }
    }
}

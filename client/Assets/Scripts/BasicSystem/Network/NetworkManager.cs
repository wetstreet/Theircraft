using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using System.Threading;
using ProtoBuf;
using protocol.cs_enum;

public static class NetworkManager
{
    public static bool IsSingle;

    struct NetworkCallback
    {
        public Action<object> func;
        public bool isDelete;
    }

    class NetworkCoroutine : MonoBehaviour
    {
        public static NetworkCoroutine _instance;
        public static NetworkCoroutine Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject obj = new GameObject
                    {
                        name = "NetworkCoroutine"
                    };
                    DontDestroyOnLoad(obj);
                    _instance = obj.AddComponent<NetworkCoroutine>();
                }
                return _instance;
            }
        }

        void OnApplicationQuit()
        {
            Disconnect();
        }
    }

    public static bool connected;
    
    public static string ip;
    static readonly int port = 8848;

    static TcpClient tcpClient;

    static Queue<byte[]> _message;
    static Dictionary<ENUM_CMD, List<NetworkCallback>> callbackDict = new Dictionary<ENUM_CMD, List<NetworkCallback>>();

    static BinaryFormatter formatter = new BinaryFormatter();

    public static void Register(ENUM_CMD type, Action<object> _func)
    {
        if (!callbackDict.ContainsKey(type))
        {
            callbackDict[type] = new List<NetworkCallback>();
        }
        callbackDict[type].Add(new NetworkCallback { func = _func, isDelete = false });
    }

    static void Send()
    {
        while (connected)
        {
            if (_message.Count > 0)
            {
                byte[] data = _message.Dequeue();
                NetworkStream stream = tcpClient.GetStream();
                stream.Write(data, 0, data.Length);
            }
            else
                Thread.Sleep(100);
        }
    }

    static Queue<Package> packageQueue = new Queue<Package>();
    struct Package
    {
        public ENUM_CMD type;
        public byte[] data;
    }
    
    static IEnumerator HandlePackage()
    {
        while (connected)
        {
            lock (packageQueue)
            {
                if (packageQueue.Count > 0)
                {
                    Package package = packageQueue.Dequeue();
                    if (callbackDict.ContainsKey(package.type))
                    {
                        List<NetworkCallback> callbackList = callbackDict[package.type];

                        for (int i = callbackList.Count - 1; i >= 0; i--)
                        {
                            NetworkCallback callback = callbackList[i];
                            callback.func(package.data);
                            if (callback.isDelete)
                            {
                                callbackList.RemoveAt(i);
                            }
                        }
                    }
                }
            }
            yield return null;
        }
        DisconnectedUI.Show();
    }


    static void Receive()
    {
        while (connected)
        {
            NetworkStream stream = tcpClient.GetStream();
            
            byte[] data = new byte[6];
            IAsyncResult headerResult = stream.BeginRead(data, 0, data.Length, null, null);
            while (!headerResult.IsCompleted)
            {
                Thread.Sleep(1);
            }
            int readNum = 0;
            try
            {
                readNum = stream.EndRead(headerResult);
            }
            catch
            {
                Disconnect();
            }
            if (readNum > 0)
            {
                MemoryStream lengthStream = new MemoryStream(data);
                BinaryReader binary = new BinaryReader(lengthStream, Encoding.UTF8);
                ENUM_CMD type = (ENUM_CMD)binary.ReadUInt16();
                uint length = binary.ReadUInt32();
                
                MemoryStream bodyStream = new MemoryStream();
                int totalBytesRead = 0;
                byte[] bufdata;
                if (length > 1024)
                    bufdata = new byte[1024];
                else
                    bufdata = new byte[length];

                int tick1 = Environment.TickCount;


                do
                {
                    if (stream.DataAvailable)
                    {
                        int bytesRead = stream.Read(bufdata, 0, bufdata.Length);
                        totalBytesRead += bytesRead;
                        bodyStream.Write(bufdata, 0, bytesRead);
                    }
                    else
                        Thread.Sleep(1);

                } while (totalBytesRead < length);

                float s = (Environment.TickCount - tick1) / 1000f;
                //Debug.Log("time = " + s);

                if (type != ENUM_CMD.CS_PLAYER_MOVE_NOTIFY)
                    Debug.Log("receive message, type=" + type + ",length=" + length + ",totalBytesRead=" + totalBytesRead);

                Package package = new Package();
                package.type = type;
                package.data = bodyStream.ToArray();
                lock (packageQueue)
                {
                    packageQueue.Enqueue(package);
                }
            }
            else
            {
                Disconnect();
            }
        }
    }

    public static T Deserialize<T>(object data)
    {
        if (IsSingle)
        {
            return (T)data;
        }
        MemoryStream stream = new MemoryStream((byte[])data);
        return Serializer.Deserialize<T>(stream);
    }

    public static byte[] Serialize<T>(T obj)
    {
        MemoryStream stream = new MemoryStream();
        Serializer.Serialize(stream, obj);
        return stream.ToArray();
    }

    public static void SendPkgToServer<T>(ENUM_CMD cmdID, T obj, Action<object> callback = null)
    {
        //Debug.Log("SendPkgToServer,cmd=" + cmdID);
        if (IsSingle && LocalServer.ProcessRequest(cmdID, obj, callback))
        {
            return;
        }

        byte[] data = Serialize(obj);
        int length = data.Length;
        List<byte> bytes = new List<byte>();
        bytes.AddRange(BitConverter.GetBytes((ushort)cmdID));
        bytes.AddRange(BitConverter.GetBytes((uint)length));
        bytes.AddRange(data);
        _message.Enqueue(bytes.ToArray());

        if (callback != null)
        {
            if (!callbackDict.ContainsKey(cmdID))
            {
                callbackDict[cmdID] = new List<NetworkCallback>();
            }
            callbackDict[cmdID].Add(new NetworkCallback { func = callback, isDelete = true });
        }
    }

    public static bool Connect()
    {
        if (ip == null)
            return false;
        Debug.Log("connecting to server...");
        tcpClient = new TcpClient();
        try
        {
            tcpClient.Connect(ip, port);
        }
        catch(Exception ex)
        {
            Debug.Log("connection failed!");
            Debug.Log(ex.ToString());
            return false;
        }
        connected = true;
        Debug.Log("connection success!");

        _message = new Queue<byte[]>();

        new Thread(Send).Start();
        new Thread(Receive).Start();
        NetworkCoroutine.Instance.StartCoroutine(HandlePackage());
        return true;
    }

    public static void Disconnect()
    {
        if (tcpClient != null)
            tcpClient.Close();
        connected = false;
        Debug.Log("disconnected!");
    }

    public static void Notify(ENUM_CMD cmd, object data)
    {
        if (callbackDict.ContainsKey(cmd))
        {
            List<NetworkCallback> callbackList = callbackDict[cmd];

            for (int i = callbackList.Count - 1; i >= 0; i--)
            {
                NetworkCallback callback = callbackList[i];
                callback.func(data);
                if (callback.isDelete)
                {
                    callbackList.RemoveAt(i);
                }
            }
        }
    }
}

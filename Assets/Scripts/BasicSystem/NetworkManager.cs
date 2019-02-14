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

public delegate void CallbackFunction(byte[] data);

public static class NetworkManager
{
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
    static Dictionary<ENUM_CMD, CallbackFunction> _callback = new Dictionary<ENUM_CMD, CallbackFunction>();

    static BinaryFormatter formatter = new BinaryFormatter();

    public static void Register(ENUM_CMD type, CallbackFunction func)
    {
        if (!_callback.ContainsKey(type))
        {
            _callback[type] = func;
        }
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
        while (true)
        {
            lock (packageQueue)
            {
                if (packageQueue.Count > 0)
                {
                    Package package = packageQueue.Dequeue();
                    if (_callback.ContainsKey(package.type))
                    {
                        CallbackFunction func = _callback[package.type];
                        func(package.data);
                    }
                }
            }
            yield return null;
        }
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
                Disconnect(true);
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

                DateTime t1 = DateTime.Now;
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
                Debug.Log((DateTime.Now - t1).ToString());
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
                Disconnect(true);
            }
        }
    }

    public static T Deserialize<T>(byte[] data)
    {
        MemoryStream stream = new MemoryStream(data);
        return Serializer.Deserialize<T>(stream);
    }

    public static void Enqueue<T>(ENUM_CMD cmdID, T obj)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            Serializer.Serialize(ms, obj);
            byte[] data = ms.ToArray();
            int length = data.Length;
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes((ushort)cmdID));
            bytes.AddRange(BitConverter.GetBytes((uint)length));
            bytes.AddRange(data);
            _message.Enqueue(bytes.ToArray());
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

    public static void Disconnect(bool showUI = false)
    {
        if (tcpClient != null)
            tcpClient.Close();
        connected = false;
        Debug.Log("disconnected!");
        if (showUI)
        {
            DisconnectedUI.Show();
        }
    }
}

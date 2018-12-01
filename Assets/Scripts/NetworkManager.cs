using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Theircraft;
using UnityEngine;

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
    static Dictionary<CSMessageType, CallbackFunction> _callback = new Dictionary<CSMessageType, CallbackFunction>();

    static BinaryFormatter formatter = new BinaryFormatter();

    public static void Register(CSMessageType type, CallbackFunction func)
    {
        if (!_callback.ContainsKey(type))
        {
            _callback[type] = func;
        }
    }

    static IEnumerator Send()
    {
        while (connected)
        {
            if (_message.Count > 0)
            {
                byte[] data = _message.Dequeue();
                NetworkStream stream = tcpClient.GetStream();
                stream.Write(data, 0, data.Length);
            }
            yield return null;
        }
        yield return null;
    }

    static Queue<Package> packageQueue = new Queue<Package>();
    struct Package
    {
        public CSMessageType type;
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


    static IEnumerator Receive()
    {
        yield return null;
        while (connected)
        {
            NetworkStream stream = tcpClient.GetStream();
            
            byte[] data = new byte[6];
            IAsyncResult headerResult = stream.BeginRead(data, 0, data.Length, null, null);
            while (!headerResult.IsCompleted)
            {
                yield return null;
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
                CSMessageType type = (CSMessageType)binary.ReadUInt16();
                uint length = binary.ReadUInt32();
                
                MemoryStream bodyStream = new MemoryStream();
                Debug.Log("length=" + length);
                int totalBytesRead = 0;
                byte[] bufdata;
                if (length > 1024)
                    bufdata = new byte[1024];
                else
                    bufdata = new byte[length];
                do
                {
                    if (stream.DataAvailable)
                    {
                        int bytesRead = stream.Read(bufdata, 0, bufdata.Length);
                        totalBytesRead += bytesRead;
                        bodyStream.Write(bufdata, 0, bytesRead);
                    }
                    else
                        yield return new WaitForEndOfFrame();

                } while (totalBytesRead < length);
                Debug.Log("length=" + length + ",totalBytesRead=" + totalBytesRead);

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
            yield return null;
        }
    }

    public static T Deserialzie<T>(byte[] data)
    {
        MemoryStream stream = new MemoryStream(data);
        return (T)formatter.Deserialize(stream);
    }

    public static void Enqueue(CSMessageType type, object obj)
    {
        MemoryStream stream = new MemoryStream();
        formatter.Serialize(stream, obj);
        byte[] data = stream.ToArray();
        int length = data.Length;
        List<byte> bytes = new List<byte>();
        bytes.AddRange(BitConverter.GetBytes((ushort)type));
        bytes.AddRange(BitConverter.GetBytes((uint)length));
        bytes.AddRange(data);
        _message.Enqueue(bytes.ToArray());
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

        NetworkCoroutine.Instance.StartCoroutine(Send());
        NetworkCoroutine.Instance.StartCoroutine(Receive());
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

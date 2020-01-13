using System;
using StackExchange.Redis;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using protocol.cs_theircraft;
using System.Collections.Generic;

namespace ChatRoomServer
{
    [Serializable]
    public struct AccountData
    {
        public string account;
        public string password;
        public uint playerID;
        public string name;
    }

    public static class Redis
    {
        static BinaryFormatter formatter = new BinaryFormatter();

        static ConnectionMultiplexer redis;
        static IDatabase database { get { return redis.GetDatabase(); } }

        static readonly string KEY_PLAYER_IDNEX = "player_index";
        static readonly string KEY_ACCOUNT_DICT = "account_dict";
        static readonly string KEY_CHUNK_DICT = "chunk_dict";
        static readonly string KEY_PLAYER_DICT = "player_dict";

        public static void Init()
        {
            if (redis == null)
            {
                redis = ConnectionMultiplexer.Connect("127.0.0.1");
                Ultilities.Print("Redis Ready!");
            }
        }

        static void Set<Key,Value>(Key key, Value value)
        {
            MemoryStream keyStream = new MemoryStream();
            formatter.Serialize(keyStream, key);
            MemoryStream valueStream = new MemoryStream();
            formatter.Serialize(valueStream, value);
            string s = Convert.ToBase64String(valueStream.ToArray());
            database.StringSet(keyStream.ToArray(), s);
        }
        
        static bool Get<Key, Value>(Key key, out Value value)
        {
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, key);
            string s = database.StringGet(stream.ToArray());
            if (s == null)
            {
                value = default(Value);
                return false;
            }
            byte[] data = Convert.FromBase64String(s);
            MemoryStream valueStream = new MemoryStream(data);
            value = (Value)formatter.Deserialize(valueStream);
            return true;
        }

        public static void SetChunkData(Vector2Int chunkPos, List<CSBlock> blockList)
        {
            MemoryStream keyStream = new MemoryStream();
            formatter.Serialize(keyStream, chunkPos);

            MemoryStream valueStream = new MemoryStream();
            BlobChunkData chunkData = new BlobChunkData();
            chunkData.BlockList.AddRange(blockList);
            ProtoBuf.Serializer.Serialize(valueStream, chunkData);

            database.HashSet(KEY_CHUNK_DICT, keyStream.ToArray(), valueStream.ToArray());
        }

        public static bool GetChunkData(Vector2Int chunkPos, out List<CSBlock> blockList)
        {
            MemoryStream keyStream = new MemoryStream();
            formatter.Serialize(keyStream, chunkPos);

            byte[] data = database.HashGet(KEY_CHUNK_DICT, keyStream.ToArray());
            if (data != null)
            {
                MemoryStream valueStream = new MemoryStream(data);
                BlobChunkData chunkData = new BlobChunkData();
                chunkData = ProtoBuf.Serializer.Deserialize<BlobChunkData> (valueStream);
                blockList = chunkData.BlockList;
                return true;
            }
            else
            {
                blockList = null;
                return false;
            }
        }

        public static void SetPlayerData(uint playerID, CSPlayer playerData)
        {
            MemoryStream valueStream = new MemoryStream();
            ProtoBuf.Serializer.Serialize(valueStream, playerData);

            database.HashSet(KEY_PLAYER_DICT, playerID, valueStream.ToArray());
        }

        public static bool GetPlayerData(uint playerID, out CSPlayer playerData)
        {
            byte[] data = database.HashGet(KEY_PLAYER_DICT, playerID);
            if (data != null)
            {
                MemoryStream valueStream = new MemoryStream(data);
                playerData = ProtoBuf.Serializer.Deserialize<CSPlayer>(valueStream);
                return true;
            }
            else
            {
                playerData = null;
                return false;
            }
        }

        public static void SetAccountData(string account, AccountData accountData)
        {
            MemoryStream valueStream = new MemoryStream();
            formatter.Serialize(valueStream, accountData);

            database.HashSet(KEY_ACCOUNT_DICT, account, valueStream.ToArray());
        }

        public static bool GetAccountData(string account, out AccountData accountData)
        {
            byte[] data = database.HashGet(KEY_ACCOUNT_DICT, account);
            if (data != null)
            {
                MemoryStream valueStream = new MemoryStream(data);
                accountData = (AccountData)formatter.Deserialize(valueStream);
                return true;
            }
            else
            {
                accountData = new AccountData();
                return false;
            }
        }

        public static uint GetPlayerIndexAdd()
        {
            uint index = GetPlayerIndex();
            AddPlayerIndex();
            return index;
        }

        public static uint GetPlayerIndex()
        {
            bool b = Get(KEY_PLAYER_IDNEX, out uint playerIndex);
            if (!b)
            {
                playerIndex = 0;
                Set(KEY_PLAYER_IDNEX, playerIndex);
            }
            return playerIndex;
        }

        public static void AddPlayerIndex()
        {
            bool b = Get(KEY_PLAYER_IDNEX, out uint playerIndex);
            if (!b)
            {
                playerIndex = 0;
            }
            Set(KEY_PLAYER_IDNEX, playerIndex + 1);
        }
    }
}

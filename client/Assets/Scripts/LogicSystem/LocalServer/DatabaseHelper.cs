using protocol.cs_theircraft;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class DatabaseHelper
{
    public static BinaryFormatter bf = new BinaryFormatter();

    public static void Save(string prefKey, object serializableObject)
    {
        MemoryStream memoryStream = new MemoryStream();
        bf.Serialize(memoryStream, serializableObject);
        string tmp = Convert.ToBase64String(memoryStream.ToArray());
        PlayerPrefs.SetString(prefKey, tmp);
    }

    public static bool CanLoad(string key)
    {
        return PlayerPrefs.HasKey(key);
    }

    public static T Load<T>(string prefKey)
    {
        if (!PlayerPrefs.HasKey(prefKey))
            return default;

        string serializedData = PlayerPrefs.GetString(prefKey);
        MemoryStream dataStream = new MemoryStream(Convert.FromBase64String(serializedData));

        T deserializedObject = (T)bf.Deserialize(dataStream);

        return deserializedObject;
    }

    static readonly string KEY_GENERATE_FLAG = "KEY_GENERATE_FLAG";
    static readonly string KEY_CHUNK_DATA = "KEY_CHUNK_DATA";
    static readonly string KEY_DEPENDENCE = "KEY_DEPENDENCE";
    static readonly string KEY_ORIENTATION = "KEY_ORIENTATION";

    public static readonly string KEY_PLAYER_DATA = "KEY_PLAYER_DATA";

    public static void ClearAll()
    {
        PlayerPrefs.DeleteKey(KEY_GENERATE_FLAG);
        PlayerPrefs.DeleteKey(KEY_CHUNK_DATA);
        PlayerPrefs.DeleteKey(KEY_DEPENDENCE);
        PlayerPrefs.DeleteKey(KEY_ORIENTATION);
        PlayerPrefs.DeleteKey(KEY_PLAYER_DATA);
    }

    [Serializable]
    struct Vector2IntSerializable
    {
        int x;
        int y;

        public Vector2IntSerializable(Vector2Int v)
        {
            x = v.x;
            y = v.y;
        }

        public Vector2Int ToVector2Int()
        {
            return new Vector2Int(x, y);
        }
    }

    public static void SaveGenerateFlag(HashSet<Vector2Int> chunkGenerateFlagSet)
    {
        HashSet<Vector2IntSerializable> _chunkGenerateFlagSet = new HashSet<Vector2IntSerializable>();
        foreach (Vector2Int chunk in chunkGenerateFlagSet)
        {
            _chunkGenerateFlagSet.Add(new Vector2IntSerializable(chunk));
        }
        Save(KEY_GENERATE_FLAG, _chunkGenerateFlagSet);
    }

    public static HashSet<Vector2Int> LoadGenerateFlag()
    {
        HashSet<Vector2Int> chunkGenerateFlagSet = new HashSet<Vector2Int>();

        if (CanLoad(KEY_GENERATE_FLAG))
        {
            HashSet<Vector2IntSerializable> _chunkGenerateFlagSet = Load<HashSet<Vector2IntSerializable>>(KEY_GENERATE_FLAG);
            foreach (Vector2IntSerializable chunk in _chunkGenerateFlagSet)
            {
                chunkGenerateFlagSet.Add(chunk.ToVector2Int());
            }
        }

        return chunkGenerateFlagSet;
    }

    public static void SaveChunkData(Dictionary<Vector2Int, byte[]> chunkDataDict)
    {
        Dictionary<Vector2IntSerializable, byte[]> _chunkDataDict = new Dictionary<Vector2IntSerializable, byte[]>();
        foreach (KeyValuePair<Vector2Int, byte[]> keyValue in chunkDataDict)
        {
            _chunkDataDict.Add(new Vector2IntSerializable(keyValue.Key), keyValue.Value);
        }
        Save(KEY_CHUNK_DATA, _chunkDataDict);
    }

    public static Dictionary<Vector2Int, byte[]> LoadChunkData()
    {
        Dictionary<Vector2Int, byte[]> chunkDataDict = new Dictionary<Vector2Int, byte[]>();

        if (CanLoad(KEY_CHUNK_DATA))
        {
            Dictionary<Vector2IntSerializable, byte[]> _chunkDataDict = Load<Dictionary<Vector2IntSerializable, byte[]>>(KEY_CHUNK_DATA);
            foreach (KeyValuePair<Vector2IntSerializable, byte[]> keyValue in _chunkDataDict)
            {
                chunkDataDict.Add(keyValue.Key.ToVector2Int(), keyValue.Value);
            }
        }

        return chunkDataDict;
    }

    [Serializable]
    struct Vector3IntSerializable
    {
        int x;
        int y;
        int z;

        public Vector3IntSerializable(Vector3Int v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        public Vector3Int ToVector3Int()
        {
            return new Vector3Int(x, y, z);
        }
    }

    public static void SaveDependence(Dictionary<Vector3Int, Vector3Int> dependenceDict)
    {
        Dictionary<Vector3IntSerializable, Vector3IntSerializable> _dependenceDict = new Dictionary<Vector3IntSerializable, Vector3IntSerializable>();
        foreach (KeyValuePair<Vector3Int, Vector3Int> keyValue in dependenceDict)
        {
            _dependenceDict.Add(new Vector3IntSerializable(keyValue.Key), new Vector3IntSerializable(keyValue.Value));
        }
        Save(KEY_DEPENDENCE, _dependenceDict);
    }

    public static Dictionary<Vector3Int, Vector3Int> LoadDependence()
    {
        Dictionary<Vector3Int, Vector3Int> dependenceDict = new Dictionary<Vector3Int, Vector3Int>();

        if (CanLoad(KEY_DEPENDENCE))
        {
            Dictionary<Vector3IntSerializable, Vector3IntSerializable> _dependenceDict = Load<Dictionary<Vector3IntSerializable, Vector3IntSerializable>>(KEY_DEPENDENCE);
            foreach (KeyValuePair<Vector3IntSerializable, Vector3IntSerializable> keyValue in _dependenceDict)
            {
                dependenceDict.Add(keyValue.Key.ToVector3Int(), keyValue.Value.ToVector3Int());
            }
        }

        return dependenceDict;
    }

    public static void SaveOrientation(Dictionary<Vector3Int, CSBlockOrientation> orientationDict)
    {
        Dictionary<Vector3IntSerializable, CSBlockOrientation> _orientationDict = new Dictionary<Vector3IntSerializable, CSBlockOrientation>();
        foreach (KeyValuePair<Vector3Int, CSBlockOrientation> keyValue in orientationDict)
        {
            _orientationDict.Add(new Vector3IntSerializable(keyValue.Key), keyValue.Value);
        }
        Save(KEY_ORIENTATION, _orientationDict);
    }

    public static Dictionary<Vector3Int, CSBlockOrientation> LoadOrientation()
    {
        Dictionary<Vector3Int, CSBlockOrientation> orientationDict = new Dictionary<Vector3Int, CSBlockOrientation>();

        if (CanLoad(KEY_ORIENTATION))
        {
            Dictionary<Vector3IntSerializable, CSBlockOrientation> _orientationDict = Load<Dictionary<Vector3IntSerializable, CSBlockOrientation>>(KEY_ORIENTATION);
            foreach (KeyValuePair<Vector3IntSerializable, CSBlockOrientation> keyValue in _orientationDict)
            {
                orientationDict.Add(keyValue.Key.ToVector3Int(), keyValue.Value);
            }
        }

        return orientationDict;
    }
}

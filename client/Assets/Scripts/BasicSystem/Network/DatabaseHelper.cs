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
        string tmp = System.Convert.ToBase64String(memoryStream.ToArray());
        PlayerPrefs.SetString(prefKey, tmp);
    }

    public static object Load<T>(string prefKey)
    {
        if (!PlayerPrefs.HasKey(prefKey))
            return default(T);

        string serializedData = PlayerPrefs.GetString(prefKey);
        MemoryStream dataStream = new MemoryStream(System.Convert.FromBase64String(serializedData));

        T deserializedObject = (T)bf.Deserialize(dataStream);

        return deserializedObject;
    }
}

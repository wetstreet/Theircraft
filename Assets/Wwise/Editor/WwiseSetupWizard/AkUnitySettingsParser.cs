#if UNITY_EDITOR


// Note, this code was only written to parse the AudioManager.asset file.
// It has been written to try to be as generic as possible, but might not 
// work on other .asset settings files.
public class AkUnitySettingsParser
{
	public static bool SetBoolValue(string ValueName, bool ValueToSet, string ConfigFileName)
	{
		try
		{
			var SettingsPath = UnityEngine.Application.dataPath.Remove(UnityEngine.Application.dataPath.LastIndexOf("/")) +
			                   "/ProjectSettings/" + ConfigFileName + ".asset";

			System.IO.FileStream fs;
			System.IO.BinaryReader br;
			fs = System.IO.File.Open(SettingsPath, System.IO.FileMode.Open);
			br = new System.IO.BinaryReader(fs);

			// Read the unsigned int at offset 0x0C in the file. 
			// This contains the offset at which the setting's numerical values are stored.
			br.BaseStream.Seek(0x0C, System.IO.SeekOrigin.Begin);

			// For some reason, the offset is Big Endian in the file.
			var SettingsOffset = GetBigEndianIntFromBinaryReader(br);

			// In the file, we start with 0x14 bytes, then a string containing the unity version, 
			// then 0x0C bytes, then a string containing the base class name, followed by a string containing "base".
			string tempStr;
			br.BaseStream.Seek(0x14, System.IO.SeekOrigin.Begin);
			tempStr = GetStringFromBinaryReader(br); // Unity Version
			br.BaseStream.Seek(0x0C, System.IO.SeekOrigin.Current);
			tempStr = GetStringFromBinaryReader(br); // Config file Name
			if (tempStr != ConfigFileName)
				return false;

			tempStr = GetStringFromBinaryReader(br); // "Base"
			if (tempStr != "Base")
				return false;

			// This string is then followed by 24 bytes
			br.BaseStream.Seek(24, System.IO.SeekOrigin.Current);

			// We then have a series of String (type), String (variable name), and 24 bytes
			// We can use the type of the settings before the field we are looking for to
			// find its offset after SettingsOffset.
			while (br.BaseStream.Position < br.BaseStream.Length)
			{
				var SettingType = GetStringFromBinaryReader(br);
				var SettingName = GetStringFromBinaryReader(br);

				if (SettingName == ValueName)
					break;

				br.BaseStream.Seek(24, System.IO.SeekOrigin.Current);

				if (GetSizeofTypeByString(SettingType) == -1)
					return false;

				SettingsOffset += GetSizeofTypeByString(SettingType);
			}

			// Set the setting in the file
			var bw = new System.IO.BinaryWriter(fs);
			bw.Seek(SettingsOffset, System.IO.SeekOrigin.Begin);
			bw.Write(ValueToSet ? (byte) 1 : (byte) 0);
			bw.Close();
			fs.Close();
		}
		catch (System.Exception)
		{
			// Error happened
			return false;
		}

		// Success!
		return true;
	}

	// Read a big endian Int, and advances the BinaryReader's position
	private static int GetBigEndianIntFromBinaryReader(System.IO.BinaryReader br)
	{
		var tempBytes = new byte[4];
		tempBytes = br.ReadBytes(4);
		return (tempBytes[0] << 24) | (tempBytes[1] << 16) | (tempBytes[2] << 8) | tempBytes[3];
	}

	// Reads a zero-terminated string at the BinaryReader's current position, and advances position
	private static string GetStringFromBinaryReader(System.IO.BinaryReader br)
	{
		var list = new System.Collections.Generic.List<byte>();

		// Do not add the \0 in the string, because comparison won't work
		var currentByte = br.ReadByte();
		while (currentByte != 0)
		{
			list.Add(currentByte);
			currentByte = br.ReadByte();
		}

		return System.Text.Encoding.Default.GetString(list.ToArray());
	}

	// Returns the size in bytes of a type, as specified in the .asset file
	// NOTE: this function only supports types found in AudioManager.asset.
	private static int GetSizeofTypeByString(string typeStr)
	{
		switch (typeStr)
		{
			case "int":
			case "unsigned int":
			case "float":
			case "UInt32":
			case "SInt32":
				return 4;
			case "bool":
				return 1;
			default:
				return -1;
		}
	}
}

#endif // UNITY_EDITOR
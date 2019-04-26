#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
#if UNITY_EDITOR
[System.Serializable]
public class WwiseSettings
{
	public const string WwiseSettingsFilename = "WwiseSettings.xml";

	private static WwiseSettings s_Instance;
	public bool CopySoundBanksAsPreBuildStep = true;
	public bool CreatedPicker = false;
	public bool CreateWwiseGlobal = true;
	public bool CreateWwiseListener = true;
	public bool GenerateSoundBanksAsPreBuildStep = false;
	public bool ShowMissingRigidBodyWarning = true;
	public string SoundbankPath;
	public string WwiseInstallationPathMac;
	public string WwiseInstallationPathWindows;
	public string WwiseProjectPath;

	// Save the WwiseSettings structure to a serialized XML file
	public static void SaveSettings(WwiseSettings Settings)
	{
		try
		{
			var xmlDoc = new System.Xml.XmlDocument();
			var xmlSerializer = new System.Xml.Serialization.XmlSerializer(Settings.GetType());
			using (var xmlStream = new System.IO.MemoryStream())
			{
				var streamWriter = new System.IO.StreamWriter(xmlStream, System.Text.Encoding.UTF8);
				xmlSerializer.Serialize(streamWriter, Settings);
				xmlStream.Position = 0;
				xmlDoc.Load(xmlStream);
				xmlDoc.Save(System.IO.Path.Combine(UnityEngine.Application.dataPath, WwiseSettingsFilename));
			}
		}
		catch
		{
		}
	}

	// Load the WwiseSettings structure from a serialized XML file
	public static WwiseSettings LoadSettings(bool ForceLoad = false)
	{
		if (s_Instance != null && !ForceLoad)
			return s_Instance;

		var Settings = new WwiseSettings();
		try
		{
			if (System.IO.File.Exists(System.IO.Path.Combine(UnityEngine.Application.dataPath, WwiseSettingsFilename)))
			{
				var xmlSerializer = new System.Xml.Serialization.XmlSerializer(Settings.GetType());
				var xmlFileStream = new System.IO.FileStream(UnityEngine.Application.dataPath + "/" + WwiseSettingsFilename,
					System.IO.FileMode.Open, System.IO.FileAccess.Read);
				Settings = (WwiseSettings) xmlSerializer.Deserialize(xmlFileStream);
				xmlFileStream.Close();
			}
			else
			{
				var projectDir = System.IO.Path.GetDirectoryName(UnityEngine.Application.dataPath);
				var foundWwiseProjects = System.IO.Directory.GetFiles(projectDir, "*.wproj", System.IO.SearchOption.AllDirectories);

				if (foundWwiseProjects.Length == 0)
					Settings.WwiseProjectPath = "";
				else
				{
					Settings.WwiseProjectPath =
						AkUtilities.MakeRelativePath(UnityEngine.Application.dataPath, foundWwiseProjects[0]);
				}

				Settings.SoundbankPath = AkBasePathGetter.DefaultBasePath;
			}

			s_Instance = Settings;
		}
		catch
		{
		}

		return Settings;
	}
}

public partial class AkUtilities
{
	/// <summary>
	/// These values represent the maximum value of the "Unity Integration Version" number in the Version.txt file that will migrated.
	/// For example, in Wwise v2019.1.0, "Unity Integration Version" is 18 which means that all migrations up until this version are required.
	/// </summary>
	public enum MigrationStep
	{
		AkGameObjListenerMask_v2016_1_0 = 9,
		AkGameObjPositionOffsetData_v2016_2_0 = 10,
		AkAudioListener_v2017_1_0 = 14,
		InitializationSettings_v2018_1_0 = 15,
		WwiseTypes_v2018_1_6 = 16,
		AkEventCallback_v2018_1_6 = 16,
		AkAmbient_v2019_1_0 = 17,
		/// <summary>
		/// The value that is currently in the Version.txt file.
		/// </summary>
		Current
	}

	public static bool IsMigrationRequired(MigrationStep step)
	{
		return migrationStartIndex <= (int)step;
	}

	public static bool IsMigrating
	{
		get { return migrationStartIndex < MigrationStopIndex; }
	}

	public static void BeginMigration(int startIndex)
	{
		if (startIndex < MigrationStopIndex)
			migrationStartIndex = startIndex;
	}

	public static void EndMigration()
	{
		migrationStartIndex = MigrationStopIndex;
	}

	public static int MigrationStartIndex { get { return migrationStartIndex; } }
	public const int MigrationStopIndex = (int)MigrationStep.Current;

	private static int migrationStartIndex = MigrationStopIndex;

	public static bool IsWwiseProjectAvailable { set; get; }

	private const string DragAndDropId = "AkDragDropId";

	public static WwiseObjectReference DragAndDropObjectReference
	{
		set { UnityEditor.DragAndDrop.SetGenericData(DragAndDropId, value); }
		get { return UnityEditor.DragAndDrop.GetGenericData(DragAndDropId) as WwiseObjectReference; }
	}

	private static readonly System.Collections.Generic.Dictionary<string, string> s_ProjectBankPaths =
		new System.Collections.Generic.Dictionary<string, string>();

	private static System.DateTime s_LastBankPathUpdate = System.DateTime.MinValue;

	private static readonly System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>>
		s_BaseToCustomPF = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>>();

	public static bool IsSoundbankGenerationAvailable()
	{
		return GetWwiseCLI() != null;
	}

	/// Executes a command-line. Blocks the calling thread until the new process has completed. Returns the logged stdout in one big string.
	public static string ExecuteCommandLine(string command, string arguments)
	{
		var process = new System.Diagnostics.Process();
		process.StartInfo.FileName = command;
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.CreateNoWindow = true;
		process.StartInfo.Arguments = arguments;
		process.Start();

		// Synchronously read the standard output of the spawned process. 
		var reader = process.StandardOutput;
		var output = reader.ReadToEnd();

		// Waiting for the process to exit directly in the UI thread. Similar cases are working that way too.

		// TODO: Is it better to provide a timeout avoid any issues of forever blocking the UI thread? If so, what is
		// a relevant timeout value for soundbank generation?
		process.WaitForExit();
		process.Close();

		return output;
	}

	private static string GetWwiseCLI()
	{
		string result = null;

		var settings = WwiseSettings.LoadSettings();

#if UNITY_EDITOR_WIN
		if (!string.IsNullOrEmpty(settings.WwiseInstallationPathWindows))
		{
			result = System.IO.Path.Combine(settings.WwiseInstallationPathWindows, @"Authoring\x64\Release\bin\WwiseCLI.exe");

			if (!System.IO.File.Exists(result))
				result = System.IO.Path.Combine(settings.WwiseInstallationPathWindows, @"Authoring\Win32\Release\bin\WwiseCLI.exe");
		}
#elif UNITY_EDITOR_OSX
		if (!string.IsNullOrEmpty(settings.WwiseInstallationPathMac))
			result = System.IO.Path.Combine(settings.WwiseInstallationPathMac, "Contents/Tools/WwiseCLI.sh");
#endif

		if (result != null && System.IO.File.Exists(result))
			return result;

		return null;
	}

	// Generate all the SoundBanks for all the supported platforms in the Wwise project. This effectively calls Wwise for the project
	// that is configured in the UnityWwise integration.
	public static void GenerateSoundbanks(System.Collections.Generic.List<string> platforms = null)
	{
		var Settings = WwiseSettings.LoadSettings();
		var wwiseProjectFullPath = GetFullPath(UnityEngine.Application.dataPath, Settings.WwiseProjectPath);

		if (IsSoundbankOverrideEnabled(wwiseProjectFullPath))
		{
			UnityEngine.Debug.LogWarning(
				"The SoundBank generation process ignores the SoundBank Settings' Overrides currently enabled in the User settings. The project's SoundBank settings will be used.");
		}

		var wwiseCli = GetWwiseCLI();

		if (wwiseCli == null)
		{
			UnityEngine.Debug.LogError("Couldn't locate WwiseCLI, unable to generate SoundBanks.");
			return;
		}

#if UNITY_EDITOR_WIN
		var command = wwiseCli;
		var arguments = "";
#elif UNITY_EDITOR_OSX
		var command = "/bin/sh";
		var arguments = "\"" + wwiseCli + "\"";
#else
		var command = "";
		var arguments = "";
#endif

		arguments += " \"" + wwiseProjectFullPath + "\"";

		if (platforms != null)
		{
			foreach (var platform in platforms)
			{
				if (!string.IsNullOrEmpty(platform))
					arguments += " -Platform " + platform;
			}
		}

		arguments += " -GenerateSoundBanks";

		var output = ExecuteCommandLine(command, arguments);

		var success = output.Contains("Process completed successfully.");
		var warning = output.Contains("Process completed with warning");

		var message = "WwiseUnity: SoundBanks generation " +
					  (success ? "successful" : (warning ? "has warning(s)" : "error")) + ":\n" + output;

		if (success)
		{
			UnityEngine.Debug.Log(message);
		}
		else if (warning)
		{
			UnityEngine.Debug.LogWarning(message);
		}
		else
		{
			UnityEngine.Debug.LogError(message);
		}

		UnityEditor.AssetDatabase.Refresh();
	}

	/// Reads the user settings (not the project settings) to check if there is an override currently defined for the soundbank generation folders.
	public static bool IsSoundbankOverrideEnabled(string wwiseProjectPath)
	{
		var userConfigFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(wwiseProjectPath),
			System.IO.Path.GetFileNameWithoutExtension(wwiseProjectPath) + "." + System.Environment.UserName + ".wsettings");

		if (!System.IO.File.Exists(userConfigFile))
			return false;

		var userConfigDoc = new System.Xml.XmlDocument();
		userConfigDoc.Load(userConfigFile);
		var userConfigNavigator = userConfigDoc.CreateNavigator();

		var userConfigNode = userConfigNavigator.SelectSingleNode(
			System.Xml.XPath.XPathExpression.Compile("//Property[@Name='SoundBankPathUserOverride' and @Value = 'True']"));

		return userConfigNode != null;
	}

	public static System.Collections.Generic.IDictionary<string, System.Collections.Generic.List<string>> PlatformMapping
	{
		get { return s_BaseToCustomPF; }
	}

	public static System.Collections.Generic.IDictionary<string, string> GetAllBankPaths()
	{
		var Settings = WwiseSettings.LoadSettings();
		var WwiseProjectFullPath = GetFullPath(UnityEngine.Application.dataPath, Settings.WwiseProjectPath);
		UpdateSoundbanksDestinationFolders(WwiseProjectFullPath);
		return s_ProjectBankPaths;
	}

	private static void UpdateSoundbanksDestinationFolders(string WwiseProjectPath)
	{
		try
		{
			if (WwiseProjectPath.Length == 0)
				return;

			if (!System.IO.File.Exists(WwiseProjectPath))
				return;

			var t = System.IO.File.GetLastWriteTime(WwiseProjectPath);
			if (t <= s_LastBankPathUpdate)
				return;

			s_LastBankPathUpdate = t;
			s_ProjectBankPaths.Clear();

			var doc = new System.Xml.XmlDocument();
			doc.Load(WwiseProjectPath);
			var Navigator = doc.CreateNavigator();

			// Gather the mapping of Custom platform to Base platform
			var itpf = Navigator.Select("//Platform");
			s_BaseToCustomPF.Clear();
			foreach (System.Xml.XPath.XPathNavigator node in itpf)
			{
				System.Collections.Generic.List<string> customList = null;
				var basePF = node.GetAttribute("ReferencePlatform", "");
				if (!s_BaseToCustomPF.TryGetValue(basePF, out customList))
				{
					customList = new System.Collections.Generic.List<string>();
					s_BaseToCustomPF[basePF] = customList;
				}

				customList.Add(node.GetAttribute("Name", ""));
			}

			// Navigate the wproj file (XML format) to where generated soundbank paths are stored
			var it = Navigator.Select("//Property[@Name='SoundBankPaths']/ValueList/Value");
			foreach (System.Xml.XPath.XPathNavigator node in it)
			{
				var path = node.Value;
				AkBasePathGetter.FixSlashes(ref path);
				var pf = node.GetAttribute("Platform", "");
				s_ProjectBankPaths[pf] = path;
			}
		}
		catch (System.Exception ex)
		{
			// Error happened, return empty string
			UnityEngine.Debug.LogError("Wwise: Error while reading project " + WwiseProjectPath + ".  Exception: " + ex.Message);
		}
	}

	// Parses the .wproj to find out where soundbanks are generated for the given path.
	public static string GetWwiseSoundBankDestinationFolder(string Platform, string WwiseProjectPath)
	{
		try
		{
			UpdateSoundbanksDestinationFolders(WwiseProjectPath);
			return s_ProjectBankPaths[Platform];
		}
		catch
		{
			return "";
		}
	}

	// Set soundbank-related bool settings in the wproj file.
	public static bool EnableBoolSoundbankSettingInWproj(string SettingName, string WwiseProjectPath)
	{
		try
		{
			if (WwiseProjectPath.Length == 0)
				return true;

			var doc = new System.Xml.XmlDocument();
			doc.PreserveWhitespace = true;
			doc.Load(WwiseProjectPath);
			var Navigator = doc.CreateNavigator();

			// Navigate the wproj file (XML format) to where our setting should be
			var pathInXml = string.Format("/WwiseDocument/ProjectInfo/Project/PropertyList/Property[@Name='{0}']", SettingName);
			var expression = System.Xml.XPath.XPathExpression.Compile(pathInXml);
			var node = Navigator.SelectSingleNode(expression);
			if (node == null)
			{
				// Setting isn't in the wproj, add it
				// Navigate to the SoundBankHeaderFilePath property (it is always there)
				expression =
					System.Xml.XPath.XPathExpression.Compile(
						"/WwiseDocument/ProjectInfo/Project/PropertyList/Property[@Name='SoundBankHeaderFilePath']");
				node = Navigator.SelectSingleNode(expression);
				if (node == null)
				{
					// SoundBankHeaderFilePath not in wproj, invalid wproj file
					UnityEngine.Debug.LogError(
						"WwiseUnity: Could not find SoundBankHeaderFilePath property in Wwise project file. File is invalid.");
					return false;
				}

				// Add the setting right above SoundBankHeaderFilePath
				var propertyToInsert = string.Format("<Property Name=\"{0}\" Type=\"bool\" Value=\"True\"/>", SettingName);
				node.InsertBefore(propertyToInsert);
			}
			else if (node.GetAttribute("Value", "") == "False")
			{
				// Value is present, we simply have to modify it.
				if (!node.MoveToAttribute("Value", ""))
					return false;

				// Modify the value to true
				node.SetValue("True");
			}
			else
			{
				// Parameter already set, nothing to do!
				return true;
			}

			doc.Save(WwiseProjectPath);
			return true;
		}
		catch
		{
			return false;
		}
	}

	public static bool SetSoundbankHeaderFilePath(string WwiseProjectPath, string SoundbankPath)
	{
		try
		{
			if (WwiseProjectPath.Length == 0)
				return true;

			var doc = new System.Xml.XmlDocument();
			doc.PreserveWhitespace = true;
			doc.Load(WwiseProjectPath);
			var Navigator = doc.CreateNavigator();

			// Navigate to where the header file path is saved. The node has to be there, or else the wproj is invalid.
			var expression =
				System.Xml.XPath.XPathExpression.Compile(
					"/WwiseDocument/ProjectInfo/Project/PropertyList/Property[@Name='SoundBankHeaderFilePath']");
			var node = Navigator.SelectSingleNode(expression);
			if (node == null)
			{
				UnityEngine.Debug.LogError(
					"Could not find SoundBankHeaderFilePath property in Wwise project file. File is invalid.");
				return false;
			}

			// Change the "Value" attribute
			if (!node.MoveToAttribute("Value", ""))
				return false;

			node.SetValue(SoundbankPath);
			doc.Save(WwiseProjectPath);
			return true;
		}
		catch
		{
			return false;
		}
	}

	// Make two paths relative to each other
	public static string MakeRelativePath(string fromPath, string toPath)
	{
		// MONO BUG: https://github.com/mono/mono/pull/471
		// In the editor, Application.dataPath returns <Project Folder>/Assets. There is a bug in
		// mono for method Uri.GetRelativeUri where if the path ends in a folder, it will
		// ignore the last part of the path. Thus, we need to add fake depth to get the "real"
		// relative path.
		fromPath += "/fake_depth";
		try
		{
			if (string.IsNullOrEmpty(fromPath))
				return toPath;

			if (string.IsNullOrEmpty(toPath))
				return "";

			var fromUri = new System.Uri(fromPath);
			var toUri = new System.Uri(toPath);

			if (fromUri.Scheme != toUri.Scheme)
				return toPath;

			var relativeUri = fromUri.MakeRelativeUri(toUri);
			var relativePath = System.Uri.UnescapeDataString(relativeUri.ToString());

			return relativePath;
		}
		catch
		{
			return toPath;
		}
	}

	// Reconcile a base path and a relative path to give a full path without any ".."
	public static string GetFullPath(string BasePath, string RelativePath)
	{
		string tmpString;
		if (string.IsNullOrEmpty(BasePath))
			return "";

		var wrongSeparatorChar = System.IO.Path.DirectorySeparatorChar == '/' ? '\\' : '/';

		if (string.IsNullOrEmpty(RelativePath))
			return BasePath.Replace(wrongSeparatorChar, System.IO.Path.DirectorySeparatorChar);

		if (System.IO.Path.GetPathRoot(RelativePath) != "")
			return RelativePath.Replace(wrongSeparatorChar, System.IO.Path.DirectorySeparatorChar);

		tmpString = System.IO.Path.Combine(BasePath, RelativePath);
		tmpString = System.IO.Path.GetFullPath(new System.Uri(tmpString).LocalPath);

		return tmpString.Replace(wrongSeparatorChar, System.IO.Path.DirectorySeparatorChar);
	}


	public static bool DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
	{
		// Get the subdirectories for the specified directory.
		var dir = new System.IO.DirectoryInfo(sourceDirName);

		if (!dir.Exists)
		{
			UnityEngine.Debug.LogError("WwiseUnity: Source directory doesn't exist");
			return false;
		}

		var dirs = dir.GetDirectories();

		// If the destination directory doesn't exist, create it. 
		if (!System.IO.Directory.Exists(destDirName))
			System.IO.Directory.CreateDirectory(destDirName);

		// Get the files in the directory and copy them to the new location.
		var files = dir.GetFiles();
		foreach (var file in files)
		{
			var temppath = System.IO.Path.Combine(destDirName, file.Name);
			file.CopyTo(temppath, true);
		}

		// If copying subdirectories, copy them and their contents to new location. 
		if (copySubDirs)
		{
			foreach (var subdir in dirs)
			{
				var temppath = System.IO.Path.Combine(destDirName, subdir.Name);
				DirectoryCopy(subdir.FullName, temppath, copySubDirs);
			}
		}

		return true;
	}

	public static byte[] GetByteArrayProperty(UnityEditor.SerializedProperty property)
	{
		if (!property.isArray || property.arraySize == 0)
			return null;

		var byteArray = new byte[property.arraySize];

		for (var i = 0; i < byteArray.Length; i++)
			byteArray[i] = (byte)property.GetArrayElementAtIndex(i).intValue;

		return byteArray;
	}

	public static void SetByteArrayProperty(UnityEditor.SerializedProperty property, byte[] byteArray)
	{
		if (!property.isArray)
			return;

		var iterator = property.Copy();

		iterator.arraySize = byteArray.Length;

		while (iterator.name != "data")
			iterator.Next(true);

		for (var i = 0; i < byteArray.Length; i++)
		{
			iterator.intValue = byteArray[i];
			iterator.Next(true);
		}
	}

	///This function returns the absolute position and the width and height of the last drawn GuiLayout(or EditorGuiLayout) element in the inspector window.
	///This function must be called in the OnInspectorGUI function
	/// 
	///The inspector must be in repaint mode in order to get the correct position 
	///Example => if(Event.current.type == EventType.Repaint) Rect pos = AkUtilities.GetLastRectAbsolute();
	public static UnityEngine.Rect GetLastRectAbsolute(UnityEngine.Rect relativePos)
	{
		var lastRectAbsolute = relativePos;
		lastRectAbsolute.x += UnityEditor.EditorWindow.focusedWindow.position.x;
		lastRectAbsolute.y += UnityEditor.EditorWindow.focusedWindow.position.y;

		try
		{
			var inspectorType = UnityEditor.EditorWindow.focusedWindow.GetType();

			var currentInspectorFieldInfo = inspectorType.GetField("s_CurrentInspectorWindow",
				System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

			var scrollPosInfo = inspectorType.GetField("m_ScrollPosition",
				System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

			var scrollPos = (UnityEngine.Vector2)scrollPosInfo.GetValue(currentInspectorFieldInfo.GetValue(null));
			lastRectAbsolute.x -= scrollPos.x;
			lastRectAbsolute.y -= scrollPos.y;
		}
		catch
		{
		}

		return lastRectAbsolute;
	}

	public static void RepaintInspector()
	{
		var windows = UnityEngine.Resources.FindObjectsOfTypeAll<UnityEditor.EditorWindow>();
		foreach (var win in windows)
			if (win.titleContent.text == "Inspector")
				win.Repaint();
	}

	public static void AddAkAudioListenerToMainCamera(bool logWarning = false)
	{
		UnityEngine.Camera camera = UnityEngine.Camera.main;

		// Workaround for some versions of Unity not setting properly the MainCamera tag
		// on the first scene of a new project
		if (camera == null)
		{
			var cameraArray = UnityEngine.Object.FindObjectsOfType<UnityEngine.Camera>();
			if (cameraArray.Length > 0)
			{
				foreach (var entry in cameraArray)
				{
					if (entry.name == "Main Camera")
					{
						camera = entry;
						break;
					}
				}
			}

			if (camera == null)
			{
				return;
			}
		}

		if (camera.GetComponentInChildren<AkAudioListener>())
		{
			return;
		}

		var oldListener = camera.gameObject.GetComponent<UnityEngine.AudioListener>();
		if (oldListener != null)
		{
			UnityEditor.Undo.DestroyObjectImmediate(oldListener);
		}

		var akAudioListener = UnityEditor.Undo.AddComponent<AkAudioListener>(camera.gameObject);
		if (!akAudioListener)
		{
			return;
		}

		var akGameObj = akAudioListener.GetComponentInChildren<AkGameObj>();
		if (akGameObj)
		{
			akGameObj.isEnvironmentAware = false;
		}

		if (logWarning)
		{
			UnityEngine.Debug.LogWarning(
				"Automatically added AkAudioListener to Main Camera. Go to \"Edit > Wwise Settings...\" to disable this functionality.");
		}
	}

	#region Tooltip Workaround
	private static System.Reflection.FieldInfo GetFieldInfoFromProperty(UnityEditor.SerializedProperty property)
	{
		var serializedProperty = property.serializedObject.FindProperty("m_Script");
		if (serializedProperty == null)
			return null;

		var monoScript = serializedProperty.objectReferenceValue as UnityEditor.MonoScript;
		if (monoScript == null)
			return null;

		var scriptTypeFromProperty = monoScript.GetClass();
		if (scriptTypeFromProperty == null)
			return null;

		return GetFieldInfoFromPropertyPath(scriptTypeFromProperty, property.propertyPath);
	}

	private static System.Reflection.FieldInfo GetFieldInfoFromPropertyPath(System.Type host, string path)
	{
		System.Reflection.FieldInfo fieldInfo = null;

		var type = host;
		var array = path.Split('.');
		for (int i = 0; i < array.Length; ++i)
		{
			string text = array[i];
			if (i < array.Length - 1 && text == "Array" && array[i + 1].StartsWith("data["))
			{
				if (type.IsArray)
					type = type.GetElementType();
				else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(System.Collections.Generic.List<>))
					type = type.GetGenericArguments()[0];

				i++;
			}
			else
			{
				var type2 = type;
				while (type2 != null)
				{
					fieldInfo = type2.GetField(text, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
					if (fieldInfo != null)
					{
						type = fieldInfo.FieldType;
						break;
					}

					type2 = type2.BaseType;
					if (type2 == null)
						return null;
				}
			}
		}

		return fieldInfo;
	}

	private static string GetTooltip(System.Reflection.FieldInfo field, bool inherit)
	{
		var attributes = field.GetCustomAttributes(typeof(UnityEngine.TooltipAttribute), inherit) as UnityEngine.TooltipAttribute[];
		if (attributes != null && attributes.Length > 0)
			return attributes[0].tooltip;

		return string.Empty;
	}

	public static string GetTooltip(UnityEditor.SerializedProperty property)
	{
		return GetTooltip(GetFieldInfoFromProperty(property), true);
	}
	#endregion
}
#endif // UNITY_EDITOR

public partial class AkUtilities
{
	/// <summary>
	///     This is based on FNVHash as used by the DataManager
	///     to assign short IDs to objects. Be sure to keep them both in sync
	///     when making changes!
	/// </summary>
	public class ShortIDGenerator
	{
		private const uint s_prime32 = 16777619;
		private const uint s_offsetBasis32 = 2166136261;

		private static byte s_hashSize;
		private static uint s_mask;

		static ShortIDGenerator()
		{
			HashSize = 32;
		}

		public static byte HashSize
		{
			get { return s_hashSize; }

			set
			{
				s_hashSize = value;
				s_mask = (uint) ((1 << s_hashSize) - 1);
			}
		}

		public static uint Compute(string in_name)
		{
			var buffer = System.Text.Encoding.UTF8.GetBytes(in_name.ToLower());

			// Start with the basis value
			var hval = s_offsetBasis32;

			for (var i = 0; i < buffer.Length; i++)
			{
				// multiply by the 32 bit FNV magic prime mod 2^32
				hval *= s_prime32;

				// xor the bottom with the current octet
				hval ^= buffer[i];
			}

			if (s_hashSize == 32)
				return hval;

			// XOR-Fold to the required number of bits
			return (hval >> s_hashSize) ^ (hval & s_mask);
		}
	}
}

#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
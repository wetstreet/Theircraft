#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

/// <summary>
///     This class is responsible for determining the path where sound banks are located. When using custom platforms, this
///     class needs to be extended.
/// </summary>
public partial class AkBasePathGetter
{
	/// <summary>
	///     User hook called to retrieve the custom platform name used to determine the base path. Do not modify platformName
	///     to use default platform names.
	/// </summary>
	/// <param name="platformName">The custom platform name. Leave unaffected if the default location is acceptable.</param>
	static partial void GetCustomPlatformName(ref string platformName);

	/// <summary>
	///     Determines the platform name which is also the sub-folder within the base path where sound banks are located for
	///     this platform.
	/// </summary>
	/// <returns>The platform name.</returns>
	public static string GetPlatformName()
	{
		var platformSubDir = string.Empty;
		GetCustomPlatformName(ref platformSubDir);
		if (!string.IsNullOrEmpty(platformSubDir))
			return platformSubDir;

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
		platformSubDir = "Windows";
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
		platformSubDir = "Mac";
#elif UNITY_STANDALONE_LINUX
		platformSubDir = "Linux";
#elif UNITY_XBOXONE
		platformSubDir = "XBoxOne";
#elif UNITY_IOS || UNITY_TVOS
		platformSubDir = "iOS";
#elif UNITY_ANDROID
		platformSubDir = "Android";
#elif PLATFORM_LUMIN
		platformSubDir = "Lumin";
#elif UNITY_PS4
		platformSubDir = "PS4";
#elif UNITY_WP_8_1
		platformSubDir = "WindowsPhone";
#elif UNITY_SWITCH
		platformSubDir = "Switch";
#elif UNITY_PSP2
#if AK_ARCH_VITA_SW || !AK_ARCH_VITA_HW
		platformSubDir = "VitaSW";
#else
		platformSubDir = "VitaHW";
#endif
#else
		platformSubDir = "Undefined platform sub-folder";
#endif

		return platformSubDir;
	}
}

public partial class AkBasePathGetter
{
	public static string DefaultBasePath = System.IO.Path.Combine("Audio", "GeneratedSoundBanks");

	/// <summary>
	///     Returns the absolute path to the platform specific sound banks.
	/// </summary>
	/// <returns>The absolute path to the platform specific sound banks.</returns>
	public static string GetPlatformBasePath()
	{
		var platformName = GetPlatformName();

#if UNITY_EDITOR
		var platformBasePathEditor = GetPlatformBasePathEditor(platformName);
		if (!string.IsNullOrEmpty(platformBasePathEditor))
			return platformBasePathEditor;
#endif

		// Combine base path with platform sub-folder
		var platformBasePath = System.IO.Path.Combine(GetFullSoundBankPath(), platformName);
		FixSlashes(ref platformBasePath);
		return platformBasePath;
	}

	/// <summary>
	///     Returns the absolute path to the folder above the platform specific sound banks sub-folders.
	/// </summary>
	/// <returns>The absolute sound bank base path.</returns>
	public static string GetFullSoundBankPath()
	{
		// Get full path of base path
		string fullBasePath = string.Empty;

#if UNITY_EDITOR
		fullBasePath = WwiseSettings.LoadSettings().SoundbankPath;
#endif

		if (string.IsNullOrEmpty(fullBasePath))
			fullBasePath = AkWwiseInitializationSettings.ActivePlatformSettings.SoundbankPath;

#if UNITY_EDITOR || !UNITY_ANDROID
		fullBasePath = System.IO.Path.Combine(UnityEngine.Application.streamingAssetsPath, fullBasePath);
#endif

#if UNITY_SWITCH
		if (fullBasePath.StartsWith("/"))
			fullBasePath = fullBasePath.Substring(1);
#endif

		FixSlashes(ref fullBasePath);
		return fullBasePath;
	}

#if UNITY_EDITOR
	/// <summary>
	///     Determines the platform base path for use within the Editor.
	/// </summary>
	/// <param name="platformName">The platform name.</param>
	/// <returns>The full path to the sound banks for use within the Editor.</returns>
	public static string GetPlatformBasePathEditor(string platformName)
	{
		var Settings = WwiseSettings.LoadSettings();
		var WwiseProjectFullPath = AkUtilities.GetFullPath(UnityEngine.Application.dataPath, Settings.WwiseProjectPath);
		var SoundBankDest = AkUtilities.GetWwiseSoundBankDestinationFolder(platformName, WwiseProjectFullPath);

		try
		{
			if (System.IO.Path.GetPathRoot(SoundBankDest) == "")
			{
				// Path is relative, make it full
				SoundBankDest = AkUtilities.GetFullPath(System.IO.Path.GetDirectoryName(WwiseProjectFullPath), SoundBankDest);
			}
		}
		catch
		{
			SoundBankDest = string.Empty;
		}

		if (string.IsNullOrEmpty(SoundBankDest))
		{
			UnityEngine.Debug.LogWarning("WwiseUnity: The platform SoundBank subfolder within the Wwise project could not be found.");
		}
		else
		{
			try
			{
				// Verify if there are banks in there
				var di = new System.IO.DirectoryInfo(SoundBankDest);
				var foundBanks = di.GetFiles("*.bnk", System.IO.SearchOption.AllDirectories);
				if (foundBanks.Length == 0)
					SoundBankDest = string.Empty;
				else if (!SoundBankDest.Contains(platformName))
				{
					UnityEngine.Debug.LogWarning(
						"WwiseUnity: The platform SoundBank subfolder does not match your platform name. You will need to create a custom platform name getter for your game. See section \"Using Wwise Custom Platforms in Unity\" of the Wwise Unity integration documentation for more information");
				}
			}
			catch
			{
				SoundBankDest = string.Empty;
			}
		}

		return SoundBankDest;
	}
#endif

	public static void FixSlashes(ref string path, char separatorChar, char badChar, bool addTrailingSlash)
	{
		if (string.IsNullOrEmpty(path))
			return;

		path = path.Trim().Replace(badChar, separatorChar).TrimStart('\\');

		// Append a trailing slash to play nicely with Wwise
		if (addTrailingSlash && !path.EndsWith(separatorChar.ToString()))
			path += separatorChar;
	}

	public static void FixSlashes(ref string path)
	{
#if UNITY_WSA
		var separatorChar = '\\';
#else
		var separatorChar = System.IO.Path.DirectorySeparatorChar;
#endif // UNITY_WSA
		var badChar = separatorChar == '\\' ? '/' : '\\';
		FixSlashes(ref path, separatorChar, badChar, true);
	}

	/// <summary>
	///     Returns the absolute path to the platform specific sound banks, and logs an error if the Init bank cannot be found.
	/// </summary>
	/// <returns>The absolute path to the platform specific sound banks.</returns>
	public static string GetSoundbankBasePath()
	{
		var basePathToSet = GetPlatformBasePath();
		var InitBnkFound = true;
#if UNITY_EDITOR || !(UNITY_ANDROID || PLATFORM_LUMIN)// Can't use File.Exists on Android, assume banks are there
		var InitBankPath = System.IO.Path.Combine(basePathToSet, "Init.bnk");
		if (!System.IO.File.Exists(InitBankPath))
			InitBnkFound = false;
#endif

		if (basePathToSet == string.Empty || InitBnkFound == false)
		{
			UnityEngine.Debug.Log("WwiseUnity: Looking for SoundBanks in " + basePathToSet);

#if UNITY_EDITOR
			UnityEngine.Debug.LogError("WwiseUnity: Could not locate the SoundBanks. Did you make sure to generate them?");
#else
			UnityEngine.Debug.LogError("WwiseUnity: Could not locate the SoundBanks. Did you make sure to copy them to the StreamingAssets folder?");
#endif
		}

		return basePathToSet;
	}
}

#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
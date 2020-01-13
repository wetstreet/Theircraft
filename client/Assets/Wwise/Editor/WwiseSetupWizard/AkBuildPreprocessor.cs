#if UNITY_EDITOR && UNITY_5_6_OR_NEWER
public partial class AkBuildPreprocessor
{
	/// <summary>
	///     User hook called to retrieve the custom platform name used to determine the base path. Do not modify platformName
	///     to use default platform names.
	/// </summary>
	/// <param name="platformName">The custom platform name.</param>
	static partial void GetCustomPlatformName(ref string platformName, UnityEditor.BuildTarget target);

	private static string GetPlatformName(UnityEditor.BuildTarget target)
	{
		var platformSubDir = string.Empty;
		GetCustomPlatformName(ref platformSubDir, target);
		if (!string.IsNullOrEmpty(platformSubDir))
			return platformSubDir;

		switch (target)
		{
			case UnityEditor.BuildTarget.Android:
				return "Android";

			case UnityEditor.BuildTarget.iOS:
			case UnityEditor.BuildTarget.tvOS:
				return "iOS";

			case UnityEditor.BuildTarget.StandaloneLinux:
			case UnityEditor.BuildTarget.StandaloneLinux64:
			case UnityEditor.BuildTarget.StandaloneLinuxUniversal:
				return "Linux";

#if UNITY_2017_3_OR_NEWER
			case UnityEditor.BuildTarget.StandaloneOSX:
#else
			case UnityEditor.BuildTarget.StandaloneOSXIntel:
			case UnityEditor.BuildTarget.StandaloneOSXIntel64:
			case UnityEditor.BuildTarget.StandaloneOSXUniversal:
#endif
				return "Mac";

			case (UnityEditor.BuildTarget)39: // UnityEditor.BuildTarget.Lumin
				return "Lumin";
				
			case UnityEditor.BuildTarget.PS4:
				return "PS4";

#if !UNITY_2018_3_OR_NEWER
            case UnityEditor.BuildTarget.PSP2:
				return "Vita";
#endif
			case UnityEditor.BuildTarget.StandaloneWindows:
			case UnityEditor.BuildTarget.StandaloneWindows64:
			case UnityEditor.BuildTarget.WSAPlayer:
				return "Windows";

			case UnityEditor.BuildTarget.XboxOne:
				return "XboxOne";

			case UnityEditor.BuildTarget.Switch:
				return "Switch";
		}

		return target.ToString();
	}
}

#if UNITY_2018_1_OR_NEWER
public partial class AkBuildPreprocessor : UnityEditor.Build.IPreprocessBuildWithReport, UnityEditor.Build.IPostprocessBuildWithReport
#else
public partial class AkBuildPreprocessor : UnityEditor.Build.IPreprocessBuild, UnityEditor.Build.IPostprocessBuild
#endif
{
	public int callbackOrder
	{
		get { return 0; }
	}

	private string destinationSoundBankFolder = string.Empty;

	private static bool SetDestinationPath(string platformName, ref string destinationFolder)
	{
		destinationFolder = System.IO.Path.Combine(AkBasePathGetter.GetFullSoundBankPath(), platformName);
		return !string.IsNullOrEmpty(destinationFolder);
	}

	public static bool CopySoundbanks(bool generate, string platformName, ref string destinationFolder)
	{
		if (string.IsNullOrEmpty(platformName))
			UnityEngine.Debug.LogError("WwiseUnity: Could not determine platform name for <" + platformName + "> platform");
		else
		{
			if (generate)
			{
				var platforms = new System.Collections.Generic.List<string> { platformName };
				AkUtilities.GenerateSoundbanks(platforms);
			}

			var sourceFolder = AkBasePathGetter.GetPlatformBasePathEditor(platformName);
			if (string.IsNullOrEmpty(sourceFolder))
			{
				UnityEngine.Debug.LogError("WwiseUnity: Could not find source folder for <" + platformName +
				                           "> platform. Did you remember to generate your banks?");
			}
			else if (!SetDestinationPath(platformName, ref destinationFolder))
				UnityEngine.Debug.LogError("WwiseUnity: Could not find destination folder for <" + platformName + "> platform");
			else if (!AkUtilities.DirectoryCopy(sourceFolder, destinationFolder, true))
			{
				destinationFolder = null;
				UnityEngine.Debug.LogError("WwiseUnity: Could not copy soundbank folder for <" + platformName + "> platform");
			}
			else
			{
				UnityEngine.Debug.Log("WwiseUnity: Copied soundbank folder to streaming assets folder <" + destinationFolder +
				                      "> for <" + platformName + "> platform build");
				return true;
			}
		}

		return false;
	}

	public static void DeleteSoundbanks(string destinationFolder)
	{
		if (string.IsNullOrEmpty(destinationFolder))
			return;

		System.IO.Directory.Delete(destinationFolder, true);
		UnityEngine.Debug.Log("WwiseUnity: Deleting streaming assets folder <" + destinationFolder + ">");
	}

	public void OnPreprocessBuildInternal(UnityEditor.BuildTarget target, string path)
	{
		if (WwiseSetupWizard.Settings.CopySoundBanksAsPreBuildStep)
		{
			var platformName = GetPlatformName(target);
			if (!CopySoundbanks(WwiseSetupWizard.Settings.GenerateSoundBanksAsPreBuildStep, platformName,
				ref destinationSoundBankFolder))
			{
				UnityEngine.Debug.LogError("WwiseUnity: Soundbank folder has not been copied for <" + target + "> target at <" +
				                           path + ">. This will likely result in a build without sound!!!");
			}
		}

		// @todo sjl - only update for target platform
		AkPluginActivator.Update(true);
		AkPluginActivator.ActivatePluginsForDeployment(target, true);
	}

	public void OnPostprocessBuildInternal(UnityEditor.BuildTarget target, string path)
	{
		AkPluginActivator.ActivatePluginsForDeployment(target, false);
		DeleteSoundbanks(destinationSoundBankFolder);
		destinationSoundBankFolder = string.Empty;
	}

#if UNITY_2018_1_OR_NEWER
    public void OnPreprocessBuild(UnityEditor.Build.Reporting.BuildReport report)
    {
        OnPreprocessBuildInternal(report.summary.platform, report.summary.outputPath);
    }

    public void OnPostprocessBuild(UnityEditor.Build.Reporting.BuildReport report)
    {
        OnPostprocessBuildInternal(report.summary.platform, report.summary.outputPath);
    }
#else
    public void OnPreprocessBuild(UnityEditor.BuildTarget target, string path)
	{
		OnPreprocessBuildInternal(target, path);
	}

	public void OnPostprocessBuild(UnityEditor.BuildTarget target, string path)
	{
		OnPostprocessBuildInternal(target, path);
	}
#endif
}
#endif // #if UNITY_EDITOR && UNITY_5_6_OR_NEWER
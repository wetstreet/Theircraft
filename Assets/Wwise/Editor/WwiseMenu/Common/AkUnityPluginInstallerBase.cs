#if UNITY_EDITOR


// This sets the order in which the menus appear.
public enum AkWwiseMenuOrder
{
	ConvertIDs = 200
}

public enum AkWwiseWindowOrder
{
	WwiseSettings = 305,
	WwisePicker = 2300
}

public enum AkWwiseHelpOrder
{
	WwiseHelpOrder = 200
}

public class AkUnityAssetsInstaller
{
	public string[] m_arches = { };
	protected string m_assetsDir = UnityEngine.Application.dataPath;
	protected System.Collections.Generic.List<string> m_excludes = new System.Collections.Generic.List<string> { ".meta" };
	protected string m_platform = "Undefined";
	protected string m_pluginDir = System.IO.Path.Combine(UnityEngine.Application.dataPath, "Plugins");

	// Copy file to destination directory and create the directory when none exists.
	public static bool CopyFileToDirectory(string srcFilePath, string destDir)
	{
		var fi = new System.IO.FileInfo(srcFilePath);
		if (!fi.Exists)
		{
			UnityEngine.Debug.LogError(string.Format("WwiseUnity: Failed to copy. Source is missing: {0}.", srcFilePath));
			return false;
		}

		var di = new System.IO.DirectoryInfo(destDir);

		if (!di.Exists)
			di.Create();

		const bool IsToOverwrite = true;
		try
		{
			fi.CopyTo(System.IO.Path.Combine(di.FullName, fi.Name), IsToOverwrite);
		}
		catch (System.Exception ex)
		{
			UnityEngine.Debug.LogError(string.Format("WwiseUnity: Error during installation: {0}.", ex.Message));
			return false;
		}

		return true;
	}

	// Copy or overwrite destination file with source file.
	public static bool OverwriteFile(string srcFilePath, string destFilePath)
	{
		var fi = new System.IO.FileInfo(srcFilePath);
		if (!fi.Exists)
		{
			UnityEngine.Debug.LogError(string.Format("WwiseUnity: Failed to overwrite. Source is missing: {0}.", srcFilePath));
			return false;
		}

		var di = new System.IO.DirectoryInfo(System.IO.Path.GetDirectoryName(destFilePath));

		if (!di.Exists)
			di.Create();

		const bool IsToOverwrite = true;
		try
		{
			fi.CopyTo(destFilePath, IsToOverwrite);
		}
		catch (System.Exception ex)
		{
			UnityEngine.Debug.LogError(string.Format("WwiseUnity: Error during installation: {0}.", ex.Message));
			return false;
		}

		return true;
	}

	// Move file to destination directory and create the directory when none exists.
	public static void MoveFileToDirectory(string srcFilePath, string destDir)
	{
		var fi = new System.IO.FileInfo(srcFilePath);
		if (!fi.Exists)
		{
			UnityEngine.Debug.LogError(string.Format("WwiseUnity: Failed to move. Source is missing: {0}.", srcFilePath));
			return;
		}

		var di = new System.IO.DirectoryInfo(destDir);

		if (!di.Exists)
			di.Create();

		var destFilePath = System.IO.Path.Combine(di.FullName, fi.Name);
		try
		{
			fi.MoveTo(destFilePath);
		}
		catch (System.Exception ex)
		{
			UnityEngine.Debug.LogError(string.Format("WwiseUnity: Error during installation: {0}.", ex.Message));
		}
	}

	// Recursively copy a directory to its destination.
	public static bool RecursiveCopyDirectory(System.IO.DirectoryInfo srcDir, System.IO.DirectoryInfo destDir,
		System.Collections.Generic.List<string> excludeExtensions = null)
	{
		if (!srcDir.Exists)
		{
			UnityEngine.Debug.LogError(string.Format("WwiseUnity: Failed to copy. Source is missing: {0}.", srcDir));
			return false;
		}

		if (!destDir.Exists)
			destDir.Create();

		// Copy all files.
		var files = srcDir.GetFiles();
		foreach (var file in files)
		{
			if (excludeExtensions != null)
			{
				var fileExt = System.IO.Path.GetExtension(file.Name);
				var isFileExcluded = false;
				foreach (var ext in excludeExtensions)
				{
					if (fileExt.ToLower() == ext)
					{
						isFileExcluded = true;
						break;
					}
				}

				if (isFileExcluded)
					continue;
			}

			const bool IsToOverwrite = true;
			try
			{
				file.CopyTo(System.IO.Path.Combine(destDir.FullName, file.Name), IsToOverwrite);
			}
			catch (System.Exception ex)
			{
				UnityEngine.Debug.LogError(string.Format("WwiseUnity: Error during installation: {0}.", ex.Message));
				return false;
			}
		}

		// Process subdirectories.
		var dirs = srcDir.GetDirectories();
		foreach (var dir in dirs)
		{
			// Get destination directory.
			var destFullPath = System.IO.Path.Combine(destDir.FullName, dir.Name);

			// Recurse
			var isSuccess = RecursiveCopyDirectory(dir, new System.IO.DirectoryInfo(destFullPath), excludeExtensions);
			if (!isSuccess)
				return false;
		}

		return true;
	}
}

public class AkUnityPluginInstallerBase : AkUnityAssetsInstaller
{
	private readonly string m_progTitle = "WwiseUnity: Plugin Installation Progress";

	public bool InstallPluginByConfig(string config)
	{
		var pluginSrc = GetPluginSrcPathByConfig(config);
		var pluginDest = GetPluginDestPath("");

		var progMsg = string.Format("Installing plugin for {0} ({1}) from {2} to {3}.", m_platform, config, pluginSrc,
			pluginDest);
		UnityEditor.EditorUtility.DisplayProgressBar(m_progTitle, progMsg, 0.5f);

		var isSuccess = RecursiveCopyDirectory(new System.IO.DirectoryInfo(pluginSrc),
			new System.IO.DirectoryInfo(pluginDest), m_excludes);
		if (!isSuccess)
		{
			UnityEngine.Debug.LogError(string.Format("WwiseUnity: Failed to install plugin for {0} ({1}) from {2} to {3}.",
				m_platform, config, pluginSrc, pluginDest));
			UnityEditor.EditorUtility.ClearProgressBar();
			return false;
		}

		UnityEditor.EditorUtility.DisplayProgressBar(m_progTitle, progMsg, 1.0f);
		UnityEditor.AssetDatabase.Refresh();

		UnityEditor.EditorUtility.ClearProgressBar();
		UnityEngine.Debug.Log(string.Format("WwiseUnity: Plugin for {0} {1} installed from {2} to {3}.", m_platform, config,
			pluginSrc, pluginDest));

		return true;
	}

	public virtual bool InstallPluginByArchConfig(string arch, string config)
	{
		var pluginSrc = GetPluginSrcPathByArchConfig(arch, config);
		var pluginDest = GetPluginDestPath(arch);

		var progMsg = string.Format("Installing plugin for {0} ({1}, {2}) from {3} to {4}.", m_platform, arch, config,
			pluginSrc, pluginDest);
		UnityEditor.EditorUtility.DisplayProgressBar(m_progTitle, progMsg, 0.5f);

		var isSuccess = RecursiveCopyDirectory(new System.IO.DirectoryInfo(pluginSrc),
			new System.IO.DirectoryInfo(pluginDest), m_excludes);
		if (!isSuccess)
		{
			UnityEngine.Debug.LogError(string.Format("WwiseUnity: Failed to install plugin for {0} ({1}, {2}) from {3} to {4}.",
				m_platform, arch, config, pluginSrc, pluginDest));
			UnityEditor.EditorUtility.ClearProgressBar();
			return false;
		}

		UnityEditor.EditorUtility.DisplayProgressBar(m_progTitle, progMsg, 1.0f);
		UnityEditor.AssetDatabase.Refresh();

		UnityEditor.EditorUtility.ClearProgressBar();
		UnityEngine.Debug.Log(string.Format("WwiseUnity: Plugin for {0} {1} {2} installed from {3} to {4}.", m_platform, arch,
			config, pluginSrc, pluginDest));

		return true;
	}

	protected string GetPluginSrcPathByConfig(string config)
	{
		return System.IO.Path.Combine(
			System.IO.Path.Combine(
				System.IO.Path.Combine(System.IO.Path.Combine(System.IO.Path.Combine(m_assetsDir, "Wwise"), "Deployment"),
					"Plugins"), m_platform), config);
	}

	protected string GetPluginSrcPathByArchConfig(string arch, string config)
	{
		return System.IO.Path.Combine(
			System.IO.Path.Combine(
				System.IO.Path.Combine(
					System.IO.Path.Combine(System.IO.Path.Combine(System.IO.Path.Combine(m_assetsDir, "Wwise"), "Deployment"),
						"Plugins"), m_platform), arch), config);
	}

	protected virtual string GetPluginDestPath(string arch)
	{
		return m_pluginDir;
	}
}

public class AkUnityPluginInstallerMultiArchBase : AkUnityPluginInstallerBase
{
	protected override string GetPluginDestPath(string arch)
	{
		return System.IO.Path.Combine(System.IO.Path.Combine(m_pluginDir, m_platform), arch);
	}
}

public class AkDocHelper
{
	private static string m_WwiseVersionString = string.Empty;

	public static void OpenDoc(string platform)
	{
		if (m_WwiseVersionString == string.Empty)
		{
			var temp = AkSoundEngine.GetMajorMinorVersion();
			var temp2 = AkSoundEngine.GetSubminorBuildVersion();
			m_WwiseVersionString = (temp >> 16) + "." + (temp & 0xFFFF);
			if (temp2 >> 16 != 0)
				m_WwiseVersionString += "." + (temp2 >> 16);

			m_WwiseVersionString += "_" + (temp2 & 0xFFFF);
		}

		var docUrl = "http://www.audiokinetic.com/library/" + m_WwiseVersionString + "/?source=Unity&id=main.html";
		var isConnected = false;
		try
		{
			var request = (System.Net.HttpWebRequest) System.Net.WebRequest.Create("http://www.audiokinetic.com/robots.txt");
			request.Timeout = 1000;
			request.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
			var response = (System.Net.HttpWebResponse) request.GetResponse();

			isConnected = response.StatusCode == System.Net.HttpStatusCode.OK;
		}
		catch (System.Exception)
		{
			isConnected = false;
		}

		if (!isConnected)
		{
			// Can't access audiokinetic.com, open local doc.
			docUrl = GetLocalDocUrl(platform);
			if (string.IsNullOrEmpty(docUrl))
				return;
		}

		UnityEngine.Application.OpenURL(docUrl);
	}

	private static string GetLocalDocUrl(string platform)
	{
		var docUrl = string.Empty;
		var docPath = string.Empty;
		var dataPath = UnityEngine.Application.dataPath;

#if UNITY_EDITOR_WIN
		var format = (platform == "Windows")
			? "{0}/Wwise/Documentation/{1}/en/WwiseUnityIntegrationHelp_en.chm"
			: "{0}/Wwise/Documentation/{1}/en/WwiseUnityIntegrationHelp_{1}_en.chm";

		docPath = string.Format(format, dataPath, platform);
#else
		string DestPath = AkUtilities.GetFullPath(dataPath, "../WwiseUnityIntegrationHelp_en");
		docPath = string.Format ("{0}/html/index.html", DestPath);
		if (!System.IO.File.Exists(docPath))
			UnzipHelp(DestPath);

		if (!System.IO.File.Exists(docPath))
		{
			UnityEngine.Debug.Log("WwiseUnity: Unable to show documentation. Please unzip WwiseUnityIntegrationHelp_AppleCommon_en.zip manually.");
			return string.Empty;
		}
#endif

		var fi = new System.IO.FileInfo(docPath);
		if (!fi.Exists)
		{
			UnityEngine.Debug.LogError(string.Format("WwiseUnity: Failed to find documentation: {0}. Aborted.", docPath));
			return string.Empty;
		}

		docUrl = string.Format("file:///{0}", docPath.Replace(" ", "%20"));

		return docUrl;
	}

	public static void UnzipHelp(string DestPath)
	{
		// Start by extracting the zip, if it exists
		var ZipPath = System.IO.Path.Combine(
			System.IO.Path.Combine(
				System.IO.Path.Combine(
					System.IO.Path.Combine(System.IO.Path.Combine(UnityEngine.Application.dataPath, "Wwise"), "Documentation"),
					"AppleCommon"), "en"), "WwiseUnityIntegrationHelp_en.zip");

		if (System.IO.File.Exists(ZipPath))
		{
			var start = new System.Diagnostics.ProcessStartInfo();
			start.FileName = "unzip";

			start.Arguments = "\"" + ZipPath + "\" -d \"" + DestPath + "\"";

			start.UseShellExecute = true;
			start.RedirectStandardOutput = false;

			var progMsg = "WwiseUnity: Unzipping documentation...";
			var progTitle = "Unzipping Wwise documentation";
			UnityEditor.EditorUtility.DisplayProgressBar(progTitle, progMsg, 0.5f);

			using (var process = System.Diagnostics.Process.Start(start))
			{
				while (!process.WaitForExit(1000))
					System.Threading.Thread.Sleep(100);
				try
				{
					//ExitCode throws InvalidOperationException if the process is hanging
					var returnCode = process.ExitCode;

					var isBuildSucceeded = returnCode == 0;
					if (isBuildSucceeded)
					{
						UnityEditor.EditorUtility.DisplayProgressBar(progTitle, progMsg, 1.0f);
						UnityEngine.Debug.Log("WwiseUnity: Documentation extraction succeeded. ");
					}
					else
						UnityEngine.Debug.LogError("WwiseUnity: Extraction failed.");

					UnityEditor.EditorUtility.ClearProgressBar();
				}
				catch (System.Exception ex)
				{
					UnityEditor.EditorUtility.ClearProgressBar();
					UnityEngine.Debug.LogError(ex.ToString());
					UnityEditor.EditorUtility.ClearProgressBar();
				}
			}
		}
	}
}

#endif // #if UNITY_EDITOR
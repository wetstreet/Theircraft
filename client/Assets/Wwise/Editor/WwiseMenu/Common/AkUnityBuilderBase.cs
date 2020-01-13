#if UNITY_EDITOR

public class AkUnityIntegrationBuilderBase
{
	private readonly string m_progTitle = "WwiseUnity: Rebuilding Unity Integration Progress";
	protected string m_assetsDir = "Undefined";
	protected string m_assetsPluginsDir = "Undefined";
	protected string m_buildScriptDir = "Undefined";
	protected string m_buildScriptFile = "Undefined";
	protected string m_platform = "Undefined";
	protected string m_shell = "python";
	protected string m_wwiseSdkDir = "";

	public AkUnityIntegrationBuilderBase()
	{
		var unityProjectRoot = System.IO.Directory.GetCurrentDirectory();
		m_assetsDir = System.IO.Path.Combine(unityProjectRoot, "Assets");
		m_assetsPluginsDir = System.IO.Path.Combine(m_assetsDir, "Plugins");
		m_buildScriptDir =
			System.IO.Path.Combine(System.IO.Path.Combine(System.IO.Path.Combine(m_assetsDir, "Wwise"), "AkSoundEngine"),
				"Common");
		m_buildScriptFile = "BuildWwiseUnityIntegration.py";
	}

	public void BuildByConfig(string config, string arch)
	{
		if (UnityEditor.EditorApplication.isPlaying)
		{
			UnityEngine.Debug.LogWarning("WwiseUnity: Editor is in play mode. Stop playing any scenes and retry. Aborted.");
			return;
		}

		// Try to parse config to get Wwise location.
		var configPath = System.IO.Path.Combine(m_buildScriptDir, "BuildWwiseUnityIntegration.json");
		var fi = new System.IO.FileInfo(configPath);
		if (fi.Exists)
		{
			var msg = string.Format("WwiseUnity: Found preference file: {0}. Use build variables defined in it.", configPath);
			UnityEngine.Debug.Log(msg);
		}
		else
		{
			var msg = string.Format("WwiseUnity: Preference file: {0} is unavailable. Need user input.", configPath);
			UnityEngine.Debug.Log(msg);

			m_wwiseSdkDir = UnityEditor.EditorUtility.OpenFolderPanel("Choose Wwise SDK folder", ".", "");

			var isUserCancelledBuild = m_wwiseSdkDir == "";
			if (isUserCancelledBuild)
			{
				UnityEngine.Debug.Log("WwiseUnity: User cancelled the build.");
				return;
			}
		}

		if (!PreBuild())
			return;

		// On Windows, separate shell console window will open. When building is done, close the Window yourself if it stays active. Usually at the end you will see the last line says "Build succeeded" or "Build failed".
		var progMsg = string.Format("WwiseUnity: Rebuilding Wwise Unity Integration for {0} ({1}) ...", m_platform, config);
		UnityEngine.Debug.Log(progMsg);

		var start = new System.Diagnostics.ProcessStartInfo();
		start.FileName = m_shell;

		start.Arguments = GetProcessArgs(config, arch);
		if (start.Arguments == "")
			return;
		start.UseShellExecute = false;
		start.RedirectStandardOutput = true;

		UnityEditor.EditorUtility.DisplayProgressBar(m_progTitle, progMsg, 0.5f);

		using (var process = System.Diagnostics.Process.Start(start))
		{
			using (var reader = process.StandardOutput)
			{
				process.WaitForExit();

				try
				{
					//ExitCode throws InvalidOperationException if the process is hanging

					var isBuildSucceeded = process.ExitCode == 0;
					if (isBuildSucceeded)
					{
						UnityEditor.EditorUtility.DisplayProgressBar(m_progTitle, progMsg, 1.0f);
						UnityEngine.Debug.Log("WwiseUnity: Build succeeded. Check detailed logs under the Logs folder.");
					}
					else
						UnityEngine.Debug.LogError("WwiseUnity: Build failed. Check detailed logs under the Logs folder.");

					UnityEditor.AssetDatabase.Refresh();

					UnityEditor.EditorUtility.ClearProgressBar();
				}
				catch (System.Exception ex)
				{
					UnityEditor.AssetDatabase.Refresh();

					UnityEngine.Debug.LogError(string.Format(
						"WwiseUnity: Build process failed with exception: {}. Check detailed logs under the Logs folder.", ex));
					UnityEditor.EditorUtility.ClearProgressBar();
				}
			}
		}
	}

	protected virtual string GetProcessArgs(string config, string arch)
	{
		var scriptPath = System.IO.Path.Combine(m_buildScriptDir, m_buildScriptFile);
		var args = string.Format("\"{0}\" -p {1} -c {2}", scriptPath, m_platform, config);
		if (arch != null)
			args += string.Format(" -a {0}", arch);

		if (m_wwiseSdkDir != "")
			args += string.Format(" -w \"{0}\" -u", m_wwiseSdkDir);

		return args;
	}

	protected virtual bool PreBuild()
	{
		return true;
	}
}

#endif // #if UNITY_EDITOR
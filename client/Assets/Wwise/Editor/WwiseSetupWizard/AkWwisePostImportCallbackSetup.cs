#if UNITY_EDITOR
[UnityEditor.InitializeOnLoad]
public class AkWwisePostImportCallbackSetup
{
	private static int m_scheduledMigrationStart;
	private static bool m_scheduledReturnToLauncher;
	private static bool m_pendingExecuteMethodCalled;
	private static string s_CurrentScene;

	static AkWwisePostImportCallbackSetup()
	{
		var arguments = System.Environment.GetCommandLineArgs();
		if (System.Array.IndexOf(arguments, "-nographics") != -1 &&
		    System.Array.IndexOf(arguments, "-wwiseEnableWithNoGraphics") == -1)
			return;

		UnityEditor.EditorApplication.delayCall += CheckMigrationStatus;
	}

	private static void CheckMigrationStatus()
	{
		try
		{
			int migrationStart;
			bool returnToLauncher;

			if (IsMigrationPending(out migrationStart, out returnToLauncher))
			{
				m_scheduledMigrationStart = migrationStart;
				m_scheduledReturnToLauncher = returnToLauncher;
				ScheduleMigration();
			}
			else
				RefreshCallback();
		}
		catch (System.Exception e)
		{
			UnityEngine.Debug.LogError("WwiseUnity: Error during migration: " + e);
		}
	}

	private static void ScheduleMigration()
	{
		// TODO: Is delayCall wiped out during a script reload?
		// If not, guard against having a delayCall from a previously loaded code being run after the new loading.

		if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode || UnityEditor.EditorApplication.isCompiling)
		{
			// Skip if not in the right mode, wait for the next callback to see if we can proceed then.
			UnityEditor.EditorApplication.delayCall += ScheduleMigration;
			return;
		}

		try
		{
			WwiseSetupWizard.PerformMigration(m_scheduledMigrationStart);

			// Force the user to return to the launcher to perform the post-installation process if necessary
			if (m_scheduledReturnToLauncher)
			{
				if (UnityEditor.EditorUtility.DisplayDialog("Wwise Migration Successful!",
					"Please close Unity and go back to the Wwise Launcher to complete the installation.", "Quit"))
					UnityEditor.EditorApplication.Exit(0);
			}
			else
				UnityEditor.EditorApplication.delayCall += RefreshCallback;
		}
		catch (System.Exception e)
		{
			UnityEngine.Debug.LogError("WwiseUnity: Error during migration: " + e);
		}
	}

	private static bool IsMigrationPending(out int migrationStart, out bool returnToLauncher)
	{
		migrationStart = 0;
		returnToLauncher = false;

		var filename = UnityEngine.Application.dataPath + "/../.WwiseLauncherLockFile";

		if (!System.IO.File.Exists(filename))
			return false;

		var fileContent = System.IO.File.ReadAllText(filename);

		// Instantiate the regular expression object.
		var r = new System.Text.RegularExpressions.Regex(
			"{\"migrateStart\":(\\d+),\"migrateStop\":(\\d+)(,\"returnToLauncher\":(true|false))?,.*}",
			System.Text.RegularExpressions.RegexOptions.IgnoreCase);

		// Match the regular expression pattern against a text string.
		var m = r.Match(fileContent);

		if (!m.Success || m.Groups.Count < 2 || m.Groups[1].Captures.Count < 1 || m.Groups[2].Captures.Count < 1 ||
		    !int.TryParse(m.Groups[1].Captures[0].ToString(), out migrationStart))
			throw new System.Exception("Error in the file format of .WwiseLauncherLockFile.");

		// Handle optional properties
		if (m.Groups.Count > 3 && m.Groups[4].Captures.Count > 0)
			bool.TryParse(m.Groups[4].Captures[0].ToString(), out returnToLauncher);

		return true;
	}

	private static void RefreshCallback()
	{
		PostImportFunction();
		if (System.IO.File.Exists(System.IO.Path.Combine(UnityEngine.Application.dataPath,
			WwiseSettings.WwiseSettingsFilename)))
		{
			AkPluginActivator.Update();
			AkPluginActivator.ActivatePluginsForEditor();
		}
	}

	private static void PostImportFunction()
	{
#if UNITY_2018_1_OR_NEWER
        UnityEditor.EditorApplication.hierarchyChanged += CheckWwiseGlobalExistance;
#else
        UnityEditor.EditorApplication.hierarchyWindowChanged += CheckWwiseGlobalExistance;
#endif
        UnityEditor.EditorApplication.delayCall += CheckPicker;

		if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode || UnityEditor.EditorApplication.isCompiling)
			return;

		try
		{
			if (System.IO.File.Exists(UnityEngine.Application.dataPath + System.IO.Path.DirectorySeparatorChar +
			                          WwiseSettings.WwiseSettingsFilename))
			{
				WwiseSetupWizard.Settings = WwiseSettings.LoadSettings();
				AkWwiseProjectInfo.GetData();
			}

			if (!string.IsNullOrEmpty(WwiseSetupWizard.Settings.WwiseProjectPath))
			{
				AkWwisePicker.PopulateTreeview();
				if (AkWwiseProjectInfo.GetData().autoPopulateEnabled)
					AkWwiseWWUBuilder.StartWWUWatcher();
			}
		}
		catch (System.Exception e)
		{
			UnityEngine.Debug.Log(e.ToString());
		}

		//Check if a WwiseGlobal object exists in the current scene	
		CheckWwiseGlobalExistance();
	}

	private static void RefreshPlugins()
	{
		if (string.IsNullOrEmpty(AkWwiseProjectInfo.GetData().CurrentPluginConfig))
			AkWwiseProjectInfo.GetData().CurrentPluginConfig = AkPluginActivator.CONFIG_PROFILE;

		AkPluginActivator.ActivatePluginsForEditor();
	}

	public static void CheckPicker()
	{
		if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode || UnityEditor.EditorApplication.isCompiling)
		{
			// Skip if not in the right mode, wait for the next callback to see if we can proceed then.
			UnityEditor.EditorApplication.delayCall += CheckPicker;
			return;
		}

		var settings = WwiseSettings.LoadSettings();
		if (!settings.CreatedPicker)
		{
			// Delete all the ghost tabs (Failed to load).
			var windows = UnityEngine.Resources.FindObjectsOfTypeAll<UnityEditor.EditorWindow>();
			if (windows != null && windows.Length > 0)
			{
				foreach (var window in windows)
				{
					var windowTitle = window.titleContent.text;

					if (windowTitle.Equals("Failed to load") || windowTitle.Equals("AkWwisePicker"))
					{
						try
						{
							window.Close();
						}
						catch (System.Exception)
						{
							// Do nothing here, this shoudn't cause any problem, however there has been
							// occurences of Unity crashing on a null reference inside that method.
						}
					}
				}
			}

			// TODO: If no scene is loaded and we are using the demo scene, automatically load it to display it.

			// Populate the picker
			AkWwiseProjectInfo.GetData(); // Load data
			if (!string.IsNullOrEmpty(settings.WwiseProjectPath))
			{
				AkWwiseProjectInfo.Populate();
				AkWwisePicker.init();

				if (AkWwiseProjectInfo.GetData().autoPopulateEnabled)
					AkWwiseWWUBuilder.StartWWUWatcher();

				settings.CreatedPicker = true;
				WwiseSettings.SaveSettings(settings);
			}
		}

		UnityEditor.EditorApplication.delayCall += CheckPendingExecuteMethod;
	}

	// TODO: Put this in AkUtilities?
	private static void ExecuteMethod(string method)
	{
		string className = null;
		string methodName = null;

		var r = new System.Text.RegularExpressions.Regex("(.+)\\.(.+)",
			System.Text.RegularExpressions.RegexOptions.IgnoreCase);

		var m = r.Match(method);

		if (!m.Success || m.Groups.Count < 3 || m.Groups[1].Captures.Count < 1 || m.Groups[2].Captures.Count < 1)
		{
			UnityEngine.Debug.LogError("WwiseUnity: Error parsing wwiseExecuteMethod parameter: " + method);
			return;
		}

		className = m.Groups[1].Captures[0].ToString();
		methodName = m.Groups[2].Captures[0].ToString();

		try
		{
			var type = System.Type.GetType(className);
			var clearMethod = type.GetMethod(methodName,
				System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
			clearMethod.Invoke(null, null);
		}
		catch (System.Exception e)
		{
			UnityEngine.Debug.LogError("WwiseUnity: Exception caught when calling " + method + ": " + e);
		}
	}

	private static void CheckPendingExecuteMethod()
	{
		var arguments = System.Environment.GetCommandLineArgs();
		var indexOfCommand = System.Array.IndexOf(arguments, "-wwiseExecuteMethod");

		if (!m_pendingExecuteMethodCalled && indexOfCommand != -1 && arguments.Length > indexOfCommand + 1)
		{
			var methodToExecute = arguments[indexOfCommand + 1];

			ExecuteMethod(methodToExecute);
			m_pendingExecuteMethodCalled = true;
		}
	}

	// Called when changes are made to the scene and when a new scene is created.
	public static void CheckWwiseGlobalExistance()
	{
		var settings = WwiseSettings.LoadSettings();
		var activeSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
		if (string.IsNullOrEmpty(s_CurrentScene) || !s_CurrentScene.Equals(activeSceneName))
		{
			// Look for a game object which has the initializer component
			var AkInitializers = UnityEngine.Object.FindObjectsOfType<AkInitializer>();
			if (AkInitializers.Length == 0)
			{
				if (settings.CreateWwiseGlobal)
				{
					//No Wwise object in this scene, create one so that the sound engine is initialized and terminated properly even if the scenes are loaded
					//in the wrong order.
					var objWwise = new UnityEngine.GameObject("WwiseGlobal");

					//Attach initializer and terminator components
					UnityEditor.Undo.AddComponent<AkInitializer>(objWwise);
				}
			}
			else if (settings.CreateWwiseGlobal == false && AkInitializers[0].gameObject.name == "WwiseGlobal")
					UnityEditor.Undo.DestroyObjectImmediate(AkInitializers[0].gameObject);

			if (settings.CreateWwiseListener)
			{
				AkUtilities.AddAkAudioListenerToMainCamera(true);
			}

			s_CurrentScene = activeSceneName;
		}
	}
}

#endif // UNITY_EDITOR
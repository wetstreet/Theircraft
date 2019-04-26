#if UNITY_EDITOR

[UnityEditor.InitializeOnLoad]
public class WwiseSetupWizard
{
	public static WwiseSettings Settings;

	static WwiseSetupWizard()
	{
		try
		{
			Settings = WwiseSettings.LoadSettings();
		}
		catch (System.Exception e)
		{
			UnityEngine.Debug.LogError("WwiseUnity: Failed to load the settings, exception caught: " + e);
		}
	}

	public static void RunModify()
	{
		try
		{
			UnityEngine.Debug.Log("WwiseUnity: Running modify setup...");

			UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects);

			ModifySetup();

			UnityEngine.Debug.Log("WwiseUnity: Refreshing asset database.");
			UnityEditor.AssetDatabase.Refresh();

            // IMPORTANT: This log line is parsed by the Launcher. Do not modify it.
			UnityEngine.Debug.Log("WwiseUnity: End of setup, exiting Unity.");
		}
		catch (System.Exception e)
		{
			UnityEngine.Debug.LogError("WwiseUnity: Exception caught: " + e);
		}
	}

	public static void RunSetup()
	{
		try
		{
			UnityEngine.Debug.Log("WwiseUnity: Running install setup...");

			Setup();

			UnityEngine.Debug.Log("WwiseUnity: Refreshing asset database.");
			UnityEditor.AssetDatabase.Refresh();

            // IMPORTANT: This log line is parsed by the Launcher. Do not modify it.
			UnityEngine.Debug.Log("WwiseUnity: End of setup, exiting Unity.");
		}
		catch (System.Exception e)
		{
			UnityEngine.Debug.LogError("WwiseUnity: Exception caught: " + e);
		}
	}

	public static void RunDemoSceneSetup()
	{
		try
		{
			UnityEngine.Debug.Log("WwiseUnity: Running demo scene setup...");

			Setup();

			UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/WwiseDemoScene/WwiseDemoScene.unity");

			UnityEngine.Debug.Log("WwiseUnity: Refreshing asset database.");
			UnityEditor.AssetDatabase.Refresh();

			UnityEngine.Debug.Log("WwiseUnity: End of demo scene setup, exiting Unity.");
		}
		catch (System.Exception e)
		{
			UnityEngine.Debug.LogError("WwiseUnity: Exception caught: " + e);
		}
	}

	private static int TotalNumberOfSections = 3;
	private static void UpdateProgressBar(int sectionIndex, int subSectionIndex = 0, int totalSubSections = 1)
	{
		subSectionIndex = UnityEngine.Mathf.Clamp(subSectionIndex, 0, totalSubSections);
		sectionIndex = UnityEngine.Mathf.Clamp(sectionIndex, 0, TotalNumberOfSections);

		float progress = ((float)subSectionIndex / totalSubSections + sectionIndex) / TotalNumberOfSections;
		UnityEditor.EditorUtility.DisplayProgressBar("Wwise Integration", "Migration in progress - Please wait...", progress);
	}

	public static void RunMigrate()
	{
		try
		{
			UnityEngine.Debug.Log("WwiseUnity: Running migration setup...");

			UnityEngine.Debug.Log("WwiseUnity: Reading parameters...");

			var arguments = System.Environment.GetCommandLineArgs();
			string migrateStartString = null;
			var indexMigrateStart = System.Array.IndexOf(arguments, "-wwiseInstallMigrateStart");

			if (indexMigrateStart != -1)
				migrateStartString = arguments[indexMigrateStart + 1];
			else
			{
				UnityEngine.Debug.LogError("WwiseUnity: ERROR: Missing parameter wwiseInstallMigrateStart.");
                return;
			}

			int migrateStart;

			if (!int.TryParse(migrateStartString, out migrateStart))
			{
				UnityEngine.Debug.LogError("WwiseUnity: ERROR: wwiseInstallMigrateStart is not a number.");
				return;
			}

			PerformMigration(migrateStart - 1);

			UnityEditor.AssetDatabase.SaveAssets();

			UnityEngine.Debug.Log("WwiseUnity: Refreshing asset database.");
			UnityEditor.AssetDatabase.Refresh();

            // IMPORTANT: This log line is parsed by the Launcher. Do not modify it.
			UnityEngine.Debug.Log("WwiseUnity: End of setup, exiting Unity.");
		}
		catch (System.Exception e)
		{
			UnityEngine.Debug.LogError("WwiseUnity: Exception caught: " + e);
		}
	}

	private static void MigrateCurrentScene(System.Type[] wwiseComponentTypes)
	{
		var objectTypeMap = new System.Collections.Generic.Dictionary<System.Type, UnityEngine.Object[]>();

		foreach (var objectType in wwiseComponentTypes)
		{
			// Get all objects in the scene with the specified type.
			var objects = UnityEngine.Object.FindObjectsOfType(objectType);
			if (objects != null && objects.Length > 0)
				objectTypeMap[objectType] = objects;
		}

		for (var ii = AkUtilities.MigrationStartIndex; ii < AkUtilities.MigrationStopIndex; ++ii)
		{
			var migrationMethodName = "Migrate" + ii;
			var preMigrationMethodName = "PreMigration" + ii;
			var postMigrationMethodName = "PostMigration" + ii;

			foreach (var objectTypePair in objectTypeMap)
			{
				var objectType = objectTypePair.Key;
				var objects = objectTypePair.Value;
				var className = objectType.Name;

				var preMigrationMethodInfo = objectType.GetMethod(preMigrationMethodName,
					System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
				if (preMigrationMethodInfo != null)
				{
					UnityEngine.Debug.Log("WwiseUnity: PreMigration step <" + ii + "> for class <" + className + ">");
					preMigrationMethodInfo.Invoke(null, null);
				}

				var migrationMethodInfo = objectType.GetMethod(migrationMethodName,
					System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
				if (migrationMethodInfo != null)
				{
					UnityEngine.Debug.Log("WwiseUnity: Migration step <" + ii + "> for class <" + className + ">");

					// Call the migration method of each object.
					foreach (var currentObject in objects)
						migrationMethodInfo.Invoke(currentObject, null);
				}

				var postMigrationMethodInfo = objectType.GetMethod(postMigrationMethodName,
					System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
				if (postMigrationMethodInfo != null)
				{
					UnityEngine.Debug.Log("WwiseUnity: PostMigration step <" + ii + "> for class <" + className + ">");
					postMigrationMethodInfo.Invoke(null, null);
				}
			}
		}
	}

	private static System.Type[] GetWwiseComponentTypes()
	{
		var wwiseComponentTypes = new System.Collections.Generic.List<System.Type>();

		var wwiseComponentFolder = UnityEngine.Application.dataPath + "/Wwise/Deployment/Components";
		var files = new System.IO.DirectoryInfo(wwiseComponentFolder).GetFiles("*.cs", System.IO.SearchOption.AllDirectories);
		foreach (var file in files)
		{
			var className = System.IO.Path.GetFileNameWithoutExtension(file.Name);
			var objectType = System.Type.GetType(className + ", Assembly-CSharp");
			if (objectType != null && objectType.IsSubclassOf(typeof(UnityEngine.Object)))
				wwiseComponentTypes.Add(objectType);
		}

		return wwiseComponentTypes.ToArray();
	}

	private static void MigrateObject(UnityEngine.Object obj)
	{
		if (obj == null)
		{
			UnityEngine.Debug.LogWarning("WwiseUnity: Missing script! Please consider resolving the missing scripts before migrating your Unity project. Any WwiseType on this object will NOT be migrated!");
			return;
		}

		var migratable = obj as AK.Wwise.IMigratable;
		if (migratable == null && !AkUtilities.IsMigrationRequired(AkUtilities.MigrationStep.WwiseTypes_v2018_1_6))
			return;

		var hasChanged = false;
		var serializedObject = new UnityEditor.SerializedObject(obj);
		if (migratable != null)
			hasChanged = migratable.Migrate(serializedObject);
		else
			hasChanged = AK.Wwise.TypeMigration.SearchAndProcessWwiseTypes(serializedObject.GetIterator());

		if (hasChanged)
			serializedObject.ApplyModifiedPropertiesWithoutUndo();
	}

	private static void MigratePrefabs()
	{
		var guids = UnityEditor.AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });
		for (var i = 0; i < guids.Length; i++)
		{
			UpdateProgressBar(0, i, guids.Length);

			var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
			UnityEngine.Debug.Log("WwiseUnity: Migrating prefab: " + path);

			var prefabObject = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>(path);
			if (prefabObject == null)
			{
				UnityEngine.Debug.LogWarning("WwiseUnity: Failed to migrate prefab: " + path);
				continue;
			}

			var objects = prefabObject.GetComponents<UnityEngine.MonoBehaviour>();

#if UNITY_2018_3_OR_NEWER
			var instanceIds = new System.Collections.Generic.List<int>();
			foreach (var obj in objects)
			{
				if (obj == null)
					continue;

				var id = obj.GetInstanceID();
				if (!instanceIds.Contains(id))
					instanceIds.Add(id);
			}

			for (; instanceIds.Count > 0; instanceIds.RemoveAt(0))
			{
				var id = instanceIds[0];
				objects = prefabObject.GetComponents<UnityEngine.MonoBehaviour>();
				foreach (var obj in objects)
				{
					if (obj && obj.GetInstanceID() == id)
					{
						MigrateObject(obj);
						break;
					}
				}
			}
#else
			foreach (var obj in objects)
				MigrateObject(obj);
#endif
		}
	}

	private static string[] ScriptableObjectGuids = null;

	private static void MigrateScriptableObjects()
	{
		var guids = ScriptableObjectGuids;
		if (guids == null)
			return;

		var processedGuids = new System.Collections.Generic.HashSet<string>();

		for (var i = 0; i < guids.Length; i++)
		{
			UpdateProgressBar(2, i, guids.Length);

			var guid = guids[i];
			if (processedGuids.Contains(guid))
				continue;

			processedGuids.Add(guid);

			var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
			UnityEngine.Debug.Log("WwiseUnity: Migrating ScriptableObject: " + path);

			var objects = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(path);
			foreach (var obj in objects)
				if (obj == null || obj is UnityEngine.ScriptableObject)
					MigrateObject(obj);
		}
	}

	private static void MigrateScenes()
	{
		var wwiseComponentTypes = GetWwiseComponentTypes();
		var guids = UnityEditor.AssetDatabase.FindAssets("t:Scene", new[] { "Assets" });
		for (var i = 0; i < guids.Length; i++)
		{
			UpdateProgressBar(1, i, guids.Length);

			var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
			UnityEngine.Debug.Log("WwiseUnity: Migrating scene: " + path);

			UnityEditor.EditorUtility.UnloadUnusedAssetsImmediate();

			var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(path);

			MigrateCurrentScene(wwiseComponentTypes);

			var objects = UnityEngine.Resources.FindObjectsOfTypeAll<UnityEngine.MonoBehaviour>();

#if UNITY_2018_3_OR_NEWER
			var instanceIds = new System.Collections.Generic.List<int>();
			foreach (var obj in objects)
			{
				if (obj == null)
					continue;

				var id = obj.GetInstanceID();
				if (!instanceIds.Contains(id))
					instanceIds.Add(id);
			}

			for (; instanceIds.Count > 0; instanceIds.RemoveAt(0))
			{
				var id = instanceIds[0];
				objects = UnityEngine.Resources.FindObjectsOfTypeAll<UnityEngine.MonoBehaviour>();
				foreach (var obj in objects)
				{
					if (obj && obj.GetInstanceID() == id)
					{
						MigrateObject(obj);
						break;
					}
				}
			}
#else
			foreach (var obj in objects)
			{
				var isPrefabInstance = false;
				if (obj != null)
				{
#if UNITY_2018_2
					isPrefabInstance = UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource(obj) != null;
#else
					isPrefabInstance = UnityEditor.PrefabUtility.GetPrefabParent(obj) != null;
#endif

					if (!isPrefabInstance)
					{
						var isSceneObject = !UnityEditor.EditorUtility.IsPersistent(obj);
						var isEditableAndSavable = (obj.hideFlags & (UnityEngine.HideFlags.NotEditable | UnityEngine.HideFlags.DontSave)) == 0;
						if (!isSceneObject || !isEditableAndSavable)
							continue;
					}
				}

				MigrateObject(obj);

				if (isPrefabInstance)
					UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(obj);
			}
#endif

			if (UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene))
				if (!UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene))
					throw new System.Exception("Error occurred while saving migrated scenes.");
		}
	}

	public static void PerformMigration(int migrateStart)
	{
		UpdateProgressBar(0);

		UnityEngine.Debug.Log("WwiseUnity: Migrating from Unity Integration Version " + migrateStart + " to " + AkUtilities.MigrationStopIndex);

		AkPluginActivator.DeactivateAllPlugins();
		AkPluginActivator.Update();
		AkPluginActivator.ActivatePluginsForEditor();

		// Get the name of the currently opened scene.
		var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
		var loadedScenePath = activeScene.path;
		
		if (!string.IsNullOrEmpty(loadedScenePath))
			AkBasePathGetter.FixSlashes(ref loadedScenePath, '\\', '/', false);

		UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects);

		// obtain a list of ScriptableObjects before any migration is performed
		ScriptableObjectGuids = UnityEditor.AssetDatabase.FindAssets("t:ScriptableObject", new[] { "Assets" });

		AkUtilities.BeginMigration(migrateStart);
		AkWwiseProjectInfo.GetData().Migrate();
		AkWwiseWWUBuilder.UpdateWwiseObjectReferenceData();

		MigratePrefabs();
		MigrateScenes();
		MigrateScriptableObjects();

		UnityEditor.EditorUtility.UnloadUnusedAssetsImmediate();

		UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects);
		AkUtilities.EndMigration();

		UpdateProgressBar(TotalNumberOfSections);

		// Reopen the scene that was opened before the migration process started.
		if (!string.IsNullOrEmpty(loadedScenePath))
			UnityEditor.SceneManagement.EditorSceneManager.OpenScene(loadedScenePath);

		UnityEngine.Debug.Log("WwiseUnity: Removing lock for launcher.");

		// TODO: Moving one folder up is not nice at all. How to find the current project path?
		try
		{
			System.IO.File.Delete(UnityEngine.Application.dataPath + "/../.WwiseLauncherLockFile");
		}
		catch (System.Exception)
		{
			// Ignore if not present.
		}

		UnityEditor.EditorUtility.ClearProgressBar();
	}

	public static void ModifySetup()
	{
		var currentConfig = AkPluginActivator.GetCurrentConfig();

		if (string.IsNullOrEmpty(currentConfig))
			currentConfig = AkPluginActivator.CONFIG_PROFILE;

		AkPluginActivator.DeactivateAllPlugins();
		AkPluginActivator.Update();
		AkPluginActivator.ActivatePluginsForEditor();
	}

	// Perform all necessary steps to use the Wwise Unity integration.
	private static void Setup()
	{
		UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects);

		AkPluginActivator.DeactivateAllPlugins();

		// 0. Make sure the soundbank directory exists
		var sbPath = AkUtilities.GetFullPath(UnityEngine.Application.streamingAssetsPath, Settings.SoundbankPath);
		if (!System.IO.Directory.Exists(sbPath))
			System.IO.Directory.CreateDirectory(sbPath);

		// 1. Disable built-in audio
		if (!DisableBuiltInAudio())
		{
			UnityEngine.Debug.LogWarning(
				"WwiseUnity: Could not disable built-in audio. Please disable built-in audio by going to Project->Project Settings->Audio, and check \"Disable Audio\".");
		}

		// 2. Create a "WwiseGlobal" game object and set the AkSoundEngineInitializer and terminator scripts
		// 3. Set the SoundBank path property on AkSoundEngineInitializer
		CreateWwiseGlobalObject();

		// 5. Disable the built-in audio listener, and add AkAudioListener component to camera
		if (WwiseSettings.LoadSettings().CreateWwiseListener)
		{
			AkUtilities.AddAkAudioListenerToMainCamera();
		}

		// 6. Enable "Run In Background" in PlayerSettings (PlayerSettings.runInbackground property)
		UnityEditor.PlayerSettings.runInBackground = true;

		AkPluginActivator.Update();
		AkPluginActivator.ActivatePluginsForEditor();

		// 9. Activate WwiseIDs file generation, and point Wwise to the Assets/Wwise folder
		// 10. Change the SoundBanks options so it adds Max Radius information in the Wwise project
		if (!SetSoundbankSettings())
			UnityEngine.Debug.LogWarning("WwiseUnity: Could not modify Wwise Project to generate the header file!");

		// 11. Activate XboxOne network sockets.
		AkXboxOneUtils.EnableXboxOneNetworkSockets();
	}

	// Create a Wwise Global object containing the initializer and terminator scripts. Set the soundbank path of the initializer script.
	// This game object will live for the whole project; there is no need to instanciate one per scene.
	private static void CreateWwiseGlobalObject()
	{
		// Look for a game object which has the initializer component
		var AkInitializers = UnityEngine.Object.FindObjectsOfType<AkInitializer>();
		if (AkInitializers.Length > 0)
			UnityEditor.Undo.DestroyObjectImmediate(AkInitializers[0].gameObject);

		var WwiseGlobalGameObject = new UnityEngine.GameObject("WwiseGlobal");

		// attach initializer component
		UnityEditor.Undo.AddComponent<AkInitializer>(WwiseGlobalGameObject);

		// Set focus on WwiseGlobal
		UnityEditor.Selection.activeGameObject = WwiseGlobalGameObject;
	}

	private static bool DisableBuiltInAudio()
	{
		UnityEditor.SerializedObject audioSettingsAsset = null;
		UnityEditor.SerializedProperty disableAudioProperty = null;

		var assets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/AudioManager.asset");
		if (assets.Length > 0)
			audioSettingsAsset = new UnityEditor.SerializedObject(assets[0]);

		if (audioSettingsAsset != null)
			disableAudioProperty = audioSettingsAsset.FindProperty("m_DisableAudio");

		if (disableAudioProperty == null)
			return false;

		disableAudioProperty.boolValue = true;
		audioSettingsAsset.ApplyModifiedProperties();
		return true;
	}

	// Modify the .wproj file to set needed soundbank settings
	private static bool SetSoundbankSettings()
	{
		if (string.IsNullOrEmpty(Settings.WwiseProjectPath))
			return true;

		var r = new System.Text.RegularExpressions.Regex("_WwiseIntegrationTemp.*?([/\\\\])");
		var SoundbankPath = AkUtilities.GetFullPath(r.Replace(UnityEngine.Application.streamingAssetsPath, "$1"),
			Settings.SoundbankPath);
		var WprojPath = AkUtilities.GetFullPath(UnityEngine.Application.dataPath, Settings.WwiseProjectPath);
#if UNITY_EDITOR_OSX
		SoundbankPath = "Z:" + SoundbankPath;
#endif

		SoundbankPath = AkUtilities.MakeRelativePath(System.IO.Path.GetDirectoryName(WprojPath), SoundbankPath);
		if (AkUtilities.EnableBoolSoundbankSettingInWproj("SoundBankGenerateHeaderFile", WprojPath))
			if (AkUtilities.SetSoundbankHeaderFilePath(WprojPath, SoundbankPath))
				return AkUtilities.EnableBoolSoundbankSettingInWproj("SoundBankGenerateMaxAttenuationInfo", WprojPath);

		return false;
	}
}

#endif // UNITY_EDITOR
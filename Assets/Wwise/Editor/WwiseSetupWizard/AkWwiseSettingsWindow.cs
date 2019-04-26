#if UNITY_EDITOR

public class AkWwiseSettingsWindow : UnityEditor.EditorWindow
{
	private static bool m_oldCreateWwiseGlobal = true;
	private static bool m_oldCreateWwiseListener = true;
	private static bool m_oldShowMissingRigidBodyWarning = true;
	private static string m_WwiseVersionString;

	private static UnityEngine.GUIStyle VersionStyle;

	private bool ApplyNewProject;

	private void SetTextColor(UnityEngine.GUIStyle style, UnityEngine.Color color)
	{
		style.active.textColor = color;
		style.focused.textColor = color;
		style.hover.textColor = color;
		style.normal.textColor = color;
	}

	// Initialize our required styles
	protected void InitGuiStyles()
	{
		VersionStyle = new UnityEngine.GUIStyle(UnityEditor.EditorStyles.whiteLargeLabel);
		if (!UnityEngine.Application.HasProLicense())
			SetTextColor(VersionStyle, UnityEngine.Color.black);
	}

	public void DrawSettingsPart()
	{
		string description;
		string tooltip;
		string labelTitle;

		description = "Wwise Project Path*:";
		tooltip =
			"Location of the Wwise project associated with this game. It is recommended to put it in the Unity Project root folder, outside the Assets folder.";
		labelTitle = "Wwise Project";

		UnityEngine.GUILayout.Label(labelTitle, UnityEditor.EditorStyles.boldLabel);

		using (new UnityEngine.GUILayout.HorizontalScope("box"))
		{
			UnityEngine.GUILayout.Label(new UnityEngine.GUIContent(description, tooltip), UnityEngine.GUILayout.Width(330));
			UnityEditor.EditorGUILayout.SelectableLabel(WwiseSetupWizard.Settings.WwiseProjectPath, "textfield",
				UnityEngine.GUILayout.Height(17));
			if (UnityEngine.GUILayout.Button("...", UnityEngine.GUILayout.Width(30)))
			{
				var OpenInPath = System.IO.Path.GetDirectoryName(AkUtilities.GetFullPath(UnityEngine.Application.dataPath,
					WwiseSetupWizard.Settings.WwiseProjectPath));
				var WwiseProjectPathNew = UnityEditor.EditorUtility.OpenFilePanel("Select your Wwise Project", OpenInPath, "wproj");
				if (WwiseProjectPathNew.Length != 0)
				{
					if (WwiseProjectPathNew.EndsWith(".wproj") == false)
						UnityEditor.EditorUtility.DisplayDialog("Error", "Please select a valid .wproj file", "Ok");
					else
					{
						WwiseSetupWizard.Settings.WwiseProjectPath =
							AkUtilities.MakeRelativePath(UnityEngine.Application.dataPath, WwiseProjectPathNew);
					}
				}

				Repaint();
			}
		}

#if UNITY_EDITOR_OSX
		description = "Wwise Application:";
		tooltip = "Location of the Wwise Application. This is required to generate the SoundBanks in Unity.";
		labelTitle = "Wwise Application";
#else
		description = "Wwise Windows Installation Path:";
		tooltip = "Location of the Wwise Windows Installation Path. This is required to generate the SoundBanks in Unity.";
		labelTitle = "Wwise Windows Installation Path";
#endif

		UnityEngine.GUILayout.Label(labelTitle, UnityEditor.EditorStyles.boldLabel);

		using (new UnityEngine.GUILayout.HorizontalScope("box"))
		{
			UnityEngine.GUILayout.Label(new UnityEngine.GUIContent(description, tooltip), UnityEngine.GUILayout.Width(330));

			string wwiseInstallationPath;

#if UNITY_EDITOR_OSX
			wwiseInstallationPath = WwiseSetupWizard.Settings.WwiseInstallationPathMac;
#else
			wwiseInstallationPath = WwiseSetupWizard.Settings.WwiseInstallationPathWindows;
#endif

			UnityEditor.EditorGUILayout.SelectableLabel(wwiseInstallationPath, "textfield", UnityEngine.GUILayout.Height(17));

			if (UnityEngine.GUILayout.Button("...", UnityEngine.GUILayout.Width(30)))
			{
#if UNITY_EDITOR_OSX
				var installationPathNew = UnityEditor.EditorUtility.OpenFilePanel("Select your Wwise application.",
					"/Applications/", "");
#else
				var installationPathNew = UnityEditor.EditorUtility.OpenFolderPanel("Select your Wwise application.",
					System.Environment.GetEnvironmentVariable("ProgramFiles(x86)"), "");
#endif

				if (installationPathNew.Length != 0)
				{
					wwiseInstallationPath = System.IO.Path.GetFullPath(installationPathNew);

#if UNITY_EDITOR_OSX
					WwiseSetupWizard.Settings.WwiseInstallationPathMac = wwiseInstallationPath;
#else
					WwiseSetupWizard.Settings.WwiseInstallationPathWindows = wwiseInstallationPath;
#endif
				}

				Repaint();
			}
		}

		description = "SoundBanks Path* (relative to StreamingAssets folder):";
		tooltip = "Location of the SoundBanks are for the game. This has to reside within the StreamingAssets folder.";
		labelTitle = "Asset Management";

		UnityEngine.GUILayout.Label(labelTitle, UnityEditor.EditorStyles.boldLabel);

		using (new UnityEngine.GUILayout.VerticalScope("box"))
		{
			using (new UnityEngine.GUILayout.HorizontalScope())
			{
				UnityEngine.GUILayout.Label(new UnityEngine.GUIContent(description, tooltip), UnityEngine.GUILayout.Width(330));
				UnityEditor.EditorGUILayout.SelectableLabel(WwiseSetupWizard.Settings.SoundbankPath, "textfield",
					UnityEngine.GUILayout.Height(17));

				if (UnityEngine.GUILayout.Button("...", UnityEngine.GUILayout.Width(30)))
				{
					var OpenInPath = System.IO.Path.GetDirectoryName(
						AkUtilities.GetFullPath(UnityEngine.Application.streamingAssetsPath, WwiseSetupWizard.Settings.SoundbankPath));
					var SoundbankPathNew =
						UnityEditor.EditorUtility.OpenFolderPanel("Select your SoundBanks destination folder", OpenInPath, "");
					if (SoundbankPathNew.Length != 0)
					{
						var stremingAssetsIndex = UnityEngine.Application.dataPath.Split('/').Length;
						var folders = SoundbankPathNew.Split('/');

						if (folders.Length - 1 < stremingAssetsIndex || !string.Equals(folders[stremingAssetsIndex], "StreamingAssets",
							    System.StringComparison.OrdinalIgnoreCase))
						{
							UnityEditor.EditorUtility.DisplayDialog("Error",
								"The soundbank destination folder must be located within the Unity project 'StreamingAssets' folder.", "Ok");
						}
						else
						{
							WwiseSetupWizard.Settings.SoundbankPath =
								AkUtilities.MakeRelativePath(UnityEngine.Application.streamingAssetsPath, SoundbankPathNew) +
								"/";
						}
					}

					Repaint();
				}
			}

#if UNITY_5_6_OR_NEWER
			description = "Enable copying of soundbanks at pre-Build step";
			tooltip =
				"Copies the soundbanks in the appropriate location for building and deployment. It is recommended to leave this box checked.";
			WwiseSetupWizard.Settings.CopySoundBanksAsPreBuildStep = UnityEngine.GUILayout.Toggle(
				WwiseSetupWizard.Settings.CopySoundBanksAsPreBuildStep, new UnityEngine.GUIContent(description, tooltip));

			UnityEngine.GUI.enabled = WwiseSetupWizard.Settings.CopySoundBanksAsPreBuildStep;

			description = "Enable soundbank generation at pre-Build step";
			tooltip =
				"Generates the soundbanks before copying them during pre-Build step. It is recommended to leave this box unchecked if soundbanks are generated on a specific build machine.";
			WwiseSetupWizard.Settings.GenerateSoundBanksAsPreBuildStep = UnityEngine.GUILayout.Toggle(
				WwiseSetupWizard.Settings.GenerateSoundBanksAsPreBuildStep, new UnityEngine.GUIContent(description, tooltip));

			UnityEngine.GUI.enabled = true;
#endif

			description = "Create WwiseGlobal GameObject";
			tooltip =
				"The WwiseGlobal object is a GameObject that contains the Initializing and Terminating scripts for the Wwise Sound Engine. In the Editor workflow, it is added to every scene, so that it can be properly previewed in the Editor. In the game, only one instance is created, in the first scene, and it is persisted throughout the game. It is recommended to leave this box checked.";
			WwiseSetupWizard.Settings.CreateWwiseGlobal =
				UnityEngine.GUILayout.Toggle(WwiseSetupWizard.Settings.CreateWwiseGlobal,
					new UnityEngine.GUIContent(description, tooltip));

			description = "Automatically add Listener to Main Camera";
			tooltip =
				"In order for positioning to work, the Ak Audio Listener script needs to be attached to the main camera in every scene. If you wish for your listener to be attached to another GameObject, uncheck this box";
			WwiseSetupWizard.Settings.CreateWwiseListener = UnityEngine.GUILayout.Toggle(
				WwiseSetupWizard.Settings.CreateWwiseListener, new UnityEngine.GUIContent(description, tooltip));
		}

		UnityEngine.GUILayout.Label("In Editor Warnings", UnityEditor.EditorStyles.boldLabel);

		using (new UnityEngine.GUILayout.VerticalScope("box"))
		{
			description = "Show Warning for Missing RigidBody";
			tooltip =
				"Interactions between AkGameObj and AkEnvironment or AkRoom require a Rigidbody component on the object or the environment/room. It is recommended to leave this box checked.";
			WwiseSetupWizard.Settings.ShowMissingRigidBodyWarning = UnityEngine.GUILayout.Toggle(
				WwiseSetupWizard.Settings.ShowMissingRigidBodyWarning, new UnityEngine.GUIContent(description, tooltip));
		}

		using (new UnityEngine.GUILayout.HorizontalScope())
		{
			UnityEngine.GUILayout.Label("* Mandatory settings");
		}

		UnityEngine.GUILayout.FlexibleSpace();
	}

	[UnityEditor.MenuItem("Edit/Wwise Settings...", false, (int) AkWwiseWindowOrder.WwiseSettings)]
	public static void Init()
	{
		// Get existing open window or if none, make a new one:
		var window = GetWindow(typeof(AkWwiseSettingsWindow));

		window.position = new UnityEngine.Rect(100, 100, 850, 360);

		window.titleContent = new UnityEngine.GUIContent("Wwise Settings");

		m_oldCreateWwiseGlobal = WwiseSetupWizard.Settings.CreateWwiseGlobal;
		m_oldCreateWwiseListener = WwiseSetupWizard.Settings.CreateWwiseListener;
		m_oldShowMissingRigidBodyWarning = WwiseSetupWizard.Settings.ShowMissingRigidBodyWarning;

		var temp = AkSoundEngine.GetMajorMinorVersion();
		var temp2 = AkSoundEngine.GetSubminorBuildVersion();
		m_WwiseVersionString = "Wwise v" + (temp >> 16) + "." + (temp & 0xFFFF);
		if (temp2 >> 16 != 0)
			m_WwiseVersionString += "." + (temp2 >> 16);

		m_WwiseVersionString += " Build " + (temp2 & 0xFFFF) + " Settings.";
	}

	private void OnGUI()
	{
		// Make sure everything is initialized
		// Use soundbank path, because Wwise project path can be empty.
		if (string.IsNullOrEmpty(WwiseSetupWizard.Settings.SoundbankPath) &&
		    WwiseSetupWizard.Settings.WwiseProjectPath == null)
			WwiseSetupWizard.Settings = WwiseSettings.LoadSettings();

		var initialProject = WwiseSetupWizard.Settings.WwiseProjectPath;

		if (VersionStyle == null)
			InitGuiStyles();
		UnityEngine.GUILayout.Label(m_WwiseVersionString, VersionStyle);

		DrawSettingsPart();

		// DrawSettingsPart modifies WwiseSetupWizard.Settings.WwiseProjectPath directly.
		var newProject = WwiseSetupWizard.Settings.WwiseProjectPath;
		if (initialProject != newProject)
			ApplyNewProject = true;

		using (new UnityEngine.GUILayout.VerticalScope())
		{
			UnityEngine.GUILayout.FlexibleSpace();

			using (new UnityEngine.GUILayout.HorizontalScope())
			{
				UnityEngine.GUILayout.FlexibleSpace();
				if (UnityEngine.GUILayout.Button("OK", UnityEngine.GUILayout.Width(60)))
				{
					if (string.IsNullOrEmpty(WwiseSetupWizard.Settings.SoundbankPath))
						UnityEditor.EditorUtility.DisplayDialog("Error", "Please fill in the required settings", "Ok");

					if (m_oldCreateWwiseGlobal != WwiseSetupWizard.Settings.CreateWwiseGlobal)
					{
						var AkInitializers = FindObjectsOfType<AkInitializer>();
						if (WwiseSetupWizard.Settings.CreateWwiseGlobal)
						{
							if (AkInitializers.Length == 0)
							{
								//No Wwise object in this scene, create one so that the sound engine is initialized and terminated properly even if the scenes are loaded
								//in the wrong order.
								var objWwise = new UnityEngine.GameObject("WwiseGlobal");

								UnityEditor.Undo.AddComponent<AkInitializer>(objWwise);
							}
						}
						else if (AkInitializers.Length != 0 && AkInitializers[0].gameObject.name == "WwiseGlobal")
							UnityEditor.Undo.DestroyObjectImmediate(AkInitializers[0].gameObject);
					}

					if (m_oldCreateWwiseListener != WwiseSetupWizard.Settings.CreateWwiseListener)
					{
						if (WwiseSetupWizard.Settings.CreateWwiseListener)
						{
							AkUtilities.AddAkAudioListenerToMainCamera();
						}
					}

					if (m_oldShowMissingRigidBodyWarning != WwiseSetupWizard.Settings.ShowMissingRigidBodyWarning)
						UnityEditorInternal.InternalEditorUtility.RepaintAllViews();

					WwiseSettings.SaveSettings(WwiseSetupWizard.Settings);

					CloseWindow();

					// Pop the Picker window so the user can start working right away
					AkWwiseProjectInfo.GetData(); // Load data
					if (ApplyNewProject)
					{
						//Clear the data, the project path changed.
						AkWwiseProjectInfo.GetData().Reset();
						ApplyNewProject = false;
						AkUtilities.IsWwiseProjectAvailable = true;
					}

					AkWwiseProjectInfo.Populate();
					AkWwisePicker.PopulateTreeview();
					AkWwisePicker.init();
				}

				if (UnityEngine.GUILayout.Button("Cancel", UnityEngine.GUILayout.Width(60)))
				{
					WwiseSetupWizard.Settings = WwiseSettings.LoadSettings(true);
					CloseWindow();
				}

				UnityEngine.GUILayout.Space(UnityEditor.EditorGUIUtility.standardVerticalSpacing);
			}

			UnityEngine.GUILayout.Space(UnityEditor.EditorGUIUtility.standardVerticalSpacing);
		}
	}

	private void CloseWindow()
	{
		var SetupWindow = GetWindow<AkWwiseSettingsWindow>();
		SetupWindow.Close();
	}
}

#endif // UNITY_EDITOR
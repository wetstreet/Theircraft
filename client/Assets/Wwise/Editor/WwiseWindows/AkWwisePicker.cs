#if UNITY_EDITOR
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2014 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

public class AkWwisePicker : UnityEditor.EditorWindow
{
	public static AkWwiseTreeView treeView = new AkWwiseTreeView();

	[UnityEditor.MenuItem("Window/Wwise Picker", false, (int) AkWwiseWindowOrder.WwisePicker)]
	public static void init()
	{
		GetWindow<AkWwisePicker>("Wwise Picker", true,
			typeof(UnityEditor.EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow"));
	}

	public void OnEnable()
	{
		PopulateTreeview();
	}

	public void OnGUI()
	{
		using (new UnityEngine.GUILayout.HorizontalScope("box"))
		{
			AkWwiseProjectInfo.GetData().autoPopulateEnabled =
				UnityEngine.GUILayout.Toggle(AkWwiseProjectInfo.GetData().autoPopulateEnabled, "Auto populate");

			if (AkWwiseProjectInfo.GetData().autoPopulateEnabled && AkUtilities.IsWwiseProjectAvailable)
				AkWwiseWWUBuilder.StartWWUWatcher();
			else
				AkWwiseWWUBuilder.StopWWUWatcher();
			UnityEngine.GUILayout.FlexibleSpace();
			if (UnityEngine.GUILayout.Button("Refresh Project", UnityEngine.GUILayout.Width(200)))
			{
				treeView.SaveExpansionStatus();
				if (AkWwiseProjectInfo.Populate())
					PopulateTreeview();
			}

			if (UnityEngine.GUILayout.Button("Generate SoundBanks", UnityEngine.GUILayout.Width(200)))
			{
				if (AkUtilities.IsSoundbankGenerationAvailable())
				{
					AkUtilities.GenerateSoundbanks();
				}
				else
				{
					string errorMessage;

#if UNITY_EDITOR_WIN
					errorMessage =
						"Access to Wwise is required to generate the SoundBanks. Please select the Wwise Windows Installation Path from the Edit > Wwise Settings... menu.";
#elif UNITY_EDITOR_OSX
					errorMessage =
						"Access to Wwise is required to generate the SoundBanks. Please select the Wwise Application from the Edit > Wwise Settings... menu.";
#endif

					UnityEngine.Debug.LogError(errorMessage);
				}
			}
		}

		UnityEngine.GUILayout.Space(UnityEditor.EditorGUIUtility.standardVerticalSpacing);

		treeView.DisplayTreeView(AK.Wwise.TreeView.TreeViewControl.DisplayTypes.USE_SCROLL_VIEW);

		if (UnityEngine.GUI.changed && AkUtilities.IsWwiseProjectAvailable)
			UnityEditor.EditorUtility.SetDirty(AkWwiseProjectInfo.GetData());
		// TODO: RTP Parameters List
	}

	public static void PopulateTreeview()
	{
		treeView.AssignDefaults();
		treeView.SetRootItem(System.IO.Path.GetFileNameWithoutExtension(WwiseSetupWizard.Settings.WwiseProjectPath), WwiseObjectType.Project);
		treeView.PopulateItem(treeView.RootItem, "Events", AkWwiseProjectInfo.GetData().EventWwu);
		treeView.PopulateItem(treeView.RootItem, "Switches", AkWwiseProjectInfo.GetData().SwitchWwu);
		treeView.PopulateItem(treeView.RootItem, "States", AkWwiseProjectInfo.GetData().StateWwu);
		treeView.PopulateItem(treeView.RootItem, "SoundBanks", AkWwiseProjectInfo.GetData().BankWwu);
		treeView.PopulateItem(treeView.RootItem, "Auxiliary Busses", AkWwiseProjectInfo.GetData().AuxBusWwu);
		//treeView.PopulateItem(treeView.RootItem, "Game Parameters", AkWwiseProjectInfo.GetData().RtpcWwu);
		//treeView.PopulateItem(treeView.RootItem, "Triggers", AkWwiseProjectInfo.GetData().TriggerWwu);
		treeView.PopulateItem(treeView.RootItem, "Virtual Acoustics", AkWwiseProjectInfo.GetData().AcousticTextureWwu);
	}

	public class Postprocessor : UnityEditor.AssetPostprocessor
	{
		//This function will be called before script compilation and will save the picker's expantion 
		private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
			string[] movedFromAssetPaths)
		{
			if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode || UnityEditor.EditorApplication.isCompiling)
				return;

			treeView.SaveExpansionStatus();
		}
	}
}
#endif

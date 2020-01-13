#if UNITY_EDITOR
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2014 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

#pragma warning disable 0168
[UnityEditor.InitializeOnLoad]
public class AkWwiseWWUBuilder
{
	private const string s_progTitle = "Populating Wwise Picker";
	private const int s_SecondsBetweenChecks = 3;

	private static string s_wwiseProjectPath = System.IO.Path.GetDirectoryName(
		AkUtilities.GetFullPath(UnityEngine.Application.dataPath, WwiseSettings.LoadSettings().WwiseProjectPath));

	private static System.DateTime s_lastFileCheck = System.DateTime.Now.AddSeconds(-s_SecondsBetweenChecks);
	private static readonly FileInfo_CompareByPath s_FileInfo_CompareByPath = new FileInfo_CompareByPath();

	private readonly System.Collections.Generic.HashSet<string> m_WwuToProcess =
		new System.Collections.Generic.HashSet<string>();

	private int m_currentWwuCnt;
	private int m_totWwuCnt = 1;

	private static void Tick()
	{
		isTicking = true;

		if (AkWwiseProjectInfo.GetData() != null)
		{
			if (System.DateTime.Now.Subtract(s_lastFileCheck).Seconds > s_SecondsBetweenChecks &&
			    !UnityEditor.EditorApplication.isCompiling && !UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode &&
			    AkWwiseProjectInfo.GetData().autoPopulateEnabled)
			{
				AkWwisePicker.treeView.SaveExpansionStatus();
				if (Populate())
				{
					AkWwisePicker.PopulateTreeview();
					//Make sure that the Wwise picker and the inspector are updated
					AkUtilities.RepaintInspector();
				}

				s_lastFileCheck = System.DateTime.Now;
			}
		}
	}

	public static void InitializeWwiseProjectData()
	{
		try
		{
			if (WwiseSetupWizard.Settings.WwiseProjectPath == null)
				WwiseSettings.LoadSettings();

			if (string.IsNullOrEmpty(WwiseSetupWizard.Settings.WwiseProjectPath))
			{
				UnityEngine.Debug.LogError("WwiseUnity: Wwise project needed to populate from Work Units. Aborting.");
				return;
			}

			var fullWwiseProjectPath = AkUtilities.GetFullPath(UnityEngine.Application.dataPath, WwiseSetupWizard.Settings.WwiseProjectPath);
			s_wwiseProjectPath = System.IO.Path.GetDirectoryName(fullWwiseProjectPath);

			AkUtilities.IsWwiseProjectAvailable = System.IO.File.Exists(fullWwiseProjectPath);
			if (!AkUtilities.IsWwiseProjectAvailable || UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode || string.IsNullOrEmpty(s_wwiseProjectPath) ||
				UnityEditor.EditorApplication.isCompiling)
				return;

			var builder = new AkWwiseWWUBuilder();
			builder.GatherModifiedFiles();
			builder.UpdateFiles();
		}
		catch
		{
		}
	}

	public static bool Populate()
	{
		try
		{
			if (WwiseSetupWizard.Settings.WwiseProjectPath == null)
				WwiseSettings.LoadSettings();

			if (string.IsNullOrEmpty(WwiseSetupWizard.Settings.WwiseProjectPath))
			{
				UnityEngine.Debug.LogError("WwiseUnity: Wwise project needed to populate from Work Units. Aborting.");
				return false;
			}

			var fullWwiseProjectPath = AkUtilities.GetFullPath(UnityEngine.Application.dataPath, WwiseSetupWizard.Settings.WwiseProjectPath);
			s_wwiseProjectPath = System.IO.Path.GetDirectoryName(fullWwiseProjectPath);

			AkUtilities.IsWwiseProjectAvailable = System.IO.File.Exists(fullWwiseProjectPath);
			if (!AkUtilities.IsWwiseProjectAvailable || UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode || string.IsNullOrEmpty(s_wwiseProjectPath) ||
			    (UnityEditor.EditorApplication.isCompiling && !AkUtilities.IsMigrating))
				return false;

			AkPluginActivator.Update();

			var builder = new AkWwiseWWUBuilder();
			if (!builder.GatherModifiedFiles())
				return false;

			builder.UpdateFiles();
			return true;
		}
		catch (System.Exception e)
		{
			UnityEngine.Debug.LogError(e.ToString());
			UnityEditor.EditorUtility.ClearProgressBar();
			return true;
		}
	}

	public static void UpdateWwiseObjectReferenceData()
	{
		UnityEngine.Debug.Log("WwiseUnity: Updating Wwise Object References");

		WwiseObjectReference.ClearWwiseObjectDataMap();
		UpdateWwiseObjectReference(WwiseObjectType.AuxBus, AkWwiseProjectInfo.GetData().AuxBusWwu);
		UpdateWwiseObjectReference(WwiseObjectType.Event, AkWwiseProjectInfo.GetData().EventWwu);
		UpdateWwiseObjectReference(WwiseObjectType.Soundbank, AkWwiseProjectInfo.GetData().BankWwu);
		UpdateWwiseObjectReference(WwiseObjectType.GameParameter, AkWwiseProjectInfo.GetData().RtpcWwu);
		UpdateWwiseObjectReference(WwiseObjectType.Trigger, AkWwiseProjectInfo.GetData().TriggerWwu);
		UpdateWwiseObjectReference(WwiseObjectType.AcousticTexture, AkWwiseProjectInfo.GetData().AcousticTextureWwu);
		UpdateWwiseObjectReference(WwiseObjectType.StateGroup, WwiseObjectType.State, AkWwiseProjectInfo.GetData().StateWwu);
		UpdateWwiseObjectReference(WwiseObjectType.SwitchGroup, WwiseObjectType.Switch, AkWwiseProjectInfo.GetData().SwitchWwu);
	}

	private int RecurseWorkUnit(AssetType in_type, System.IO.FileInfo in_workUnit, string in_currentPathInProj,
		string in_currentPhysicalPath, System.Collections.Generic.LinkedList<AkWwiseProjectData.PathElement> in_pathAndIcons,
		string in_parentPhysicalPath = "")
	{
		m_WwuToProcess.Remove(in_workUnit.FullName);
		System.Xml.XmlReader reader = null;
		var wwuIndex = -1;
		try
		{
			//Progress bar stuff
			var msg = "Parsing Work Unit " + in_workUnit.Name;
			UnityEditor.EditorUtility.DisplayProgressBar(s_progTitle, msg, m_currentWwuCnt / (float) m_totWwuCnt);
			m_currentWwuCnt++;

			in_currentPathInProj =
				System.IO.Path.Combine(in_currentPathInProj, System.IO.Path.GetFileNameWithoutExtension(in_workUnit.Name));
			in_pathAndIcons.AddLast(new AkWwiseProjectData.PathElement(
				System.IO.Path.GetFileNameWithoutExtension(in_workUnit.Name), WwiseObjectType.WorkUnit));
			var WwuPhysicalPath = System.IO.Path.Combine(in_currentPhysicalPath, in_workUnit.Name);

			var wwu = ReplaceWwuEntry(WwuPhysicalPath, in_type, out wwuIndex);

			wwu.ParentPhysicalPath = in_parentPhysicalPath;
			wwu.PhysicalPath = WwuPhysicalPath;
			wwu.Guid = System.Guid.Empty;
			wwu.LastTime = System.IO.File.GetLastWriteTime(in_workUnit.FullName);

			reader = System.Xml.XmlReader.Create(in_workUnit.FullName);

			reader.MoveToContent();
			reader.Read();
			while (!reader.EOF && reader.ReadState == System.Xml.ReadState.Interactive)
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element && reader.Name.Equals("WorkUnit"))
				{
					if (wwu.Guid.Equals(System.Guid.Empty))
					{
						var ID = reader.GetAttribute("ID");
						try
						{
							wwu.Guid = new System.Guid(ID);
						}
						catch
						{
							UnityEngine.Debug.LogWarning("WwiseUnity: Error reading ID <" + ID + "> from work unit <" + in_workUnit.FullName + ">.");
							throw;
						}
					}

					var persistMode = reader.GetAttribute("PersistMode");
					if (persistMode == "Reference")
					{
						// ReadFrom advances the reader
						var matchedElement = System.Xml.Linq.XNode.ReadFrom(reader) as System.Xml.Linq.XElement;
						var newWorkUnitPath =
							System.IO.Path.Combine(in_workUnit.Directory.FullName, matchedElement.Attribute("Name").Value + ".wwu");
						var newWorkUnit = new System.IO.FileInfo(newWorkUnitPath);

						// Parse the referenced Work Unit
						if (m_WwuToProcess.Contains(newWorkUnit.FullName))
						{
							RecurseWorkUnit(in_type, newWorkUnit, in_currentPathInProj, in_currentPhysicalPath, in_pathAndIcons,
								WwuPhysicalPath);
						}
					}
					else
					{
						// If the persist mode is "Standalone" or "Nested", it means the current XML tag
						// is the one corresponding to the current file. We can ignore it and advance the reader
						reader.Read();
					}
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.Element && reader.Name.Equals("AuxBus"))
				{
					in_currentPathInProj = System.IO.Path.Combine(in_currentPathInProj, reader.GetAttribute("Name"));
					in_pathAndIcons.AddLast(new AkWwiseProjectData.PathElement(reader.GetAttribute("Name"), WwiseObjectType.AuxBus));
					var isEmpty = reader.IsEmptyElement;
					AddElementToList(in_currentPathInProj, reader, in_type, in_pathAndIcons, wwuIndex);

					if (isEmpty)
					{
						in_currentPathInProj =
							in_currentPathInProj.Remove(in_currentPathInProj.LastIndexOf(System.IO.Path.DirectorySeparatorChar));
						in_pathAndIcons.RemoveLast();
					}
				}
				// Busses and folders act the same for the Hierarchy: simply add them to the path
				else if (reader.NodeType == System.Xml.XmlNodeType.Element &&
				         (reader.Name.Equals("Folder") || reader.Name.Equals("Bus")))
				{
					//check if node has children
					if (!reader.IsEmptyElement)
					{
						// Add the folder/bus to the path
						in_currentPathInProj = System.IO.Path.Combine(in_currentPathInProj, reader.GetAttribute("Name"));
						if (reader.Name.Equals("Folder"))
							in_pathAndIcons.AddLast(new AkWwiseProjectData.PathElement(reader.GetAttribute("Name"), WwiseObjectType.Folder));
						else if (reader.Name.Equals("Bus"))
							in_pathAndIcons.AddLast(new AkWwiseProjectData.PathElement(reader.GetAttribute("Name"), WwiseObjectType.Bus));
					}

					// Advance the reader
					reader.Read();
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement &&
				         (reader.Name.Equals("Folder") || reader.Name.Equals("Bus") || reader.Name.Equals("AuxBus")))
				{
					// Remove the folder/bus from the path
					in_currentPathInProj =
						in_currentPathInProj.Remove(in_currentPathInProj.LastIndexOf(System.IO.Path.DirectorySeparatorChar));
					in_pathAndIcons.RemoveLast();

					// Advance the reader
					reader.Read();
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.Element && reader.Name.Equals(in_type.XmlElementName))
				{
					// Add the element to the list
					AddElementToList(in_currentPathInProj, reader, in_type, in_pathAndIcons, wwuIndex);
				}
				else
					reader.Read();
			}

			// Sort the newly populated Wwu alphabetically
			SortWwu(in_type, wwuIndex);
		}
		catch (System.Exception e)
		{
			UnityEngine.Debug.LogError(e.ToString());
			wwuIndex = -1;
		}

		if (reader != null)
			reader.Close();

		in_pathAndIcons.RemoveLast();
		return wwuIndex;
	}

	private static bool isTicking = false;

	public static void StartWWUWatcher()
	{
		if (isTicking)
			return;

		Tick();
		UnityEditor.EditorApplication.update += Tick;
	}

	public static void StopWWUWatcher()
	{
		if (isTicking)
			UnityEditor.EditorApplication.update -= Tick;
	}

	private static void RestartWWUWatcher()
	{
		if (AkWwiseProjectInfo.GetData().autoPopulateEnabled)
			StartWWUWatcher();
	}

	static AkWwiseWWUBuilder()
	{
		InitializeWwiseProjectData();

#if UNITY_2017_2_OR_NEWER
		UnityEditor.EditorApplication.playModeStateChanged += (UnityEditor.PlayModeStateChange playMode) => 
		{
			if (playMode == UnityEditor.PlayModeStateChange.EnteredEditMode)
				RestartWWUWatcher();
		};
#else
		UnityEditor.EditorApplication.playmodeStateChanged += () =>
		{
			if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode && UnityEditor.EditorApplication.isPlaying)
				RestartWWUWatcher();
		};
#endif
	}

	private static System.Collections.Generic.Dictionary<WwiseObjectType, System.Collections.Generic.List<AkWwiseProjectData.AkBaseInformation>> _WwiseObjectsToRemove 
		= new System.Collections.Generic.Dictionary<WwiseObjectType, System.Collections.Generic.List<AkWwiseProjectData.AkBaseInformation>>();

	private static System.Collections.Generic.Dictionary<WwiseObjectType, System.Collections.Generic.List<AkWwiseProjectData.AkBaseInformation>> _WwiseObjectsToAdd
		= new System.Collections.Generic.Dictionary<WwiseObjectType, System.Collections.Generic.List<AkWwiseProjectData.AkBaseInformation>>();

	private static void FlagForRemoval(WwiseObjectType type, int wwuIndex)
	{
		switch (type)
		{
			case WwiseObjectType.AuxBus:
				foreach (var wwobject in AkWwiseProjectInfo.GetData().AuxBusWwu[wwuIndex].List)
					FlagForRemoval(wwobject, type);
				break;

			case WwiseObjectType.Event:
				foreach (var wwobject in AkWwiseProjectInfo.GetData().EventWwu[wwuIndex].List)
					FlagForRemoval(wwobject, type);
				break;

			case WwiseObjectType.Soundbank:
				foreach (var wwobject in AkWwiseProjectInfo.GetData().BankWwu[wwuIndex].List)
					FlagForRemoval(wwobject, type);
				break;

			case WwiseObjectType.GameParameter:
				foreach (var wwobject in AkWwiseProjectInfo.GetData().RtpcWwu[wwuIndex].List)
					FlagForRemoval(wwobject, type);
				break;

			case WwiseObjectType.Trigger:
				foreach (var wwobject in AkWwiseProjectInfo.GetData().TriggerWwu[wwuIndex].List)
					FlagForRemoval(wwobject, type);
				break;

			case WwiseObjectType.AcousticTexture:
				foreach (var wwobject in AkWwiseProjectInfo.GetData().AcousticTextureWwu[wwuIndex].List)
					FlagForRemoval(wwobject, type);
				break;

			case WwiseObjectType.StateGroup:
				foreach (var wwobject in AkWwiseProjectInfo.GetData().StateWwu[wwuIndex].List)
					FlagForRemoval(wwobject, type);
				break;

			case WwiseObjectType.SwitchGroup:
				foreach (var wwobject in AkWwiseProjectInfo.GetData().SwitchWwu[wwuIndex].List)
					FlagForRemoval(wwobject, type);
				break;
		}
	}

	private static void FlagForInsertion(AkWwiseProjectData.AkBaseInformation info, WwiseObjectType type)
	{
		if (!_WwiseObjectsToAdd.ContainsKey(type))
			_WwiseObjectsToAdd[type] = new System.Collections.Generic.List<AkWwiseProjectData.AkBaseInformation>();

		_WwiseObjectsToAdd[type].Add(info);

		if (!AkUtilities.IsMigrating)
			WwiseObjectReference.UpdateWwiseObject(type, info.Name, info.Guid);
	}

	private static void FlagForRemoval(AkWwiseProjectData.AkBaseInformation info, WwiseObjectType type)
	{
		if (!_WwiseObjectsToRemove.ContainsKey(type))
			_WwiseObjectsToRemove[type] = new System.Collections.Generic.List<AkWwiseProjectData.AkBaseInformation>();

		_WwiseObjectsToRemove[type].Add(info);
	}

	private bool GatherModifiedFiles()
	{
		_WwiseObjectsToRemove.Clear();
		_WwiseObjectsToAdd.Clear();

		var bChanged = false;
		var iBasePathLen = s_wwiseProjectPath.Length + 1;
		foreach (var scannedAsset in AssetType.ScannedAssets)
		{
			var dir = scannedAsset.RootDirectoryName;
			var deleted = new System.Collections.Generic.List<int>();
			var knownFiles = AkWwiseProjectInfo.GetData().GetWwuListByString(dir);
			var cKnownBefore = knownFiles.Count;

			try
			{
				//Get all Wwus in this folder.
				var di = new System.IO.DirectoryInfo(System.IO.Path.Combine(s_wwiseProjectPath, dir));
				var files = di.GetFiles("*.wwu", System.IO.SearchOption.AllDirectories);
				System.Array.Sort(files, s_FileInfo_CompareByPath);

				//Walk both arrays
				var iKnown = 0;
				var iFound = 0;

				while (iFound < files.Length && iKnown < knownFiles.Count)
				{
					var workunit = knownFiles[iKnown] as AkWwiseProjectData.WorkUnit;
					var foundRelPath = files[iFound].FullName.Substring(iBasePathLen);
					switch (workunit.PhysicalPath.CompareTo(foundRelPath))
					{
						case 0:
							//File was there and is still there.  Check the FileTimes.
							try
							{
								if (files[iFound].LastWriteTime > workunit.LastTime)
								{
									//File has been changed!   
									//If this file had a parent, parse recursively the parent itself
									m_WwuToProcess.Add(files[iFound].FullName);
									FlagForRemoval(scannedAsset.Type, iKnown);
									bChanged = true;
								}
							}
							catch
							{
								//Access denied probably (file does exists since it was picked up by GetFiles).
								//Just ignore this file.
							}

							iFound++;
							iKnown++;
							break;

						case 1:
							m_WwuToProcess.Add(files[iFound].FullName);
							iFound++;
							break;

						case -1:
							//A file was deleted.  Can't process it now, it would change the array indices.                                
							deleted.Add(iKnown);
							iKnown++;
							break;
					}
				}

				//The remainder from the files found on disk are all new files.
				for (; iFound < files.Length; iFound++)
					m_WwuToProcess.Add(files[iFound].FullName);

				//All the remainder is deleted.  From the end, of course.
				if (iKnown < knownFiles.Count)
				{
					for (var i = iKnown; i < knownFiles.Count; ++i)
						FlagForRemoval(scannedAsset.Type, i);

					knownFiles.RemoveRange(iKnown, knownFiles.Count - iKnown);
				}

				//Delete those tagged.
				for (var i = deleted.Count - 1; i >= 0; i--)
				{
					FlagForRemoval(scannedAsset.Type, deleted[i]);
					knownFiles.RemoveAt(deleted[i]);
				}

				bChanged |= cKnownBefore != knownFiles.Count;
			}
			catch
			{
				return false;
			}
		}

		return bChanged || m_WwuToProcess.Count > 0;
	}

	private void UpdateFiles()
	{
		m_totWwuCnt = m_WwuToProcess.Count;

		var iBasePathLen = s_wwiseProjectPath.Length + 1;
		var iUnprocessed = 0;
		while (m_WwuToProcess.Count - iUnprocessed > 0)
		{
			System.Collections.IEnumerator e = m_WwuToProcess.GetEnumerator();
			for (var i = 0; i < iUnprocessed + 1; i++)
				e.MoveNext();

			var fullPath = e.Current as string;
			var relPath = fullPath.Substring(iBasePathLen);
			var typeStr = relPath.Remove(relPath.IndexOf(System.IO.Path.DirectorySeparatorChar));
			if (!CreateWorkUnit(relPath, typeStr, fullPath))
				iUnprocessed++;
		}

		//Add the unprocessed directly.  This can happen if we don't find the parent WorkUnit.  
		//Normally, it should never happen, this is just a safe guard.
		while (m_WwuToProcess.Count > 0)
		{
			System.Collections.IEnumerator e = m_WwuToProcess.GetEnumerator();
			e.MoveNext();
			var fullPath = e.Current as string;
			var relPath = fullPath.Substring(iBasePathLen);
			var typeStr = relPath.Remove(relPath.IndexOf(System.IO.Path.DirectorySeparatorChar));
			UpdateWorkUnit(string.Empty, fullPath, typeStr, relPath);
		}

		foreach (var pair in _WwiseObjectsToAdd)
		{
			System.Collections.Generic.List<AkWwiseProjectData.AkBaseInformation> removeList = null;
			if (!_WwiseObjectsToRemove.TryGetValue(pair.Key, out removeList))
				continue;

			removeList.Sort(AkWwiseProjectData.AkBaseInformation.CompareByGuid);
			foreach (var info in pair.Value)
			{
				var index = removeList.BinarySearch(info, AkWwiseProjectData.AkBaseInformation.CompareByGuid);
				if (index >= 0)
					removeList.RemoveAt(index);
			}
		}

		foreach (var pair in _WwiseObjectsToRemove)
		{
			var type = pair.Key;
			var childType = type == WwiseObjectType.StateGroup ? WwiseObjectType.State : WwiseObjectType.Switch;

			foreach (var info in pair.Value)
			{
				var groupValue = info as AkWwiseProjectData.GroupValue;
				if (groupValue != null)
					foreach (var value in groupValue.values)
						WwiseObjectReference.DeleteWwiseObject(childType, value.Guid);

				WwiseObjectReference.DeleteWwiseObject(type, info.Guid);
			}
		}

		UnityEditor.EditorUtility.ClearProgressBar();
	}

	private static void UpdateWwiseObjectReference(WwiseObjectType type, System.Collections.Generic.List<AkWwiseProjectData.AkInfoWorkUnit> infoWwus)
	{
		foreach (var infoWwu in infoWwus)
			foreach (var info in infoWwu.List)
				WwiseObjectReference.UpdateWwiseObjectDataMap(type, info.Name, info.Guid);
	}

	private static void UpdateWwiseObjectReference(WwiseObjectType type, System.Collections.Generic.List<AkWwiseProjectData.EventWorkUnit> infoWwus)
	{
		foreach (var infoWwu in infoWwus)
			foreach (var info in infoWwu.List)
				WwiseObjectReference.UpdateWwiseObjectDataMap(type, info.Name, info.Guid);
	}

	private static void UpdateWwiseObjectReference(WwiseObjectType groupType, WwiseObjectType type, System.Collections.Generic.List<AkWwiseProjectData.GroupValWorkUnit> infoWwus)
	{
		foreach (var infoWwu in infoWwus)
		{
			foreach (var info in infoWwu.List)
			{
				WwiseObjectReference.UpdateWwiseObjectDataMap(groupType, info.Name, info.Guid);
				foreach (var subTypeInfo in info.values)
					WwiseObjectReference.UpdateWwiseObjectDataMap(type, subTypeInfo.Name, subTypeInfo.Guid);
			}
		}
	}

	private static void SortWwu(AssetType in_type, int in_wwuIndex)
	{
		switch (in_type.Type)
		{
			case WwiseObjectType.AuxBus:
				AkWwiseProjectInfo.GetData().AuxBusWwu[in_wwuIndex].List.Sort();
				break;

			case WwiseObjectType.Event:
				AkWwiseProjectInfo.GetData().EventWwu[in_wwuIndex].List.Sort();
				break;

			case WwiseObjectType.Soundbank:
				AkWwiseProjectInfo.GetData().BankWwu[in_wwuIndex].List.Sort();
				break;

			case WwiseObjectType.GameParameter:
				AkWwiseProjectInfo.GetData().RtpcWwu[in_wwuIndex].List.Sort();
				break;

			case WwiseObjectType.Trigger:
				AkWwiseProjectInfo.GetData().TriggerWwu[in_wwuIndex].List.Sort();
				break;

			case WwiseObjectType.AcousticTexture:
				AkWwiseProjectInfo.GetData().AcousticTextureWwu[in_wwuIndex].List.Sort();
				break;

			case WwiseObjectType.StateGroup:
				var stateList = AkWwiseProjectInfo.GetData().StateWwu[in_wwuIndex].List;
				stateList.Sort();
				foreach (var group in stateList)
					if (group.values.Count > 0)
						group.values.Sort();
				break;

			case WwiseObjectType.SwitchGroup:
				var switchList = AkWwiseProjectInfo.GetData().SwitchWwu[in_wwuIndex].List;
				switchList.Sort();
				foreach (var group in switchList)
					if (group.values.Count > 0)
						group.values.Sort();
				break;
		}
	}

	private static AkWwiseProjectData.WorkUnit CreateWorkUnit(WwiseObjectType type)
	{
		switch (type)
		{
			case WwiseObjectType.Event:
				return new AkWwiseProjectData.EventWorkUnit();

			case WwiseObjectType.StateGroup:
			case WwiseObjectType.SwitchGroup:
				return new AkWwiseProjectData.GroupValWorkUnit();

			case WwiseObjectType.AuxBus:
			case WwiseObjectType.Soundbank:
			case WwiseObjectType.GameParameter:
			case WwiseObjectType.Trigger:
			case WwiseObjectType.AcousticTexture:
				return new AkWwiseProjectData.AkInfoWorkUnit();
		}

		return null;
	}

	private static AkWwiseProjectData.WorkUnit ReplaceWwuEntry(string in_currentPhysicalPath, AssetType in_type, out int out_wwuIndex)
	{
		var list = AkWwiseProjectInfo.GetData().GetWwuListByString(in_type.RootDirectoryName);
		out_wwuIndex = list.BinarySearch(new AkWwiseProjectData.WorkUnit { PhysicalPath = in_currentPhysicalPath });
		var out_wwu = CreateWorkUnit(in_type.Type);

		if (out_wwuIndex < 0)
		{
			out_wwuIndex = ~out_wwuIndex;
			list.Insert(out_wwuIndex, out_wwu);
		}
		else
			list[out_wwuIndex] = out_wwu;

		return out_wwu;
	}

	private static void AddElementToList(string in_currentPathInProj, System.Xml.XmlReader in_reader, AssetType in_type,
		System.Collections.Generic.LinkedList<AkWwiseProjectData.PathElement> in_pathAndIcons, int in_wwuIndex)
	{
		switch (in_type.Type)
		{
			case WwiseObjectType.AuxBus:
			case WwiseObjectType.Event:
			case WwiseObjectType.Soundbank:
			case WwiseObjectType.GameParameter:
			case WwiseObjectType.Trigger:
			case WwiseObjectType.AcousticTexture:
				{
					var name = in_reader.GetAttribute("Name");
					var valueToAdd = in_type.Type == WwiseObjectType.Event ? new AkWwiseProjectData.Event() : new AkWwiseProjectData.AkInformation();
					valueToAdd.Name = name;
					valueToAdd.Guid = new System.Guid(in_reader.GetAttribute("ID"));
					valueToAdd.PathAndIcons = new System.Collections.Generic.List<AkWwiseProjectData.PathElement>(in_pathAndIcons);

					FlagForInsertion(valueToAdd, in_type.Type);

					switch (in_type.Type)
					{
						case WwiseObjectType.AuxBus:
							valueToAdd.Path = in_currentPathInProj;
							break;

						default:
							valueToAdd.Path = System.IO.Path.Combine(in_currentPathInProj, name);
							valueToAdd.PathAndIcons.Add(new AkWwiseProjectData.PathElement(name, in_type.Type));
							break;
					}

					switch (in_type.Type)
					{
						case WwiseObjectType.AuxBus:
							AkWwiseProjectInfo.GetData().AuxBusWwu[in_wwuIndex].List.Add(valueToAdd);
							break;

						case WwiseObjectType.Event:
							AkWwiseProjectInfo.GetData().EventWwu[in_wwuIndex].List.Add(valueToAdd as AkWwiseProjectData.Event);
							break;

						case WwiseObjectType.Soundbank:
							AkWwiseProjectInfo.GetData().BankWwu[in_wwuIndex].List.Add(valueToAdd);
							break;

						case WwiseObjectType.GameParameter:
							AkWwiseProjectInfo.GetData().RtpcWwu[in_wwuIndex].List.Add(valueToAdd);
							break;

						case WwiseObjectType.Trigger:
							AkWwiseProjectInfo.GetData().TriggerWwu[in_wwuIndex].List.Add(valueToAdd);
							break;

						case WwiseObjectType.AcousticTexture:
							AkWwiseProjectInfo.GetData().AcousticTextureWwu[in_wwuIndex].List.Add(valueToAdd);
							break;
					}
				}

				in_reader.Read();
				break;

			case WwiseObjectType.StateGroup:
			case WwiseObjectType.SwitchGroup:
				{
					var XmlElement = System.Xml.Linq.XNode.ReadFrom(in_reader) as System.Xml.Linq.XElement;
					var ChildrenList = System.Xml.Linq.XName.Get("ChildrenList");
					var ChildrenElement = XmlElement.Element(ChildrenList);
					if (ChildrenElement != null)
					{
						var name = XmlElement.Attribute("Name").Value;
						var valueToAdd = new AkWwiseProjectData.GroupValue
						{
							Name = name,
							Guid = new System.Guid(XmlElement.Attribute("ID").Value),
							Path = System.IO.Path.Combine(in_currentPathInProj, name),
							PathAndIcons = new System.Collections.Generic.List<AkWwiseProjectData.PathElement>(in_pathAndIcons),
						};
						valueToAdd.PathAndIcons.Add(new AkWwiseProjectData.PathElement(name, in_type.Type));

						FlagForInsertion(valueToAdd, in_type.Type);

						var ChildElem = System.Xml.Linq.XName.Get(in_type.ChildElementName);
						foreach (var element in ChildrenElement.Elements(ChildElem))
						{
							if (element.Name != in_type.ChildElementName)
								continue;

							var elementName = element.Attribute("Name").Value;
							var childValue = new AkWwiseProjectData.AkBaseInformation
							{
								Name = elementName,
								Guid = new System.Guid(element.Attribute("ID").Value),
							};
							childValue.PathAndIcons.Add(new AkWwiseProjectData.PathElement(elementName, in_type.ChildType));
							valueToAdd.values.Add(childValue);

							FlagForInsertion(childValue, in_type.ChildType);
						}

						switch (in_type.Type)
						{
							case WwiseObjectType.StateGroup:
								AkWwiseProjectInfo.GetData().StateWwu[in_wwuIndex].List.Add(valueToAdd);
								break;

							case WwiseObjectType.SwitchGroup:
								AkWwiseProjectInfo.GetData().SwitchWwu[in_wwuIndex].List.Add(valueToAdd);
								break;
						}
					}
				}
				break;

			default:
				UnityEngine.Debug.LogError("WwiseUnity: Unknown asset type in WWU parser");
				break;
		}
	}

	private bool CreateWorkUnit(string in_relativePath, string in_wwuType, string in_fullPath)
	{
		var ParentID = string.Empty;
		try
		{
			var reader = System.Xml.XmlReader.Create(in_fullPath);
			reader.MoveToContent();

			//We check if the current work unit has a parent and save its guid if its the case
			while (!reader.EOF && reader.ReadState == System.Xml.ReadState.Interactive)
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element && reader.Name.Equals("WorkUnit"))
				{
					if (reader.GetAttribute("PersistMode").Equals("Nested"))
						ParentID = reader.GetAttribute("OwnerID");
					break;
				}

				reader.Read();
			}

			reader.Close();
		}
		catch (System.Exception e)
		{
			UnityEngine.Debug.Log("WwiseUnity: A changed Work unit wasn't found. It must have been deleted " + in_fullPath);
			return false;
		}

		if (!string.IsNullOrEmpty(ParentID))
		{
			System.Guid parentGuid = System.Guid.Empty;

			try
			{
				parentGuid = new System.Guid(ParentID);
			}
			catch
			{
				UnityEngine.Debug.LogWarning("WwiseUnity: \"OwnerID\" in <" + in_fullPath + "> cannot be converted to a GUID (" + ParentID + ")");
				return false;
			}

			var list = AkWwiseProjectInfo.GetData().GetWwuListByString(in_wwuType);

			//search for the parent and save its physical path
			for (var i = 0; i < list.Count; i++)
			{
				var wwu = list[i] as AkWwiseProjectData.WorkUnit;
				if (wwu.Guid.Equals(parentGuid))
				{
					UpdateWorkUnit(wwu.PhysicalPath, in_fullPath, in_wwuType, in_relativePath);
					return true;
				}
			}

			//Not found.  Wait for it to load
			return false;
		}

		//Root Wwu
		UpdateWorkUnit(string.Empty, in_fullPath, in_wwuType, in_relativePath);
		return true;
	}

	private void UpdateWorkUnit(string in_parentRelativePath, string in_wwuFullPath, string in_wwuType,
		string in_relativePath)
	{
		var wwuRelPath = in_parentRelativePath;

		var PathAndIcons = new System.Collections.Generic.LinkedList<AkWwiseProjectData.PathElement>();

		//We need to build the work unit's hierarchy to display it in the right place in the picker
		var currentPathInProj = string.Empty;
		while (!wwuRelPath.Equals(string.Empty))
		{
			//Add work unit name to the hierarchy
			var wwuName = System.IO.Path.GetFileNameWithoutExtension(wwuRelPath);
			currentPathInProj = System.IO.Path.Combine(wwuName, currentPathInProj);
			//Add work unit icon to the hierarchy
			PathAndIcons.AddFirst(new AkWwiseProjectData.PathElement(wwuName, WwiseObjectType.WorkUnit));

			//Get the physical path of the parent work unit if any
			var list = AkWwiseProjectInfo.GetData().GetWwuListByString(in_wwuType);
			var index = list.BinarySearch(new AkWwiseProjectData.WorkUnit { PhysicalPath = wwuRelPath });
			wwuRelPath = (list[index] as AkWwiseProjectData.WorkUnit).ParentPhysicalPath;
		}

		//Add physical folders to the hierarchy if the work unit isn't in the root folder
		var physicalPath = in_relativePath.Split(System.IO.Path.DirectorySeparatorChar);
		for (var i = physicalPath.Length - 2; i > 0; i--)
		{
			PathAndIcons.AddFirst(
				new AkWwiseProjectData.PathElement(physicalPath[i], WwiseObjectType.PhysicalFolder));
			currentPathInProj = System.IO.Path.Combine(physicalPath[i], currentPathInProj);
		}

		//Parse the work unit file
		RecurseWorkUnit(AssetType.Create(in_wwuType), new System.IO.FileInfo(in_wwuFullPath), currentPathInProj,
			in_relativePath.Remove(in_relativePath.LastIndexOf(System.IO.Path.DirectorySeparatorChar)), PathAndIcons,
			in_parentRelativePath);
	}

	public class AssetType
	{
		public string RootDirectoryName { get; set; }
		public string XmlElementName;
		public string ChildElementName;
		public WwiseObjectType Type = WwiseObjectType.None;
		public WwiseObjectType ChildType = WwiseObjectType.None;

		public static AssetType[] ScannedAssets
		{
			get { return _ScannedAssets; }
		}

		public static AssetType Create(string rootDirectoryName)
		{
			foreach (var asset in ScannedAssets)
				if (string.Equals(rootDirectoryName, asset.RootDirectoryName, System.StringComparison.OrdinalIgnoreCase))
					return asset;

			return null;
		}

		private AssetType(string RootFolder, string XmlElemName, WwiseObjectType type)
		{
			RootDirectoryName = RootFolder;
			XmlElementName = XmlElemName;
			Type = type;
		}

		private static readonly AssetType[] _ScannedAssets = new AssetType[]
		{
			new AssetType("Master-Mixer Hierarchy", "AuxBus", WwiseObjectType.AuxBus),
			new AssetType("Events", "Event", WwiseObjectType.Event),
			new AssetType("SoundBanks", "SoundBank", WwiseObjectType.Soundbank),
			new AssetType("States", "StateGroup", WwiseObjectType.StateGroup) { ChildElementName = "State", ChildType = WwiseObjectType.State },
			new AssetType("Switches", "SwitchGroup", WwiseObjectType.SwitchGroup) { ChildElementName = "Switch", ChildType = WwiseObjectType.Switch },
			new AssetType("Game Parameters", "GameParameter", WwiseObjectType.GameParameter),
			new AssetType("Triggers", "Trigger", WwiseObjectType.Trigger),
			new AssetType("Virtual Acoustics", "AcousticTexture", WwiseObjectType.AcousticTexture),
		};
	}

	private class FileInfo_CompareByPath : System.Collections.Generic.IComparer<System.IO.FileInfo>
	{
		int System.Collections.Generic.IComparer<System.IO.FileInfo>.Compare(System.IO.FileInfo wwuA, System.IO.FileInfo wwuB)
		{
			return wwuA.FullName.CompareTo(wwuB.FullName);
		}
	}
}
#endif

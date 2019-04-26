#if UNITY_EDITOR
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2014 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

public class AkWwiseTreeView : AK.Wwise.TreeView.TreeViewControl
{
	public AK.Wwise.TreeView.TreeViewItem LastDoubleClickedItem;

	private UnityEngine.GUIStyle m_filterBoxStyle;
	private UnityEngine.GUIStyle m_filterBoxCancelButtonStyle;
	private string m_filterString = string.Empty;
	private static System.Collections.Generic.Dictionary<WwiseObjectType, UnityEditor.MonoScript> DragDropMonoScriptMap;

	public AkWwiseTreeView()
	{
#if UNITY_2017_2_OR_NEWER
		UnityEditor.EditorApplication.playModeStateChanged += (UnityEditor.PlayModeStateChange playMode) =>
		{
			if (playMode == UnityEditor.PlayModeStateChange.ExitingEditMode)
				SaveExpansionStatus();
		};
#else
		UnityEditor.EditorApplication.playmodeStateChanged += () =>
		{
			if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode && !UnityEditor.EditorApplication.isPlaying)
				SaveExpansionStatus();
		};
#endif
	}

	public class AkTreeInfo
	{
		public System.Guid Guid;
		public WwiseObjectType ObjectType;

		public AkTreeInfo(WwiseObjectType objType)
		{
			ObjectType = objType;
		}

		public AkTreeInfo(System.Guid guid, WwiseObjectType objType)
		{
			ObjectType = objType;
			Guid = guid;
		}
	}

	private AK.Wwise.TreeView.TreeViewItem AddPathToTreeItem(AK.Wwise.TreeView.TreeViewItem item,
		AkWwiseProjectData.AkInformation AkInfo)
	{
		var parentItem = item;

		var path = "/" + RootItem.Header + "/" + item.Header;

		for (var i = 0; i < AkInfo.PathAndIcons.Count; i++)
		{
			var PathElem = AkInfo.PathAndIcons[i];
			var childItem = parentItem.FindItemByName(PathElem.ElementName);

			path = path + "/" + PathElem.ElementName;

			if (childItem == null)
			{
				if (i != AkInfo.PathAndIcons.Count - 1)
				{
					childItem = parentItem.AddItem(PathElem.ElementName,
						new AkTreeInfo(System.Guid.Empty, PathElem.ObjectType), GetExpansionStatus(path));
				}
				else
				{
					var isDraggable = !(PathElem.ObjectType == WwiseObjectType.StateGroup ||
					                    PathElem.ObjectType == WwiseObjectType.SwitchGroup);
					childItem = parentItem.AddItem(PathElem.ElementName, isDraggable, GetExpansionStatus(path),
						new AkTreeInfo(AkInfo.Guid, PathElem.ObjectType));
				}
			}

			AddHandlerEvents(childItem);
			parentItem = childItem;
		}

		return parentItem;
	}

	public void SetRootItem(string Header, WwiseObjectType ObjType)
	{
		RootItem.Items.Clear();
		RootItem.Header = Header;
		RootItem.DataContext = new AkTreeInfo(ObjType);
		AddHandlerEvents(RootItem);

		RootItem.IsExpanded = GetExpansionStatus("/" + RootItem.Header);
	}

	public void PopulateItem(AK.Wwise.TreeView.TreeViewItem attachTo, string itemName,
		System.Collections.Generic.List<AkWwiseProjectData.AkInfoWorkUnit> workUnits)
	{
		var attachPoint = attachTo.AddItem(itemName, false, GetExpansionStatus("/" + RootItem.Header + "/" + itemName),
			new AkTreeInfo(WwiseObjectType.PhysicalFolder));

		foreach (var wwu in workUnits)
		{
			foreach (var akInfo in wwu.List)
				AddHandlerEvents(AddPathToTreeItem(attachPoint, akInfo));
		}

		AddHandlerEvents(attachPoint);
	}

	public void PopulateItem(AK.Wwise.TreeView.TreeViewItem attachTo, string itemName,
		System.Collections.Generic.List<AkWwiseProjectData.EventWorkUnit> Events)
	{
		var akInfoWwu = new System.Collections.Generic.List<AkWwiseProjectData.AkInfoWorkUnit>(Events.Count);
		for (var i = 0; i < Events.Count; i++)
		{
			akInfoWwu.Add(new AkWwiseProjectData.AkInfoWorkUnit());
			akInfoWwu[i].PhysicalPath = Events[i].PhysicalPath;
			akInfoWwu[i].ParentPhysicalPath = Events[i].ParentPhysicalPath;
			akInfoWwu[i].Guid = Events[i].Guid;
			akInfoWwu[i].List = Events[i].List.ConvertAll(x => (AkWwiseProjectData.AkInformation) x);
		}

		PopulateItem(attachTo, itemName, akInfoWwu);
	}

	public void PopulateItem(AK.Wwise.TreeView.TreeViewItem attachTo, string itemName,
		System.Collections.Generic.List<AkWwiseProjectData.GroupValWorkUnit> GroupWorkUnits)
	{
		var attachPoint = attachTo.AddItem(itemName, false, GetExpansionStatus("/" + RootItem.Header + "/" + itemName),
			new AkTreeInfo(WwiseObjectType.PhysicalFolder));

		foreach (var wwu in GroupWorkUnits)
		{
			foreach (var group in wwu.List)
			{
				var groupItem = AddPathToTreeItem(attachPoint, group);
				AddHandlerEvents(groupItem);

				foreach (var child in group.values)
				{
					var item = groupItem.AddItem(child.Name, true, false, new AkTreeInfo(child.Guid, child.PathAndIcons[0].ObjectType));
					AddHandlerEvents(item);
				}
			}
		}

		AddHandlerEvents(attachPoint);
	}

	/// <summary>
	///     Handler functions for TreeViewControl
	/// </summary>
	private void AddHandlerEvents(AK.Wwise.TreeView.TreeViewItem item)
	{
		// Uncomment this when we support right click
		item.Click = HandleClick;
		item.Dragged = PrepareDragDrop;
		item.CustomIconBuilder = CustomIconHandler;
	}

	private void HandleClick(object sender, System.EventArgs args)
	{
		if (UnityEngine.Event.current.button == 0)
		{
			if ((args as AK.Wwise.TreeView.TreeViewItem.ClickEventArgs).m_clickCount == 2)
			{
				LastDoubleClickedItem = (AK.Wwise.TreeView.TreeViewItem) sender;

				if (LastDoubleClickedItem.HasChildItems())
					LastDoubleClickedItem.IsExpanded = !LastDoubleClickedItem.IsExpanded;
			}
		}
	}

	private void PrepareDragDrop(object sender, System.EventArgs args)
	{
		var item = (AK.Wwise.TreeView.TreeViewItem) sender;
		try
		{
			if (item == null || !item.IsDraggable)
				return;

			var treeInfo = (AkTreeInfo)item.DataContext;
			var reference = WwiseObjectReference.FindOrCreateWwiseObject(treeInfo.ObjectType, item.Header, treeInfo.Guid);
			if (!reference)
				return;

			var groupReference = reference as WwiseGroupValueObjectReference;
			if (groupReference)
			{
				var ParentTreeInfo = (AkTreeInfo)item.Parent.DataContext;
				groupReference.SetupGroupObjectReference(item.Parent.Header, ParentTreeInfo.Guid);
			}

			UnityEditor.MonoScript script;
			if (DragDropMonoScriptMap.TryGetValue(reference.WwiseObjectType, out script))
			{
				UnityEngine.GUIUtility.hotControl = 0;
				UnityEditor.DragAndDrop.PrepareStartDrag();
				UnityEditor.DragAndDrop.objectReferences = new UnityEngine.Object[] { script };
				AkUtilities.DragAndDropObjectReference = reference;
				UnityEditor.DragAndDrop.StartDrag("Dragging an AkObject");
			}
		}
		catch (System.Exception e)
		{
			UnityEngine.Debug.Log(e.ToString());
		}
	}

	private void ShowButtonTextureInternal(UnityEngine.Texture2D texture)
	{
		if (null == texture || m_forceButtonText)
			UnityEngine.GUILayout.Button("", UnityEngine.GUILayout.MaxWidth(16));
		else
			ShowButtonTexture(texture);
	}

	public void CustomIconHandler(object sender, System.EventArgs args)
	{
		var item = (AK.Wwise.TreeView.TreeViewItem) sender;
		var treeInfo = (AkTreeInfo) item.DataContext;

		switch (treeInfo.ObjectType)
		{
			case WwiseObjectType.AcousticTexture:
				ShowButtonTextureInternal(m_textureWwiseAcousticTextureIcon);
				break;
			case WwiseObjectType.AuxBus:
				ShowButtonTextureInternal(m_textureWwiseAuxBusIcon);
				break;
			case WwiseObjectType.Bus:
				ShowButtonTextureInternal(m_textureWwiseBusIcon);
				break;
			case WwiseObjectType.Event:
				ShowButtonTextureInternal(m_textureWwiseEventIcon);
				break;
			case WwiseObjectType.Folder:
				ShowButtonTextureInternal(m_textureWwiseFolderIcon);
				break;
			case WwiseObjectType.GameParameter:
				ShowButtonTextureInternal(m_textureWwiseGameParameterIcon);
				break;
			case WwiseObjectType.PhysicalFolder:
				ShowButtonTextureInternal(m_textureWwisePhysicalFolderIcon);
				break;
			case WwiseObjectType.Project:
				ShowButtonTextureInternal(m_textureWwiseProjectIcon);
				break;
			case WwiseObjectType.Soundbank:
				ShowButtonTextureInternal(m_textureWwiseSoundbankIcon);
				break;
			case WwiseObjectType.State:
				ShowButtonTextureInternal(m_textureWwiseStateIcon);
				break;
			case WwiseObjectType.StateGroup:
				ShowButtonTextureInternal(m_textureWwiseStateGroupIcon);
				break;
			case WwiseObjectType.Switch:
				ShowButtonTextureInternal(m_textureWwiseSwitchIcon);
				break;
			case WwiseObjectType.SwitchGroup:
				ShowButtonTextureInternal(m_textureWwiseSwitchGroupIcon);
				break;
			case WwiseObjectType.WorkUnit:
				ShowButtonTextureInternal(m_textureWwiseWorkUnitIcon);
				break;
			default:
				break;
		}
	}

	/// <summary>
	///     Wwise logos
	/// </summary>

	private UnityEngine.Texture2D m_textureWwiseAcousticTextureIcon;
	private UnityEngine.Texture2D m_textureWwiseAuxBusIcon;
	private UnityEngine.Texture2D m_textureWwiseBusIcon;
	private UnityEngine.Texture2D m_textureWwiseEventIcon;
	private UnityEngine.Texture2D m_textureWwiseFolderIcon;
	private UnityEngine.Texture2D m_textureWwiseGameParameterIcon;
	private UnityEngine.Texture2D m_textureWwisePhysicalFolderIcon;
	private UnityEngine.Texture2D m_textureWwiseProjectIcon;
	private UnityEngine.Texture2D m_textureWwiseSoundbankIcon;
	private UnityEngine.Texture2D m_textureWwiseStateIcon;
	private UnityEngine.Texture2D m_textureWwiseStateGroupIcon;
	private UnityEngine.Texture2D m_textureWwiseSwitchIcon;
	private UnityEngine.Texture2D m_textureWwiseSwitchGroupIcon;
	private UnityEngine.Texture2D m_textureWwiseWorkUnitIcon;

	/// <summary>
	///     TreeViewControl overrides for our custom logos
	/// </summary>
	public override void AssignDefaults()
	{
		base.AssignDefaults();
		var tempWwisePath = "Assets/Wwise/Editor/WwiseWindows/TreeViewControl/";

		m_textureWwiseAcousticTextureIcon = GetTexture(tempWwisePath + "acoustictexture_nor.png");
		m_textureWwiseAuxBusIcon = GetTexture(tempWwisePath + "auxbus_nor.png");
		m_textureWwiseBusIcon = GetTexture(tempWwisePath + "bus_nor.png");
		m_textureWwiseEventIcon = GetTexture(tempWwisePath + "event_nor.png");
		m_textureWwiseFolderIcon = GetTexture(tempWwisePath + "folder_nor.png");
		m_textureWwiseGameParameterIcon = GetTexture(tempWwisePath + "gameparameter_nor.png");
		m_textureWwisePhysicalFolderIcon = GetTexture(tempWwisePath + "physical_folder_nor.png");
		m_textureWwiseProjectIcon = GetTexture(tempWwisePath + "wproj.png");
		m_textureWwiseSoundbankIcon = GetTexture(tempWwisePath + "soundbank_nor.png");
		m_textureWwiseStateIcon = GetTexture(tempWwisePath + "state_nor.png");
		m_textureWwiseStateGroupIcon = GetTexture(tempWwisePath + "stategroup_nor.png");
		m_textureWwiseSwitchIcon = GetTexture(tempWwisePath + "switch_nor.png");
		m_textureWwiseSwitchGroupIcon = GetTexture(tempWwisePath + "switchgroup_nor.png");
		m_textureWwiseWorkUnitIcon = GetTexture(tempWwisePath + "workunit_nor.png");

		if (m_filterBoxStyle == null)
		{
			var InspectorSkin =
				UnityEngine.Object.Instantiate(UnityEditor.EditorGUIUtility.GetBuiltinSkin(UnityEditor.EditorSkin.Inspector));
			InspectorSkin.hideFlags = UnityEngine.HideFlags.HideAndDontSave;
			m_filterBoxStyle = InspectorSkin.FindStyle("SearchTextField");
			m_filterBoxCancelButtonStyle = InspectorSkin.FindStyle("SearchCancelButton");
		}

		if (DragDropMonoScriptMap == null)
		{
			DragDropMonoScriptMap = new System.Collections.Generic.Dictionary<WwiseObjectType, UnityEditor.MonoScript>();

			var scripts = UnityEngine.Resources.FindObjectsOfTypeAll<UnityEditor.MonoScript>();
			foreach (var script in scripts)
			{
				WwiseObjectType wwiseObjectType;
				var type = script.GetClass();
				if (type != null && ScriptTypeMap.TryGetValue(type, out wwiseObjectType))
					DragDropMonoScriptMap[wwiseObjectType] = script;
			}
		}
	}

	private static System.Collections.Generic.Dictionary<System.Type, WwiseObjectType> ScriptTypeMap
		= new System.Collections.Generic.Dictionary<System.Type, WwiseObjectType>{
		{ typeof(AkAmbient), WwiseObjectType.Event },
		{ typeof(AkBank), WwiseObjectType.Soundbank },
		{ typeof(AkEnvironment), WwiseObjectType.AuxBus },
		{ typeof(AkState), WwiseObjectType.State },
		{ typeof(AkSurfaceReflector), WwiseObjectType.AcousticTexture },
		{ typeof(AkSwitch), WwiseObjectType.Switch },
	};

	public override void DisplayTreeView(DisplayTypes displayType)
	{
		if (AkUtilities.IsWwiseProjectAvailable)
		{
			var filterString = m_filterString;

			if (m_filterBoxStyle == null)
			{
				m_filterBoxStyle = UnityEngine.Object
					.Instantiate(UnityEditor.EditorGUIUtility.GetBuiltinSkin(UnityEditor.EditorSkin.Inspector))
					.FindStyle("SearchTextField");
				m_filterBoxCancelButtonStyle = UnityEngine.Object
					.Instantiate(UnityEditor.EditorGUIUtility.GetBuiltinSkin(UnityEditor.EditorSkin.Inspector))
					.FindStyle("SearchCancelButton");
			}

			using (new UnityEngine.GUILayout.HorizontalScope("box"))
			{
				m_filterString = UnityEngine.GUILayout.TextField(m_filterString, m_filterBoxStyle);
				if (UnityEngine.GUILayout.Button("", m_filterBoxCancelButtonStyle))
					m_filterString = "";
			}

			if (!m_filterString.Equals(filterString))
			{
				if (filterString.Equals(string.Empty))
					SaveExpansionStatus();

				FilterTreeview(RootItem);

				if (m_filterString.Equals(string.Empty))
				{
					var path = "";
					RestoreExpansionStatus(RootItem, ref path);
				}
			}

			base.DisplayTreeView(displayType);
		}
		else
		{
			UnityEngine.GUILayout.Label("Wwise Project not found at path:");
			UnityEngine.GUILayout.Label(AkUtilities.GetFullPath(UnityEngine.Application.dataPath,
				WwiseSetupWizard.Settings.WwiseProjectPath));
			UnityEngine.GUILayout.Label("Wwise Picker will not be usable.");
		}
	}

	private bool FilterTreeview(AK.Wwise.TreeView.TreeViewItem in_item)
	{
		in_item.IsHidden = in_item.Header.IndexOf(m_filterString, System.StringComparison.OrdinalIgnoreCase) < 0;
		in_item.IsExpanded = true;

		for (var i = 0; i < in_item.Items.Count; i++)
		{
			if (!FilterTreeview(in_item.Items[i]))
				in_item.IsHidden = false;
		}

		return in_item.IsHidden;
	}

	private void RestoreExpansionStatus(AK.Wwise.TreeView.TreeViewItem in_item, ref string in_path)
	{
		in_path = in_path + "/" + in_item.Header;

		in_item.IsExpanded = GetExpansionStatus(in_path);

		for (var i = 0; i < in_item.Items.Count; i++)
			RestoreExpansionStatus(in_item.Items[i], ref in_path);

		in_path = in_path.Remove(in_path.LastIndexOf('/'));
	}

	public void SaveExpansionStatus()
	{
		if (AkUtilities.IsWwiseProjectAvailable)
		{
			if (RootItem.Header == "Root item")
			{
				// We were unpopulated, no need to save. But we still need to display the correct data, though.
				AkWwisePicker.PopulateTreeview();
				return;
			}

			if (AkWwiseProjectInfo.GetData() != null)
			{
                var PreviousExpandedItems = AkWwiseProjectInfo.GetData().ExpandedItems;
				AkWwiseProjectInfo.GetData().ExpandedItems.Clear();

				var path = string.Empty;

				if (RootItem.HasChildItems() && RootItem.IsExpanded)
					SaveExpansionStatus(RootItem, path);

				AkWwiseProjectInfo.GetData().ExpandedItems.Sort();

                if (System.Linq.Enumerable.Count(System.Linq.Enumerable.Except(AkWwiseProjectInfo.GetData().ExpandedItems, PreviousExpandedItems)) > 0)
                    UnityEditor.EditorUtility.SetDirty(AkWwiseProjectInfo.GetData());
			}
		}
	}

	private void SaveExpansionStatus(AK.Wwise.TreeView.TreeViewItem in_item, string in_path)
	{
		in_path = in_path + "/" + in_item.Header;

		AkWwiseProjectInfo.GetData().ExpandedItems.Add(in_path);

		for (var i = 0; i < in_item.Items.Count; i++)
		{
			if (in_item.Items[i].HasChildItems() && in_item.Items[i].IsExpanded)
				SaveExpansionStatus(in_item.Items[i], in_path);
		}

		in_path = in_path.Remove(in_path.LastIndexOf('/'));
	}

	public bool GetExpansionStatus(string in_path)
	{
		return AkWwiseProjectInfo.GetData().ExpandedItems.BinarySearch(in_path) >= 0;
	}

	public void SetScrollViewPosition(UnityEngine.Vector2 in_pos)
	{
		m_scrollView = in_pos;
	}

	public AK.Wwise.TreeView.TreeViewItem GetItemByPath(string in_path)
	{
		var headers = in_path.Split('/');

		if (!RootItem.Header.Equals(headers[0]))
			return null;

		var item = RootItem;

		for (var i = 1; i < headers.Length; i++)
		{
			item = item.Items.Find(x => x.Header.Equals(headers[i]));

			if (item == null)
				return null;
		}

		return item;
	}

	public AK.Wwise.TreeView.TreeViewItem GetItemByGuid(System.Guid in_guid)
	{
		return GetItemByGuid(RootItem, in_guid);
	}

	public AK.Wwise.TreeView.TreeViewItem GetItemByGuid(AK.Wwise.TreeView.TreeViewItem in_item, System.Guid in_guid)
	{
		var itemGuid = (in_item.DataContext as AkTreeInfo).Guid;

		if (itemGuid.Equals(in_guid))
			return in_item;

		for (var i = 0; i < in_item.Items.Count; i++)
		{
			var item = GetItemByGuid(in_item.Items[i], in_guid);

			if (item != null)
				return item;
		}

		return null;
	}

	public AK.Wwise.TreeView.TreeViewItem GetItemByType(WwiseObjectType type)
	{
		return GetItemByType(RootItem, type);
	}

	public AK.Wwise.TreeView.TreeViewItem GetItemByType(AK.Wwise.TreeView.TreeViewItem in_item, WwiseObjectType type)
	{
		if ((in_item.DataContext as AkTreeInfo).ObjectType == type)
			return in_item;

		for (var i = 0; i < in_item.Items.Count; i++)
		{
			var item = GetItemByType(in_item.Items[i], type);
			if (item != null)
				return item;
		}

		return null;
	}

	public AK.Wwise.TreeView.TreeViewItem GetSelectedItem()
	{
		return GetSelectedItem(RootItem);
	}

	public AK.Wwise.TreeView.TreeViewItem GetSelectedItem(AK.Wwise.TreeView.TreeViewItem in_item)
	{
		if (in_item.IsSelected)
			return in_item;

		for (var i = 0; i < in_item.Items.Count; i++)
		{
			var item = GetSelectedItem(in_item.Items[i]);

			if (item != null)
				return item;
		}

		return null;
	}
}
#endif

//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2018 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

/// @brief Represents Wwise objects as Unity assets.
public abstract class WwiseObjectReference : UnityEngine.ScriptableObject
{
	#region Serialized fields
	[AkShowOnly]
	[UnityEngine.SerializeField]
	private string objectName;

	[AkShowOnly]
	[UnityEngine.SerializeField]
	private uint id = AkSoundEngine.AK_INVALID_UNIQUE_ID;

	[AkShowOnly]
	[UnityEngine.SerializeField]
	private string guid;
	#endregion

	#region Properties
	/// <summary>
	/// The Wwise GUID which is represented by the ScriptableObject's asset file name.
	/// </summary>
	public System.Guid Guid
	{
		get { return string.IsNullOrEmpty(guid) ? System.Guid.Empty : new System.Guid(guid); }
	}

	/// <summary>
	/// The name of the Wwise object.
	/// </summary>
	public string ObjectName
	{
		get { return objectName; }
	}

	/// <summary>
	/// The display name for the Wwise object.
	/// </summary>
	public virtual string DisplayName
	{
		get { return ObjectName; }
	}

	/// <summary>
	/// The Wwise ID.
	/// </summary>
	public uint Id
	{
		get { return id; }
	}

	/// <summary>
	/// The type of the Wwise object resource (for example: Event, State or Switch).
	/// </summary>
	public abstract WwiseObjectType WwiseObjectType
	{
		get;
	}
	#endregion

#if UNITY_EDITOR
	#region Creation and File Management
	private static System.Collections.Generic.Dictionary<WwiseObjectType, System.Type> m_WwiseObjectReferenceClasses
		= new System.Collections.Generic.Dictionary<WwiseObjectType, System.Type>();

	protected static void RegisterWwiseObjectReferenceClass<T>(WwiseObjectType wwiseObjectType) where T : WwiseObjectReference
	{
		var type = typeof(T);
		if (m_WwiseObjectReferenceClasses.ContainsKey(wwiseObjectType))
		{
			UnityEngine.Debug.LogError("WwiseUnity: WwiseObjectReference subclass <" + type.Name + "> already registered for <WwiseObjectType." + wwiseObjectType + ">.");
			return;
		}

		m_WwiseObjectReferenceClasses.Add(wwiseObjectType, type);
	}

	private static WwiseObjectReference Create(WwiseObjectType wwiseObjectType)
	{
		System.Type type = null;
		if (m_WwiseObjectReferenceClasses.TryGetValue(wwiseObjectType, out type))
			return (WwiseObjectReference)CreateInstance(type);

		return CreateInstance<WwiseObjectReference>();
	}

	public static WwiseObjectReference FindOrCreateWwiseObject(WwiseObjectType wwiseObjectType, string name, System.Guid guid)
	{
		var path = AssetFilePath(wwiseObjectType, guid, true);
		var loadedAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<WwiseObjectReference>(path);
		var asset = loadedAsset ? loadedAsset : Create(wwiseObjectType);
		asset.objectName = name;
		asset.id = AkUtilities.ShortIDGenerator.Compute(name);
		asset.guid = guid.ToString().ToUpper();

		if (loadedAsset)
			UnityEditor.EditorUtility.SetDirty(asset);
		else
			UnityEditor.AssetDatabase.CreateAsset(asset, path);

		return asset;
	}

	public static void UpdateWwiseObject(WwiseObjectType wwiseObjectType, string name, System.Guid guid)
	{
		var path = AssetFilePath(wwiseObjectType, guid, false);
		var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<WwiseObjectReference>(path);
		if (!asset)
			return;

		asset.objectName = name;
		asset.id = AkUtilities.ShortIDGenerator.Compute(name);

		UnityEditor.EditorUtility.SetDirty(asset);
	}

	public static void DeleteWwiseObject(WwiseObjectType wwiseObjectType, System.Guid guid)
	{
		var path = AssetFilePath(wwiseObjectType, guid, false);
		UnityEditor.AssetDatabase.DeleteAsset(path);
	}

	private static string AssetFilePath(string relativePath, string fileName, bool createDirectory)
	{
		if (createDirectory)
		{
			var fullPath = System.IO.Path.Combine(UnityEngine.Application.dataPath, relativePath);
			if (!System.IO.Directory.Exists(fullPath))
				System.IO.Directory.CreateDirectory(fullPath);
		}

		return System.IO.Path.Combine(System.IO.Path.Combine("Assets", relativePath), fileName + ".asset");
	}

	private static string AssetFilePath(WwiseObjectType wwiseObjectType, System.Guid guid, bool createDirectory)
	{
		var relativePath = System.IO.Path.Combine(System.IO.Path.Combine("Wwise", "Resources"), wwiseObjectType.ToString());
		var fileName = guid.ToString().ToUpper();
		return AssetFilePath(relativePath, fileName, createDirectory);
	}
	#endregion

	#region WwiseMigration
	private class WwiseObjectData
	{
		public string objectName;
	}

	private static System.Collections.Generic.Dictionary<WwiseObjectType, System.Collections.Generic.Dictionary<System.Guid, WwiseObjectData>> WwiseObjectDataMap
		= new System.Collections.Generic.Dictionary<WwiseObjectType, System.Collections.Generic.Dictionary<System.Guid, WwiseObjectData>>();

	public static void ClearWwiseObjectDataMap()
	{
		WwiseObjectDataMap.Clear();
	}

	public static void UpdateWwiseObjectDataMap(WwiseObjectType wwiseObjectType, string name, System.Guid guid)
	{
		System.Collections.Generic.Dictionary<System.Guid, WwiseObjectData> map = null;
		if (!WwiseObjectDataMap.TryGetValue(wwiseObjectType, out map))
		{
			map = new System.Collections.Generic.Dictionary<System.Guid, WwiseObjectData>();
			WwiseObjectDataMap.Add(wwiseObjectType, map);
		}

		WwiseObjectData data = null;
		if (!map.TryGetValue(guid, out data))
		{
			data = new WwiseObjectData();
			map.Add(guid, data);
		}

		data.objectName = name;
	}

	public static WwiseObjectReference GetWwiseObjectForMigration(WwiseObjectType wwiseObjectType, byte[] valueGuid, int id)
	{
		if (valueGuid == null)
		{
			return null;
		}

		System.Collections.Generic.Dictionary<System.Guid, WwiseObjectData> map = null;
		if (!WwiseObjectDataMap.TryGetValue(wwiseObjectType, out map) || map == null)
		{
			UnityEngine.Debug.LogWarning("WwiseUnity: Cannot find WwiseObjectReferences of type <WwiseObjectType." + wwiseObjectType + ">.");
			return null;
		}

		var guid = System.Guid.Empty;
		WwiseObjectData data = null;

		try
		{
			guid = new System.Guid(valueGuid);
		}
		catch
		{
			UnityEngine.Debug.LogWarning("WwiseUnity: Invalid guid for WwiseObjectReference of type <WwiseObjectType." + wwiseObjectType + ">.");
			return null;
		}

		if (guid != System.Guid.Empty && !map.TryGetValue(guid, out data))
		{
			UnityEngine.Debug.LogWarning("WwiseUnity: Cannot find guid <" + guid.ToString() + "> for WwiseObjectReference of type <WwiseObjectType." + wwiseObjectType + "> in Wwise Project.");

			foreach (var pair in map)
			{
				if ((int)AkUtilities.ShortIDGenerator.Compute(pair.Value.objectName) == id)
				{
					guid = pair.Key;
					data = pair.Value;
					UnityEngine.Debug.LogWarning("WwiseUnity: Found guid <" + guid.ToString() + "> for <" + pair.Value.objectName + ">.");
					break;
				}
			}
		}

		if (data == null)
		{
			return null;
		}

		var objectReference = FindOrCreateWwiseObject(wwiseObjectType, data.objectName, guid);
		if (objectReference && objectReference.Id != (uint)id)
		{
			UnityEngine.Debug.LogWarning("WwiseUnity: ID mismatch for WwiseObjectReference of type <WwiseObjectType." + wwiseObjectType + ">. Expected <" + ((uint)id) + ">. Found <" + objectReference.Id + ">.");
		}

		return objectReference;
	}
	#endregion
#endif
}

/// @brief Represents Wwise group value objects (such as states and switches) as Unity assets.
public abstract class WwiseGroupValueObjectReference : WwiseObjectReference
{
	#region Properties
	/// <summary>
	/// The group object reference.
	/// </summary>
	public abstract WwiseObjectReference GroupObjectReference
	{
		get; set;
	}

	/// <summary>
	/// The type of the Wwise object resource (for example: Event, State or Switch).
	/// </summary>
	public abstract WwiseObjectType GroupWwiseObjectType
	{
		get;
	}

	/// <summary>
	/// The display name for the Wwise object.
	/// </summary>
	public override string DisplayName
	{
		get
		{
#if AK_DISPLAY_GROUP_TYPES_WITH_SINGLE_NAME
			return ObjectName;
#else
			var groupReference = GroupObjectReference;
			if (!groupReference)
				return ObjectName;

			return groupReference.ObjectName + " / " + ObjectName;
#endif // AK_DISPLAY_GROUP_TYPES_WITH_SINGLE_NAME
		}
	}
	#endregion

#if UNITY_EDITOR
	public void SetupGroupObjectReference(string name, System.Guid guid)
	{
		GroupObjectReference = FindOrCreateWwiseObject(GroupWwiseObjectType, name, guid);
	}

	#region WwiseMigration
	public static WwiseGroupValueObjectReference GetWwiseObjectForMigration(WwiseObjectType wwiseObjectType, byte[] valueGuid, int id, byte[] groupGuid, int groupId)
	{
		var objectReference = GetWwiseObjectForMigration(wwiseObjectType, valueGuid, id);
		if (!objectReference)
			return null;

		var groupValueObjectReference = objectReference as WwiseGroupValueObjectReference;
		if (!groupValueObjectReference)
		{
			UnityEngine.Debug.LogWarning("WwiseUnity: Not setting WwiseObjectReference since it is not a WwiseGroupValueObjectReference.");
			return null;
		}

		var groupObjectReference = GetWwiseObjectForMigration(groupValueObjectReference.GroupWwiseObjectType, groupGuid, groupId);
		if (!groupObjectReference)
		{
			UnityEngine.Debug.LogWarning("WwiseUnity: Not setting WwiseObjectReference since its GroupObjectReference cannot be determined.");
			return null;
		}

		groupValueObjectReference.GroupObjectReference = groupObjectReference;
		UnityEditor.EditorUtility.SetDirty(groupValueObjectReference);
		return groupValueObjectReference;
	}
	#endregion
#endif
}

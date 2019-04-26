#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2014 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

[UnityEngine.AddComponentMenu("Wwise/AkBank")]
[UnityEngine.ExecuteInEditMode]
/// @brief Loads and unloads a SoundBank at a specified moment. Vorbis sounds can be decompressed at a specified moment using the decode compressed data option. In that case, the SoundBank will be prepared.
public class AkBank : AkTriggerHandler
#if UNITY_EDITOR
	, AK.Wwise.IMigratable
#endif
{
	public AK.Wwise.Bank data = new AK.Wwise.Bank();

	/// Decode this SoundBank upon load
	public bool decodeBank = false;

	/// Check this to load the SoundBank in the background. Be careful, if Events are triggered and the SoundBank hasn't finished loading, you'll have "Event not found" errors.
	public bool loadAsynchronous = false;

	/// Save the decoded SoundBank to disk for faster loads in the future
	public bool saveDecodedBank = false;

	/// Reserved.
	public System.Collections.Generic.List<int> unloadTriggerList =
		new System.Collections.Generic.List<int> { DESTROY_TRIGGER_ID };

	protected override void Awake()
	{
#if UNITY_EDITOR
		if (UnityEditor.BuildPipeline.isBuildingPlayer || AkUtilities.IsMigrating)
			return;

		var reference = AkUtilities.DragAndDropObjectReference;
		if (reference)
		{
			UnityEngine.GUIUtility.hotControl = 0;
			data.ObjectReference = reference;
		}
#endif

		base.Awake();

		RegisterTriggers(unloadTriggerList, UnloadBank);

		//Call the UnloadBank function if registered to the Awake Trigger
		if (unloadTriggerList.Contains(AWAKE_TRIGGER_ID))
			UnloadBank(null);
	}

	protected override void Start()
	{
#if UNITY_EDITOR
		if (UnityEditor.BuildPipeline.isBuildingPlayer || AkUtilities.IsMigrating)
			return;
#endif

		base.Start();

		//Call the UnloadBank function if registered to the Start Trigger
		if (unloadTriggerList.Contains(START_TRIGGER_ID))
			UnloadBank(null);
	}

	/// Loads the SoundBank
	public override void HandleEvent(UnityEngine.GameObject in_gameObject)
	{
		if (!loadAsynchronous)
			data.Load(decodeBank, saveDecodedBank);
		else
			data.LoadAsync();
	}

	/// Unloads a SoundBank
	public void UnloadBank(UnityEngine.GameObject in_gameObject)
	{
		data.Unload();
	}

	protected override void OnDestroy()
	{
#if UNITY_EDITOR
		if (UnityEditor.BuildPipeline.isBuildingPlayer || AkUtilities.IsMigrating)
			return;
#endif

		base.OnDestroy();

		UnregisterTriggers(unloadTriggerList, UnloadBank);

		if (unloadTriggerList.Contains(DESTROY_TRIGGER_ID))
			UnloadBank(null);
	}

	#region Obsolete
	[System.Obsolete(AkSoundEngine.Deprecation_2018_1_6)]
	public string bankName { get { return data == null ? string.Empty : data.Name; } }

	[System.Obsolete(AkSoundEngine.Deprecation_2018_1_6)]
	public byte[] valueGuid
	{
		get
		{
			if (data == null)
				return null;

			var objRef = data.ObjectReference;
			return !objRef ? null : objRef.Guid.ToByteArray();
		}
	}
	#endregion

	#region WwiseMigration
#pragma warning disable 0414 // private field assigned but not used.
	[UnityEngine.HideInInspector]
	[UnityEngine.SerializeField]
	[UnityEngine.Serialization.FormerlySerializedAs("bankName")]
	private string bankNameInternal;
	[UnityEngine.HideInInspector]
	[UnityEngine.SerializeField]
	[UnityEngine.Serialization.FormerlySerializedAs("valueGuid")]
	private byte[] valueGuidInternal;
#pragma warning restore 0414 // private field assigned but not used.

#if UNITY_EDITOR
	bool AK.Wwise.IMigratable.Migrate(UnityEditor.SerializedObject obj)
	{
		if (!AkUtilities.IsMigrationRequired(AkUtilities.MigrationStep.WwiseTypes_v2018_1_6))
			return false;

		return AK.Wwise.TypeMigration.ProcessSingleGuidType(obj.FindProperty("data.WwiseObjectReference"), WwiseObjectType.Soundbank, 
			obj.FindProperty("valueGuidInternal"), obj.FindProperty("bankNameInternal"));
	}
#endif
	#endregion
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2014 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

public abstract class AkTriggerHandler : UnityEngine.MonoBehaviour
{
	public const int AWAKE_TRIGGER_ID = 1151176110;
	public const int START_TRIGGER_ID = 1281810935;
	public const int DESTROY_TRIGGER_ID = unchecked((int) 3936390293);

	///Since our mask is a 32 bits integer, we can't have more than 32 triggers
	public const int MAX_NB_TRIGGERS = 32;

	///Will contain the types of all the triggers derived from AkTriggerBase at runtime
	public static System.Collections.Generic.Dictionary<uint, string> triggerTypes = AkTriggerBase.GetAllDerivedTypes();

	private bool didDestroy;

	///List containing the enabled triggers.
	public System.Collections.Generic.List<int>
		triggerList = new System.Collections.Generic.List<int> { START_TRIGGER_ID };

	///This property is usefull only when used with colliders.  When enabled, the target of the action will be the other colliding object.  When disabled, it will be the current object.
	public bool useOtherObject = false;

	public abstract void HandleEvent(UnityEngine.GameObject in_gameObject);

	protected virtual void Awake()
	{
#if UNITY_EDITOR
		if (UnityEditor.BuildPipeline.isBuildingPlayer)
			return;
#endif

		RegisterTriggers(triggerList, HandleEvent);

		//Call the Handle event function if registered to the Awake Trigger
		if (triggerList.Contains(AWAKE_TRIGGER_ID))
			HandleEvent(null);
	}

	protected virtual void Start()
	{
		//Call the Handle event function if registered to the Start Trigger
		if (triggerList.Contains(START_TRIGGER_ID))
			HandleEvent(null);
	}

	protected virtual void OnDestroy()
	{
		if (!didDestroy)
			DoDestroy();
	}

	public void DoDestroy()
	{
		UnregisterTriggers(triggerList, HandleEvent);

		didDestroy = true;

#if UNITY_EDITOR
		if (UnityEditor.BuildPipeline.isBuildingPlayer)
			return;
#endif

		if (triggerList.Contains(DESTROY_TRIGGER_ID))
			HandleEvent(null);
	}

	protected void RegisterTriggers(System.Collections.Generic.List<int> in_triggerList, AkTriggerBase.Trigger in_delegate)
	{
		//Register to the appropriate triggers
		foreach (uint triggerID in in_triggerList)
		{
			var triggerName = string.Empty;
			if (triggerTypes.TryGetValue(triggerID, out triggerName))
			{
				// These special triggers are handled differently
				if (triggerName == "Awake" || triggerName == "Start" || triggerName == "Destroy")
					continue;

				var trigger = (AkTriggerBase) GetComponent(System.Type.GetType(triggerName));
				if (trigger == null)
					trigger = (AkTriggerBase) gameObject.AddComponent(System.Type.GetType(triggerName));

				trigger.triggerDelegate += in_delegate;
			}
		}
	}

	protected void UnregisterTriggers(System.Collections.Generic.List<int> in_triggerList,
		AkTriggerBase.Trigger in_delegate)
	{
		//Unregister all the triggers and delete them if no one else is registered to them
		foreach (uint triggerID in in_triggerList)
		{
			var triggerName = string.Empty;
			if (triggerTypes.TryGetValue(triggerID, out triggerName))
			{
				// These special triggers are handled differently
				if (triggerName == "Awake" || triggerName == "Start" || triggerName == "Destroy")
					continue;

				var trigger = (AkTriggerBase) GetComponent(System.Type.GetType(triggerName));

				if (trigger != null)
				{
					trigger.triggerDelegate -= in_delegate;

					if (trigger.triggerDelegate == null)
						Destroy(trigger);
				}
			}
		}
	}
}

[UnityEngine.ExecuteInEditMode]
public abstract class AkDragDropTriggerHandler : AkTriggerHandler
{
	protected abstract AK.Wwise.BaseType WwiseType { get; }

	protected override void Awake()
	{
#if UNITY_EDITOR
		if (UnityEditor.BuildPipeline.isBuildingPlayer || AkUtilities.IsMigrating)
			return;

		var reference = AkUtilities.DragAndDropObjectReference;
		if (reference)
		{
			UnityEngine.GUIUtility.hotControl = 0;
			WwiseType.ObjectReference = reference;
		}

		if (!UnityEditor.EditorApplication.isPlaying)
			return;
#endif

		base.Awake();
	}

	protected override void Start()
	{
#if UNITY_EDITOR
		if (UnityEditor.BuildPipeline.isBuildingPlayer || AkUtilities.IsMigrating || !UnityEditor.EditorApplication.isPlaying)
			return;
#endif

		base.Start();
	}

	protected override void OnDestroy()
	{
#if UNITY_EDITOR
		if (UnityEditor.BuildPipeline.isBuildingPlayer || AkUtilities.IsMigrating || !UnityEditor.EditorApplication.isPlaying)
			return;
#endif

		base.OnDestroy();
	}
}

#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
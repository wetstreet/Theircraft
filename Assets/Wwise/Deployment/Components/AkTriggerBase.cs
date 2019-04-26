#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2014 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

/// Base class for the generic triggering mechanism for Wwise Integration.
/// All Wwise components will use this mechanism to drive their behavior.
/// Derive from this class to add your own triggering condition, as described in \ref unity_add_triggers
public abstract class AkTriggerBase : UnityEngine.MonoBehaviour
{
	/// Delegate declaration for all Wwise Triggers.
	public delegate void Trigger(
		UnityEngine.GameObject in_gameObject ///< in_gameObject is used to pass "Collidee" objects when Colliders are used.  Some components have the option "Use other object", this is the object they'll use.
	);

	/// All components reacting to the trigger will be registered in this delegate.
	public Trigger triggerDelegate = null;

	public static System.Collections.Generic.Dictionary<uint, string> GetAllDerivedTypes()
	{
		var derivedTypes = new System.Collections.Generic.Dictionary<uint, string>();

		var baseType = typeof(AkTriggerBase);

#if UNITY_WSA && !UNITY_EDITOR
		var baseTypeInfo = System.Reflection.IntrospectionExtensions.GetTypeInfo(baseType);
		var typeInfos = baseTypeInfo.Assembly.DefinedTypes;

		foreach (var typeInfo in typeInfos)
		{
			if (typeInfo.IsClass && (typeInfo.IsSubclassOf(baseType) || baseTypeInfo.IsAssignableFrom(typeInfo) && baseType != typeInfo.AsType()))
			{
				var typeName = typeInfo.Name;
				derivedTypes.Add(AkUtilities.ShortIDGenerator.Compute(typeName), typeName);
			}
		}
#else
		var types = baseType.Assembly.GetTypes();

		for (var i = 0; i < types.Length; i++)
		{
			if (types[i].IsClass &&
			    (types[i].IsSubclassOf(baseType) || baseType.IsAssignableFrom(types[i]) && baseType != types[i]))
			{
				var typeName = types[i].Name;
				derivedTypes.Add(AkUtilities.ShortIDGenerator.Compute(typeName), typeName);
			}
		}
#endif

		//Add the Awake, Start and Destroy triggers and build the displayed list.
		derivedTypes.Add(AkUtilities.ShortIDGenerator.Compute("Awake"), "Awake");
		derivedTypes.Add(AkUtilities.ShortIDGenerator.Compute("Start"), "Start");
		derivedTypes.Add(AkUtilities.ShortIDGenerator.Compute("Destroy"), "Destroy");

		return derivedTypes;
	}
}

#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
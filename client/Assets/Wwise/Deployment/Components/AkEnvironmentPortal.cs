#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2014 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

/// @brief Use this component to define an area that straddles two different AkEnvironment's zones and allow mixing between both zones. \ref unity_use_AkEvironment_AkEvironmentPortal
[UnityEngine.AddComponentMenu("Wwise/AkEnvironmentPortal")]
[UnityEngine.RequireComponent(typeof(UnityEngine.BoxCollider))]
[UnityEngine.ExecuteInEditMode]
public class AkEnvironmentPortal : UnityEngine.MonoBehaviour
{
	public const int MAX_ENVIRONMENTS_PER_PORTAL = 2;

	///The axis used to find the contribution of each environment
	public UnityEngine.Vector3 axis = new UnityEngine.Vector3(1, 0, 0);

	///The array is already sorted by position.
	///The first environment is on the negative side of the portal(opposite to the direction of the chosen axis)
	///The second environment is on the positive side of the portal
	public AkEnvironment[] environments = new AkEnvironment[MAX_ENVIRONMENTS_PER_PORTAL];

	public float GetAuxSendValueForPosition(UnityEngine.Vector3 in_position, int index)
	{
		//total lenght of the portal in the direction of axis
		var portalLenght =
			UnityEngine.Vector3.Dot(
				UnityEngine.Vector3.Scale(GetComponent<UnityEngine.BoxCollider>().size, transform.lossyScale), axis);

		//transform axis to world coordinates 
		var axisWorld = UnityEngine.Vector3.Normalize(transform.rotation * axis);

		//Get distance form left side of the portal(opposite to the direction of axis) to the game object in the direction of axisWorld
		var dist = UnityEngine.Vector3.Dot(in_position - (transform.position - portalLenght * 0.5f * axisWorld), axisWorld);

		//calculate value of the environment referred by index 
		if (index == 0)
			return (portalLenght - dist) * (portalLenght - dist) / (portalLenght * portalLenght);

		return dist * dist / (portalLenght * portalLenght);
	}


#if UNITY_EDITOR
	/// This enables us to detect intersections between portals and environments in the editor
	[System.Serializable]
	public class EnvListWrapper
	{
		public System.Collections.Generic.List<AkEnvironment> list = new System.Collections.Generic.List<AkEnvironment>();
	}

	/// Unity can't serialize an array of list so we wrap the list in a serializable class 
	public EnvListWrapper[] envList =
	{
		new EnvListWrapper(), //All environments on the negative side of each portal(opposite to the direction of the chosen axis)
		new EnvListWrapper() //All environments on the positive side of each portal(same direction as the chosen axis)
	};
#endif
}

#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
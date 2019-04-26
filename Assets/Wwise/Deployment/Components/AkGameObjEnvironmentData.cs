#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2014 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

public class AkGameObjEnvironmentData
{
	/// Contains all active environments sorted by default, excludeOthers and priority, even those inside a portal.
	private readonly System.Collections.Generic.List<AkEnvironment> activeEnvironments =
		new System.Collections.Generic.List<AkEnvironment>();

	/// Contains all active environments sorted by priority, even those inside a portal.
	private readonly System.Collections.Generic.List<AkEnvironment> activeEnvironmentsFromPortals =
		new System.Collections.Generic.List<AkEnvironment>();

	/// Contains all active portals.
	private readonly System.Collections.Generic.List<AkEnvironmentPortal> activePortals =
		new System.Collections.Generic.List<AkEnvironmentPortal>();

	private readonly AkAuxSendArray auxSendValues = new AkAuxSendArray();
	private UnityEngine.Vector3 lastPosition = UnityEngine.Vector3.zero;
	private bool hasEnvironmentListChanged = true;
	private bool hasActivePortalListChanged = true;
	private bool hasSentZero = false;

	private void AddHighestPriorityEnvironmentsFromPortals(UnityEngine.Vector3 position)
	{
		for (var i = 0; i < activePortals.Count; i++)
		for (var j = 0; j < AkEnvironmentPortal.MAX_ENVIRONMENTS_PER_PORTAL; j++)
		{
			var env = activePortals[i].environments[j];
			if (env != null)
			{
				var index = activeEnvironmentsFromPortals.BinarySearch(env, AkEnvironment.s_compareByPriority);
				if (index >= 0 && index < AkEnvironment.MAX_NB_ENVIRONMENTS)
				{
					auxSendValues.Add(env.data.Id, activePortals[i].GetAuxSendValueForPosition(position, j));
					if (auxSendValues.isFull)
						return;
				}
			}
		}
	}

	private void AddHighestPriorityEnvironments(UnityEngine.Vector3 position)
	{
		if (!auxSendValues.isFull && auxSendValues.Count() < activeEnvironments.Count)
		{
			for (var i = 0; i < activeEnvironments.Count; i++)
			{
				var env = activeEnvironments[i];
				var auxBusID = env.data.Id;

				if ((!env.isDefault || i == 0) && !auxSendValues.Contains(auxBusID))
				{
					auxSendValues.Add(auxBusID, env.GetAuxSendValueForPosition(position));

					//No other environment can be added after an environment with the excludeOthers flag set to true
					if (env.excludeOthers || auxSendValues.isFull)
						break;
				}
			}
		}
	}

	public void UpdateAuxSend(UnityEngine.GameObject gameObject, UnityEngine.Vector3 position)
	{
		if (!hasEnvironmentListChanged && !hasActivePortalListChanged && lastPosition == position)
			return;

		auxSendValues.Reset();
		AddHighestPriorityEnvironmentsFromPortals(position);
		AddHighestPriorityEnvironments(position);

		bool isSendingZero = auxSendValues.Count() == 0;
		if (!hasSentZero || !isSendingZero)
			AkSoundEngine.SetEmitterAuxSendValues(gameObject, auxSendValues, (uint) auxSendValues.Count());

		hasSentZero = isSendingZero;
		lastPosition = position;
		hasActivePortalListChanged = false;
		hasEnvironmentListChanged = false;
	}

	private void TryAddEnvironment(AkEnvironment env)
	{
		if (env != null)
		{
			var index = activeEnvironmentsFromPortals.BinarySearch(env, AkEnvironment.s_compareByPriority);
			if (index < 0)
			{
				activeEnvironmentsFromPortals.Insert(~index, env);

				index = activeEnvironments.BinarySearch(env, AkEnvironment.s_compareBySelectionAlgorithm);
				if (index < 0)
					activeEnvironments.Insert(~index, env);

				hasEnvironmentListChanged = true;
			}
		}
	}

	private void RemoveEnvironment(AkEnvironment env)
	{
		activeEnvironmentsFromPortals.Remove(env);
		activeEnvironments.Remove(env);
		hasEnvironmentListChanged = true;
	}

	public void AddAkEnvironment(UnityEngine.Collider environmentCollider, UnityEngine.Collider gameObjectCollider)
	{
		var portal = environmentCollider.GetComponent<AkEnvironmentPortal>();
		if (portal != null)
		{
			activePortals.Add(portal);
			hasActivePortalListChanged = true;

			for (var i = 0; i < AkEnvironmentPortal.MAX_ENVIRONMENTS_PER_PORTAL; i++)
				TryAddEnvironment(portal.environments[i]);
		}
		else
		{
			var env = environmentCollider.GetComponent<AkEnvironment>();
			TryAddEnvironment(env);
		}
	}

	private bool AkEnvironmentBelongsToActivePortals(AkEnvironment env)
	{
		for (var i = 0; i < activePortals.Count; i++)
		for (var j = 0; j < AkEnvironmentPortal.MAX_ENVIRONMENTS_PER_PORTAL; j++)
		{
			if (env == activePortals[i].environments[j])
				return true;
		}

		return false;
	}

	public void RemoveAkEnvironment(UnityEngine.Collider environmentCollider, UnityEngine.Collider gameObjectCollider)
	{
		var portal = environmentCollider.GetComponent<AkEnvironmentPortal>();
		if (portal != null)
		{
			for (var i = 0; i < AkEnvironmentPortal.MAX_ENVIRONMENTS_PER_PORTAL; i++)
			{
				var env = portal.environments[i];
				if (env != null && !gameObjectCollider.bounds.Intersects(env.Collider.bounds))
					RemoveEnvironment(env);
			}

			activePortals.Remove(portal);
			hasActivePortalListChanged = true;
		}
		else
		{
			var env = environmentCollider.GetComponent<AkEnvironment>();
			if (env != null && !AkEnvironmentBelongsToActivePortals(env))
				RemoveEnvironment(env);
		}
	}
}

#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
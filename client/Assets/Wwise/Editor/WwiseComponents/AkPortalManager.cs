#if UNITY_EDITOR
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2014 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

[UnityEditor.InitializeOnLoad]
public class AkPortalManager
{
	private static AkPortalManager s_portalManager;

	public System.Collections.Generic.List<AkEnvironment> EnvironmentList =
		new System.Collections.Generic.List<AkEnvironment>();

	public System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<AkEnvironment>>[]
		IntersectingEnvironments =
		{
			//All environments on the negative side of each portal(opposite to the direction of the chosen axis)
			new System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<AkEnvironment>>(),
			//All environments on the positive side of each portal(same direction as the chosen axis)
			new System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<AkEnvironment>>()
		};

	private float m_timeStamp = UnityEngine.Time.realtimeSinceStartup;

	public System.Collections.Generic.List<AkEnvironmentPortal> PortalList =
		new System.Collections.Generic.List<AkEnvironmentPortal>();

	static AkPortalManager()
	{
		//This constructor is called before any game object is created when there is a compilation which makes the 'FindObjectsOfType' function return null.
		//So we register the init function to be called at hte first update.
		UnityEditor.EditorApplication.update += Init;
	}

	public static void Init()
	{
		//Do nothing if Manager exists
		if (s_portalManager == null)
		{
			s_portalManager = new AkPortalManager();
			s_portalManager.Populate();

			//Register the update function to be called at each frame
			UnityEditor.EditorApplication.update += s_portalManager.UpdateEnvironments;
		}

		//Unregister in case we were registered
		UnityEditor.EditorApplication.update -= Init;
	}

	public static AkPortalManager GetManager()
	{
		return s_portalManager;
	}

	public void Populate()
	{
		//Add all environments in the scene to the environment list 
		var akEnv = UnityEngine.Object.FindObjectsOfType<AkEnvironment>();
		s_portalManager.EnvironmentList.Clear();
		s_portalManager.EnvironmentList.AddRange(akEnv);

		//Add all portals in the scene to the portal list 
		var akPortals = UnityEngine.Object.FindObjectsOfType<AkEnvironmentPortal>();
		s_portalManager.PortalList.Clear();
		s_portalManager.PortalList.AddRange(akPortals);

		//check for portal-environment intersections and populate the IntersectingEnvironments dictionary
		for (var i = 0; i < s_portalManager.PortalList.Count; i++)
			s_portalManager.UpdatePortal(s_portalManager.PortalList[i]);
	}

	public void UpdatePortal(AkEnvironmentPortal in_portal)
	{
		var envList = new System.Collections.Generic.List<AkEnvironment>[2];

		for (var i = 0; i < 2; i++)
		{
			if (!IntersectingEnvironments[i].TryGetValue(in_portal.GetInstanceID(), out envList[i]))
			{
				envList[i] = new System.Collections.Generic.List<AkEnvironment>();
				IntersectingEnvironments[i][in_portal.GetInstanceID()] = envList[i];
			}
			else
				envList[i].Clear();
		}

		//We check if a portal intersects any environment 
		//Iterate in reverse order for safe removal form list while iterating
		for (var i = EnvironmentList.Count - 1; i >= 0; i--)
		{
			if (EnvironmentList[i] != null)
			{
				//if there is an intersection
				if (in_portal.GetComponent<UnityEngine.Collider>().bounds
					.Intersects(EnvironmentList[i].GetComponent<UnityEngine.Collider>().bounds))
				{
					if (UnityEngine.Vector3.Dot(in_portal.transform.rotation * in_portal.axis,
						    EnvironmentList[i].transform.position - in_portal.transform.position) >= 0)
						envList[1].Add(EnvironmentList[i]);
					else
						envList[0].Add(EnvironmentList[i]);
				}
			}
			else
				EnvironmentList.RemoveAt(i);
		}
	}

	private void UpdateEnvironment(AkEnvironment in_env)
	{
		for (var i = PortalList.Count - 1; i >= 0; i--) //Iterate in reverse order for safe removal form list while iterating
		{
			if (PortalList[i] != null)
			{
				if (in_env.GetComponent<UnityEngine.Collider>().bounds
					.Intersects(PortalList[i].GetComponent<UnityEngine.Collider>().bounds))
				{
					System.Collections.Generic.List<AkEnvironment> envList = null;

					//Get index of the list that should contain this environment
					//Index = 0 means that the enviroment is on the negative side of the portal (opposite to the direction of the chosen axis)
					//Index = 1 means that the enviroment is on the positive side of the portal (same direction as the chosen axis)
					var index = UnityEngine.Vector3.Dot(PortalList[i].transform.rotation * PortalList[i].axis,
						            in_env.transform.position - PortalList[i].transform.position) >= 0
						? 1
						: 0;

					if (!IntersectingEnvironments[index].TryGetValue(PortalList[i].GetInstanceID(), out envList))
					{
						envList = new System.Collections.Generic.List<AkEnvironment>();
						envList.Add(in_env);
						IntersectingEnvironments[index][PortalList[i].GetInstanceID()] = envList;
					}
					else if (!envList.Contains(in_env))
						envList.Add(in_env);
				}
			}
			else
				PortalList.RemoveAt(i);
		}
	}

	public void UpdateEnvironments()
	{
		//Timer is reset when starting play mode and when coming back to editor mode 
		if (UnityEngine.Time.realtimeSinceStartup < m_timeStamp)
		{
			m_timeStamp = UnityEngine.Time.realtimeSinceStartup;

			//The PortalManager object doesn't get destroyed but all game objects in our lists become null
			//So we populate
			Populate();
			return;
		}

		//The update is done once every second
		if (UnityEngine.Time.realtimeSinceStartup - m_timeStamp < 1.0f)
			return;

		m_timeStamp = UnityEngine.Time.realtimeSinceStartup;

		var portals = UnityEditor.Selection.GetFiltered(typeof(AkEnvironmentPortal), UnityEditor.SelectionMode.Unfiltered);
		if (portals != null)
		{
			for (var i = 0; i < portals.Length; i++)
			{
				if (!PortalList.Contains((AkEnvironmentPortal) portals[i]))
					PortalList.Add((AkEnvironmentPortal) portals[i]);

				UpdatePortal((AkEnvironmentPortal) portals[i]);
			}
		}

		var envs = UnityEditor.Selection.GetFiltered(typeof(AkEnvironment), UnityEditor.SelectionMode.Unfiltered);
		if (envs != null)
		{
			for (var i = 0; i < envs.Length; i++)
			{
				if (!EnvironmentList.Contains((AkEnvironment) envs[i]))
					EnvironmentList.Add((AkEnvironment) envs[i]);

				UpdateEnvironment((AkEnvironment) envs[i]);
			}
		}
	}
}

#endif
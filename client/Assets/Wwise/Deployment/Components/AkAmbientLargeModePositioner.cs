#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
public class AkAmbientLargeModePositioner : UnityEngine.MonoBehaviour
{
	public UnityEngine.Vector3 Position
	{
		get
		{
			return transform.position;
		}
	}

	public UnityEngine.Vector3 Forward
	{
		get
		{
			return transform.forward;
		}
	}

	public UnityEngine.Vector3 Up
	{
		get
		{
			return transform.up;
		}
	}

#if UNITY_EDITOR
	void OnDrawGizmosSelected()
	{
		UnityEngine.Gizmos.color = UnityEngine.Color.green;
		UnityEngine.Gizmos.DrawSphere(transform.position, 0.1f);

		UnityEditor.Handles.Label(transform.position, name);
	}
#endif
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
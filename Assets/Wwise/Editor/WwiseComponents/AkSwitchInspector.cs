#if UNITY_EDITOR
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2014 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

[UnityEditor.CanEditMultipleObjects]
[UnityEditor.CustomEditor(typeof(AkSwitch))]
public class AkSwitchInspector : AkBaseInspector
{
	private readonly AkUnityEventHandlerInspector m_UnityEventHandlerInspector = new AkUnityEventHandlerInspector();

	private void OnEnable()
	{
		m_UnityEventHandlerInspector.Init(serializedObject);
	}

	public override void OnChildInspectorGUI()
	{
		m_UnityEventHandlerInspector.OnGUI();
	}
}
#endif

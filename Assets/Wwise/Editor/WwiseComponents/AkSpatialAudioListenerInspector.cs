#if UNITY_EDITOR
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2017 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

[UnityEditor.CustomEditor(typeof(AkSpatialAudioListener))]
public class AkSpatialAudioListenerInspector : UnityEditor.Editor
{
	public override void OnInspectorGUI()
	{
		UnityEditor.EditorGUILayout.HelpBox(
			"The current version of Spatial Audio only supports one listener. Make sure to only have one AkSpatialAudioListener active at a time.",
			UnityEditor.MessageType.Info);
	}
}
#endif
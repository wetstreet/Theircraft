#if UNITY_EDITOR
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2014 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

[UnityEditor.CanEditMultipleObjects]
[UnityEditor.CustomEditor(typeof(AkEvent))]
public class AkEventInspector : AkBaseInspector
{
	private readonly AkUnityEventHandlerInspector m_UnityEventHandlerInspector = new AkUnityEventHandlerInspector();
	private UnityEditor.SerializedProperty actionOnEventType;
	private UnityEditor.SerializedProperty callbackData;
	private UnityEditor.SerializedProperty curveInterpolation;
	private UnityEditor.SerializedProperty enableActionOnEvent;
	private UnityEditor.SerializedProperty transitionDuration;
	private UnityEditor.SerializedProperty useCallbacks;

	public void OnEnable()
	{
		m_UnityEventHandlerInspector.Init(serializedObject);

		enableActionOnEvent = serializedObject.FindProperty("enableActionOnEvent");
		actionOnEventType = serializedObject.FindProperty("actionOnEventType");
		curveInterpolation = serializedObject.FindProperty("curveInterpolation");
		transitionDuration = serializedObject.FindProperty("transitionDuration");
		useCallbacks = serializedObject.FindProperty("useCallbacks");

		callbackData = serializedObject.FindProperty("Callbacks");

		AkEditorEventPlayer.Instance.RefreshGUI += RefreshGUI;
	}

	public void OnDisable()
	{
		AkEditorEventPlayer.Instance.RefreshGUI -= RefreshGUI;
	}

	private void RefreshGUI()
	{
		Repaint();
	}

	public override void OnChildInspectorGUI()
	{
		m_UnityEventHandlerInspector.OnGUI();

		UnityEngine.GUILayout.Space(UnityEditor.EditorGUIUtility.standardVerticalSpacing);

		using (new UnityEditor.EditorGUILayout.VerticalScope("box"))
		{
			UnityEditor.EditorGUILayout.PropertyField(enableActionOnEvent, new UnityEngine.GUIContent("Action On Event: "));

			if (enableActionOnEvent.boolValue)
			{
				UnityEditor.EditorGUILayout.PropertyField(actionOnEventType, new UnityEngine.GUIContent("Action On EventType: "));
				UnityEditor.EditorGUILayout.PropertyField(curveInterpolation, new UnityEngine.GUIContent("Curve Interpolation: "));
				UnityEditor.EditorGUILayout.Slider(transitionDuration, 0.0f, 60.0f, new UnityEngine.GUIContent("Fade Time (secs): "));
			}
		}

		UnityEngine.GUILayout.Space(UnityEditor.EditorGUIUtility.standardVerticalSpacing);

		using (new UnityEditor.EditorGUILayout.VerticalScope("box"))
		{
			UnityEditor.EditorGUILayout.PropertyField(useCallbacks, new UnityEngine.GUIContent("Use Callback: "));

			if (useCallbacks.boolValue)
			{
				var emptyContent = new UnityEngine.GUIContent("");

				// ensure that there is always at least one entry since we are "using" callbacks
				if (callbackData.arraySize == 0)
					callbackData.arraySize = 1;

				const float callbackSpacerWidth = 4;
				const float removeButtonWidth = 20;
				var rect = UnityEditor.EditorGUILayout.GetControlRect();
				var callbackFieldWidth = (rect.width - removeButtonWidth) / 3;
				rect.width = callbackFieldWidth - callbackSpacerWidth;

				UnityEngine.GUI.Label(rect, "Game Object");

				rect.x += callbackFieldWidth;
				UnityEngine.GUI.Label(rect, "Callback Function");

				rect.x += callbackFieldWidth;
				UnityEngine.GUI.Label(rect, "Callback Flags");

				for (var i = 0; i < callbackData.arraySize; ++i)
				{
					var data = callbackData.GetArrayElementAtIndex(i);
					rect = UnityEditor.EditorGUILayout.GetControlRect();
					rect.width = callbackFieldWidth - callbackSpacerWidth;
					UnityEditor.EditorGUI.PropertyField(rect, data.FindPropertyRelative("GameObject"), emptyContent);

					rect.x += callbackFieldWidth;
					UnityEditor.EditorGUI.PropertyField(rect, data.FindPropertyRelative("FunctionName"), emptyContent);

					rect.x += callbackFieldWidth;
					UnityEditor.EditorGUI.PropertyField(rect, data.FindPropertyRelative("Flags"), emptyContent);

					rect.x += callbackFieldWidth;
					rect.width = removeButtonWidth;
					if (UnityEngine.GUI.Button(rect, "X"))
						callbackData.DeleteArrayElementAtIndex(i);
				}

				if (UnityEngine.GUI.Button(UnityEditor.EditorGUILayout.GetControlRect(), "Add"))
				{
					var i = callbackData.arraySize++;
					var data = callbackData.GetArrayElementAtIndex(i);
					data.FindPropertyRelative("GameObject").objectReferenceValue = null;
					data.FindPropertyRelative("FunctionName").stringValue = string.Empty;
					data.FindPropertyRelative("Flags.value").intValue = 0;
				}
			}
			else if (callbackData.arraySize == 1)
			{
				var data = callbackData.GetArrayElementAtIndex(0);
				if (data.FindPropertyRelative("GameObject").objectReferenceValue == null)
					if (string.IsNullOrEmpty(data.FindPropertyRelative("FunctionName").stringValue))
						if (data.FindPropertyRelative("Flags.value").intValue == 0)
							callbackData.arraySize = 0;
			}
		}

		using (new UnityEditor.EditorGUILayout.VerticalScope("box"))
		{
			var style = new UnityEngine.GUIStyle(UnityEngine.GUI.skin.button);
			float inspectorWidth = UnityEngine.Screen.width - UnityEngine.GUI.skin.box.margin.left -
			                       UnityEngine.GUI.skin.box.margin.right;

			if (targets.Length == 1)
			{
				var akEvent = (AkEvent) target;
				var eventPlaying = AkEditorEventPlayer.Instance.IsEventPlaying(akEvent);
				if (eventPlaying)
				{
					if (UnityEngine.GUILayout.Button("Stop", style, UnityEngine.GUILayout.MaxWidth(inspectorWidth)))
					{
						UnityEngine.GUIUtility.hotControl = 0;
						AkEditorEventPlayer.Instance.StopEvent(akEvent);
					}
				}
				else
				{
					if (UnityEngine.GUILayout.Button("Play", style, UnityEngine.GUILayout.MaxWidth(inspectorWidth)))
					{
						UnityEngine.GUIUtility.hotControl = 0;
						AkEditorEventPlayer.Instance.PlayEvent(akEvent);
					}
				}
			}
			else
			{
				var playingEventsSelected = false;
				var stoppedEventsSelected = false;
				for (var i = 0; i < targets.Length; ++i)
				{
					var akEventTarget = targets[i] as AkEvent;
					if (akEventTarget != null)
					{
						if (AkEditorEventPlayer.Instance.IsEventPlaying(akEventTarget))
						{
							playingEventsSelected = true;
						}
						else
						{
							stoppedEventsSelected = true;
						}

						if (playingEventsSelected && stoppedEventsSelected)
						{
							break;
						}
					}
				}

				if (stoppedEventsSelected &&
				    UnityEngine.GUILayout.Button("Play Multiple", style, UnityEngine.GUILayout.MaxWidth(inspectorWidth)))
				{
					for (var i = 0; i < targets.Length; ++i)
					{
						var akEventTarget = targets[i] as AkEvent;
						if (akEventTarget != null)
						{
							AkEditorEventPlayer.Instance.PlayEvent(akEventTarget);
						}
					}
				}

				if (playingEventsSelected &&
				    UnityEngine.GUILayout.Button("Stop Multiple", style, UnityEngine.GUILayout.MaxWidth(inspectorWidth)))
				{
					for (var i = 0; i < targets.Length; ++i)
					{
						var akEventTarget = targets[i] as AkEvent;
						if (akEventTarget != null)
						{
							AkEditorEventPlayer.Instance.StopEvent(akEventTarget);
						}
					}
				}
			}

			if (UnityEngine.GUILayout.Button("Stop All", style, UnityEngine.GUILayout.MaxWidth(inspectorWidth)))
			{
				UnityEngine.GUIUtility.hotControl = 0;
				AkEditorEventPlayer.Instance.StopAll();
			}
		}
	}

	public class AkEditorEventPlayer
	{
		private static AkEditorEventPlayer ms_Instance;

		private readonly System.Collections.Generic.List<AkEvent> akEvents = new System.Collections.Generic.List<AkEvent>();

		public event System.Action RefreshGUI;

		public static AkEditorEventPlayer Instance
		{
			get
			{
				if (ms_Instance == null)
				{
					ms_Instance = new AkEditorEventPlayer();
				}

				return ms_Instance;
			}
		}

		private void CallbackHandler(object in_cookie, AkCallbackType in_type, AkCallbackInfo in_info)
		{
			if (in_type == AkCallbackType.AK_EndOfEvent)
			{
				RemoveAkEvent(in_cookie as AkEvent);

				var refreshGUI = RefreshGUI;
				if (refreshGUI != null)
				{
					refreshGUI.Invoke();
				}
			}
		}

		public void PlayEvent(AkEvent akEvent)
		{
			if (IsEventPlaying(akEvent))
			{
				return;
			}

			var playingID = akEvent.data.Post(akEvent.gameObject, (uint)AkCallbackType.AK_EndOfEvent, CallbackHandler, akEvent);
			if (playingID != AkSoundEngine.AK_INVALID_PLAYING_ID)
			{
				AddAkEvent(akEvent);
			}
		}

		public void StopEvent(AkEvent akEvent)
		{
			if (!IsEventPlaying(akEvent))
			{
				return;
			}

			akEvent.data.Stop(akEvent.gameObject);

			RemoveAkEvent(akEvent);
		}

		private void AddAkEvent(AkEvent akEvent)
		{
			akEvents.Add(akEvent);

			// In the case where objects are being placed in edit mode and then previewed, their positions won't yet be updated so we ensure they're updated here.
			AkSoundEngine.SetObjectPosition(akEvent.gameObject, akEvent.transform);
		}

		private void RemoveAkEvent(AkEvent akEvent)
		{
			if (akEvent != null)
			{
				akEvents.Remove(akEvent);
			}
		}

		public bool IsEventPlaying(AkEvent akEvent)
		{
			return akEvents.Contains(akEvent);
		}

		public void StopAll()
		{
			akEvents.Clear();

			AkSoundEngine.StopAll();
		}
	}
}
#endif

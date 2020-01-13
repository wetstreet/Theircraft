#if !(UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.

namespace AK.Wwise
{
	[System.Serializable]
	///@brief This type represents the values of the flags used when posting an Event with a callback.
	public class CallbackFlags
	{
		public uint value = 0;

#if UNITY_EDITOR
		[UnityEditor.CustomPropertyDrawer(typeof(CallbackFlags))]
		private class CallbackFlagsDrawer : UnityEditor.PropertyDrawer
		{
			private static string[] m_supportedCallbackFlags;
			private static int[] m_supportedCallbackValues;

			public static string[] SupportedCallbackFlags
			{
				get
				{
					if (m_supportedCallbackFlags == null)
						SetupSupportedCallbackValuesAndFlags();

					return m_supportedCallbackFlags;
				}
			}

			private static void SetupSupportedCallbackValuesAndFlags()
			{
				var callbacktypes = System.Enum.GetValues(typeof(AkCallbackType)) as AkCallbackType[];
				var unsupportedCallbackValues = new[]
				{
					AkCallbackType.AK_SpeakerVolumeMatrix, AkCallbackType.AK_MusicSyncAll,
					AkCallbackType.AK_CallbackBits, AkCallbackType.AK_Monitoring, AkCallbackType.AK_AudioSourceChange,
					AkCallbackType.AK_Bank, AkCallbackType.AK_AudioInterruption
				};

				m_supportedCallbackFlags = new string[callbacktypes.Length - unsupportedCallbackValues.Length];
				m_supportedCallbackValues = new int[callbacktypes.Length - unsupportedCallbackValues.Length];

				var index = 0;
				for (var i = 0; i < callbacktypes.Length; i++)
				{
					if (!Contains(unsupportedCallbackValues, callbacktypes[i]))
					{
						m_supportedCallbackFlags[index] = System.Enum.GetName(typeof(AkCallbackType), callbacktypes[i]).Substring(3);
						m_supportedCallbackValues[index] = (int)callbacktypes[i];
						index++;
					}
				}
			}

			private static int GetDisplayMask(int in_wwiseCallbackMask)
			{
				if (m_supportedCallbackValues == null)
					SetupSupportedCallbackValuesAndFlags();

				var displayMask = 0;
				for (var i = 0; i < m_supportedCallbackValues.Length; i++)
				{
					if ((m_supportedCallbackValues[i] & in_wwiseCallbackMask) != 0)
						displayMask |= 1 << i;
				}

				return displayMask;
			}

			private static int GetWwiseCallbackMask(int in_displayMask)
			{
				if (m_supportedCallbackValues == null)
					SetupSupportedCallbackValuesAndFlags();

				var wwiseCallbackMask = 0;
				for (var i = 0; i < m_supportedCallbackValues.Length; i++)
				{
					if ((in_displayMask & (1 << i)) != 0)
						wwiseCallbackMask |= m_supportedCallbackValues[i];
				}

				return wwiseCallbackMask;
			}

			private static bool Contains<T>(T[] in_array, T in_value)
			{
				for (var i = 0; i < in_array.Length; i++)
				{
					if (in_array[i].Equals(in_value))
						return true;
				}

				return false;
			}

			public override void OnGUI(UnityEngine.Rect position, UnityEditor.SerializedProperty property,
				UnityEngine.GUIContent label)
			{
				UnityEditor.EditorGUI.BeginProperty(position, label, property);

				if (label != null && !string.IsNullOrEmpty(label.text))
				{
					UnityEditor.EditorGUI.LabelField(position, label);
					position.x += UnityEditor.EditorGUIUtility.labelWidth;
					position.width -= UnityEditor.EditorGUIUtility.labelWidth;
				}

				var indent = UnityEditor.EditorGUI.indentLevel;
				UnityEditor.EditorGUI.indentLevel = 0;

				var value = property.FindPropertyRelative("value");

				//Since some callback flags are unsupported, some bits are not used.
				//But when using EditorGUILayout.MaskField, clicking the third flag will set the third bit to one even if the third flag in the AkCallbackType enum is unsupported.
				//This is a problem because clicking the third supported flag would internally select the third flag in the AkCallbackType enum which is unsupported.
				//To solve this problem we use a mask for display and another one for the actual callback
				var displayMask = GetDisplayMask(value.intValue);
				displayMask = UnityEditor.EditorGUI.MaskField(position, displayMask, SupportedCallbackFlags);
				value.intValue = GetWwiseCallbackMask(displayMask);

				UnityEditor.EditorGUI.indentLevel = indent;

				UnityEditor.EditorGUI.EndProperty();
			}
		}
#endif
	}
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
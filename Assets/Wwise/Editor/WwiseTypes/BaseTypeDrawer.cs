namespace AK.Wwise.Editor
{
	public abstract class BaseTypeDrawer : UnityEditor.PropertyDrawer
	{
		public override void OnGUI(UnityEngine.Rect position, UnityEditor.SerializedProperty property, UnityEngine.GUIContent label)
		{
			UnityEditor.EditorGUI.BeginProperty(position, label, property);

			var wwiseObjectReference = property.FindPropertyRelative("WwiseObjectReference");
			HandleDragAndDrop(wwiseObjectReference, position);

			position = UnityEditor.EditorGUI.PrefixLabel(position,
				UnityEngine.GUIUtility.GetControlID(UnityEngine.FocusType.Passive), label);

			var style = new UnityEngine.GUIStyle(UnityEngine.GUI.skin.button);
			style.alignment = UnityEngine.TextAnchor.MiddleLeft;
			style.fontStyle = UnityEngine.FontStyle.Normal;

			var componentName = GetComponentName(wwiseObjectReference);
			if (string.IsNullOrEmpty(componentName))
			{
				componentName = "No " + WwiseObjectType + " is currently selected";
				style.normal.textColor = UnityEngine.Color.red;
			}

			if (UnityEngine.GUI.Button(position, componentName, style))
			{
				new AkWwiseComponentPicker.PickerCreator
				{
					objectType = WwiseObjectType,
					wwiseObjectReference = wwiseObjectReference,
					pickerPosition = AkUtilities.GetLastRectAbsolute(position),
					serializedObject = property.serializedObject
				};
			}

			UnityEditor.EditorGUI.EndProperty();
		}

		protected abstract WwiseObjectType WwiseObjectType { get; }

		protected virtual string GetComponentName(UnityEditor.SerializedProperty wwiseObjectReference)
		{
			var reference = wwiseObjectReference.objectReferenceValue as WwiseObjectReference;
			return reference ? reference.DisplayName : string.Empty;
		}

		private void HandleDragAndDrop(UnityEditor.SerializedProperty wwiseObjectReference, UnityEngine.Rect dropArea)
		{
			var currentEvent = UnityEngine.Event.current;
			if (!dropArea.Contains(currentEvent.mousePosition))
				return;

			if (currentEvent.type != UnityEngine.EventType.DragUpdated && currentEvent.type != UnityEngine.EventType.DragPerform)
				return;

			var reference = AkUtilities.DragAndDropObjectReference;
			if (reference != null && reference.WwiseObjectType != WwiseObjectType)
				reference = null;

			UnityEditor.DragAndDrop.visualMode = reference != null ? UnityEditor.DragAndDropVisualMode.Link : UnityEditor.DragAndDropVisualMode.Rejected;

			if (currentEvent.type == UnityEngine.EventType.DragPerform)
			{
				UnityEditor.DragAndDrop.AcceptDrag();

				if (reference != null)
					wwiseObjectReference.objectReferenceValue = reference;

				UnityEditor.DragAndDrop.PrepareStartDrag();
				UnityEngine.GUIUtility.hotControl = 0;
			}

			currentEvent.Use();
		}
	}
}

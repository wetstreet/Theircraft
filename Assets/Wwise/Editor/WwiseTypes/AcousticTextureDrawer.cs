namespace AK.Wwise.Editor
{
	[UnityEditor.CustomPropertyDrawer(typeof(AcousticTexture))]
	public class AcousticTextureDrawer : BaseTypeDrawer
	{
		protected override string GetComponentName(UnityEditor.SerializedProperty wwiseObjectReference)
		{
			var componentName = base.GetComponentName(wwiseObjectReference);
			return string.IsNullOrEmpty(componentName) ? "None" : componentName;
		}

		protected override WwiseObjectType WwiseObjectType { get { return WwiseObjectType.AcousticTexture; } }
	}
}

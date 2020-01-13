namespace AK.Wwise.Editor
{
	[UnityEditor.CustomPropertyDrawer(typeof(Switch))]
	public class SwitchDrawer : BaseTypeDrawer
	{
		protected override WwiseObjectType WwiseObjectType { get { return WwiseObjectType.Switch; } }
	}
}

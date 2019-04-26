namespace AK.Wwise.Editor
{
	[UnityEditor.CustomPropertyDrawer(typeof(State))]
	public class StateDrawer : BaseTypeDrawer
	{
		protected override WwiseObjectType WwiseObjectType { get { return WwiseObjectType.State; } }
	}
}

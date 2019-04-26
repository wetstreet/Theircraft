namespace AK.Wwise.Editor
{
	[UnityEditor.CustomPropertyDrawer(typeof(Event))]
	public class EventDrawer : BaseTypeDrawer
	{
		protected override WwiseObjectType WwiseObjectType { get { return WwiseObjectType.Event; } }
	}
}
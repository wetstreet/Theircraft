namespace AK.Wwise.Editor
{
	[UnityEditor.CustomPropertyDrawer(typeof(Trigger))]
	public class TriggerDrawer : BaseTypeDrawer
	{
		protected override WwiseObjectType WwiseObjectType { get { return WwiseObjectType.Trigger; } }
	}
}

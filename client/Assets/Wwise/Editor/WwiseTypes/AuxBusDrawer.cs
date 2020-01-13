namespace AK.Wwise.Editor
{
	[UnityEditor.CustomPropertyDrawer(typeof(AuxBus))]
	public class AuxBusDrawer : BaseTypeDrawer
	{
		protected override WwiseObjectType WwiseObjectType { get { return WwiseObjectType.AuxBus; } }
	}
}

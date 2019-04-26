namespace AK.Wwise.Editor
{
	[UnityEditor.CustomPropertyDrawer(typeof(RTPC))]
	public class RTPCDrawer : BaseTypeDrawer
	{
		protected override WwiseObjectType WwiseObjectType { get { return WwiseObjectType.GameParameter; } }
	}
}

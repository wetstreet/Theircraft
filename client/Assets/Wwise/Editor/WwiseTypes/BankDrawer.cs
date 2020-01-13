namespace AK.Wwise.Editor
{
	[UnityEditor.CustomPropertyDrawer(typeof(Bank))]
	public class BankDrawer : BaseTypeDrawer
	{
		protected override WwiseObjectType WwiseObjectType { get { return WwiseObjectType.Soundbank; } }
	}
}

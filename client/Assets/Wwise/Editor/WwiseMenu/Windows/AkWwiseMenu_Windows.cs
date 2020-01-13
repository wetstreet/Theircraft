#if UNITY_EDITOR
public class AkWwiseMenu_Windows
{
	private const string MENU_PATH = "Help/Wwise Help/";
	private const string Platform = "Windows";

	[UnityEditor.MenuItem(MENU_PATH + Platform, false, (int) AkWwiseHelpOrder.WwiseHelpOrder)]
	public static void OpenDoc()
	{
		AkDocHelper.OpenDoc(Platform);
	}
}
#endif // #if UNITY_EDITOR
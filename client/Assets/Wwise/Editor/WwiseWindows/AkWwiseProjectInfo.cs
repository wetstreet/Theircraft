#if UNITY_EDITOR
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2014 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

public static class AkWwiseProjectInfo
{
	private const string DataFileName = "AkWwiseProjectData.asset";
	private static string DataRelativeDirectory = System.IO.Path.Combine(System.IO.Path.Combine("Wwise", "Editor"), "ProjectData");
	private static string DataRelativePath = System.IO.Path.Combine(DataRelativeDirectory, DataFileName);
	private static string DataAssetPath = System.IO.Path.Combine("Assets", DataRelativePath);

	public static AkWwiseProjectData m_Data;

	private static bool WwiseFolderExists()
	{
		return System.IO.Directory.Exists(System.IO.Path.Combine(UnityEngine.Application.dataPath, "Wwise"));
	}

	public static AkWwiseProjectData GetData()
	{
		if (m_Data == null && WwiseFolderExists())
		{
			try
			{
				m_Data = UnityEditor.AssetDatabase.LoadAssetAtPath<AkWwiseProjectData>(DataAssetPath);

				if (m_Data == null)
				{
					var dataAbsolutePath = System.IO.Path.Combine(UnityEngine.Application.dataPath, DataRelativePath);
					var dataExists = System.IO.File.Exists(dataAbsolutePath);

					if (!dataExists)
					{
						var dataAbsoluteDirectory = System.IO.Path.Combine(UnityEngine.Application.dataPath, DataRelativeDirectory);
						if (!System.IO.Directory.Exists(dataAbsoluteDirectory))
							System.IO.Directory.CreateDirectory(dataAbsoluteDirectory);
					}

					m_Data = UnityEngine.ScriptableObject.CreateInstance<AkWwiseProjectData>();

					if (dataExists)
						UnityEngine.Debug.LogWarning("WwiseUnity: Unable to load asset at <" + dataAbsolutePath + ">.");
					else
						UnityEditor.AssetDatabase.CreateAsset(m_Data, DataAssetPath);
				}
			}
			catch (System.Exception e)
			{
				UnityEngine.Debug.LogError("WwiseUnity: Unable to load Wwise Data: " + e);
			}
		}

		return m_Data;
	}

	public static bool Populate()
	{
		var bDirty = false;
		if (AkUtilities.IsWwiseProjectAvailable)
		{
			bDirty = AkWwiseWWUBuilder.Populate();
			bDirty |= AkWwiseXMLBuilder.Populate();
			if (bDirty)
			{
				UnityEditor.EditorUtility.SetDirty(GetData());
				UnityEditor.AssetDatabase.SaveAssets();
				UnityEditor.AssetDatabase.Refresh();
			}
		}

		return bDirty;
	}
}
#endif

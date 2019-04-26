#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

/// This class is an example of how to load banks in Wwise, if the bank data was preloaded in memory.  
/// This would be useful for situations where you use the WWW class
public class AkMemBankLoader : UnityEngine.MonoBehaviour
{
	private const int WaitMs = 50;
	private const long AK_BANK_PLATFORM_DATA_ALIGNMENT = AkSoundEngine.AK_BANK_PLATFORM_DATA_ALIGNMENT;
	private const long AK_BANK_PLATFORM_DATA_ALIGNMENT_MASK = AK_BANK_PLATFORM_DATA_ALIGNMENT - 1;

	/// Name of the bank to load
	public string bankName = "";

	/// Is the bank localized (situated in the language specific folders)
	public bool isLocalizedBank = false;

	private string m_bankPath;

	[UnityEngine.HideInInspector] public uint ms_bankID = AkSoundEngine.AK_INVALID_BANK_ID;

	private System.IntPtr ms_pInMemoryBankPtr = System.IntPtr.Zero;
	private System.Runtime.InteropServices.GCHandle ms_pinnedArray;

#if UNITY_2018_3_OR_NEWER
    private UnityEngine.Networking.UnityWebRequest ms_www;
#else
    private UnityEngine.WWW ms_www;
#endif

	private void Start()
	{
        
		if (isLocalizedBank)
			LoadLocalizedBank(bankName);
		else
			LoadNonLocalizedBank(bankName);
	}

	/// Load a sound bank from WWW object
	public void LoadNonLocalizedBank(string in_bankFilename)
	{
		var bankPath = "file://" + System.IO.Path.Combine(AkBasePathGetter.GetPlatformBasePath(), in_bankFilename);
		DoLoadBank(bankPath);
	}

	/// Load a language-specific bank from WWW object
	public void LoadLocalizedBank(string in_bankFilename)
	{
		var bankPath = "file://" + System.IO.Path.Combine(
			               System.IO.Path.Combine(AkBasePathGetter.GetPlatformBasePath(), AkSoundEngine.GetCurrentLanguage()),
			               in_bankFilename);
		DoLoadBank(bankPath);
	}

    private uint AllocateAlignedBuffer(byte[] data)
    {
        uint uInMemoryBankSize = 0;

        // Allocate an aligned buffer
        try
        {
            ms_pinnedArray =
                System.Runtime.InteropServices.GCHandle.Alloc(data, System.Runtime.InteropServices.GCHandleType.Pinned);
            ms_pInMemoryBankPtr = ms_pinnedArray.AddrOfPinnedObject();
            uInMemoryBankSize = (uint)data.Length;

            // Array inside the WWW object is not aligned. Allocate a new array for which we can guarantee the alignment.
            if ((ms_pInMemoryBankPtr.ToInt64() & AK_BANK_PLATFORM_DATA_ALIGNMENT_MASK) != 0)
            {
                var alignedBytes = new byte[data.Length + AK_BANK_PLATFORM_DATA_ALIGNMENT];
                var new_pinnedArray =
                    System.Runtime.InteropServices.GCHandle.Alloc(alignedBytes, System.Runtime.InteropServices.GCHandleType.Pinned);
                var new_pInMemoryBankPtr = new_pinnedArray.AddrOfPinnedObject();
                var alignedOffset = 0;

                // New array is not aligned, so we will need to use an offset inside it to align our data.
                if ((new_pInMemoryBankPtr.ToInt64() & AK_BANK_PLATFORM_DATA_ALIGNMENT_MASK) != 0)
                {
                    var alignedPtr = (new_pInMemoryBankPtr.ToInt64() + AK_BANK_PLATFORM_DATA_ALIGNMENT_MASK) &
                                     ~AK_BANK_PLATFORM_DATA_ALIGNMENT_MASK;
                    alignedOffset = (int)(alignedPtr - new_pInMemoryBankPtr.ToInt64());
                    new_pInMemoryBankPtr = new System.IntPtr(alignedPtr);
                }

                // Copy the bank's bytes in our new array, at the correct aligned offset.
                System.Array.Copy(data, 0, alignedBytes, alignedOffset, data.Length);

                ms_pInMemoryBankPtr = new_pInMemoryBankPtr;
                ms_pinnedArray.Free();
                ms_pinnedArray = new_pinnedArray;
            }
        }
        catch
        {
        }
        return uInMemoryBankSize;
    }

    private System.Collections.IEnumerator LoadFile()
	{
#if UNITY_2018_3_OR_NEWER
        ms_www = UnityEngine.Networking.UnityWebRequest.Get(m_bankPath);
        yield return ms_www.SendWebRequest();
        uint uInMemoryBankSize = AllocateAlignedBuffer(ms_www.downloadHandler.data);
#else
        ms_www = new UnityEngine.WWW(m_bankPath);
        yield return ms_www;
        uint uInMemoryBankSize = AllocateAlignedBuffer(ms_www.bytes);
#endif

        var result = AkSoundEngine.LoadBank(ms_pInMemoryBankPtr, uInMemoryBankSize, out ms_bankID);
		if (result != AKRESULT.AK_Success)
			UnityEngine.Debug.LogError("WwiseUnity: AkMemBankLoader: bank loading failed with result " + result);
	}

	private void DoLoadBank(string in_bankPath)
	{
		m_bankPath = in_bankPath;
		StartCoroutine(LoadFile());
	}

	private void OnDestroy()
	{
		if (ms_pInMemoryBankPtr != System.IntPtr.Zero)
		{
			var result = AkSoundEngine.UnloadBank(ms_bankID, ms_pInMemoryBankPtr);
			if (result == AKRESULT.AK_Success)
				ms_pinnedArray.Free();
		}
	}
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
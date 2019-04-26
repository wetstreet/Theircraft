#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2014 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

/// @brief Maintains the list of loaded SoundBanks loaded. This is currently used only with AkAmbient objects.
public static class AkBankManager
{
	private static readonly System.Collections.Generic.Dictionary<string, BankHandle> m_BankHandles =
		new System.Collections.Generic.Dictionary<string, BankHandle>();

	private static readonly System.Collections.Generic.List<BankHandle> BanksToUnload =
		new System.Collections.Generic.List<BankHandle>();

	private static readonly System.Threading.Mutex m_Mutex = new System.Threading.Mutex();

	internal static void DoUnloadBanks()
	{
		var count = BanksToUnload.Count;
		for (var i = 0; i < count; ++i)
			BanksToUnload[i].UnloadBank();

		BanksToUnload.Clear();
	}

	internal static void Reset()
	{
		m_BankHandles.Clear();
		BanksToUnload.Clear();
	}

#if UNITY_EDITOR
	internal static void ReloadAllBanks()
	{
		m_Mutex.WaitOne();
		foreach (var bankHandle in m_BankHandles.Values)
		{
			if (bankHandle != null)
			{
				bankHandle.UnloadBank();
			}
		}

		UnloadInitBank();
		LoadInitBank(false);

		foreach(var bankHandle in m_BankHandles.Values)
		{
			if (bankHandle != null)
			{
				bankHandle.DoLoadBank();
			}
		}
		m_Mutex.ReleaseMutex();
	}
#endif
	public static void LoadInitBank(bool doReset = true)
	{
		if (doReset)
		{
			Reset();
		}

		uint BankID;
		var result = AkSoundEngine.LoadBank("Init.bnk", AkSoundEngine.AK_DEFAULT_POOL_ID, out BankID);
		if (result != AKRESULT.AK_Success)
		{
			UnityEngine.Debug.LogError("WwiseUnity: Failed load Init.bnk with result: " + result);
		}
	}

	public static void UnloadInitBank()
	{
		AkSoundEngine.UnloadBank("Init.bnk", System.IntPtr.Zero);
	}

	/// Loads a SoundBank. This version blocks until the bank is loaded. See AK::SoundEngine::LoadBank for more information.
	public static void LoadBank(string name, bool decodeBank, bool saveDecodedBank)
	{
#if UNITY_SWITCH
// No bank decoding on Nintendo switch
		decodeBank = false;
#endif

		m_Mutex.WaitOne();
		BankHandle handle = null;
		if (!m_BankHandles.TryGetValue(name, out handle))
		{
			handle = decodeBank ? new DecodableBankHandle(name, saveDecodedBank) : new BankHandle(name);
			m_BankHandles.Add(name, handle);
			m_Mutex.ReleaseMutex();
			handle.LoadBank();
		}
		else
		{
			// Bank already loaded, increment its ref count.
			handle.IncRef();
			m_Mutex.ReleaseMutex();
		}
	}

	/// Loads a SoundBank. This version returns right away and loads in background. See AK::SoundEngine::LoadBank for more information.
	public static void LoadBankAsync(string name, AkCallbackManager.BankCallback callback = null)
	{
		m_Mutex.WaitOne();
		BankHandle handle = null;
		if (!m_BankHandles.TryGetValue(name, out handle))
		{
			var asyncHandle = new AsyncBankHandle(name, callback);
			m_BankHandles.Add(name, asyncHandle);
			m_Mutex.ReleaseMutex();
			asyncHandle.LoadBank();
		}
		else
		{
			// Bank already loaded, increment its ref count.
			handle.IncRef();
			m_Mutex.ReleaseMutex();
		}
	}

	/// Unloads a SoundBank. See AK::SoundEngine::UnloadBank for more information.
	public static void UnloadBank(string name)
	{
		m_Mutex.WaitOne();
		BankHandle handle = null;
		if (m_BankHandles.TryGetValue(name, out handle))
		{
			handle.DecRef();
			if (handle.RefCount == 0)
				m_BankHandles.Remove(name);
		}

		m_Mutex.ReleaseMutex();
	}

	private class BankHandle
	{
		protected readonly string bankName;
		protected uint m_BankID;

		public BankHandle(string name)
		{
			bankName = name;
		}

		public int RefCount { get; private set; }

		/// Loads a bank. This version blocks until the bank is loaded. See AK::SoundEngine::LoadBank for more information.
		public virtual AKRESULT DoLoadBank()
		{
			return AkSoundEngine.LoadBank(bankName, AkSoundEngine.AK_DEFAULT_POOL_ID, out m_BankID);
		}

		public void LoadBank()
		{
#if UNITY_EDITOR
			if (!AkSoundEngineController.Instance.IsSoundEngineLoaded)
				return;
#endif

			if (RefCount == 0)
			{
				if (BanksToUnload.Contains(this))
					BanksToUnload.Remove(this);
				else
				{
					var res = DoLoadBank();
					LogLoadResult(res);
				}
			}

			IncRef();
		}

		/// Unloads a bank.
		public virtual void UnloadBank()
		{
			AkSoundEngine.UnloadBank(m_BankID, System.IntPtr.Zero, null, null);
		}

		public void IncRef()
		{
			RefCount++;
		}

		public void DecRef()
		{
			RefCount--;
			if (RefCount == 0)
				BanksToUnload.Add(this);
		}

		protected void LogLoadResult(AKRESULT result)
		{
			if (result != AKRESULT.AK_Success && AkSoundEngine.IsInitialized())
				UnityEngine.Debug.LogWarning("WwiseUnity: Bank " + bankName + " failed to load (" + result + ")");
		}
	}

	private class AsyncBankHandle : BankHandle
	{
		private readonly AkCallbackManager.BankCallback bankCallback;

		public AsyncBankHandle(string name, AkCallbackManager.BankCallback callback) : base(name)
		{
			bankCallback = callback;
		}

		private static void GlobalBankCallback(uint in_bankID, System.IntPtr in_pInMemoryBankPtr, AKRESULT in_eLoadResult,
			uint in_memPoolId, object in_Cookie)
		{
			m_Mutex.WaitOne();
			var handle = (AsyncBankHandle) in_Cookie;
			var callback = handle.bankCallback;
			if (in_eLoadResult != AKRESULT.AK_Success)
			{
				handle.LogLoadResult(in_eLoadResult);
				m_BankHandles.Remove(handle.bankName);
			}

			m_Mutex.ReleaseMutex();

			if (callback != null)
				callback(in_bankID, in_pInMemoryBankPtr, in_eLoadResult, in_memPoolId, null);
		}

		/// Loads a bank.  This version returns right away and loads in background. See AK::SoundEngine::LoadBank for more information
		public override AKRESULT DoLoadBank()
		{
			return AkSoundEngine.LoadBank(bankName, GlobalBankCallback, this, AkSoundEngine.AK_DEFAULT_POOL_ID, out m_BankID);
		}
	}

	private class DecodableBankHandle : BankHandle
	{
		private readonly bool decodeBank = true;
		private readonly string decodedBankPath;
		private readonly bool saveDecodedBank;

		public DecodableBankHandle(string name, bool save) : base(name)
		{
			saveDecodedBank = save;

			var bankFileName = bankName + ".bnk";

			// test language-specific decoded file path
			var language = AkSoundEngine.GetCurrentLanguage();
			var decodedBankFullPath = AkSoundEngineController.GetDecodedBankFullPath();

			decodedBankPath = System.IO.Path.Combine(decodedBankFullPath, language);
			var decodedBankFilePath = System.IO.Path.Combine(decodedBankPath, bankFileName);

			var decodedFileExists = System.IO.File.Exists(decodedBankFilePath);
			if (!decodedFileExists)
			{
				// test non-language-specific decoded file path
				decodedBankPath = decodedBankFullPath;
				decodedBankFilePath = System.IO.Path.Combine(decodedBankPath, bankFileName);
				decodedFileExists = System.IO.File.Exists(decodedBankFilePath);
			}

			if (decodedFileExists)
			{
				try
				{
					var decodedFileTime = System.IO.File.GetLastWriteTime(decodedBankFilePath);
					var defaultBankPath = AkBasePathGetter.GetSoundbankBasePath();
					var encodedBankFilePath = System.IO.Path.Combine(defaultBankPath, bankFileName);
					var encodedFileTime = System.IO.File.GetLastWriteTime(encodedBankFilePath);

					decodeBank = decodedFileTime <= encodedFileTime;
				}
				catch
				{
					// Assume the decoded bank exists, but is not accessible. Re-decode it anyway, so we do nothing.
				}
			}
		}

		/// Loads a bank. This version blocks until the bank is loaded. See AK::SoundEngine::LoadBank for more information.
		public override AKRESULT DoLoadBank()
		{
			if (decodeBank)
				return AkSoundEngine.LoadAndDecodeBank(bankName, saveDecodedBank, out m_BankID);

			var res = AKRESULT.AK_Success;

			if (!string.IsNullOrEmpty(decodedBankPath))
				res = AkSoundEngine.SetBasePath(decodedBankPath);

			if (res == AKRESULT.AK_Success)
			{
				res = AkSoundEngine.LoadBank(bankName, AkSoundEngine.AK_DEFAULT_POOL_ID, out m_BankID);

				if (!string.IsNullOrEmpty(decodedBankPath))
					AkSoundEngine.SetBasePath(AkBasePathGetter.GetSoundbankBasePath());
			}

			return res;
		}

		/// Unloads a bank.
		public override void UnloadBank()
		{
			if (decodeBank && !saveDecodedBank)
				AkSoundEngine.PrepareBank(AkPreparationType.Preparation_Unload, m_BankID);
			else
				base.UnloadBank();
		}
	}
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
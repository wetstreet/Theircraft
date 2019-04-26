#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

public class AkAuxSendArray : System.IDisposable
{
	private const int MAX_COUNT = AkEnvironment.MAX_NB_ENVIRONMENTS;
	private readonly int SIZE_OF_AKAUXSENDVALUE = AkSoundEnginePINVOKE.CSharp_AkAuxSendValue_GetSizeOf();

	private System.IntPtr m_Buffer;
	private int m_Count;

	public AkAuxSendArray()
	{
		m_Buffer = System.Runtime.InteropServices.Marshal.AllocHGlobal(MAX_COUNT * SIZE_OF_AKAUXSENDVALUE);
		m_Count = 0;
	}

	public AkAuxSendValue this[int index]
	{
		get
		{
			if (index >= m_Count)
				throw new System.IndexOutOfRangeException("Out of range access in AkAuxSendArray");

			return new AkAuxSendValue(GetObjectPtr(index), false);
		}
	}

	public bool isFull
	{
		get { return m_Count >= MAX_COUNT || m_Buffer == System.IntPtr.Zero; }
	}

	public void Dispose()
	{
		if (m_Buffer != System.IntPtr.Zero)
		{
			System.Runtime.InteropServices.Marshal.FreeHGlobal(m_Buffer);
			m_Buffer = System.IntPtr.Zero;
			m_Count = 0;
		}
	}

	~AkAuxSendArray()
	{
		Dispose();
	}

	public void Reset()
	{
		m_Count = 0;
	}

	public bool Add(UnityEngine.GameObject in_listenerGameObj, uint in_AuxBusID, float in_fValue)
	{
		if (isFull)
			return false;

		AkSoundEnginePINVOKE.CSharp_AkAuxSendValue_Set(GetObjectPtr(m_Count),
			AkSoundEngine.GetAkGameObjectID(in_listenerGameObj), in_AuxBusID, in_fValue);
		m_Count++;
		return true;
	}

	public bool Add(uint in_AuxBusID, float in_fValue)
	{
		if (isFull)
			return false;

		AkSoundEnginePINVOKE.CSharp_AkAuxSendValue_Set(GetObjectPtr(m_Count), AkSoundEngine.AK_INVALID_GAME_OBJECT,
			in_AuxBusID, in_fValue);
		m_Count++;
		return true;
	}

	public bool Contains(UnityEngine.GameObject in_listenerGameObj, uint in_AuxBusID)
	{
		if (m_Buffer == System.IntPtr.Zero)
			return false;

		for (var i = 0; i < m_Count; i++)
		{
			if (AkSoundEnginePINVOKE.CSharp_AkAuxSendValue_IsSame(GetObjectPtr(i),
				AkSoundEngine.GetAkGameObjectID(in_listenerGameObj), in_AuxBusID))
				return true;
		}

		return false;
	}

	public bool Contains(uint in_AuxBusID)
	{
		if (m_Buffer == System.IntPtr.Zero)
			return false;

		for (var i = 0; i < m_Count; i++)
		{
			if (AkSoundEnginePINVOKE.CSharp_AkAuxSendValue_IsSame(GetObjectPtr(i), AkSoundEngine.AK_INVALID_GAME_OBJECT,
				in_AuxBusID))
				return true;
		}

		return false;
	}

	public AKRESULT SetValues(UnityEngine.GameObject gameObject)
	{
		return (AKRESULT) AkSoundEnginePINVOKE.CSharp_AkAuxSendValue_SetGameObjectAuxSendValues(m_Buffer,
			AkSoundEngine.GetAkGameObjectID(gameObject), (uint) m_Count);
	}

	public AKRESULT GetValues(UnityEngine.GameObject gameObject)
	{
		uint count = MAX_COUNT;
		var res = (AKRESULT) AkSoundEnginePINVOKE.CSharp_AkAuxSendValue_GetGameObjectAuxSendValues(m_Buffer,
			AkSoundEngine.GetAkGameObjectID(gameObject), ref count);
		m_Count = (int) count;
		return res;
	}

	public System.IntPtr GetBuffer()
	{
		return m_Buffer;
	}

	public int Count()
	{
		return m_Count;
	}

	private System.IntPtr GetObjectPtr(int index)
	{
		return (System.IntPtr) (m_Buffer.ToInt64() + SIZE_OF_AKAUXSENDVALUE * index);
	}
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
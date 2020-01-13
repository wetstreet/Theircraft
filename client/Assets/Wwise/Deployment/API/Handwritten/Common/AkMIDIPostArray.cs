#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

public class AkMIDIPostArray
{
	private readonly int m_Count;
	private readonly int SIZE_OF = AkSoundEnginePINVOKE.CSharp_AkMIDIPost_GetSizeOf();
	private System.IntPtr m_Buffer = System.IntPtr.Zero;

	public AkMIDIPostArray(int size)
	{
		m_Count = size;
		m_Buffer = System.Runtime.InteropServices.Marshal.AllocHGlobal(m_Count * SIZE_OF);
	}

	public AkMIDIPost this[int index]
	{
		get
		{
			if (index >= m_Count)
				throw new System.IndexOutOfRangeException("Out of range access in AkMIDIPostArray");

			return new AkMIDIPost(GetObjectPtr(index), false);
		}

		set
		{
			if (index >= m_Count)
				throw new System.IndexOutOfRangeException("Out of range access in AkMIDIPostArray");

			AkSoundEnginePINVOKE.CSharp_AkMIDIPost_Clone(GetObjectPtr(index), AkMIDIPost.getCPtr(value));
		}
	}

	~AkMIDIPostArray()
	{
		System.Runtime.InteropServices.Marshal.FreeHGlobal(m_Buffer);
		m_Buffer = System.IntPtr.Zero;
	}

	public void PostOnEvent(uint in_eventID, UnityEngine.GameObject gameObject)
	{
		var gameObjectID = AkSoundEngine.GetAkGameObjectID(gameObject);
		AkSoundEngine.PreGameObjectAPICall(gameObject, gameObjectID);
		AkSoundEnginePINVOKE.CSharp_AkMIDIPost_PostOnEvent(m_Buffer, in_eventID, gameObjectID, (uint) m_Count);
	}

	public void PostOnEvent(uint in_eventID, UnityEngine.GameObject gameObject, int count)
	{
		if (count >= m_Count)
			throw new System.IndexOutOfRangeException("Out of range access in AkMIDIPostArray");

		var gameObjectID = AkSoundEngine.GetAkGameObjectID(gameObject);
		AkSoundEngine.PreGameObjectAPICall(gameObject, gameObjectID);
		AkSoundEnginePINVOKE.CSharp_AkMIDIPost_PostOnEvent(m_Buffer, in_eventID, gameObjectID, (uint) count);
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
		return (System.IntPtr) (m_Buffer.ToInt64() + SIZE_OF * index);
	}
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
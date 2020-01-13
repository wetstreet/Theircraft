#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

public class AkPositionArray : System.IDisposable
{
	public System.IntPtr m_Buffer;
	private System.IntPtr m_Current;
	private uint m_MaxCount;

	public AkPositionArray(uint in_Count)
	{
		m_Buffer = System.Runtime.InteropServices.Marshal.AllocHGlobal((int) in_Count * sizeof(float) * 9);
		m_Current = m_Buffer;
		m_MaxCount = in_Count;
		Count = 0;
	}

	public uint Count { get; private set; }

	public void Dispose()
	{
		if (m_Buffer != System.IntPtr.Zero)
		{
			System.Runtime.InteropServices.Marshal.FreeHGlobal(m_Buffer);
			m_Buffer = System.IntPtr.Zero;
			m_MaxCount = 0;
		}
	}

	~AkPositionArray()
	{
		Dispose();
	}

	public void Reset()
	{
		m_Current = m_Buffer;
		Count = 0;
	}

	public void Add(UnityEngine.Vector3 in_Pos, UnityEngine.Vector3 in_Forward, UnityEngine.Vector3 in_Top)
	{
		if (Count >= m_MaxCount)
			throw new System.IndexOutOfRangeException("Out of range access in AkPositionArray");

		//Marshal doesn't do floats.  So copy the bytes themselves.  Grrr.
		System.Runtime.InteropServices.Marshal.WriteInt32(m_Current,
			System.BitConverter.ToInt32(System.BitConverter.GetBytes(in_Forward.x), 0));
		m_Current = (System.IntPtr) (m_Current.ToInt64() + sizeof(float));
		System.Runtime.InteropServices.Marshal.WriteInt32(m_Current,
			System.BitConverter.ToInt32(System.BitConverter.GetBytes(in_Forward.y), 0));
		m_Current = (System.IntPtr) (m_Current.ToInt64() + sizeof(float));
		System.Runtime.InteropServices.Marshal.WriteInt32(m_Current,
			System.BitConverter.ToInt32(System.BitConverter.GetBytes(in_Forward.z), 0));
		m_Current = (System.IntPtr) (m_Current.ToInt64() + sizeof(float));
		System.Runtime.InteropServices.Marshal.WriteInt32(m_Current,
			System.BitConverter.ToInt32(System.BitConverter.GetBytes(in_Top.x), 0));
		m_Current = (System.IntPtr) (m_Current.ToInt64() + sizeof(float));
		System.Runtime.InteropServices.Marshal.WriteInt32(m_Current,
			System.BitConverter.ToInt32(System.BitConverter.GetBytes(in_Top.y), 0));
		m_Current = (System.IntPtr) (m_Current.ToInt64() + sizeof(float));
		System.Runtime.InteropServices.Marshal.WriteInt32(m_Current,
			System.BitConverter.ToInt32(System.BitConverter.GetBytes(in_Top.z), 0));
		m_Current = (System.IntPtr) (m_Current.ToInt64() + sizeof(float));
		System.Runtime.InteropServices.Marshal.WriteInt32(m_Current,
			System.BitConverter.ToInt32(System.BitConverter.GetBytes(in_Pos.x), 0));
		m_Current = (System.IntPtr) (m_Current.ToInt64() + sizeof(float));
		System.Runtime.InteropServices.Marshal.WriteInt32(m_Current,
			System.BitConverter.ToInt32(System.BitConverter.GetBytes(in_Pos.y), 0));
		m_Current = (System.IntPtr) (m_Current.ToInt64() + sizeof(float));
		System.Runtime.InteropServices.Marshal.WriteInt32(m_Current,
			System.BitConverter.ToInt32(System.BitConverter.GetBytes(in_Pos.z), 0));
		m_Current = (System.IntPtr) (m_Current.ToInt64() + sizeof(float));

		Count++;
	}
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

public abstract class AkBaseArray<T> : System.IDisposable
{
	public AkBaseArray(int capacity)
	{
		m_Buffer = System.Runtime.InteropServices.Marshal.AllocHGlobal(capacity * StructureSize);
		if (m_Buffer != System.IntPtr.Zero)
		{
			Capacity = capacity;

			for (var index = 0; index < capacity; ++index)
				ClearAtIntPtr(GetObjectPtr(index));
		}
	}

	public void Dispose()
	{
		if (m_Buffer != System.IntPtr.Zero)
		{
			System.Runtime.InteropServices.Marshal.FreeHGlobal(m_Buffer);
			m_Buffer = System.IntPtr.Zero;
			Capacity = 0;
		}
	}

	~AkBaseArray()
	{
		Dispose();
	}

	public int Capacity { get; private set; }

	public virtual int Count()
	{
		return Capacity;
	}

	protected abstract int StructureSize { get; }

	protected virtual void ClearAtIntPtr(System.IntPtr address) { }

	protected abstract T CreateNewReferenceFromIntPtr(System.IntPtr address);

	protected abstract void CloneIntoReferenceFromIntPtr(System.IntPtr address, T other);

	public T this[int index]
	{
		get { return CreateNewReferenceFromIntPtr(GetObjectPtr(index)); }
		set { CloneIntoReferenceFromIntPtr(GetObjectPtr(index), value); }
	}

	public System.IntPtr GetBuffer()
	{
		return m_Buffer;
	}

	private System.IntPtr m_Buffer;

	protected System.IntPtr GetObjectPtr(int index)
	{
		if (index >= Capacity)
			throw new System.IndexOutOfRangeException("Out of range access in " + GetType().Name);

		return (System.IntPtr)(m_Buffer.ToInt64() + StructureSize * index);
	}
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
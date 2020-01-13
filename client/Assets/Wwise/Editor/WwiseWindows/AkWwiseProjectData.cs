public class AkWwiseProjectData : UnityEngine.ScriptableObject
{
	public System.Collections.Generic.List<AkInfoWorkUnit> AcousticTextureWwu =
		new System.Collections.Generic.List<AkInfoWorkUnit>();

	public System.Collections.Generic.List<AkInfoWorkUnit> AuxBusWwu =
		new System.Collections.Generic.List<AkInfoWorkUnit>();

	public System.Collections.Generic.List<AkInfoWorkUnit> BankWwu = 
		new System.Collections.Generic.List<AkInfoWorkUnit>();

	public System.Collections.Generic.List<EventWorkUnit> EventWwu = 
		new System.Collections.Generic.List<EventWorkUnit>();

	public System.Collections.Generic.List<AkInfoWorkUnit> RtpcWwu = 
		new System.Collections.Generic.List<AkInfoWorkUnit>();

	public System.Collections.Generic.List<GroupValWorkUnit> StateWwu =
		new System.Collections.Generic.List<GroupValWorkUnit>();

	public System.Collections.Generic.List<GroupValWorkUnit> SwitchWwu =
		new System.Collections.Generic.List<GroupValWorkUnit>();

	public System.Collections.Generic.List<AkInfoWorkUnit> TriggerWwu =
		new System.Collections.Generic.List<AkInfoWorkUnit>();

	//Contains the path of all items that are expanded in the Wwise picker
	public System.Collections.Generic.List<string> ExpandedItems = new System.Collections.Generic.List<string>();

	public bool autoPopulateEnabled = true;
	public string CurrentPluginConfig;

	public System.Collections.ArrayList GetWwuListByString(string in_wwuType)
	{
		if (string.Equals(in_wwuType, "Events", System.StringComparison.OrdinalIgnoreCase))
			return System.Collections.ArrayList.Adapter(EventWwu);
		if (string.Equals(in_wwuType, "States", System.StringComparison.OrdinalIgnoreCase))
			return System.Collections.ArrayList.Adapter(StateWwu);
		if (string.Equals(in_wwuType, "Switches", System.StringComparison.OrdinalIgnoreCase))
			return System.Collections.ArrayList.Adapter(SwitchWwu);
		if (string.Equals(in_wwuType, "Master-Mixer Hierarchy", System.StringComparison.OrdinalIgnoreCase))
			return System.Collections.ArrayList.Adapter(AuxBusWwu);
		if (string.Equals(in_wwuType, "SoundBanks", System.StringComparison.OrdinalIgnoreCase))
			return System.Collections.ArrayList.Adapter(BankWwu);
		if (string.Equals(in_wwuType, "Game Parameters", System.StringComparison.OrdinalIgnoreCase))
			return System.Collections.ArrayList.Adapter(RtpcWwu);
		if (string.Equals(in_wwuType, "Triggers", System.StringComparison.OrdinalIgnoreCase))
			return System.Collections.ArrayList.Adapter(TriggerWwu);
		if (string.Equals(in_wwuType, "Virtual Acoustics", System.StringComparison.OrdinalIgnoreCase))
			return System.Collections.ArrayList.Adapter(AcousticTextureWwu);

		return null;
	}

	public float GetEventMaxAttenuation(uint eventID)
	{
		var Event = GetEventInfo(eventID);
		return Event != null ? Event.maxAttenuation : 0.0f;
	}

	public Event GetEventInfo(uint eventID)
	{
		foreach (var wwu in EventWwu)
		{
			foreach (var entry in wwu.List)
			{
				if (entry.Id == eventID)
				{
					return entry;
				}
			}
		}

		return null;
	}

	public void Reset()
	{
		EventWwu = new System.Collections.Generic.List<EventWorkUnit>();
		StateWwu = new System.Collections.Generic.List<GroupValWorkUnit>();
		SwitchWwu = new System.Collections.Generic.List<GroupValWorkUnit>();
		BankWwu = new System.Collections.Generic.List<AkInfoWorkUnit>();
		AuxBusWwu = new System.Collections.Generic.List<AkInfoWorkUnit>();
		RtpcWwu = new System.Collections.Generic.List<AkInfoWorkUnit>();
		TriggerWwu = new System.Collections.Generic.List<AkInfoWorkUnit>();
		AcousticTextureWwu = new System.Collections.Generic.List<AkInfoWorkUnit>();
	}

	public void Migrate()
	{
		foreach (var wwu in EventWwu)
			wwu.Migrate();

		foreach (var wwu in StateWwu)
			wwu.Migrate();

		foreach (var wwu in SwitchWwu)
			wwu.Migrate();

		foreach (var wwu in BankWwu)
			wwu.Migrate();

		foreach (var wwu in AuxBusWwu)
			wwu.Migrate();

		foreach (var wwu in RtpcWwu)
			wwu.Migrate();

		foreach (var wwu in TriggerWwu)
			wwu.Migrate();

		foreach (var wwu in AcousticTextureWwu)
			wwu.Migrate();

		UnityEditor.EditorUtility.SetDirty(this);
	}

	[System.Serializable]
	private class ByteArrayWrapper
	{
		public byte[] bytes;

		public ByteArrayWrapper(byte[] byteArray)
		{
			bytes = byteArray;
		}
	}

	private static System.Guid GetGuid(byte[] bytes)
	{
		try
		{
			return new System.Guid(bytes);
		}
		catch
		{
			return System.Guid.Empty;
		}
	}

	[System.Serializable]
	public class AkBaseInformation : System.IComparable
	{
		[UnityEngine.SerializeField]
		[UnityEngine.Serialization.FormerlySerializedAs("Name")]
		private string name;

		public string Name
		{
			get { return name; }

			set
			{
				name = value;
				id = AkUtilities.ShortIDGenerator.Compute(value);
			}
		}

		[UnityEngine.HideInInspector]
		[UnityEngine.SerializeField]
		[UnityEngine.Serialization.FormerlySerializedAs("Guid")]
		private byte[] guid = null;

		public System.Guid Guid
		{
			get { return GetGuid(guid); }
			set { guid = value.ToByteArray(); }
		}

		[UnityEngine.SerializeField]
		[UnityEngine.Serialization.FormerlySerializedAs("ID")]
		private uint id;

		public uint Id
		{
			get { return id; }
		}

		[UnityEngine.HideInInspector]
		public System.Collections.Generic.List<PathElement> PathAndIcons = new System.Collections.Generic.List<PathElement>();

		int System.IComparable.CompareTo(object other)
		{
			if (other == null)
				return 1;

			var otherAkInformation = other as AkBaseInformation;
			if (otherAkInformation == null)
				throw new System.ArgumentException("Object is not of type AkBaseInformation");

			return Name.CompareTo(otherAkInformation.Name);
		}

		private class _CompareByGuid : System.Collections.Generic.IComparer<AkBaseInformation>
		{
			int System.Collections.Generic.IComparer<AkBaseInformation>.Compare(AkBaseInformation a, AkBaseInformation b)
			{
				if (a == null)
					return b == null ? 0 : -1;

				return a.Guid.CompareTo(b.Guid);
			}
		}

		public static System.Collections.Generic.IComparer<AkBaseInformation> CompareByGuid = new _CompareByGuid();
	}

	[System.Serializable]
	public class AkInformation : AkBaseInformation
	{
		public string Path;
	}

	[System.Serializable]
	public class GroupValue : AkInformation
	{
		public System.Collections.Generic.List<AkBaseInformation> values =
			new System.Collections.Generic.List<AkBaseInformation>();

		[UnityEngine.HideInInspector]
		[UnityEngine.SerializeField]
		private System.Collections.Generic.List<ByteArrayWrapper> ValueGuids =
			new System.Collections.Generic.List<ByteArrayWrapper>();
		[UnityEngine.HideInInspector]
		[UnityEngine.SerializeField]
		private System.Collections.Generic.List<PathElement> ValueIcons = new System.Collections.Generic.List<PathElement>();
		[UnityEngine.HideInInspector]
		[UnityEngine.SerializeField]
		[UnityEngine.Serialization.FormerlySerializedAs("values")]
		private System.Collections.Generic.List<string> valuesInternal = new System.Collections.Generic.List<string>();

		public void Migrate()
		{
			var count = ValueGuids.Count;
			if (count < 1 || count != ValueIcons.Count || count != valuesInternal.Count)
				return;

			values.Clear();
			for (var i = 0; i < count; ++i)
			{
				var value = new AkBaseInformation
				{
					Name = valuesInternal[i],
					Guid = GetGuid(ValueGuids[i].bytes)
				};
				value.PathAndIcons.Add(ValueIcons[i]);
				values.Add(value);
			}
		}
	}

	[System.Serializable]
	public class Event : AkInformation
	{
		public float maxAttenuation;
		public float maxDuration;
		public float minDuration;
	}

	[System.Serializable]
	public class WorkUnit : System.IComparable
	{
		public string PhysicalPath;
		public string ParentPhysicalPath;

		[UnityEngine.HideInInspector]
		[UnityEngine.SerializeField]
		private byte[] guid = null;

		[UnityEngine.HideInInspector]
		[UnityEngine.SerializeField]
		[UnityEngine.Serialization.FormerlySerializedAs("Guid")]
		private string GuidInternal;

		public System.Guid Guid
		{
			get { return GetGuid(guid); }
			set { guid = value.ToByteArray(); }
		}

		[UnityEngine.HideInInspector]
		[UnityEngine.SerializeField]
		private long m_lastTime;

		public System.DateTime LastTime
		{
			get { return m_lastTime == 0 ? System.DateTime.MinValue : System.DateTime.FromBinary(m_lastTime); }
			set { m_lastTime = value.ToBinary(); }
		}

		public virtual void Migrate()
		{
			try
			{
				Guid = new System.Guid(GuidInternal);
			}
			catch
			{
				Guid = System.Guid.Empty;
			}
		}

		int System.IComparable.CompareTo(object other)
		{
			if (other == null)
				return 1;

			var otherWwu = other as WorkUnit;
			if (otherWwu == null)
				throw new System.ArgumentException("Object is not a WorkUnit");

			return PhysicalPath.CompareTo(otherWwu.PhysicalPath);
		}
	}

	[System.Serializable]
	public class GenericWorkUnit<T> : WorkUnit
	{
		public System.Collections.Generic.List<T> List = new System.Collections.Generic.List<T>();
	}

	[System.Serializable]
	public class AkInfoWorkUnit : GenericWorkUnit<AkInformation> { }

	[System.Serializable]
	public class EventWorkUnit : GenericWorkUnit<Event> { }

	[System.Serializable]
	public class GroupValWorkUnit : GenericWorkUnit<GroupValue>
	{
		public override void Migrate()
		{
			base.Migrate();

			foreach (var item in List)
				item.Migrate();
		}
	}

	[System.Serializable]
	public class PathElement
	{
		public string ElementName;
		public WwiseObjectType ObjectType;

		public PathElement(string Name, WwiseObjectType objType)
		{
			ElementName = Name;
			ObjectType = objType;
		}
	}
}

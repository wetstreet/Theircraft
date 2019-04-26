#if UNITY_EDITOR
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2014 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

[UnityEditor.InitializeOnLoad]
public class AkWwiseXMLBuilder
{
	private static readonly System.DateTime s_LastParsed = System.DateTime.MinValue;

	static AkWwiseXMLBuilder()
	{
		AkWwiseXMLWatcher.Instance.PopulateXML = Populate;

		AkWwiseXMLWatcher.Instance.GetEventMaxDuration = (uint eventID) =>
		{
			var eventInfo = AkWwiseProjectInfo.GetData().GetEventInfo(eventID);
			if (eventInfo != null)
			{
				return eventInfo.maxDuration;
			}

			return null;
		};
	}

	public static bool Populate()
	{
		if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode || UnityEditor.EditorApplication.isCompiling)
		{
			return false;
		}

		try
		{
			// Try getting the SoundbanksInfo.xml file for Windows or Mac first, then try to find any other available platform.
			var FullSoundbankPath = AkBasePathGetter.GetPlatformBasePath();
			var filename = System.IO.Path.Combine(FullSoundbankPath, "SoundbanksInfo.xml");
			if (!System.IO.File.Exists(filename))
			{
				FullSoundbankPath = System.IO.Path.Combine(UnityEngine.Application.streamingAssetsPath,
					WwiseSetupWizard.Settings.SoundbankPath);

				if (!System.IO.Directory.Exists(FullSoundbankPath))
					return false;

				var foundFiles =
					System.IO.Directory.GetFiles(FullSoundbankPath, "SoundbanksInfo.xml", System.IO.SearchOption.AllDirectories);

				if (foundFiles.Length == 0)
					return false;

				filename = foundFiles[0];
			}

			var time = System.IO.File.GetLastWriteTime(filename);
			if (time <= s_LastParsed)
			{
				return false;
			}

			var doc = new System.Xml.XmlDocument();
			doc.Load(filename);

			var bChanged = false;
			var soundBanks = doc.GetElementsByTagName("SoundBanks");
			for (var i = 0; i < soundBanks.Count; i++)
			{
				var soundBank = soundBanks[i].SelectNodes("SoundBank");
				for (var j = 0; j < soundBank.Count; j++)
				{
					bChanged = SerialiseSoundBank(soundBank[j]) || bChanged;
				}
			}

			return bChanged;
		}
		catch
		{
			return false;
		}
	}

	private static bool SerialiseSoundBank(System.Xml.XmlNode node)
	{
		var bChanged = false;
		var includedEvents = node.SelectNodes("IncludedEvents");
		for (var i = 0; i < includedEvents.Count; i++)
		{
			var events = includedEvents[i].SelectNodes("Event");
			for (var j = 0; j < events.Count; j++)
			{
				bChanged = SerialiseMaxAttenuation(events[j]) || SerialiseEstimatedDuration(events[j]) || bChanged;
			}
		}

		return bChanged;
	}

	private static bool SerialiseMaxAttenuation(System.Xml.XmlNode node)
	{
		var bChanged = false;
		for (var i = 0; i < AkWwiseProjectInfo.GetData().EventWwu.Count; i++)
		{
			for (var j = 0; j < AkWwiseProjectInfo.GetData().EventWwu[i].List.Count; j++)
			{
				if (node.Attributes["MaxAttenuation"] != null &&
				    node.Attributes["Name"].InnerText == AkWwiseProjectInfo.GetData().EventWwu[i].List[j].Name)
				{
					var radius = float.Parse(node.Attributes["MaxAttenuation"].InnerText);
					if (AkWwiseProjectInfo.GetData().EventWwu[i].List[j].maxAttenuation != radius)
					{
						AkWwiseProjectInfo.GetData().EventWwu[i].List[j].maxAttenuation = radius;
						bChanged = true;
					}

					break;
				}
			}
		}

		return bChanged;
	}

	private static bool SerialiseEstimatedDuration(System.Xml.XmlNode node)
	{
		var bChanged = false;
		for (var i = 0; i < AkWwiseProjectInfo.GetData().EventWwu.Count; i++)
		{
			for (var j = 0; j < AkWwiseProjectInfo.GetData().EventWwu[i].List.Count; j++)
			{
				if (node.Attributes["Name"].InnerText == AkWwiseProjectInfo.GetData().EventWwu[i].List[j].Name)
				{
					if (node.Attributes["DurationMin"] != null)
					{
						var minDuration = UnityEngine.Mathf.Infinity;
						if (string.Compare(node.Attributes["DurationMin"].InnerText, "Infinite") != 0)
						{
							minDuration = float.Parse(node.Attributes["DurationMin"].InnerText);
						}

						if (AkWwiseProjectInfo.GetData().EventWwu[i].List[j].minDuration != minDuration)
						{
							AkWwiseProjectInfo.GetData().EventWwu[i].List[j].minDuration = minDuration;
							bChanged = true;
						}
					}

					if (node.Attributes["DurationMax"] != null)
					{
						var maxDuration = UnityEngine.Mathf.Infinity;
						if (string.Compare(node.Attributes["DurationMax"].InnerText, "Infinite") != 0)
						{
							maxDuration = float.Parse(node.Attributes["DurationMax"].InnerText);
						}

						if (AkWwiseProjectInfo.GetData().EventWwu[i].List[j].maxDuration != maxDuration)
						{
							AkWwiseProjectInfo.GetData().EventWwu[i].List[j].maxDuration = maxDuration;
							bChanged = true;
						}
					}

					break;
				}
			}
		}

		return bChanged;
	}
}
#endif
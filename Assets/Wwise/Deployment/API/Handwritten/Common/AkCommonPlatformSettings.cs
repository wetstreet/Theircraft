public class AkBasePlatformSettings : UnityEngine.ScriptableObject
{
	public virtual AkInitializationSettings AkInitializationSettings
	{
		get
		{
			return new AkInitializationSettings();
		}
	}

	public virtual AkSpatialAudioInitSettings AkSpatialAudioInitSettings
	{
		get
		{
			return new AkSpatialAudioInitSettings();
		}
	}

	public virtual AkCallbackManager.InitializationSettings CallbackManagerInitializationSettings
	{
		get
		{
			return new AkCallbackManager.InitializationSettings();
		}
	}

	public virtual string InitialLanguage
	{
		get
		{
			return "English(US)";
		}
	}

	public virtual string SoundbankPath
	{
		get
		{
			return AkBasePathGetter.DefaultBasePath;
		}
	}

	public virtual AkCommunicationSettings AkCommunicationSettings
	{
		get
		{
			return new AkCommunicationSettings();
		}
	}
}

[System.Serializable]
public class AkCommonOutputSettings
{
	[UnityEngine.Tooltip("The name of a custom audio device to be used. Custom audio devices are defined in the Audio Device Shareset section of the Wwise project. Leave this empty to output normally through the default audio device.")]
	public string m_AudioDeviceShareset = string.Empty;

	[UnityEngine.Tooltip("Device specific identifier, when multiple devices of the same type are possible.  If only one device is possible, leave to 0.")]
	public uint m_DeviceID = AkSoundEngine.AK_INVALID_UNIQUE_ID;

	public enum PanningRule
	{
		Speakers = 0,
		Headphones = 1
	}

	[UnityEngine.Tooltip("Rule for 3D panning of signals routed to a stereo bus. In \"Speakers\" mode, the angle of the front loudspeakers is used. In \"Headphones\" mode, the speaker angles are superseded with constant power panning between two virtual microphones spaced 180 degrees apart.")]
	public PanningRule m_PanningRule = PanningRule.Speakers;

	[System.Serializable]
	public class ChannelConfiguration
	{
		public enum ChannelConfigType
		{
			Anonymous = 0x0,
			Standard = 0x1,
			Ambisonic = 0x2
		}

		[UnityEngine.Tooltip("A code that completes the identification of channels by uChannelMask. Anonymous: Channel mask == 0 and channels; Standard: Channels must be identified with standard defines in AkSpeakerConfigs; Ambisonic: Channel mask == 0 and channels follow standard ambisonic order.")]
		public ChannelConfigType m_ChannelConfigType = ChannelConfigType.Anonymous;

		public enum ChannelMask
		{
			NONE = 0x0,

			/// Standard speakers (channel mask):
			FRONT_LEFT = 0x1,        ///< Front left speaker bit mask
			FRONT_RIGHT = 0x2,       ///< Front right speaker bit mask
			FRONT_CENTER = 0x4,      ///< Front center speaker bit mask
			LOW_FREQUENCY = 0x8,     ///< Low-frequency speaker bit mask
			BACK_LEFT = 0x10,        ///< Rear left speaker bit mask
			BACK_RIGHT = 0x20,       ///< Rear right speaker bit mask
			BACK_CENTER = 0x100, ///< Rear center speaker ("surround speaker") bit mask
			SIDE_LEFT = 0x200,   ///< Side left speaker bit mask
			SIDE_RIGHT = 0x400,  ///< Side right speaker bit mask

			/// "Height" speakers.
			TOP = 0x800,     ///< Top speaker bit mask
			HEIGHT_FRONT_LEFT = 0x1000,  ///< Front left speaker bit mask
			HEIGHT_FRONT_CENTER = 0x2000,    ///< Front center speaker bit mask
			HEIGHT_FRONT_RIGHT = 0x4000, ///< Front right speaker bit mask
			HEIGHT_BACK_LEFT = 0x8000,   ///< Rear left speaker bit mask
			HEIGHT_BACK_CENTER = 0x10000,    ///< Rear center speaker bit mask
			HEIGHT_BACK_RIGHT = 0x20000, ///< Rear right speaker bit mask

			//
			// Supported speaker setups. Those are the ones that can be used in the Wwise Sound Engine audio pipeline.
			//
			SETUP_MONO = FRONT_CENTER,        ///< 1.0 setup channel mask
			SETUP_0POINT1 = LOW_FREQUENCY,    ///< 0.1 setup channel mask
			SETUP_1POINT1 = (FRONT_CENTER | LOW_FREQUENCY),    ///< 1.1 setup channel mask
			SETUP_STEREO = (FRONT_LEFT | FRONT_RIGHT), ///< 2.0 setup channel mask
			SETUP_2POINT1 = (SETUP_STEREO | LOW_FREQUENCY),    ///< 2.1 setup channel mask
			SETUP_3STEREO = (SETUP_STEREO | FRONT_CENTER), ///< 3.0 setup channel mask
			SETUP_3POINT1 = (SETUP_3STEREO | LOW_FREQUENCY),   ///< 3.1 setup channel mask
			SETUP_4 = (SETUP_STEREO | SIDE_LEFT | SIDE_RIGHT),  ///< 4.0 setup channel mask
			SETUP_4POINT1 = (SETUP_4 | LOW_FREQUENCY), ///< 4.1 setup channel mask
			SETUP_5 = (SETUP_4 | FRONT_CENTER),    ///< 5.0 setup channel mask
			SETUP_5POINT1 = (SETUP_5 | LOW_FREQUENCY), ///< 5.1 setup channel mask
			SETUP_6 = (SETUP_4 | BACK_LEFT | BACK_RIGHT),   ///< 6.0 setup channel mask
			SETUP_6POINT1 = (SETUP_6 | LOW_FREQUENCY), ///< 6.1 setup channel mask
			SETUP_7 = (SETUP_6 | FRONT_CENTER),    ///< 7.0 setup channel mask
			SETUP_7POINT1 = (SETUP_7 | LOW_FREQUENCY), ///< 7.1 setup channel mask
			SETUP_SURROUND = (SETUP_STEREO | BACK_CENTER), ///< Legacy surround setup channel mask

			// Note. DPL2 does not really have 4 channels, but it is used by plugins to differentiate from stereo setup.
			SETUP_DPL2 = (SETUP_4),       ///< Legacy DPL2 setup channel mask

			SETUP_HEIGHT_4 = (HEIGHT_FRONT_LEFT | HEIGHT_FRONT_RIGHT | HEIGHT_BACK_LEFT | HEIGHT_BACK_RIGHT),    ///< 4 speaker height layer.
			SETUP_HEIGHT_5 = (SETUP_HEIGHT_4 | HEIGHT_FRONT_CENTER),                                                                   ///< 5 speaker height layer.
			SETUP_HEIGHT_ALL = (SETUP_HEIGHT_5 | HEIGHT_BACK_CENTER),                                                                      ///< All height speaker layer.

			// Auro speaker setups
			SETUP_AURO_222 = (SETUP_4 | HEIGHT_FRONT_LEFT | HEIGHT_FRONT_RIGHT),    ///< Auro-222 setup channel mask
			SETUP_AURO_8 = (SETUP_AURO_222 | HEIGHT_BACK_LEFT | HEIGHT_BACK_RIGHT),     ///< Auro-8 setup channel mask
			SETUP_AURO_9 = (SETUP_AURO_8 | FRONT_CENTER),                                          ///< Auro-9.0 setup channel mask
			SETUP_AURO_9POINT1 = (SETUP_AURO_9 | LOW_FREQUENCY),                                           ///< Auro-9.1 setup channel mask
			SETUP_AURO_10 = (SETUP_AURO_9 | TOP),                                                  ///< Auro-10.0 setup channel mask		
			SETUP_AURO_10POINT1 = (SETUP_AURO_10 | LOW_FREQUENCY),                                         ///< Auro-10.1 setup channel mask	
			SETUP_AURO_11 = (SETUP_AURO_10 | HEIGHT_FRONT_CENTER),                                 ///< Auro-11.0 setup channel mask
			SETUP_AURO_11POINT1 = (SETUP_AURO_11 | LOW_FREQUENCY),                                         ///< Auro-11.1 setup channel mask	
			SETUP_AURO_11_740 = (SETUP_7 | SETUP_HEIGHT_4),                                        ///< Auro-11.0 (7+4) setup channel mask
			SETUP_AURO_11POINT1_740 = (SETUP_AURO_11_740 | LOW_FREQUENCY),                                     ///< Auro-11.1 (7+4) setup channel mask
			SETUP_AURO_13_751 = (SETUP_7 | SETUP_HEIGHT_5 | TOP),                       ///< Auro-13.0 setup channel mask
			SETUP_AURO_13POINT1_751 = (SETUP_AURO_13_751 | LOW_FREQUENCY),                                     ///< Auro-13.1 setup channel mask

			// Dolby speaker setups: in Dolby nomenclature, [#plane].[lfe].[#height]
			SETUP_DOLBY_5_0_2 = (SETUP_5 | HEIGHT_FRONT_LEFT | HEIGHT_FRONT_RIGHT), ///< Dolby 5.0.2 setup channel mask
			SETUP_DOLBY_5_1_2 = (SETUP_DOLBY_5_0_2 | LOW_FREQUENCY),                                   ///< Dolby 5.1.2 setup channel mask
			SETUP_DOLBY_6_0_2 = (SETUP_6 | HEIGHT_FRONT_LEFT | HEIGHT_FRONT_RIGHT), ///< Dolby 6.0.2 setup channel mask
			SETUP_DOLBY_6_1_2 = (SETUP_DOLBY_6_0_2 | LOW_FREQUENCY),                                   ///< Dolby 6.1.2 setup channel mask
			SETUP_DOLBY_6_0_4 = (SETUP_DOLBY_6_0_2 | HEIGHT_BACK_LEFT | HEIGHT_BACK_RIGHT), ///< Dolby 6.0.4 setup channel mask
			SETUP_DOLBY_6_1_4 = (SETUP_DOLBY_6_0_4 | LOW_FREQUENCY),                                   ///< Dolby 6.1.4 setup channel mask
			SETUP_DOLBY_7_0_2 = (SETUP_7 | HEIGHT_FRONT_LEFT | HEIGHT_FRONT_RIGHT), ///< Dolby 7.0.2 setup channel mask
			SETUP_DOLBY_7_1_2 = (SETUP_DOLBY_7_0_2 | LOW_FREQUENCY),                                   ///< Dolby 7.1.2 setup channel mask
			SETUP_DOLBY_7_0_4 = (SETUP_DOLBY_7_0_2 | HEIGHT_BACK_LEFT | HEIGHT_BACK_RIGHT), ///< Dolby 7.0.4 setup channel mask
			SETUP_DOLBY_7_1_4 = (SETUP_DOLBY_7_0_4 | LOW_FREQUENCY),                                   ///< Dolby 7.1.4 setup channel mask

			SETUP_ALL_SPEAKERS = (SETUP_7POINT1 | BACK_CENTER | SETUP_HEIGHT_ALL | TOP), ///< All speakers.
		};

		[UnityEngine.Tooltip("A bit field, whose channel identifiers depend on AkChannelConfigType (up to 20).")]
		[AkEnumFlag(typeof(ChannelMask))]
		public ChannelMask m_ChannelMask = ChannelMask.NONE;

		[UnityEngine.Tooltip("The number of channels, identified (deduced from channel mask) or anonymous (set directly).")]
		public uint m_NumberOfChannels = 0;

		public void CopyTo(AkChannelConfig config)
		{
			switch (m_ChannelConfigType)
			{
				case ChannelConfigType.Anonymous:
					config.SetAnonymous(m_NumberOfChannels);
					break;

				case ChannelConfigType.Standard:
					config.SetStandard((uint)m_ChannelMask);
					break;

				case ChannelConfigType.Ambisonic:
					config.SetAmbisonic(m_NumberOfChannels);
					break;
			}
		}
	}

	[UnityEngine.Tooltip("Channel configuration for this output. Hardware might not support the selected configuration.")]
	public ChannelConfiguration m_ChannelConfig = new ChannelConfiguration();

	public void CopyTo(AkOutputSettings settings)
	{
		settings.audioDeviceShareset = string.IsNullOrEmpty(m_AudioDeviceShareset) ? AkSoundEngine.AK_INVALID_UNIQUE_ID : AkUtilities.ShortIDGenerator.Compute(m_AudioDeviceShareset);
		settings.idDevice = m_DeviceID;
		settings.ePanningRule = (AkPanningRule)m_PanningRule;
		m_ChannelConfig.CopyTo(settings.channelConfig);
	}
}

public class AkSettingsValidationHandler
{
	public virtual void Validate() { }
}

[System.Serializable]
public class AkCommonUserSettings : AkSettingsValidationHandler
{
	[UnityEngine.Tooltip("Path for the soundbanks. This must contain one sub folder per platform, with the same as in the Wwise project.")]
	public string m_BasePath = AkBasePathGetter.DefaultBasePath;

	[UnityEngine.Tooltip("Language sub-folder used at startup.")]
	public string m_StartupLanguage = "English(US)";

	[UnityEngine.Tooltip("Prepare Pool size. This contains the banks loaded using PrepareBank (Banks decoded on load use this). Default size is 0 MB (will not allocate a Prepare Pool), and minimal size if 8096 bytes. This should be adjusted for your needs.")]
	public uint m_PreparePoolSize = 0;

	[UnityEngine.Tooltip("CallbackManager buffer size. The size of the buffer used per-frame to transfer callback data. Default size is 4 KB, but you should increase this, if required.")]
	public int m_CallbackManagerBufferSize = AkCallbackManager.InitializationSettings.DefaultBufferSize;

	[UnityEngine.Tooltip("Enable Wwise engine logging. This is used to turn on/off the logging of the Wwise engine.")]
	public bool m_EngineLogging = AkCallbackManager.InitializationSettings.DefaultIsLoggingEnabled;

	[UnityEngine.Tooltip("Maximum number of memory pools. A memory pool is required for each loaded bank.")]
	public uint m_MaximumNumberOfMemoryPools = 32;

	public void CopyTo(AkMemSettings settings)
	{
		settings.uMaxNumPools = m_MaximumNumberOfMemoryPools;
	}

	[UnityEngine.Tooltip("Maximum number of automation paths for positioning sounds.")]
	public uint m_MaximumNumberOfPositioningPaths = 255;

	[UnityEngine.Tooltip("Size of the default memory pool.")]
	public uint m_DefaultPoolSize = 16 * 1024 * 1024;

	[UnityEngine.Tooltip("The percentage of occupied memory where the sound engine should enter in Low memory Mode.")]
	[UnityEngine.Range(0, 1)]
	public float m_MemoryCutoffThreshold = 1.0f;

	[UnityEngine.Tooltip("Size of the command queue.")]
	public uint m_CommandQueueSize = 256 * 1024;

	[UnityEngine.Tooltip("Number of samples per audio frame (256, 512, 1024, or 2048).")]
	public uint m_SamplesPerFrame = 1024;

	[UnityEngine.Tooltip("Main output device settings.")]
	public AkCommonOutputSettings m_MainOutputSettings;

	public virtual void CopyTo(AkInitSettings settings)
	{
		settings.uMaxNumPaths = m_MaximumNumberOfPositioningPaths;
		settings.uDefaultPoolSize = m_DefaultPoolSize;
		settings.fDefaultPoolRatioThreshold = m_MemoryCutoffThreshold;
		settings.uCommandQueueSize = m_CommandQueueSize;
		settings.uNumSamplesPerFrame = m_SamplesPerFrame;
		m_MainOutputSettings.CopyTo(settings.settingsMainOutput);
#if PLATFORM_LUMIN && !UNITY_EDITOR
		settings.szPluginDLLPath = UnityEngine.Application.dataPath.Replace("Data", "bin") + System.IO.Path.DirectorySeparatorChar;
#elif (!UNITY_ANDROID && !UNITY_WSA) || UNITY_EDITOR
		settings.szPluginDLLPath = System.IO.Path.Combine(UnityEngine.Application.dataPath, "Plugins" + System.IO.Path.DirectorySeparatorChar);
#else
		settings.szPluginDLLPath = null;
#endif
	}

	[UnityEngine.Tooltip("Multiplication factor for all streaming look-ahead heuristic values.")]
	[UnityEngine.Range(0, 1)]
	public float m_StreamingLookAheadRatio = 1.0f;

	public void CopyTo(AkMusicSettings settings)
	{
		settings.fStreamingLookAheadRatio = m_StreamingLookAheadRatio;
	}

	[UnityEngine.Tooltip("Size of memory pool for small objects of Stream Manager. Small objects are the Stream Manager instance, devices, stream objects, user stream names, pending transfers, buffer records, pending open commands, and so on. Ideally, this pool should never run out of memory, because it may cause undesired I/O transfer cancellation, and even major CPU spikes. I/O memory should be bound by the size of each device's I/O pool instead.")]
	public uint m_StreamManagerPoolSize = 64 * 1024;

	public void CopyTo(AkStreamMgrSettings settings)
	{
		settings.uMemorySize = m_StreamManagerPoolSize;
	}

	public virtual void CopyTo(AkDeviceSettings settings)
	{
	}

	[UnityEngine.Tooltip("Sampling Rate. Default is 48000 Hz. Use 24000hz for low quality. Any positive reasonable sample rate is supported; however, be careful setting a custom value. Using an odd or really low sample rate may cause the sound engine to malfunction.")]
	public uint m_SampleRate = 48000;

	[UnityEngine.Tooltip("Lower Engine default memory pool size.")]
	public uint m_LowerEnginePoolSize = 16 * 1024 * 1024;

	[UnityEngine.Tooltip("The percentage of occupied memory where the sound engine should enter in Low memory mode.")]
	[UnityEngine.Range(0, 1)]
	public float m_LowerEngineMemoryCutoffThreshold = 1;

	[UnityEngine.Tooltip("Number of refill buffers in voice buffer. Set to 2 for double-buffered, defaults to 4.")]
	public ushort m_NumberOfRefillsInVoice = 4;

	public virtual void CopyTo(AkPlatformInitSettings settings)
	{
#if !UNITY_PS4 && !UNITY_XBOXONE
		settings.uSampleRate = m_SampleRate;
#endif
		settings.uLEngineDefaultPoolSize = m_LowerEnginePoolSize;
		settings.fLEngineDefaultPoolRatioThreshold = m_LowerEngineMemoryCutoffThreshold;
		settings.uNumRefillsInVoice = m_NumberOfRefillsInVoice;
	}

	[System.Serializable]
	public class SpatialAudioSettings
	{
		[UnityEngine.Tooltip("Desired spatial audio memory pool size.")]
		public uint m_PoolSize = 4 * 1024 * 1024;

		[UnityEngine.Tooltip("Maximum number of portals that sound can propagate through.")]
		[UnityEngine.Range(0, AkSoundEngine.AK_MAX_SOUND_PROPAGATION_DEPTH)]
		public uint m_MaxSoundPropagationDepth = AkSoundEngine.AK_MAX_SOUND_PROPAGATION_DEPTH;

		public enum DiffractionFlags
		{
			UseBuiltInParam = 1 << 0,
			UseObstruction = 1 << 1,
			CalcEmitterVirtualPosition = 1 << 3,
		}

		[UnityEngine.Tooltip("Determines whether diffraction values for sound passing through portals will be calculated, and how to apply those calculations to Wwise parameters.")]
		[AkEnumFlag(typeof(DiffractionFlags))]
		public DiffractionFlags m_DiffractionFlags = (DiffractionFlags)~0;

		[UnityEngine.Tooltip("Distance (in game units) that an emitter or listener has to move to trigger a recalculation of reflections/diffraction. Larger values can reduce the CPU load at the cost of reduced accuracy.")]
		public float m_MovementThreshold = 1.0f;
	}

	[UnityEngine.Tooltip("Spatial audio common settings.")]
	public SpatialAudioSettings m_SpatialAudioSettings;

	public virtual void CopyTo(AkSpatialAudioInitSettings settings)
	{
		settings.uPoolSize = m_SpatialAudioSettings.m_PoolSize;
		settings.uMaxSoundPropagationDepth = m_SpatialAudioSettings.m_MaxSoundPropagationDepth;
		settings.uDiffractionFlags = (uint)m_SpatialAudioSettings.m_DiffractionFlags;
		settings.fMovementThreshold = m_SpatialAudioSettings.m_MovementThreshold;
	}

	public virtual void CopyTo(AkUnityPlatformSpecificSettings settings)
	{
	}

	public override void Validate()
	{
		if (m_PreparePoolSize > 0 && m_PreparePoolSize < 8096)
		{
			m_PreparePoolSize = 8096;
		}
	}
}

[System.Serializable]
public class AkCommonAdvancedSettings : AkSettingsValidationHandler
{
	[UnityEngine.Tooltip("Size of memory pool for I/O (for automatic streams). It is passed directly to AK::MemoryMgr::CreatePool(), after having been rounded down to a multiple of uGranularity.")]
	public uint m_IOMemorySize = 2 * 1024 * 1024;

	[UnityEngine.Tooltip("Targeted automatic stream buffer length (ms). When a stream reaches that buffering, it stops being scheduled for I/O except if the scheduler is idle.")]
	public float m_TargetAutoStreamBufferLengthMs = 380.0f;

	[UnityEngine.Tooltip("If true the device attempts to reuse IO buffers that have already been streamed from disk. This is particularly useful when streaming small looping sounds. The drawback is a small CPU hit when allocating memory, and a slightly larger memory footprint in the StreamManager pool.")]
	public bool m_UseStreamCache = false;

	[UnityEngine.Tooltip("Maximum number of bytes that can be \"pinned\" using AK::SoundEngine::PinEventInStreamCache() or AK::IAkStreamMgr::PinFileInCache()")]
	public uint m_MaximumPinnedBytesInCache = unchecked((uint)(-1));

	public virtual void CopyTo(AkDeviceSettings settings)
	{
		settings.uIOMemorySize = m_IOMemorySize;
		settings.fTargetAutoStmBufferLength = m_TargetAutoStreamBufferLengthMs;
		settings.bUseStreamCache = m_UseStreamCache;
		settings.uMaxCachePinnedBytes = m_MaximumPinnedBytesInCache;
	}

	[UnityEngine.Tooltip("Memory pool where data allocated by AK::SoundEngine::PrepareEvent() and AK::SoundEngine::PrepareGameSyncs() will be done.")]
	public int m_PrepareEventMemoryPoolID = AkSoundEngine.AK_INVALID_POOL_ID;

	[UnityEngine.Tooltip("Set to true to enable AK::SoundEngine::PrepareGameSync usage.")]
	public bool m_EnableGameSyncPreparation = false;

	[UnityEngine.Tooltip("Number of quanta ahead when continuous containers should instantiate a new voice before which next sounds should start playing. This look-ahead time allows I/O to occur, and is especially useful to reduce the latency of continuous containers with trigger rate or sample-accurate transitions.")]
	public uint m_ContinuousPlaybackLookAhead = 1;

	[UnityEngine.Tooltip("Size of the monitoring pool. This parameter is not used in Release build.")]
	public uint m_MonitorPoolSize = 256 * 1024;

	[UnityEngine.Tooltip("Size of the monitoring queue pool. This parameter is not used in Release build.")]
	public uint m_MonitorQueuePoolSize = 64 * 1024;

	[UnityEngine.Tooltip("Amount of time to wait for hardware devices to trigger an audio interrupt. If there is no interrupt after that time, the sound engine will revert to silent mode and continue operating until the hardware finally comes back.")]
	public uint m_MaximumHardwareTimeoutMs = 1000;

	public virtual void CopyTo(AkInitSettings settings)
	{
		settings.uPrepareEventMemoryPoolID = m_PrepareEventMemoryPoolID;
		settings.bEnableGameSyncPreparation = m_EnableGameSyncPreparation;
		settings.uContinuousPlaybackLookAhead = m_ContinuousPlaybackLookAhead;
		settings.uMonitorPoolSize = m_MonitorPoolSize;
		settings.uMonitorQueuePoolSize = m_MonitorQueuePoolSize;
		settings.uMaxHardwareTimeoutMs = m_MaximumHardwareTimeoutMs;
	}

	public virtual void CopyTo(AkPlatformInitSettings settings)
	{
	}

	[System.Serializable]
	public class SpatialAudioSettings
	{
        [UnityEngine.Tooltip("Multiplier that is applied to the distance attenuation of diffracted sounds (sounds that are in the 'shadow region') to simulate the phenomenon where by diffracted sound waves decay faster than incident sound waves.")]
        [UnityEngine.Range(1.0f, 3.0f)]
        public float m_DiffractionShadowAttenuationFactor = 2.0f;

		[UnityEngine.Tooltip("Interpolation angle, in degrees, over which the \"Diffraction Shadow Attenuation Factor\" is applied.")]
        [UnityEngine.Range(0.1f, 90.0f)]
        public float m_DiffractionShadowDegrees = 30.0f;
	}

	[UnityEngine.Tooltip("Spatial audio advanced settings.")]
	public SpatialAudioSettings m_SpatialAudioSettings;

	public virtual void CopyTo(AkSpatialAudioInitSettings settings)
	{
        settings.fDiffractionShadowAttenFactor = m_SpatialAudioSettings.m_DiffractionShadowAttenuationFactor;
		settings.fDiffractionShadowDegrees = m_SpatialAudioSettings.m_DiffractionShadowDegrees;
	}

	public virtual void CopyTo(AkUnityPlatformSpecificSettings settings)
	{
	}

    public override void Validate()
    {
        if (m_SpatialAudioSettings.m_DiffractionShadowAttenuationFactor <= 0.0f)
        {
            UnityEngine.Debug.LogWarning("WwiseUnity: m_SpatialAudioSettings.m_DiffractionShadowAttenuationFactor must be greater than zero. Value was reset to the default (2.0)");
            m_SpatialAudioSettings.m_DiffractionShadowAttenuationFactor = 2.0f;
        }

        if (m_SpatialAudioSettings.m_DiffractionShadowDegrees <= 0.0f)
        {
            UnityEngine.Debug.LogWarning("WwiseUnity: m_SpatialAudioSettings.m_DiffractionShadowDegrees must be greater than zero. Value was reset to the default (30.0)");
            m_SpatialAudioSettings.m_DiffractionShadowDegrees = 30.0f;
        }
    }
}

[System.Serializable]
public class AkCommonCommSettings : AkSettingsValidationHandler
{
	[UnityEngine.Tooltip("Size of the communication pool.")]
	public uint m_PoolSize = 256 * 1024;

	public static ushort DefaultDiscoveryBroadcastPort = 24024;

	[UnityEngine.Tooltip("The port where the authoring application broadcasts \"Game Discovery\" requests to discover games running on the network. Default value: 24024. (Cannot be set to 0)")]
	public ushort m_DiscoveryBroadcastPort = DefaultDiscoveryBroadcastPort;

	[UnityEngine.Tooltip("The \"command\" channel port. Set to 0 to request a dynamic/ephemeral port.")]
	public ushort m_CommandPort;

	[UnityEngine.Tooltip("The \"notification\" channel port. Set to 0 to request a dynamic/ephemeral port.")]
	public ushort m_NotificationPort;

	[UnityEngine.Tooltip("Indicates whether the communication system should be initialized. Some consoles have critical requirements for initialization of their communications system. Set to false only if your game already uses sockets before sound engine initialization.")]
	public bool m_InitializeSystemComms = true;

	[UnityEngine.Tooltip("The name used to identify this game within the authoring application. Leave empty to use \"UnityEngine.Application.productName\".")]
	public string m_NetworkName;

	public virtual void CopyTo(AkCommunicationSettings settings)
	{
		settings.uPoolSize = m_PoolSize;
		settings.uDiscoveryBroadcastPort = m_DiscoveryBroadcastPort;
		settings.uCommandPort = m_CommandPort;
		settings.uNotificationPort = m_NotificationPort;
		settings.bInitSystemLib = m_InitializeSystemComms;

		string networkName = m_NetworkName;
		if (string.IsNullOrEmpty(networkName))
			networkName = UnityEngine.Application.productName;

#if UNITY_EDITOR
		networkName += " (Editor)";
#endif

		settings.szAppNetworkName = networkName;
	}
}

public abstract class AkCommonPlatformSettings : AkBasePlatformSettings
{
	protected abstract AkCommonUserSettings GetUserSettings();

	protected abstract AkCommonAdvancedSettings GetAdvancedSettings();

	protected abstract AkCommonCommSettings GetCommsSettings();

	public override AkInitializationSettings AkInitializationSettings
	{
		get
		{
			var settings = base.AkInitializationSettings;
			var userSettings = GetUserSettings();
			userSettings.CopyTo(settings.memSettings);
			userSettings.CopyTo(settings.deviceSettings);
			userSettings.CopyTo(settings.streamMgrSettings);
			userSettings.CopyTo(settings.initSettings);
			userSettings.CopyTo(settings.platformSettings);
			userSettings.CopyTo(settings.musicSettings);
			userSettings.CopyTo(settings.unityPlatformSpecificSettings);
			settings.preparePoolSize = userSettings.m_PreparePoolSize;

			var advancedSettings = GetAdvancedSettings();
			advancedSettings.CopyTo(settings.deviceSettings);
			advancedSettings.CopyTo(settings.initSettings);
			advancedSettings.CopyTo(settings.platformSettings);
			advancedSettings.CopyTo(settings.unityPlatformSpecificSettings);
			return settings;
		}
	}

	public override AkSpatialAudioInitSettings AkSpatialAudioInitSettings
	{
		get
		{
			var settings = base.AkSpatialAudioInitSettings;
			GetUserSettings().CopyTo(settings);
			GetAdvancedSettings().CopyTo(settings);
			return settings;
		}
	}

	public override AkCallbackManager.InitializationSettings CallbackManagerInitializationSettings
	{
		get
		{
			var userSettings = GetUserSettings();
			return new AkCallbackManager.InitializationSettings { BufferSize = userSettings.m_CallbackManagerBufferSize, IsLoggingEnabled = userSettings.m_EngineLogging };
		}
	}

	public override string InitialLanguage
	{
		get
		{
			return GetUserSettings().m_StartupLanguage;
		}
	}

	public override string SoundbankPath
	{
		get
		{
			return GetUserSettings().m_BasePath;
		}
	}

	public override AkCommunicationSettings AkCommunicationSettings
	{
		get
		{
			var settings = base.AkCommunicationSettings;
			GetCommsSettings().CopyTo(settings);
			return settings;
		}
	}

	#region parameter validation
#if UNITY_EDITOR
	void OnValidate()
	{
		GetUserSettings().Validate();
		GetAdvancedSettings().Validate();
		GetCommsSettings().Validate();
	}
#endif
	#endregion
}


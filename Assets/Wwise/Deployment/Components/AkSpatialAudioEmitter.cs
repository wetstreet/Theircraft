#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
[UnityEngine.AddComponentMenu("Wwise/AkSpatialAudioEmitter")]
[UnityEngine.RequireComponent(typeof(AkGameObj))]
///@brief Add this script on the GameObject which represents an emitter that uses the Spatial Audio API.
public class AkSpatialAudioEmitter : AkSpatialAudioBase
{
	[UnityEngine.Header("Early Reflections")]
	[UnityEngine.Tooltip("The Auxiliary Bus with a Reflect plug-in Effect applied.")]
	/// The Auxiliary Bus with a Reflect plug-in Effect applied.
	public AK.Wwise.AuxBus reflectAuxBus = new AK.Wwise.AuxBus();

	[UnityEngine.Tooltip("A heuristic to stop the computation of reflections. Should be no longer (and possibly shorter for less CPU usage) than the maximum attenuation of the sound emitter.")]
	/// A heuristic to stop the computation of reflections. Should be no longer (and possibly shorter for less CPU usage) than the maximum attenuation of the sound emitter.
	public float reflectionMaxPathLength = 1000;

	[UnityEngine.Range(0, 1)]
	[UnityEngine.Tooltip("The gain [0, 1] applied to the reflect auxiliary bus.")]
	/// The gain [0, 1] applied to the reflect auxiliary bus.
	public float reflectionsAuxBusGain = 1;

	[UnityEngine.Range(1, 4)]
	[UnityEngine.Tooltip("The maximum number of reflections that will be processed for a sound path before it reaches the listener.")]
	/// The maximum number of reflections that will be processed for a sound path before it reaches the listener.
	/// Reflection processing grows exponentially with the order of reflections, so this number should be kept low. Valid range: 1-4.
	public uint reflectionsOrder = 1;

	[UnityEngine.Header("Rooms")] [UnityEngine.Range(0, 1)]
	[UnityEngine.Tooltip("Send gain (0.f-1.f) that is applied when sending to the aux bus associated with the room that the emitter is in.")]
	/// Send gain (0.f-1.f) that is applied when sending to the aux bus associated with the room that the emitter is in.
	public float roomReverbAuxBusGain = 1;

	[UnityEngine.Header("Geometric Diffraction")]
	[UnityEngine.Tooltip("The maximum number of edges that the sound can diffract around between the emitter and the listener.")]
	/// The maximum number of edges that the sound can diffract around between the emitter and the listener.
	public uint diffractionMaxEdges = 0;

	[UnityEngine.Tooltip("The maximum number of paths to the listener that the sound can take around obstacles.")]
	/// The maximum number of paths to the listener that the sound can take around obstacles.
	public uint diffractionMaxPaths = 0;

	[UnityEngine.Tooltip("The maximum length that a diffracted sound can travel.")]
	/// The maximum length that a diffracted sound can travel. Should be no longer (and possibly shorter for less CPU usage) than the maximum attenuation of the sound emitter.
	public uint diffractionMaxPathLength = 0;

	private void OnEnable()
	{
		var emitterSettings = new AkEmitterSettings();

		emitterSettings.reflectAuxBusID = reflectAuxBus.Id;
		emitterSettings.reflectionMaxPathLength = reflectionMaxPathLength;
		emitterSettings.reflectionsAuxBusGain = reflectionsAuxBusGain;
		emitterSettings.reflectionsOrder = reflectionsOrder;
		emitterSettings.reflectorFilterMask = unchecked((uint)-1);
		emitterSettings.roomReverbAuxBusGain = roomReverbAuxBusGain;
		emitterSettings.useImageSources = 0;
		emitterSettings.diffractionMaxEdges = diffractionMaxEdges;
		emitterSettings.diffractionMaxPaths = diffractionMaxPaths;
		emitterSettings.diffractionMaxPathLength = diffractionMaxPathLength;

		if (AkSoundEngine.RegisterEmitter(gameObject, emitterSettings) == AKRESULT.AK_Success)
			SetGameObjectInRoom();
	}

	private void OnDisable()
	{
		AkSoundEngine.UnregisterEmitter(gameObject);
	}

#if UNITY_EDITOR
	[UnityEngine.Header("Debug Draw")]

	/// This allows you to visualize first order reflection sound paths.
	public bool drawFirstOrderReflections = false;

	/// This allows you to visualize second order reflection sound paths.
	public bool drawSecondOrderReflections = false;

	/// This allows you to visualize third or higher order reflection sound paths.
	public bool drawHigherOrderReflections = false;

	/// This allows you to visualize geometric diffraction sound paths between an obstructed emitter and the listener.
	public bool drawDiffractionPaths = false;

	private void OnDrawGizmos()
	{
		if (!UnityEngine.Application.isPlaying || !AkSoundEngine.IsInitialized())
			return;

		if (debugDrawData == null)
			debugDrawData = new DebugDrawData();

		if (drawFirstOrderReflections || drawSecondOrderReflections || drawHigherOrderReflections)
			debugDrawData.DebugDrawEarlyReflections(gameObject, drawFirstOrderReflections, drawSecondOrderReflections, drawHigherOrderReflections);

		if (drawDiffractionPaths)
			debugDrawData.DebugDrawDiffraction(gameObject);
	}

	private class DebugDrawData
	{
		// Constants
		private const uint kMaxIndirectPaths = 64;
		private readonly UnityEngine.Color32 colorLightYellow = new UnityEngine.Color32(255, 255, 121, 255);
		private readonly UnityEngine.Color32 colorDarkYellow = new UnityEngine.Color32(164, 164, 0, 255);
		private readonly UnityEngine.Color32 colorLightOrange = new UnityEngine.Color32(255, 202, 79, 255);
		private readonly UnityEngine.Color32 colorDarkOrange = new UnityEngine.Color32(164, 115, 0, 255);
		private readonly UnityEngine.Color32 colorLightRed = new UnityEngine.Color32(252, 177, 162, 255);
		private readonly UnityEngine.Color32 colorDarkRed = new UnityEngine.Color32(169, 62, 39, 255);
		private readonly UnityEngine.Color32 colorLightGrey = new UnityEngine.Color32(75, 75, 75, 255);
		private readonly UnityEngine.Color32 colorGreen = new UnityEngine.Color32(38, 113, 88, 255);
		private const float radiusSphere = 0.25f;

		// Calculated path info
		private readonly AkReflectionPathInfoArray indirectPathInfoArray = new AkReflectionPathInfoArray((int)kMaxIndirectPaths);
		private readonly AkDiffractionPathInfoArray diffractionPathInfoArray = new AkDiffractionPathInfoArray((int)AkDiffractionPathInfo.kMaxNodes);
		private readonly AkPathParams pathsParams = new AkPathParams();

		public void DebugDrawEarlyReflections(UnityEngine.GameObject gameObject, bool firstOrder, bool secondOrder, bool higherOrder)
		{
			if (AkSoundEngine.QueryIndirectPaths(gameObject, pathsParams, indirectPathInfoArray, (uint)indirectPathInfoArray.Count()) != AKRESULT.AK_Success)
				return;

			for (var idxPath = (int)pathsParams.numValidPaths - 1; idxPath >= 0; --idxPath)
			{
				var path = indirectPathInfoArray[idxPath];
				var order = path.numReflections;

				var colorLight = colorLightRed;
				var colorDark = colorDarkRed;

				if (order == 1)
				{
					if (!firstOrder)
						continue;

					colorLight = colorLightYellow;
					colorDark = colorDarkYellow;
				}
				else if (order == 2)
				{
					if (!secondOrder)
						continue;

					colorLight = colorLightOrange;
					colorDark = colorDarkOrange;
				}
				else if (order > 2 && !higherOrder)
					continue;

				var emitterPos = ConvertVector(pathsParams.emitterPos);
				var listenerPt = ConvertVector(pathsParams.listenerPos);

				for (var idxSeg = (int)path.numPathPoints - 1; idxSeg >= 0; --idxSeg)
				{
					var pt = ConvertVector(path.GetPathPoint((uint)idxSeg));

					UnityEngine.Debug.DrawLine(listenerPt, pt, path.isOccluded ? colorLightGrey : colorLight);

					UnityEngine.Gizmos.color = path.isOccluded ? colorLightGrey : colorLight;
					UnityEngine.Gizmos.DrawWireSphere(pt, radiusSphere / 2 / order);

					if (!path.isOccluded)
					{
						var surface = path.GetAcousticSurface((uint)idxSeg);
						DrawLabelInFrontOfCam(pt, surface.strName, 100000, colorDark);
					}

					float dfrnAmount = path.GetDiffraction((uint)idxSeg);
					if (dfrnAmount > 0)
					{
						string dfrnAmountStr = dfrnAmount.ToString("0.#%");
						DrawLabelInFrontOfCam(pt, dfrnAmountStr, 100000, colorDark);
					}

					listenerPt = pt;
				}

				if (!path.isOccluded)
				{
					// Finally the last path segment towards the emitter.
					UnityEngine.Debug.DrawLine(listenerPt, emitterPos, path.isOccluded ? colorLightGrey : colorLight);
				}
			}
		}

		public void DebugDrawDiffraction(UnityEngine.GameObject gameObject)
		{
			if (AkSoundEngine.QueryDiffractionPaths(gameObject, pathsParams, diffractionPathInfoArray, (uint)diffractionPathInfoArray.Count()) != AKRESULT.AK_Success)
				return;

			for (var idxPath = (int)pathsParams.numValidPaths - 1; idxPath >= 0; --idxPath)
			{
				var path = diffractionPathInfoArray[idxPath];
				if (path.nodeCount <= 0)
					continue;

				var emitterPos = ConvertVector(pathsParams.emitterPos);
				var prevPt = ConvertVector(pathsParams.listenerPos);

				for (var idxSeg = 0; idxSeg < (int)path.nodeCount; ++idxSeg)
				{
					var pt = ConvertVector(path.GetNodes((uint)idxSeg));
					UnityEngine.Debug.DrawLine(prevPt, pt, colorGreen);

					float angle = path.GetAngles((uint)idxSeg) / UnityEngine.Mathf.PI;
					if (angle > 0)
					{
						string angleStr = angle.ToString("0.#%");
						DrawLabelInFrontOfCam(pt, angleStr, 100000, colorGreen);
					}

					prevPt = pt;
				}

				UnityEngine.Debug.DrawLine(prevPt, emitterPos, colorGreen);
			}
		}
	}

	private static DebugDrawData debugDrawData = null;

	private static UnityEngine.Vector3 ConvertVector(AkVector vec)
	{
		return new UnityEngine.Vector3(vec.X, vec.Y, vec.Z);
	}

	private static void DrawLabelInFrontOfCam(UnityEngine.Vector3 position, string name, float distance, UnityEngine.Color c)
	{
		var style = new UnityEngine.GUIStyle();
		var oncam = UnityEngine.Camera.current.WorldToScreenPoint(position);

		if (oncam.x >= 0 && oncam.x <= UnityEngine.Camera.current.pixelWidth && oncam.y >= 0 &&
			oncam.y <= UnityEngine.Camera.current.pixelHeight && oncam.z > 0 && oncam.z < distance)
		{
			style.normal.textColor = c;
			UnityEditor.Handles.Label(position, name, style);
		}
	}
#endif
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
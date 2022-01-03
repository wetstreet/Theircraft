using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class UISystem : MonoBehaviour
{
    static GameObject canvas;
    public static Camera camera;
    static CanvasScaler canvasScaler;

    public static int scale
    {
        get
        {
            if (Screen.width >= 1920 && Screen.height >= 1440)
            {
                return 6;
            }
            else if (Screen.width >= 1600 && Screen.height >= 1200)
            {
                return 5;
            }
            else if (Screen.width >= 1280 && Screen.height >= 960)
            {
                return 4;
            }
            else if (Screen.width >= 960 && Screen.height >= 720)
            {
                return 3;
            }
            else if (Screen.width >= 640 && Screen.height >= 480)
            {
                return 2;
            }
            else
            {
                return 1;
            }
        }
    }

    static int lastScale;
    static void UpdateScale()
    {
        if (lastScale != scale)
        {
            lastScale = scale;
            canvasScaler.scaleFactor = scale;
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        InitCamera();
    }

    private void InitCamera()
    {
        UniversalAdditionalCameraData camData = camera.gameObject.AddMissingComponent<UniversalAdditionalCameraData>();
        camData.renderType = CameraRenderType.Overlay;
        UniversalAdditionalCameraData mainCamData = Camera.main.gameObject.AddMissingComponent<UniversalAdditionalCameraData>();
        mainCamData.cameraStack.Add(camera);
    }

    private void Awake()
    {
        lastScale = 0;
        canvas = transform.Find("Canvas").gameObject;
        camera = transform.Find("Camera").GetComponent<Camera>();

        canvasScaler = canvas.GetComponent<CanvasScaler>();
        UpdateScale();

        InitCamera();
    }

    private void Update()
    {
        UpdateScale();
    }

    static void InitializeCanvas()
    {
        GameObject prefab = Resources.Load("Prefabs/UI/ui_root") as GameObject;
        GameObject uiroot = Instantiate(prefab);
        uiroot.AddComponent<UISystem>();
        if (Application.isPlaying)
        {
            DontDestroyOnLoad(uiroot);
        }
    }

    public static void DestroyUIRoot()
    {
        if (canvas != null)
        {
            DestroyImmediate(canvas.transform.parent.gameObject);
            canvas = null;
        }
    }

    public static GameObject InstantiateUI(string name)
    {
        GameObject prefab = Resources.Load("Prefabs/UI/" + name) as GameObject;
        GameObject uiobj = Instantiate(prefab);
        return InstantiateUI(uiobj);
    }

    public static GameObject InstantiateUI(GameObject uiobj)
    {
        if (canvas == null)
        {
            canvas = GameObject.Find("Canvas");
            if (canvas == null)
            {
                InitializeCanvas();
            }
        }
        uiobj.transform.SetParent(canvas.transform);
        uiobj.transform.localScale = Vector3.one;

        if (LoadingUI.isLoading)
        {
            LoadingUI.SetOnTop();
        }

        RectTransform rt = uiobj.GetComponent<RectTransform>();
        rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
        rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 0);
        rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
        rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, 0);
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        return uiobj;
    }
}

using protocol.cs_theircraft;
using TMPro;
using UnityEngine;

public class DebugUI : MonoBehaviour
{

    static DebugUI Instance;
    public static void Show()
    {
        if (Instance != null)
        {
            Instance.gameObject.SetActive(true);
        }
        else
        {
            Instance = UISystem.InstantiateUI("DebugUI").GetComponent<DebugUI>();
        }
    }

    public static void Hide()
    {
        if (Instance != null)
        {
            Instance.gameObject.SetActive(false);
        }
    }

    TextMeshProUGUI label;
    // Start is called before the first frame update
    void Start()
    {
        label = transform.Find("Text").GetComponent<TextMeshProUGUI>();
    }

    int frameCount;
    float timeStamp;
    int fps;
    int GetFPS()
    {
        frameCount++;
        float curTime = Time.unscaledTime;
        if (curTime - timeStamp >= 1)
        {
            fps = frameCount;
            frameCount = 0;
            timeStamp = curTime;
        }
        return fps;
    }

    static readonly string version = "v0.1.0";

    // Update is called once per frame
    void Update()
    {
        string text = "Theircraft " + version;
        text += "\n" + GetFPS() + " fps";

        Vector3 pos = PlayerController.instance.transform.position;
        text += string.Format("\nXYZ: {0:0.000} / {1:0.000} / {2:0.000}", pos.x, pos.y, pos.z);
        Vector3Int curBlock = PlayerController.GetCurrentBlock();
        text += string.Format("\nBlock: {0} {1} {2}", curBlock.x, curBlock.y, curBlock.z);
        Vector2Int curChunk = PlayerController.GetCurrentChunk();
        text += string.Format("\nChunk: {0} {1}", curChunk.x, curChunk.y); 
        if (WireFrameHelper.render)
        {
            text += string.Format("\nLooking at: {0} {1} {2}", WireFrameHelper.pos.x, WireFrameHelper.pos.y, WireFrameHelper.pos.z);

            CSBlockType type = ChunkManager.GetBlockType(WireFrameHelper.pos);
            text += string.Format("\nType: {0}", type);
            if (type == CSBlockType.BrickStairs)
            {
                CSBlockOrientation orient = ChunkManager.GetBlockOrientation(WireFrameHelper.pos);
                text += string.Format("\nOrient: {0}", orient);
            }
        }
        label.text = text;
    }

    public static void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            if (Instance != null && Instance.gameObject.activeSelf)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }
    }
}

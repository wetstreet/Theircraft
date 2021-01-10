using protocol.cs_theircraft;
using TMPro;
using UnityEngine;
using System.Text;

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

    StringBuilder sb = new StringBuilder();

    // Update is called once per frame
    void Update()
    {
        sb.Clear();
        sb.Append("Theircraft ");
        sb.Append(Application.version);
        sb.Append("\n");
        sb.Append(GetFPS());
        sb.Append(" fps");

        Vector3 pos = PlayerController.instance.transform.position;
        sb.Append("\nXYZ: ");
        sb.Append(pos.x);
        sb.Append(" / ");
        sb.Append(pos.y);
        sb.Append(" / ");
        sb.Append(pos.z);

        Vector3Int curBlock = PlayerController.GetCurrentBlock();
        sb.Append("\nBlock: ");
        sb.Append(curBlock.x);
        sb.Append(" ");
        sb.Append(curBlock.y);
        sb.Append(" ");
        sb.Append(curBlock.z);

        int chunkX = Mathf.FloorToInt(curBlock.x / 16f);
        int chunkY = Mathf.FloorToInt(curBlock.y / 16f);
        int chunkZ = Mathf.FloorToInt(curBlock.z / 16f);
        int xInChunk = curBlock.x - chunkX * 16;
        int yInChunk = curBlock.y - chunkY * 16;
        int zInChunk = curBlock.z - chunkZ * 16;
        sb.Append("\nChunk: ");
        sb.Append(xInChunk);
        sb.Append(" ");
        sb.Append(yInChunk);
        sb.Append(" ");
        sb.Append(zInChunk);
        sb.Append(" in ");
        sb.Append(chunkX);
        sb.Append(" ");
        sb.Append(chunkY);
        sb.Append(" ");
        sb.Append(chunkZ);
        
        if (WireFrameHelper.render)
        {
            sb.Append("\nLooking at: ");
            sb.Append(WireFrameHelper.pos.x);
            sb.Append(" ");
            sb.Append(WireFrameHelper.pos.y);
            sb.Append(" ");
            sb.Append(WireFrameHelper.pos.z);
            
            sb.Append("\nType: ");
            NBTBlock generator = NBTGeneratorManager.GetMeshGenerator(WireFrameHelper.type);
            sb.Append(generator.name);

            sb.Append("\nData: ");
            sb.Append(WireFrameHelper.data);
        }

        label.text = sb.ToString();
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

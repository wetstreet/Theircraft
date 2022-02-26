using TMPro;
using UnityEngine;
using System.Text;
using System.Reflection;
using GameFramework;

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
    TextMeshProUGUI label2;
    // Start is called before the first frame update
    void Start()
    {
        label = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        label2 = transform.Find("topright").GetComponent<TextMeshProUGUI>();
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

    string template = "Theircraft {0}\n" +
        "{1} fps\n" +
        "XYZ: {2} / {3} / {4}\n" +
        "Block: {5} {6} {7}\n";
    string template2 = "Chunk: {0} {1} {2} in {3} {4} {5}\n" +
        "Light: {6} ({7} sky, {8} block)\n";
    string template3 = "Looking at: {0} {1} {2}\n" +
        "Type: {3}\n" +
        "Data: {4}";

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = PlayerController.instance.transform.position;
        Vector3Int curBlock = PlayerController.GetCurrentBlock();

        int chunkX = Mathf.FloorToInt(curBlock.x / 16f);
        int chunkY = Mathf.FloorToInt(curBlock.y / 16f);
        int chunkZ = Mathf.FloorToInt(curBlock.z / 16f);
        int xInChunk = curBlock.x - chunkX * 16;
        int yInChunk = curBlock.y - chunkY * 16;
        int zInChunk = curBlock.z - chunkZ * 16;

        Vector3Int posInt = pos.ToVector3Int();
        NBTHelper.GetLightsByte(posInt.x, posInt.y, posInt.z, out byte skyLight, out byte blockLight);
        byte maxLight = skyLight > blockLight ? skyLight : blockLight;

        UnityEngine.Profiling.Profiler.BeginSample("zstring");
        using (zstring.Block())
        {
            zstring text = zstring.Format(template, Application.version, GetFPS(), pos.x, pos.y, pos.z, curBlock.x, curBlock.y, curBlock.z);
            text += zstring.Format(template2, xInChunk, yInChunk, zInChunk, chunkX, chunkY, chunkZ, maxLight, skyLight, blockLight);
            if (WireFrameHelper.render)
            {
                text += zstring.Format(template3, WireFrameHelper.pos.x, WireFrameHelper.pos.y, WireFrameHelper.pos.z, WireFrameHelper.generator.name, WireFrameHelper.data);
            }
            label.text = text;
            label.ForceMeshUpdate();

            label2.text = ChunkRefresher.GetChunkUpdatesCount() + (zstring)" chunk updates";
        }
        UnityEngine.Profiling.Profiler.EndSample();
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

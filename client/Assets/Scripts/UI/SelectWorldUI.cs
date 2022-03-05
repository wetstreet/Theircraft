using Substrate.Core;
using Substrate.Nbt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Label = TMPro.TextMeshProUGUI;

public class SelectWorldUI : MonoBehaviour
{
    static SelectWorldUI instance;

    public static void Show()
    {
        instance = UISystem.InstantiateUI("SelectWorldUI").GetComponent<SelectWorldUI>();
    }

    public static void Close()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
            instance = null;
        }
    }

    struct Item
    {
        public GameObject go;
        public RawImage icon;
        public Label name;
        public string path;
        public Label time;
        public Label mode;
        public GameObject select;
    }

    List<Item> items = new List<Item>();

    Transform unit;
    GraphicRaycaster gr;
    int curSelectIndex = -1;

    Button playButton;
    Button deleteButton;

    // Start is called before the first frame update
    void Start()
    {
        Utilities.SetClickCallback(transform, "btnGrid/ButtonPlay", Play);
        Utilities.SetClickCallback(transform, "btnGrid/ButtonCreate", CreateNewWorld);
        Utilities.SetClickCallback(transform, "btnGrid/ButtonDelete", OnClickDelete);
        Utilities.SetClickCallback(transform, "btnGrid/ButtonCancel", OnClickCancel);

        playButton = transform.Find("btnGrid/ButtonPlay").GetComponent<Button>();
        deleteButton = transform.Find("btnGrid/ButtonDelete").GetComponent<Button>();

        gr = gameObject.AddComponent<GraphicRaycaster>();

        unit = transform.Find("Scroll View/Viewport/unit");
        unit.gameObject.SetActive(false);

        RefreshUI();
    }

    private void Update()
    {
        HandleMouseOperation();
    }

    void RefreshUI()
    {
        string savesDirName = Path.Combine(Application.persistentDataPath, "saves");
        DirectoryInfo dir = new DirectoryInfo(savesDirName);

        DirectoryInfo[] dirs = dir.GetDirectories();
        for (int i = 0; i < dirs.Length; i++)
        {
            DirectoryInfo subdir = dirs[i];
            if (i >= items.Count)
            {
                Transform unitTrans = Instantiate(unit);
                unitTrans.name = i.ToString();
                unitTrans.SetParent(unit.parent);
                unitTrans.localScale = Vector3.one;
                Item item = new Item();
                item.go = unitTrans.gameObject;
                item.icon = unitTrans.Find("icon").GetComponent<RawImage>();
                item.name = unitTrans.Find("name").GetComponent<Label>();
                item.time = unitTrans.Find("time").GetComponent<Label>();
                item.mode = unitTrans.Find("mode").GetComponent<Label>();
                item.select = unitTrans.Find("select").gameObject;

                item.path = subdir.Name;

                NBTFile levelFile = new NBTFile(Path.Combine(subdir.FullName, "level.dat"));
                NbtTree levelTree = new NbtTree();
                using (Stream stream = levelFile.GetDataInputStream())
                {
                    levelTree.ReadFrom(stream);
                }
                TagNodeCompound levelDat = levelTree.Root["Data"] as TagNodeCompound;
                TagNodeString name = levelDat["LevelName"] as TagNodeString;

                item.go.SetActive(true);
                item.name.text = name;

                TagNodeLong time = levelDat["LastPlayed"] as TagNodeLong;
                DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                DateTime dateTime = startTime.AddMilliseconds(time.Data);
                item.time.text = item.path + " (" + dateTime + ")";

                TagNodeInt mode = levelDat["GameType"] as TagNodeInt;
                item.mode.text = mode == 0 ? "Survival Mode" : "Creative Mode";

                byte[] bytes = File.ReadAllBytes(Path.Combine(subdir.FullName, "icon.png"));
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(bytes);
                texture.filterMode = FilterMode.Point;
                item.icon.texture = texture;

                items.Add(item);
            }
            items[i].select.SetActive(i == curSelectIndex);
        }

        playButton.interactable = curSelectIndex != -1;
        deleteButton.interactable = curSelectIndex != -1;
    }

    PointerEventData ped = new PointerEventData(null);
    List<RaycastResult> results = new List<RaycastResult>();
    void HandleMouseOperation()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ped.position = Input.mousePosition;
            results.Clear();
            gr.Raycast(ped, results);
            if (int.TryParse(results[0].gameObject.name, out int index))
            {
                curSelectIndex = index;
                RefreshUI();
            }
        }
    }

    void Play()
    {
        NBTHelper.save = items[curSelectIndex].path;
        NBTHelper.Init();

        DataCenter.name = "Steve";
        DataCenter.spawnPosition = NBTHelper.GetPlayerPos();
        DataCenter.spawnRotation = NBTHelper.GetPlayerRot();
        MainMenu.Close();
        Close();
        LoadingUI.Show();
        SceneManager.LoadScene("GameScene");

        //ChatPanel.AddLine(DataCenter.name + ", welcome!");
    }

    void CreateNewWorld()
    {
        string source = Path.Combine(Application.streamingAssetsPath, "saves", "New World1");
        string destination = Path.Combine(Application.persistentDataPath, "saves", "New World1");
        if (!Directory.Exists(destination))
        {
            DirectoryCopy(source, destination, true);
        }
        RefreshUI();
    }

    void OnClickCancel()
    {
        Close();
    }

    void OnClickDelete()
    {
        string destination = Path.Combine(Application.persistentDataPath, "saves", items[curSelectIndex].path);
        Directory.Delete(destination, true);

        Destroy(items[curSelectIndex].go);
        items.RemoveAt(curSelectIndex);

        curSelectIndex = -1;
    }

    private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
    {
        // Get the subdirectories for the specified directory.
        DirectoryInfo dir = new DirectoryInfo(sourceDirName);

        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException(
                "Source directory does not exist or could not be found: "
                + sourceDirName);
        }

        DirectoryInfo[] dirs = dir.GetDirectories();

        // If the destination directory doesn't exist, create it.       
        Directory.CreateDirectory(destDirName);

        // Get the files in the directory and copy them to the new location.
        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files)
        {
            string tempPath = Path.Combine(destDirName, file.Name);
            if (file.Extension == ".meta")
            {
                continue;
            }
            file.CopyTo(tempPath, false);
        }

        // If copying subdirectories, copy them and their contents to new location.
        if (copySubDirs)
        {
            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
            }
        }
    }
}

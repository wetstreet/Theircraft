using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using protocol.cs_enum;
using protocol.cs_theircraft;

public class ChatPanel : MonoBehaviour
{
    List<Transform> ItemList = new List<Transform>();
    Transform itemUnit;
    GridLayoutGroup grid;
    GameObject inputParent;
    InputField inputField;

    static Queue<string> _message = new Queue<string>();

    static ChatPanel instance;

    public static void ShowChatPanel()
    {
        if (instance != null)
        {
            instance.gameObject.SetActive(true);
        }
        else
            instance = UISystem.InstantiateUI("ChatPanel").GetComponent<ChatPanel>();
    }

    public static void Hide()
    {
        if (instance != null)
        {
            instance.gameObject.SetActive(false);
        }
    }

    static bool initialized;

    public static void AddLine(string content)
    {
        if (initialized)
        {
            instance.AddLineItem(content);
        }
        else
        {
            _message.Enqueue(content);
        }
    }

    // Use this for initialization
    void Start ()
    {
        initialized = true;
        NetworkManager.Register(ENUM_CMD.CS_SEND_MESSAGE_RES, OnSendMessageRes);
        NetworkManager.Register(ENUM_CMD.CS_MESSAGE_NOTIFY, OnMessageNotify);

        itemUnit = transform.Find("container/Scroll View/Viewport/Content/chat_item");
        itemUnit.gameObject.SetActive(false);
        grid = transform.Find("container/Scroll View/Viewport/Content").GetComponent<GridLayoutGroup>();

        inputParent = transform.Find("container/InputParent").gameObject;
        inputParent.SetActive(false);
        transform.Find("container/InputParent/SendButton").GetComponent<Button>().onClick.AddListener(OnClickSendButton);
        inputField = transform.Find("container/InputParent/InputField").GetComponent<InputField>();

        while (_message.Count > 0)
        {
            string content = _message.Dequeue();
            AddLineItem(content);
        }
    }

    void ShowInput()
    {
        inputParent.SetActive(true);
        inputField.ActivateInputField();
        PlayerController.LockCursor(false);
    }

    void HideInput()
    {
        inputField.DeactivateInputField();
        PlayerController.LockCursor(true);
        inputParent.SetActive(false);
    }
    
    void ProcessInput()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (inputParent.activeSelf)
            {
                if (inputField.text != "")
                {
                    OnClickSendButton();
                }
                else
                {
                    HideInput();
                }
            }
            else
            {
                ShowInput();
            }
        }
    }

    static bool scrollToBottom;
    void Update()
    {
        ProcessInput();
        if (scrollToBottom)
        {
            grid.transform.localPosition = new Vector3(0, 20 * (ItemList.Count - 10), 0);
            scrollToBottom = false;
        }
    }

    void OnDestroy()
    {
        initialized = false;
        instance = null;
        ItemList.Clear();
    }

    void AddLineItem(string content)
    {
        Transform item = Instantiate(itemUnit);
        item.gameObject.SetActive(true);
        item.SetParent(itemUnit.parent);
        item.localScale = Vector3.one;
        Text text = item.GetComponent<Text>();
        text.text = content;

        ItemList.Add(item);
        if (ItemList.Count > 10)
        {
            scrollToBottom = true;
        }
    }

    void OnClickSendButton()
    {
        if (inputField.text != "")
        {
            if (inputField.text == "@test")
            {
                // process gm
                GM.Test();

                inputField.text = "";
                inputField.ActivateInputField();
                HideInput();
            }
            else
                SendMessageReq(inputField.text);
        }
        else
        {
            FastTips.Show(1);
        }
    }

    static void SendMessageReq(string content)
    {
        CSSendMessageReq req = new CSSendMessageReq
        {
            Content = content
        };
        NetworkManager.Enqueue(ENUM_CMD.CS_SEND_MESSAGE_REQ, req);
    }

    void OnSendMessageRes(byte[] data)
    {
        CSSendMessageRes rsp = NetworkManager.Deserialize<CSSendMessageRes>(data);
        //Debug.Log("OnSendMessageRes,retCode=" + res.RetCode);
        if (rsp.RetCode == 0)
        {
            inputField.text = "";
            inputField.ActivateInputField();
            HideInput();
        }
        else
        {
            FastTips.Show(rsp.RetCode);
        }
    }
    static void OnMessageNotify(byte[] data)
    {
        CSMessageNotify notify = NetworkManager.Deserialize<CSMessageNotify>(data);
        //Debug.Log($"OnMessageNotify,name={notify.Name},content={notify.Content}");
        AddLine("[" + notify.Name + "]" + notify.Content);
    }
}

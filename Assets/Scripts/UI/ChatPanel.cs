using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using ChatRoom;

public class ChatPanel : MonoBehaviour
{
    static List<Transform> ItemList = new List<Transform>();
    static Transform itemUnit;
    static GridLayoutGroup grid;

    static Queue<string> _message = new Queue<string>();

    static InputField inputField;

    static GameObject instance;

    public static void ShowChatPanel()
    {
        if (instance != null)
        {
            instance.SetActive(true);
        }
        else
            instance = UISystem.InstantiateUI("ChatPanel");
    }

    public static void Hide()
    {
        if (instance != null)
        {
            instance.SetActive(false);
        }
    }

    static bool initialized;

    public static void AddLine(string content)
    {
        if (initialized)
        {
            AddLineItem(content);
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
        NetworkManager.Register(MessageType.SEND_MEESAGE_RES, OnSendMessageRes);
        NetworkManager.Register(MessageType.MESSAGE_NOTIFY, OnMessageNotify);

        itemUnit = transform.Find("Scroll View/Viewport/Content/chat_item");
        itemUnit.gameObject.SetActive(false);
        grid = transform.Find("Scroll View/Viewport/Content").GetComponent<GridLayoutGroup>();

        transform.Find("SendButton").GetComponent<Button>().onClick.AddListener(OnClickSendButton);
        inputField = transform.Find("InputField").GetComponent<InputField>();


        while (_message.Count > 0)
        {
            string content = _message.Dequeue();
            AddLineItem(content);
        }
    }

    // Update is called once per frame

    bool isFocused;
    void ProcessInput()
    {
        //if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        //{
        //    if (isFocused)
        //    {
        //        if (inputField.text != "")
        //        {
        //            OnClickSendButton();
        //        }
        //        else
        //            inputField.DeactivateInputField();
        //    }
        //    else
        //        inputField.ActivateInputField();
        //}
        //isFocused = inputField.isFocused;

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (inputField.text != "")
            {
                OnClickSendButton();
            }
            else
            {
                gameObject.SetActive(false);
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

    static void AddLineItem(string content)
    {
        Transform item = Instantiate(itemUnit);
        item.gameObject.SetActive(true);
        item.SetParent(itemUnit.parent);
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
            SendMessageReq(inputField.text);
        }
        else
        {
            FastTips.Show(1);
        }
    }

    static void SendMessageReq(string content)
    {
        SendMessageReq req = new SendMessageReq
        {
            Content = content
        };
        NetworkManager.Enqueue(MessageType.SEND_MESSAGE_REQ, req);
    }

    static void OnSendMessageRes(byte[] data)
    {
        SendMessageRes rsp = NetworkManager.Deserialzie<SendMessageRes>(data);
        //Debug.Log("OnSendMessageRes,retCode=" + res.RetCode);
        if (rsp.RetCode == 0)
        {
            inputField.text = "";
            inputField.ActivateInputField();
        }
        else
        {
            FastTips.Show(rsp.RetCode);
        }
    }
    static void OnMessageNotify(byte[] data)
    {
        MessageNotify notify = NetworkManager.Deserialzie<MessageNotify>(data);
        //Debug.Log($"OnMessageNotify,name={notify.Name},content={notify.Content}");
        AddLine("[" + notify.Name + "]" + notify.Content);
    }
}

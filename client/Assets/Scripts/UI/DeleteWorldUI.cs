using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeleteWorldUI : MonoBehaviour
{
    static DeleteWorldUI instance;

    // Start is called before the first frame update
    void Start()
    {
        Utilities.SetClickCallback(transform, "btnGrid/ButtonDelete", OnClickDelete);
        Utilities.SetClickCallback(transform, "btnGrid/ButtonCancel", OnClickCancel);
        
        TextMeshProUGUI text = transform.Find("label").GetComponent<TextMeshProUGUI>();
        text.text = "Local World " + LocalizationManager.GetText("selectWorld.deleteWarning");
    }

    public static void Show()
    {
        instance = UISystem.InstantiateUI("DeleteWorldUI").GetComponent<DeleteWorldUI>();
    }

    public static void Close()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
            instance = null;
        }
    }

    void OnClickCancel()
    {
        Close();
    }

    void OnClickDelete()
    {
        //FastTips.Show("Clear Map Data & Player Data Done!");
        Close();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteWorldUI : MonoBehaviour
{
    static DeleteWorldUI instance;

    // Start is called before the first frame update
    void Start()
    {
        Utilities.SetClickCallback(transform, "btnGrid/ButtonDelete", OnClickDelete);
        Utilities.SetClickCallback(transform, "btnGrid/ButtonCancel", OnClickCancel);
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
        LocalServer.ClearData();
        FastTips.Show("Clear Map Data & Player Data Done!");
        Close();
    }
}

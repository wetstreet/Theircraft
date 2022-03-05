using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LocalizationReplacer : MonoBehaviour
{
    public string key;

    TextMeshProUGUI label;

    // Start is called before the first frame update
    void Start()
    {
        label = GetComponent<TextMeshProUGUI>();

        LocalizationManager.Add(this);

        Refresh();
    }

    private void OnDestroy()
    {
        LocalizationManager.Remove(this);
    }

    public void Refresh()
    {
        if (label != null && !string.IsNullOrEmpty(key))
        {
            label.text = LocalizationManager.GetText(key);
            label.ForceMeshUpdate();
        }
    }
}

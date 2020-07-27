using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScreenshotClickHelper : MonoBehaviour
{
    TextMeshProUGUI label;

    // Start is called before the first frame update
    void Start()
    {
        label = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var wordIndex = TMP_TextUtilities.FindIntersectingWord(label, Input.mousePosition, UISystem.camera);
            
            if (wordIndex >= 3)
            {
                int startIndex = label.text.IndexOf("<u>") + 3;
                int endIndex = label.text.IndexOf("</u>");
                string fileName = label.text.Substring(startIndex, endIndex - startIndex);

                string path = Utilities.screenshotDir + fileName;
                System.Diagnostics.Process.Start(path);
            }
        }
    }
}

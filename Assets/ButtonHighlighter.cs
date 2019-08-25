using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHighlighter : MonoBehaviour ,IPointerEnterHandler ,IPointerExitHandler
{
    public Color normalColor = new Color(224 / 255f, 224 / 255f, 224 / 255f);
    public Color highlightColor = new Color(255 / 255f, 255 / 255f, 160 / 255f);
    public TextMeshProUGUI label;
    Button button;

    void Start()
    {
        button = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (label != null && button != null && button.interactable)
        {
            label.color = highlightColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (label != null && button != null && button.interactable)
        {
            label.color = normalColor;
        }
    }
}

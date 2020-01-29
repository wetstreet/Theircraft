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
    Image image;

    void Start()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        bool buttonEnabled = button != null && button.interactable;
        bool imageEnabled = image != null;
        if (label != null && (buttonEnabled || imageEnabled))
        {
            label.color = normalColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        bool buttonEnabled = button != null && button.interactable;
        bool imageEnabled = image != null;
        if (label != null && (buttonEnabled || imageEnabled))
        {
            label.color = highlightColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        bool buttonEnabled = button != null && button.interactable;
        bool imageEnabled = image != null;
        if (label != null && (buttonEnabled || imageEnabled))
        {
            label.color = normalColor;
        }
    }
}

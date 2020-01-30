using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnPointerCallback : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int index;
    public Action<int> pointerEnterCallback;
    public Action<int> pointerExitCallback;

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerEnterCallback?.Invoke(index);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerExitCallback?.Invoke(index);
    }
}

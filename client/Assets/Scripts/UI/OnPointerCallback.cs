using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnPointerCallback : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public int index;
    public Action<int> pointerEnterCallback;
    public Action<int> pointerExitCallback;
    public Action<int> pointerDownCallback;

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerEnterCallback?.Invoke(index);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerExitCallback?.Invoke(index);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDownCallback?.Invoke(index);
    }
}

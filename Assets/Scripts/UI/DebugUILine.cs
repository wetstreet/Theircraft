#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 检查Ui上是否挂了raycastTarget，如果有会绘制蓝色辅助线，避免过度捕捉射线
///
/// https://www.xuanyusong.com/archives/4291
/// </summary>
public class DebugUILine : MonoBehaviour
{
    static Vector3[] fourCorners = new Vector3[4];

    void OnDrawGizmos()
    {
        foreach (MaskableGraphic g in GameObject.FindObjectsOfType<MaskableGraphic>())
        {
            if (g.raycastTarget)
            {
                RectTransform rectTransform = g.transform as RectTransform;
                rectTransform.GetWorldCorners(fourCorners);
                Gizmos.color = Color.blue;
                for (int i = 0; i < 4; i++)
                    Gizmos.DrawLine(fourCorners[i], fourCorners[(i + 1) % 4]);
            }
        }
    }
}
#endif
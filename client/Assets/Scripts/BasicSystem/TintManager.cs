using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TintManager : MonoBehaviour
{
    public static Color tintColor { get { return new Color(0.3490196f, 0.7882353f, 0.2352941f, 1); } }

    public static Color spruceTintColor { get { return new Color(97 / 255f, 153 / 255f, 97 / 255f, 1); } }

    public static Color birchTintColor { get { return new Color(128 / 255f, 167 / 255f, 85 / 255f, 1); } }
}

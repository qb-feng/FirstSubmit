using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(UnityEngine.UI.AspectRatioFitter))]
public class ResoleAspetRadioApplyQuestion : MonoBehaviour
{
    private void Awake()
    {
        var rect = GetComponent<RectTransform>();
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.one;
    }

}

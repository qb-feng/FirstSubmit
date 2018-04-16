using UnityEngine;
using System.Collections;
using System;

public interface IUIOnRefresh
{
    void OnBeforeRefresh(Type previous, Type next, UIBaseInit nextUI);
    void OnAfterRefreshUI(Type previous, Type next, UIBaseInit nextUI);
}

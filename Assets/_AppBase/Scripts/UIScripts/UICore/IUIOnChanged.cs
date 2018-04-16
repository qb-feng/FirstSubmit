using System;

public interface IOnUIChange
{
    /// <summary>
    /// 当销毁当前状态UI并创建新状态UI前调用
    /// </summary>
    void OnBeforeChange(Type previous, Type next);

    /// <summary>
    /// 当销毁当前状态UI并创建新状态UI后调用
    /// </summary>
    /// <param name="previous"></param>
    /// <param name="next"></param>
    void OnAfterChange(Type previous, Type next, UIBaseInit nextUI);
}

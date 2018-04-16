using System;

interface IUIFactory
{
    /// <summary>
    /// 获取UI
    /// </summary>
    /// <param name="ui">预制体脚本Type对象</param>
    /// <returns></returns>
    UIBaseInit GetUI(Type ui);

    /// <summary>
    /// 删除UI
    /// </summary>
    /// <param name="ui">UI脚本Type对象</param>
    /// <param name="immediately">是否立即删除</param>
	void DeleteUI(Type ui, bool immediately = false);
}

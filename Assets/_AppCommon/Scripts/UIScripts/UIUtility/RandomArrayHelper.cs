
using System;
/// <summary>
/// 随机数帮助类
/// </summary>
public class RandomArrayHelper
{
    //随机数对象
    private Random _random;
    #region 构造函数
    /// <summary>
    /// 构造函数
    /// </summary>
    public RandomArrayHelper()
    {
        //为随机数对象赋值
        this._random = new Random();
    }
    #endregion
    #region 生成一个指定范围的随机整数
    /// <summary>
    /// 生成一个指定范围的随机整数，该随机数范围包括最小值，但不包括最大值
    /// </summary>
    /// <param name="minNum">最小值</param>
    /// <param name="maxNum">最大值</param>
    public int GetRandomInt(int minNum, int maxNum)
    {
        return this._random.Next(minNum, maxNum);
    }
    #endregion
    #region 生成一个0.0到1.0的随机小数
    /// <summary>
    /// 生成一个0.0到1.0的随机小数
    /// </summary>
    public double GetRandomDouble()
    {
        return this._random.NextDouble();
    }
    #endregion
    #region 对一个数组进行随机排序
    /// <summary>
    /// 对一个数组进行随机排序
    /// </summary>
    /// <typeparam name="T">数组的类型</typeparam>
    /// <param name="arr">需要随机排序的数组</param>
    public void GetRandomArray<T>(T[] arr)
    {
        //对数组进行随机排序的算法:随机选择两个位置，将两个位置上的值交换
        //交换的次数,这里使用数组的长度作为交换次数
        int count = arr.Length;
        //开始交换
        for (int i = 0; i < count; i++)
        {
            //生成两个随机数位置
            int randomNum1 = GetRandomInt(0, arr.Length);
            int randomNum2 = GetRandomInt(0, arr.Length);
            //定义临时变量
            T temp;
            //交换两个随机数位置的值
            temp = arr[randomNum1];
            arr[randomNum1] = arr[randomNum2];
            arr[randomNum2] = temp;
        }
    }
    #endregion


    public void GetSequenceContentNotRepeatArray<T>(T[] arr)
    {
        //对数组进行随机排序的算法:随机选择两个位置，将两个位置上的值交换
        //交换的次数,这里使用数组的长度作为交换次数

        T[] tempArr = arr;
        int count = arr.Length;
        int j = 0;
        //开始交换
        while (j != count)
        {
            //生成两个随机数位置
            int randomNum1 = GetRandomInt(0, arr.Length);
            //定义临时变量
            T temp;
            //交换两个随机数位置的值
            //交换之前进行判断该位置是否是自身原来的在数组中的位置
            if (tempArr[j].ToString() != arr[randomNum1].ToString())
            {
                temp = arr[j];
                arr[j] = arr[randomNum1];
                arr[randomNum1] = temp;
                j++;
            }
        }
    }
}

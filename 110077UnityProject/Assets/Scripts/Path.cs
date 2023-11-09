using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{
    private static int[] array02;

    public static bool Find(Cell source, Cell target, int[,] arr, out int[] array03)
    {
        array03 = new int[100];
        for (int i = 0; i < array03.Length; i++)
        {
            array03[i] = -1;//初始路径数组
        }
        array02 = array03;
        bool decision = WayFinding(source.x, source.y, target.x, target.y, arr);
        //for(int i = 0;i< array03.Length;i+=2)
        //{
        //    if(array03[i] != -1 && array03[i+1] != -1)
        //        Debug.LogError(array03[i] + "," + array03[i + 1]);
        //}
        
        return decision;
    }


    public static int[] ArrayAppendElement(int[] array, int value)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == -1)
            {
                array[i] = value;
                return array;
            }
        }
        return array;
    }
    /// <summary>
    /// 检查此节点是否已经在路径数组中出现
    /// </summary>
    /// <param name="array">想要查找的目标数组</param>
    /// <param name="line">行坐标</param>
    /// <param name="column">列坐标</param>
    /// <returns></returns>
    public static bool CheckWay(int[] array, int line, int column)
    {
        for (int i = 0; i < array.Length; i += 2)
        {
            if (array[i] == -1) return false;
            if (line == array[i])
            {
                if (column == array[i + 1])
                {
                    return true;
                }
            }
        }
        return false;
    }
    /// <summary>
    /// 删除最后n项的有值元素
    /// </summary>
    /// <param name="array">目标数组</param>
    /// <param name="num">要删除num项</param>
    public static int[] DelArrayElement(int[] array, int num)
    {
        int location = 0;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == -1)
            {
                location = i - 1;
                break;
            }
        }
        if (location != 0)
        {
            for (int i = 0; i < num; i++)
            {
                array[location - i] = -1;
            }
        }
        return array;
    }

    public static bool WayFinding(int beginline, int begincolumn, int endline, int endcolumn, int[,] array01)
    {
        //保存路径
        array02 = ArrayAppendElement(array02, beginline);//双数为行0，2，4，......
        array02 = ArrayAppendElement(array02, begincolumn);//单数为列1，3，5，......
                                                           //<End>保存路径

        //开始递归找路径

        //初始局部变量
        if (endcolumn == begincolumn && endline == beginline) return true;
        int nextline;
        int nextcolumn;
        bool decision = false;
        //<End>

        //向上递归
        nextline = beginline - 1;
        if (decision == false)
        {
            if ((nextline < 0 || array01[nextline, begincolumn] < (int)ObjType.ANT) || CheckWay(array02, nextline, begincolumn))
                decision = false;
            else
                decision = WayFinding(nextline, begincolumn, endline, endcolumn, array01);
        }
        else return true;
        //向下递归
        nextline = beginline + 1;
        if (decision == false)
        {
            if ((nextline >= array01.GetLength(0) || array01[nextline, begincolumn] <(int)ObjType.ANT) || CheckWay(array02, nextline, begincolumn))
                decision = false;
            else
                decision = WayFinding(nextline, begincolumn, endline, endcolumn, array01);
        }
        else return true;
        //向左递归
        nextcolumn = begincolumn - 1;
        if (decision == false)
        {
            if ((nextcolumn < 0 || array01[beginline, nextcolumn] < (int)ObjType.ANT) || CheckWay(array02, beginline, nextcolumn))
                decision = false;
            else
                decision = WayFinding(beginline, nextcolumn, endline, endcolumn, array01);
        }
        else return true;
        //向右递归
        nextcolumn = begincolumn + 1;
        if (decision == false)
        {
            if ((nextcolumn >= array01.GetLength(1) || array01[beginline, nextcolumn] < (int)ObjType.ANT) || CheckWay(array02, beginline, nextcolumn))
                decision = false;
            else
                decision = WayFinding(beginline, nextcolumn, endline, endcolumn, array01);
        }
        else return true;

        //最后的判断，若以上都不满足则删除此节点并返回false
        if (decision == false)
        {
            DelArrayElement(array02, 2);
            return false;
        }
        return true;
        //<End>递归结束
    }


    /// <summary>
    /// return 石头在ant哪边
    /// </summary>
    /// <param name="stoneCell"></param>
    /// <param name="antCell"></param>
    /// <returns></returns>
    public static NearType isNear(Cell stoneCell, Cell antCell)
    {
        if(stoneCell.x == antCell.x)
        {
            if (stoneCell.y - antCell.y == 1)
                return NearType.Right;
            else if(stoneCell.y - antCell.y == -1)
                return NearType.Left;
        }
        else if(stoneCell.y == antCell.y)
        {
            if (stoneCell.x - antCell.x == 1)
                return NearType.DOWN;
            else if(stoneCell.x - antCell.x == -1)
                return NearType.UP;
        }
        return NearType.NOT;
    }
}

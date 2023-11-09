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
            array03[i] = -1;//��ʼ·������
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
    /// ���˽ڵ��Ƿ��Ѿ���·�������г���
    /// </summary>
    /// <param name="array">��Ҫ���ҵ�Ŀ������</param>
    /// <param name="line">������</param>
    /// <param name="column">������</param>
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
    /// ɾ�����n�����ֵԪ��
    /// </summary>
    /// <param name="array">Ŀ������</param>
    /// <param name="num">Ҫɾ��num��</param>
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
        //����·��
        array02 = ArrayAppendElement(array02, beginline);//˫��Ϊ��0��2��4��......
        array02 = ArrayAppendElement(array02, begincolumn);//����Ϊ��1��3��5��......
                                                           //<End>����·��

        //��ʼ�ݹ���·��

        //��ʼ�ֲ�����
        if (endcolumn == begincolumn && endline == beginline) return true;
        int nextline;
        int nextcolumn;
        bool decision = false;
        //<End>

        //���ϵݹ�
        nextline = beginline - 1;
        if (decision == false)
        {
            if ((nextline < 0 || array01[nextline, begincolumn] < (int)ObjType.ANT) || CheckWay(array02, nextline, begincolumn))
                decision = false;
            else
                decision = WayFinding(nextline, begincolumn, endline, endcolumn, array01);
        }
        else return true;
        //���µݹ�
        nextline = beginline + 1;
        if (decision == false)
        {
            if ((nextline >= array01.GetLength(0) || array01[nextline, begincolumn] <(int)ObjType.ANT) || CheckWay(array02, nextline, begincolumn))
                decision = false;
            else
                decision = WayFinding(nextline, begincolumn, endline, endcolumn, array01);
        }
        else return true;
        //����ݹ�
        nextcolumn = begincolumn - 1;
        if (decision == false)
        {
            if ((nextcolumn < 0 || array01[beginline, nextcolumn] < (int)ObjType.ANT) || CheckWay(array02, beginline, nextcolumn))
                decision = false;
            else
                decision = WayFinding(beginline, nextcolumn, endline, endcolumn, array01);
        }
        else return true;
        //���ҵݹ�
        nextcolumn = begincolumn + 1;
        if (decision == false)
        {
            if ((nextcolumn >= array01.GetLength(1) || array01[beginline, nextcolumn] < (int)ObjType.ANT) || CheckWay(array02, beginline, nextcolumn))
                decision = false;
            else
                decision = WayFinding(beginline, nextcolumn, endline, endcolumn, array01);
        }
        else return true;

        //�����жϣ������϶���������ɾ���˽ڵ㲢����false
        if (decision == false)
        {
            DelArrayElement(array02, 2);
            return false;
        }
        return true;
        //<End>�ݹ����
    }


    /// <summary>
    /// return ʯͷ��ant�ı�
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Routing
{
    #region ����

    Routing() { }

    static Routing instance;

    public static Routing Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Routing();
            }
            return instance;
        }
    }

    #endregion

    /// <summary>
    /// ��ά����ĵ�ͼ
    /// </summary>
    RoutingObject[,] map;

    /// <summary>
    /// �洢��������Ѱ�����·���ĵ�
    /// </summary>
    List<RoutingObject> open = new List<RoutingObject>();

    /// <summary>
    /// �洢���ٱ�����Ѱ�����·���ĵ�
    /// </summary>
    List<RoutingObject> closed = new List<RoutingObject>();

    /// <summary>
    /// �洢·�ߵ���б�
    /// </summary>
    List<RoutingObject> route = new List<RoutingObject>();

    public int row;
    public int col;

    /// <summary>
    /// ��ʼ��
    /// </summary>
    void Init(RoutingObject[,] mapArray)
    {
        open.Clear();
        closed.Clear();
        route.Clear();
        map = mapArray;
    }

    /// <summary>
    /// �жϴ���ʼ���Ƿ��ܵ���Ŀ���
    /// </summary>
    /// <param name="start_x">��ʼ��x����</param>
    /// <param name="start_y">��ʼ��y����</param>
    /// <param name="end_x">Ŀ���x����</param>
    /// <param name="end_y">Ŀ���y����</param>
    /// <param name="map"></param>
    /// <returns></returns>
    public bool IsRouting(RoutingObject start, RoutingObject end, RoutingObject[,] mapArray)
    {
        Init(mapArray);

        Explore(start, end, start);

        // �жϴ洢·�ߵ���б����Ƿ���е�
        return route.Count > 0;
    }

    /// <summary>
    /// ̽�����ĵ����������ĸ������
    /// </summary>
    void Explore(RoutingObject center, RoutingObject end, RoutingObject start)
    {
        // ���ĵ㲻�ٿ���Ѱ��·��
        closed.Add(center);

        // �����ĵ��Ѱ���б����Ƴ�
        if (open.Contains(center))
        {
            open.Remove(center);
        }

        // �Ƿ��ҵ�Ŀ���
        if (IsGetEnd(end))
        {
            // �ҵ�Ŀ���
            ReturnRoute(end, start);
        }
        else
        {
            // �ж����ĵ��ϱߵĵ�
            if (center.y - 1 >= 0)
            {
                RoutingObject up = map[center.x, center.y - 1];
                GetMoveSumByDirection(up, center, end, Direction.up);
            }

            // �ж����ĵ��±ߵĵ�
            if (center.y + 1 < col)
            {
                RoutingObject down = map[center.x, center.y + 1];
                GetMoveSumByDirection(down, center, end, Direction.down);
            }

            // �ж����ĵ���ߵĵ�
            if (center.x - 1 >= 0)
            {
                RoutingObject left = map[center.x - 1, center.y];
                GetMoveSumByDirection(left, center, end, Direction.left);
            }

            // �ж����ĵ��ұߵĵ�
            if (center.x + 1 < row)
            {
                RoutingObject right = map[center.x + 1, center.y];
                GetMoveSumByDirection(right, center, end, Direction.right);
            }

            if (open.Count > 0)
            {
                // û���ҵ�Ŀ���,���ڱ����ǵ��б����ҳ�һ����ֵ��С��
                RoutingObject ro = GetMinimumMoveSum();
                Explore(ro, end, start);
            }
            else
            {
                Debug.Log("û���ҵ�Ŀ���");
            }
        }
    }

    /// <summary>
    /// ���ݴ������ķ���ȥ��ȡ��ֵ
    /// </summary>
    /// <param name="center"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="direction"></param>
    void GetMoveSumByDirection(RoutingObject center, RoutingObject start, RoutingObject end, Direction direction)
    {
        // �ж�������Ƿ����ƶ������Ƿ񱻿���
        if (IsForward(center))
        {
            center.direction = direction;
            // ��ȡ�ƶ�����
            center.moveDistance = GetDistance(center, start);
            // ��ȡĿ�����
            center.targetDistance = GetDistance(center, end);
            // ��ȡA*��ֵ
            center.moveSum = center.moveDistance + center.targetDistance;
            // �����ĵ���뽫Ҫ�����ǵ��б���
            open.Add(center);
        }
        else
        {
            //Debug.Log(center.name + " �����ƶ�");
        }
    }

    /// <summary>
    /// �ж�������Ƿ�����δ��������ǰ���ĵ�
    /// </summary>
    /// <param name="ro"></param>
    /// <returns></returns>
    bool IsForward(RoutingObject ro)
    {
        // �ж�������Ƿ��Ѿ��ڲ��ٿ��ǵ��б���
        if (closed.Contains(ro) || open.Contains(ro))
        {
            return false;
        }
        else
        {
            // �ж�������Ƿ�����ƶ�
            if (ro.isCanMove)
            {
                return true;
            }
            else
            {
                // �������ƶ��ͼ��벻�ٿ��ǵ��б���
                closed.Add(ro);
                return false;
            }
        }
    }

    /// <summary>
    /// ��ȡ����
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    int GetDistance(RoutingObject start, RoutingObject end)
    {
        // ����Ŀ����뷵��ֵ, --> ˭��,˭��˭
        return Mathf.Abs(start.x - end.x) + Mathf.Abs(start.y - end.y);
    }

    /// <summary>
    /// �Ƿ��ҵ�Ŀ���
    /// </summary>
    /// <returns></returns>
    bool IsGetEnd(RoutingObject end)
    {
        return closed.Contains(end);
    }

    /// <summary>
    /// �ڱ����ǵ��б��л�ȡ��ֵ��С�ĵ�
    /// </summary>
    /// <returns></returns>
    RoutingObject GetMinimumMoveSum()
    {
        RoutingObject ro = null;
        RoutingObject temporary = new RoutingObject();
        for (int i = 0; i < open.Count; i++)
        {
            //Debug.Log("��ǰ " + open[i].name + " �ĺ�ֵΪ: " + open[i].moveSum);
            // �б��еĵ�һ������Ҫ�Ƚ�,ֱ�Ӹ�ֵ
            if (i == 0)
            {
                ro = open[i];
                temporary = open[i];
            }
            else
            {
                // Ѱ���б��к�ֵ��С�ĵ�
                if (open[i].moveSum < temporary.moveSum)
                {
                    ro = open[i];
                    temporary = open[i];
                }
            }
        }
        //Debug.Log("���� " + ro.name + " �ĺ�ֵΪ: " + ro.moveSum);
        return ro;
    }


    /// <summary>
    /// ����·��
    /// </summary>
    /// <param name="center"></param>
    /// <param name="start"></param>
    void ReturnRoute(RoutingObject center, RoutingObject start)
    {
        // �������洢��·���б���
        route.Add(center);
        // �ж�·���б����Ƿ������ʼ��
        if (!route.Contains(start))
        {
            // û�а���
            // ����·��ȡ�����ķ�����
            switch (center.direction)
            {
                case Direction.up:
                    ReturnRoute(map[center.x, center.y + 1], start);
                    break;
                case Direction.down:
                    ReturnRoute(map[center.x, center.y - 1], start);
                    break;
                case Direction.left:
                    ReturnRoute(map[center.x + 1, center.y], start);
                    break;
                case Direction.right:
                    ReturnRoute(map[center.x - 1, center.y], start);
                    break;
            }

        }
        else
        {
            RouteSort(start);
        }
    }

    /// <summary>
    /// ·������(����ʼ��Ӵ洢·�ߵ���б����Ƴ�,������ʼ�㵽Ŀ�����������)
    /// </summary>
    void RouteSort(RoutingObject start)
    {
        List<RoutingObject> list = new List<RoutingObject>(route);

        route.Clear();

        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (list[i] != start)
            {
                route.Add(list[i]);
            }
        }
    }

    /// <summary>
    /// �������·��
    /// </summary>
    /// <returns></returns>
    public List<RoutingObject> GetRoute()
    {
        return route;
    }
}

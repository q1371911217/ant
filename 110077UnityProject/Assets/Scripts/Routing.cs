using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Routing
{
    #region 单例

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
    /// 二维数组的地图
    /// </summary>
    RoutingObject[,] map;

    /// <summary>
    /// 存储被考虑来寻找最短路径的点
    /// </summary>
    List<RoutingObject> open = new List<RoutingObject>();

    /// <summary>
    /// 存储不再被考虑寻找最短路径的点
    /// </summary>
    List<RoutingObject> closed = new List<RoutingObject>();

    /// <summary>
    /// 存储路线点的列表
    /// </summary>
    List<RoutingObject> route = new List<RoutingObject>();

    public int row;
    public int col;

    /// <summary>
    /// 初始化
    /// </summary>
    void Init(RoutingObject[,] mapArray)
    {
        open.Clear();
        closed.Clear();
        route.Clear();
        map = mapArray;
    }

    /// <summary>
    /// 判断从起始点是否能到达目标点
    /// </summary>
    /// <param name="start_x">起始点x坐标</param>
    /// <param name="start_y">起始点y坐标</param>
    /// <param name="end_x">目标点x坐标</param>
    /// <param name="end_y">目标点y坐标</param>
    /// <param name="map"></param>
    /// <returns></returns>
    public bool IsRouting(RoutingObject start, RoutingObject end, RoutingObject[,] mapArray)
    {
        Init(mapArray);

        Explore(start, end, start);

        // 判断存储路线点的列表里是否存有点
        return route.Count > 0;
    }

    /// <summary>
    /// 探索中心点上下左右四个方向点
    /// </summary>
    void Explore(RoutingObject center, RoutingObject end, RoutingObject start)
    {
        // 中心点不再考虑寻找路径
        closed.Add(center);

        // 将中心点从寻找列表中移除
        if (open.Contains(center))
        {
            open.Remove(center);
        }

        // 是否找到目标点
        if (IsGetEnd(end))
        {
            // 找到目标点
            ReturnRoute(end, start);
        }
        else
        {
            // 判断中心点上边的点
            if (center.y - 1 >= 0)
            {
                RoutingObject up = map[center.x, center.y - 1];
                GetMoveSumByDirection(up, center, end, Direction.up);
            }

            // 判断中心点下边的点
            if (center.y + 1 < col)
            {
                RoutingObject down = map[center.x, center.y + 1];
                GetMoveSumByDirection(down, center, end, Direction.down);
            }

            // 判断中心点左边的点
            if (center.x - 1 >= 0)
            {
                RoutingObject left = map[center.x - 1, center.y];
                GetMoveSumByDirection(left, center, end, Direction.left);
            }

            // 判断中心点右边的点
            if (center.x + 1 < row)
            {
                RoutingObject right = map[center.x + 1, center.y];
                GetMoveSumByDirection(right, center, end, Direction.right);
            }

            if (open.Count > 0)
            {
                // 没有找到目标点,则在被考虑的列表中找出一个和值最小的
                RoutingObject ro = GetMinimumMoveSum();
                Explore(ro, end, start);
            }
            else
            {
                Debug.Log("没有找到目标点");
            }
        }
    }

    /// <summary>
    /// 根据传进来的方向去获取和值
    /// </summary>
    /// <param name="center"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="direction"></param>
    void GetMoveSumByDirection(RoutingObject center, RoutingObject start, RoutingObject end, Direction direction)
    {
        // 判断这个点是否能移动或者是否被考虑
        if (IsForward(center))
        {
            center.direction = direction;
            // 获取移动距离
            center.moveDistance = GetDistance(center, start);
            // 获取目标距离
            center.targetDistance = GetDistance(center, end);
            // 获取A*和值
            center.moveSum = center.moveDistance + center.targetDistance;
            // 将中心点加入将要被考虑的列表中
            open.Add(center);
        }
        else
        {
            //Debug.Log(center.name + " 不能移动");
        }
    }

    /// <summary>
    /// 判断这个点是否属于未来被考虑前进的点
    /// </summary>
    /// <param name="ro"></param>
    /// <returns></returns>
    bool IsForward(RoutingObject ro)
    {
        // 判断这个点是否已经在不再考虑的列表中
        if (closed.Contains(ro) || open.Contains(ro))
        {
            return false;
        }
        else
        {
            // 判断这个点是否可以移动
            if (ro.isCanMove)
            {
                return true;
            }
            else
            {
                // 不可以移动就加入不再考虑的列表中
                closed.Add(ro);
                return false;
            }
        }
    }

    /// <summary>
    /// 获取距离
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    int GetDistance(RoutingObject start, RoutingObject end)
    {
        // 定义目标距离返回值, --> 谁大,谁减谁
        return Mathf.Abs(start.x - end.x) + Mathf.Abs(start.y - end.y);
    }

    /// <summary>
    /// 是否找到目标点
    /// </summary>
    /// <returns></returns>
    bool IsGetEnd(RoutingObject end)
    {
        return closed.Contains(end);
    }

    /// <summary>
    /// 在被考虑的列表中获取和值最小的点
    /// </summary>
    /// <returns></returns>
    RoutingObject GetMinimumMoveSum()
    {
        RoutingObject ro = null;
        RoutingObject temporary = new RoutingObject();
        for (int i = 0; i < open.Count; i++)
        {
            //Debug.Log("当前 " + open[i].name + " 的和值为: " + open[i].moveSum);
            // 列表中的第一个不需要比较,直接赋值
            if (i == 0)
            {
                ro = open[i];
                temporary = open[i];
            }
            else
            {
                // 寻找列表中和值最小的点
                if (open[i].moveSum < temporary.moveSum)
                {
                    ro = open[i];
                    temporary = open[i];
                }
            }
        }
        //Debug.Log("最终 " + ro.name + " 的和值为: " + ro.moveSum);
        return ro;
    }


    /// <summary>
    /// 返回路线
    /// </summary>
    /// <param name="center"></param>
    /// <param name="start"></param>
    void ReturnRoute(RoutingObject center, RoutingObject start)
    {
        // 将这个点存储到路线列表中
        route.Add(center);
        // 判断路线列表中是否包含起始点
        if (!route.Contains(start))
        {
            // 没有包含
            // 返回路线取这个点的反方向
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
    /// 路线排序(将起始点从存储路线点的列表中移除,并从起始点到目标点重新排序)
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
    /// 返回最短路线
    /// </summary>
    /// <returns></returns>
    public List<RoutingObject> GetRoute()
    {
        return route;
    }
}

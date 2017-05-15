using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Path
{
    //所有路点
    public Vector3[] waypoints;
    //当前路点索引
    public int index = -1;
    //当前的路点
    public Vector3 waypoint;
    //是否循环
    bool isLoop = false;
    //到达误差
    public float deviation = 5;
    //是否完成
    public bool isFinish = false;

    //是否到达目的地
    public bool IsReach(Transform trans)
    {
        Vector3 pos = trans.position;
        float distance = Vector3.Distance(waypoint, pos);
        return distance < deviation;
    }

    /// <summary>
    /// 下一个路点
    /// </summary>
    public void NextWaypoint()
    {
        if (index < 0)
            return;
        if (index < waypoints.Length - 1)
        {
            index++;
        }
        else
        {
            if (isLoop)
            {
                index = 0;
            }
            else
            {
                isFinish = true;
            }
        }
        waypoint = waypoints[index];
    }

    /// <summary>
    /// 根据场景标识物生成路点
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="isL"></param>
    public void InitByObj(GameObject obj, bool isL = false)
    {
        int length = obj.transform.childCount;
        //没有子物体
        if (length == 0)
        {
            waypoints = null;
            index = -1;
            Debug.LogWarning("Path.InitByObj length == 0");
            return;
        }
        //遍历子物体生成路点
        waypoints = new Vector3[length];
        for (int i = 0; i < length; i++)
        {
            Transform trans = obj.transform.GetChild(i);
            waypoints[i] = trans.position;
        }
        //设置一些参数
        index = 0;
        waypoint = waypoints[index];
        this.isLoop = isLoop;
        isFinish = false;
    }

    /// <summary>
    /// 根据导航图初始化路径
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="targetPos"></param>
    public void InitByNavMeshPath(Vector3 pos, Vector3 targetPos)
    {
        //重置路线
        waypoints = null;
        index = -1;
        //计算路径
        NavMeshPath navPath = new NavMeshPath();
        bool hasFoundPath = NavMesh.CalculatePath(pos, targetPos, NavMesh.AllAreas, navPath);
        if (!hasFoundPath) return;
        int length = navPath.corners.Length;
        waypoints = new Vector3[length];
        for (int i = 0; i < length; i++) waypoints[i] = navPath.corners[i];
        index = 0;
        waypoint = waypoints[index];
        isFinish = false;
    }

    /// <summary>
    /// 调试路径
    /// </summary>
    public void DrawWaypoints()
    {
        if (waypoints == null) return;
        int length = waypoints.Length;
        for (int i = 0; i < length; i++)
        {
            if (i == index)
            {
                Gizmos.DrawSphere(waypoints[i], 1);
            }
            else
            {
                Gizmos.DrawCube(waypoints[i], Vector3.one);
            }
        }
    }
}

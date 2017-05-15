using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public enum Status
{
    Patrol,
    Attack
}

public class AI : MonoBehaviour
{
    public Tank tank;
    private Status status = Status.Patrol;
    private GameObject target;
    private float sightDistance = 30;
    private float lastSearchTargetTime = 0;
    private float searchTargetInterval = 3;
    private Path path = new Path();

    void Start()
    {
        InitWaypoint();
    }

    void Update()
    {
        if (tank.ctrlType != Tank.CtrlType.computer) return;
        TargetUpdate();

        //行走
        if (path.IsReach(transform))
        {
            path.NextWaypoint();
        }

        if (status == Status.Patrol)
        {
            PatrolUpdate();
        }
        else if (status == Status.Attack)
        {
            AttackUpdate();
        }
    }

    //初始化路径
    void InitWaypoint()
    {
        GameObject obj = GameObject.Find("WaypointContainer");
        if (obj)
        {
            path.InitByObj(obj);
        }
    }

    //搜索目标
    private void TargetUpdate()
    {
        float interval = Time.time - lastSearchTargetTime;
        if (interval < searchTargetInterval) return;
        if (target != null) HasTarget();
        else Notarget();
    }

    //已经发现坦克目标的情况下
    private void HasTarget()
    {
        Tank targetTank = target.GetComponent<Tank>();
        Vector3 pos = transform.position;
        Vector3 targetpos = target.transform.position;
        if (targetTank.ctrlType == Tank.CtrlType.none)
        {
            Debug.Log("目标死亡，丢失目标！！！");
            target = null;
        }
        else if (Vector3.Distance(pos, targetpos) > sightDistance)
        {
            Debug.Log("距离太远，丢失目标！！！");
            target = null;
        }
    }

    //没有发现坦克目标情况下
    private void Notarget()
    {
        float minHp = float.MaxValue;
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Tank");
        //Debug.Log(targets.Length);
        for (int i = 0; i < targets.Length; i++)
        {
            Tank tank = targets[i].GetComponent<Tank>();
            if (tank == null)
            {
                continue;
            }
            if (targets[i] == gameObject)
            {
                continue;
            }
            if (tank.ctrlType == Tank.CtrlType.none)
            {
                continue;
            }
            //判断距离
            Vector3 pos = transform.position;
            Vector3 targetPos = targets[i].transform.position;
            if (Vector3.Distance(pos, targetPos) > sightDistance)
            {
                continue;
            }
            //判断生命值
            if (minHp > tank.hp)
            {
                target = tank.gameObject;
            }
        }
        //调试
        if (target != null)
        {
            Debug.Log("获取目标" + target.name);
        }
    }

    //被攻击,仇恨系统
    public void OnAttecked(GameObject attackTank)
    {
        target = attackTank;
    }

    public void ChangeStatus(Status status)
    {
        if (status == Status.Patrol)
        {
            PartrolStart();
        }
        else if (status == Status.Attack)
        {
            AttackStart();
        }
    }

    //巡逻逻辑
    private void PartrolStart()
    {

    }

    //攻击逻辑
    private void AttackStart()
    {

    }

    //巡逻中
    private void PatrolUpdate()
    {

    }

    //攻击中
    private void AttackUpdate()
    {

    }

    //获取炮管和炮塔的目标角度
    public Vector3 GetTurretTarget()
    {
        //没有目标，炮塔朝向坦克的前方
        if (target == null)
        {
            float y = transform.eulerAngles.y;
            Vector3 rot = new Vector3(0, y, 0);
            return rot;
        }
        //有目标，炮塔对准目标
        else
        {
            Vector3 pos = transform.position;
            Vector3 targetpos = target.transform.position;
            Vector3 vec = targetpos - pos;
            return Quaternion.LookRotation(vec).eulerAngles;
        }
    }

    //是否发射炮弹
    public bool IsShot()
    {
        if (target == null) return false;

        //目标角度差
        float turretRoll = tank.turret.eulerAngles.y;
        float angle = turretRoll - GetTurretTarget().y;
        if (angle < 0) angle += 360;
        //30角度内可以发射炮弹
        if (angle < 5 || angle > 355)
            return true;
        else
            return false;
    }

    //获取转向角
    public float GetSteering()
    {
        if (tank == null) return 0;
        Vector3 itp = transform.InverseTransformPoint(path.waypoint);
        if (itp.x > path.deviation / 5) return tank.maxSteeringAngle;
        else if (itp.x < -path.deviation / 5) return -tank.maxSteeringAngle;
        else return 0;
    }

    //获取马力
    public float GetMotor()
    {
        if (tank == null)
            return 0;

        Vector3 itp = transform.InverseTransformPoint(path.waypoint);
        float x = itp.x;
        float z = itp.y;
        float r = 6;

        if (z < 0 && Mathf.Abs(x) < -z && Mathf.Abs(x) < r) return -tank.maxMotorTorque;
        else return tank.maxMotorTorque;
    }

    //获取刹车
    public float GetBrakeTorque()
    {
        if (path.isFinish) return tank.maxMotorTorque;
        else return 0;
    }
}

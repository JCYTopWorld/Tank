using UnityEngine;
using System.Collections;
using System;

public class CameraFollow : MonoBehaviour
{
    //纵向旋转速度
    private float rollSpeed = 0.08f;
    //横向旋转速度
    public float rotSpeed = 0.18f;
    //纵向角度
    private float roll = 30f * Mathf.PI * 2 / 360;
    //横向角度
    public float rot = 0;
    //距离
    public float distance = 8;
    //目标物体
    private GameObject target;
    //纵向角度范围
    private float maxRoll = 70f * Mathf.PI * 2 / 360;
    private float minRoll = -10f * Mathf.PI * 2 / 360;
    //距离范围
    public float maxDistance = 10f;
    public float minDistance = 5f;
    //距离变化速度
    public float zoomSpeed = 0.2f;
    void Start()
    {
        //找到坦克
        //target = GameObject.Find("Tank");
        SetTarget(GameObject.Find("Tank"));
    }

    private void SetTarget(GameObject target)
    {
        if (target.transform.FindChild("cameraPoint") != null)
            this.target = target.transform.FindChild("cameraPoint").gameObject;
        else
            this.target = target;
    }

    void LateUpdate()
    {
        //一些判断
        if (target == null)
            return;
        if (Camera.main == null)
            return;
        //目标的坐标
        Vector3 targetPos = target.transform.position;
        //用三角函数计算相机位置
        Vector3 cameraPos;
        float d = distance * Mathf.Cos(roll);
        float height = distance * Mathf.Sin(roll);
        cameraPos.x = targetPos.x + d * Mathf.Cos(rot);//计算出X的坐标位置
        cameraPos.y = targetPos.y + height;//计算出Y的坐标位置
        cameraPos.z = targetPos.z + d * Mathf.Sin(rot);
        Camera.main.transform.position = cameraPos;
        //对准目标
        Camera.main.transform.LookAt(target.transform);
        //纵向旋转
        Rotate();
        //横向旋转
        Roll();
        //调整距离
        Zoom();
    }

    private void Zoom()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (distance > minDistance)
                distance -= zoomSpeed;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (distance < maxDistance)
                distance += zoomSpeed;
        }
    }

    private void Roll()
    {
        float w = Input.GetAxis("Mouse Y") * rollSpeed * 0.5f;

        roll -= w;
        if (roll > maxRoll)
            roll = maxRoll;
        if (roll < minRoll)
            roll = minRoll;
    }

    private void Rotate()
    {
        float w = Input.GetAxis("Mouse X") * rotSpeed;
        rot -= w;
    }
}
 
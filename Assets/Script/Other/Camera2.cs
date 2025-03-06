using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera2 : MonoBehaviour
{
    public Transform player;        // 角色
    public Transform target;        // 锁定的敌人目标
    public float distance = 5.0f;   // 摄像机与角色的初始距离
    public float height = 3.0f;     // 摄像机高度
    public float moveSpeed = 5.0f;  // 摄像机移动的匀速
    public float rotationSpeed = 5.0f; // 摄像机旋转速度
    public LayerMask obstacleLayers; // 用于检测遮挡物的层级
    public float smooth = 10f;       // 摄像机响应的平滑度
    public Vector3 pivotOffset = new Vector3(0.0f, 1.7f, 0.0f); // 摄像机的枢轴偏移
    public float sphereCastRadius = 0.2f;  // SphereCast的半径，用于检测遮挡物

    private Vector3 desiredPosition;
    private Quaternion desiredRotation;

    private Vector3 smoothPivotOffset;
    private Vector3 smoothCamOffset;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag(SaveKey.Character).transform;
        // 初始化摄像机位置
        desiredPosition = player.position + new Vector3(0, height, -distance);
        transform.position = desiredPosition;
        transform.LookAt(target);

        smoothPivotOffset = pivotOffset;
        smoothCamOffset = new Vector3(0, height, -distance);
    }

    void LateUpdate()
    {
        if (target != null)
        {
            // 计算目标与角色之间的方向
            Vector3 direction = (target.position - player.position).normalized;

            // 计算理想摄像机位置，使得敌人位于屏幕中心
            desiredPosition = player.position - direction * distance + new Vector3(0, height, 0);

            // 进行遮挡检测并调整摄像机位置
            desiredPosition = CheckForObstacles(player.position, desiredPosition);

            // 以匀速移动摄像机到理想位置
            transform.position = Vector3.MoveTowards(transform.position, desiredPosition, moveSpeed * Time.deltaTime);

            // 计算理想旋转，使摄像机始终看向敌人
            desiredRotation = Quaternion.LookRotation(target.position - transform.position);

            // 以平滑的方式旋转摄像机
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);

            // 最终对齐，使角色和敌人在同一条线上
            AlignCameraWithPlayerAndTarget();
        }
        else
        {
            // 若没有锁定目标，则保持摄像机与角色的正常视角
            desiredPosition = player.position + new Vector3(0, height, -distance);
            transform.position = Vector3.MoveTowards(transform.position, desiredPosition, moveSpeed * Time.deltaTime);
            transform.LookAt(player.position);
        }
    }

    /// <summary>
    /// 最终对齐摄像机，使角色和敌人在同一条线上
    /// </summary>
    void AlignCameraWithPlayerAndTarget()
    {
        Vector3 direction = (target.position - player.position).normalized;
        Vector3 finalPosition = player.position - direction * distance + new Vector3(0, height, 0);

        if (Vector3.Distance(transform.position, finalPosition) < 0.1f)
        {
            transform.position = finalPosition;
            transform.rotation = Quaternion.LookRotation(target.position - transform.position);
        }
    }

    /// <summary>
    /// 检测摄像机与玩家之间的障碍物，并调整摄像机的位置避免遮挡
    /// </summary>
    Vector3 CheckForObstacles(Vector3 playerPos, Vector3 cameraPos)
    {
        Vector3 direction = cameraPos - playerPos;
        float currentDistance = direction.magnitude;

        // 使用 SphereCast 进行遮挡检测
        if (Physics.SphereCast(playerPos, sphereCastRadius, direction.normalized, out RaycastHit hit, currentDistance, obstacleLayers))
        {
            // 如果有障碍物，调整摄像机位置到障碍物前
            return hit.point - direction.normalized * 0.5f; // 在障碍物前留一点空间
        }

        // 没有障碍物，返回原始摄像机位置
        return cameraPos;
    }

    // 双重检测遮挡物
    bool DoubleViewingPosCheck(Vector3 checkPos)
    {
        return ViewingPosCheck(checkPos) && ReverseViewingPosCheck(checkPos);
    }

    // 从摄像机到角色的遮挡检测
    bool ViewingPosCheck(Vector3 checkPos)
    {
        Vector3 target = player.position + pivotOffset;
        Vector3 direction = target - checkPos;

        if (Physics.SphereCast(checkPos, sphereCastRadius, direction, out RaycastHit hit, direction.magnitude))
        {
            if (hit.transform != player && !hit.transform.GetComponent<Collider>().isTrigger)
            {
                return false;
            }
        }
        return true;
    }

    // 从角色到摄像机的遮挡检测
    bool ReverseViewingPosCheck(Vector3 checkPos)
    {
        Vector3 origin = player.position + pivotOffset;
        Vector3 direction = checkPos - origin;

        if (Physics.SphereCast(origin, sphereCastRadius, direction, out RaycastHit hit, direction.magnitude))
        {
            if (hit.transform != player && hit.transform != transform && !hit.transform.GetComponent<Collider>().isTrigger)
            {
                return false;
            }
        }
        return true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_move : MonoBehaviour
{
    public Vector3 pivotOffset = new Vector3(0.0f, 1.7f, 0.0f);       // Offset to repoint the camera.
    public Vector3 camOffset = new Vector3(0.0f, 0.0f, -3.0f);       // Offset to relocate the camera related to the player position.
    public float moveSpeed = 6;
    public float maxVerticalAngle = 30f;                               // Camera max clamp angle. 
    public float minVerticalAngle = -60f;                              // Camera min clamp angle.
    public Vector3 aimPivotOffset = new Vector3(0.5f, 1.2f, 0f);         // Offset to repoint the camera when aiming.
    public Vector3 aimCamOffset = new Vector3(0f, 0.4f, -0.7f);         // Offset to relocate the camera when aiming.
    public float smooth = 10f;                                         // Speed of camera responsiveness.

    private Transform player;                                           // Player's reference.
    private Transform cam;                                             // This transform.
    private Vector3 smoothPivotOffset;                                 // Camera current pivot offset on interpolation.
    private Vector3 smoothCamOffset;                                   // Camera current offset on interpolation.
    private float defaultFOV;                                          // Default camera Field of View.
    private float angleH = 0;                                          // Float to store camera horizontal angle related to mouse movement.
    private float angleV = 0;                                          // Float to store camera vertical angle related to mouse movement.
                                                                       //    private bool isRotating = false;
    private bool isCustomOffset = false;
    private bool isPaused = false;

    private Vector3 Pivot = new Vector3(1.3f, 1, 0.4f);
    private Vector3 Cam = new Vector3(0, 0, -3.5f);
    private Vector3 Original_pivot;
    private Vector3 Original_Cam;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag(SaveKey.Character).transform;
        cam = transform;

        // Set camera default position.
        cam.position = player.position + Quaternion.identity * pivotOffset + Quaternion.identity * camOffset;
        cam.rotation = Quaternion.identity;

        // Set up references and default values.
        smoothPivotOffset = pivotOffset;
        smoothCamOffset = camOffset;
        defaultFOV = cam.GetComponent<Camera>().fieldOfView;
        angleH = player.eulerAngles.y;
        Original_pivot = aimPivotOffset;
        Original_Cam = aimCamOffset;
    }

    private void Update()
    {
        if (isPaused)
            return;
        angleH += Mathf.Clamp(Input.GetAxis("Mouse X"), -1, 1) * moveSpeed;
        angleV += Mathf.Clamp(Input.GetAxis("Mouse Y"), -1, 1) * moveSpeed;
        // Joystick:
        angleH += Mathf.Clamp(Input.GetAxis("Analog X"), -1, 1) * 60 * moveSpeed * Time.deltaTime;
        angleV += Mathf.Clamp(Input.GetAxis("Analog Y"), -1, 1) * 60 * moveSpeed * Time.deltaTime;
        angleV = Mathf.Clamp(angleV, minVerticalAngle, maxVerticalAngle);

        // Set camera orientation.
        Quaternion camYRotation = Quaternion.Euler(0, angleH, 0);
        Quaternion aimRotation = Quaternion.Euler(-angleV, angleH, 0);
        cam.rotation = aimRotation;

        // Set FOV.
        //    cam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(cam.GetComponent<Camera>().fieldOfView, targetFOV, Time.deltaTime);

        // Test for collision with the environment based on current camera position.




        Vector3 baseTempPosition = player.position + camYRotation * aimPivotOffset;

        Vector3 noCollisionOffset = aimCamOffset;

        while (noCollisionOffset.magnitude >= 0.2f)
        {
            if (DoubleViewingPosCheck(baseTempPosition + aimRotation * noCollisionOffset))
                break;
            noCollisionOffset -= noCollisionOffset.normalized * 0.2f;
        }
        if (noCollisionOffset.magnitude < 0.2f)
            noCollisionOffset = Vector3.zero;

        // No intermediate position for custom offsets, go to 1st person.
        bool customOffsetCollision = isCustomOffset && noCollisionOffset.sqrMagnitude < aimCamOffset.sqrMagnitude;

        // Repostition the camera.
        smoothPivotOffset = Vector3.Lerp(smoothPivotOffset, customOffsetCollision ? pivotOffset : aimPivotOffset, smooth * Time.deltaTime);
        smoothCamOffset = Vector3.Lerp(smoothCamOffset, customOffsetCollision ? Vector3.zero : noCollisionOffset, smooth * Time.deltaTime);

        cam.position = player.position + camYRotation * smoothPivotOffset + aimRotation * smoothCamOffset;


    }

    public void Act_second_view(bool second_view)
    {
        if (second_view)
        {
            aimPivotOffset = Pivot;
            aimCamOffset = Cam;
        }
        else
        {
            aimPivotOffset = Original_pivot;
            aimCamOffset = Original_Cam;
        }
    }


    // Double check for collisions: concave objects doesn't detect hit from outside, so cast in both directions.
    bool DoubleViewingPosCheck(Vector3 checkPos)
    {
        return ViewingPosCheck(checkPos) && ReverseViewingPosCheck(checkPos);
    }

    // Check for collision from camera to player.
    bool ViewingPosCheck(Vector3 checkPos)
    {
        // Cast target and direction.
        Vector3 target = player.position + pivotOffset;
        Vector3 direction = target - checkPos;
        // If a raycast from the check position to the player hits something...
        if (Physics.SphereCast(checkPos, 0.2f, direction, out RaycastHit hit, direction.magnitude))
        {
            // ... if it is not the player...
            if (hit.transform != player && !hit.transform.GetComponent<Collider>().isTrigger)
            {
                // This position isn't appropriate.
                return false;
            }
        }
        // If we haven't hit anything or we've hit the player, this is an appropriate position.
        return true;
    }
    // Check for collision from player to camera.
    bool ReverseViewingPosCheck(Vector3 checkPos)
    {
        // Cast origin and direction.
        Vector3 origin = player.position + pivotOffset;
        Vector3 direction = checkPos - origin;
        if (Physics.SphereCast(origin, 0.2f, direction, out RaycastHit hit, direction.magnitude))
        {
            if (hit.transform != player && hit.transform != transform && !hit.transform.GetComponent<Collider>().isTrigger)
            {
                return false;
            }
        }
        return true;
    }


    // 添加一个方法，用于接收视角数据
    public void SetView(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;

        // 更新摄像机的水平和垂直角度参数，使其完全一致
        angleH = rotation.eulerAngles.y;
        angleV = rotation.eulerAngles.x;

        // 限制angleV在允许的范围内，避免超出最小和最大角度
        //angleV = Mathf.Clamp(angleV, minVerticalAngle, maxVerticalAngle);
    }

    public void Set_Paused(bool p)
    {
        isPaused = p;
    }


}

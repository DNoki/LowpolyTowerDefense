using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 上帝视角相机
/// カメラシステム
/// </summary>
public class GodPerspectiveCamera : MonoBehaviour
{
    /// <summary>
    /// 卷屏速度
    /// </summary>
    [SerializeField] private float trackSpeed = 20f;
    /// <summary>
    /// 水平旋转速度
    /// </summary>
    [SerializeField] private float panSpeed = 90f;
    /// <summary>
    /// 垂直旋转速度
    /// </summary>
    [SerializeField] private float tiltSpeed = 45f;
    /// <summary>
    /// 近摄拉摄速度
    /// </summary>
    [SerializeField] private float dollySpeed = 20f;

    /// <summary>
    /// 仰视最大角度限制
    /// </summary>
    [SerializeField] private float lookUpAngleLimit = 45f;
    /// <summary>
    /// 俯视最大角度限制
    /// </summary>
    [SerializeField] private float lookDownAngleLimit = 85f;

    /// <summary>
    /// 最大高度限制
    /// </summary>
    [SerializeField] private float maxHeightLimit = 10f;
    /// <summary>
    /// 最低高度限制
    /// </summary>
    [SerializeField] private float minHeightLimit = 5f;

    /// <summary>
    /// 边界限制
    /// </summary>
    public Bounds BorderLimit = new Bounds(Vector3.zero, new Vector3(200f, 0f, 200f));

    //public Transform CenterPosition = null;
    //public Camera MainCamera => Camera.main;
    //public Ray MainCameraCenterRay => this.MainCamera.ScreenPointToRay(new Vector3(this.MainCamera.pixelWidth * 0.5f, this.MainCamera.pixelHeight * 0.5f));
    public Vector3 ResetPosition = Vector3.zero;
    public Vector3 ResetRotation = Vector3.zero;

    /// <summary>
    /// 卷屏（平移相机）
    /// トラッキング（カメラを水平に移動する）
    /// </summary>
    private void CameraTrack()
    {
        if (InputCtrl.IsAlt) return;
        var move = Vector3.zero;

        var borderMouse = InputCtrl.BorderMouse;
        if (borderMouse.x > 0) move += Vector3.forward;
        if (borderMouse.y > 0) move += Vector3.back;
        if (borderMouse.z > 0) move += Vector3.left;
        if (borderMouse.w > 0) move += Vector3.right;

        move = Quaternion.LookRotation(Vector3.ProjectOnPlane(this.transform.forward, Vector3.up).normalized) * move;
        move = move * this.trackSpeed * Time.unscaledDeltaTime;

        if (this.transform.position.x + move.x < this.BorderLimit.min.x) move.x = Mathf.Max(0f, move.x);
        if (this.transform.position.x > this.BorderLimit.max.x) move.x = Mathf.Min(0f, move.x);
        if (this.transform.position.z < this.BorderLimit.min.z) move.z = Mathf.Max(0f, move.z);
        if (this.transform.position.z > this.BorderLimit.max.z) move.z = Mathf.Min(0f, move.z);
        this.transform.Translate(move, Space.World);
    }
    /// <summary>
    /// 水平旋转相机
    /// パン（カメラを水平に回転する）
    /// </summary>
    private void CameraPan()
    {
        if (!InputCtrl.IsAlt) return;

        var speed = 0f;
        RaycastHit hit;
        if (!Physics.Raycast(new Ray(this.transform.position, this.transform.forward), out hit, 1 << LayerMask.NameToLayer("Ground"))) return;

        if (InputCtrl.BorderMouse.z > 0) speed = -this.panSpeed * Time.unscaledDeltaTime;
        if (InputCtrl.BorderMouse.w > 0) speed = this.panSpeed * Time.unscaledDeltaTime;

        this.transform.RotateAround(hit.point, Vector3.up, speed);
    }
    /// <summary>
    /// 垂直旋转相机
    /// ティルト（カメラを垂直に回転する）
    /// </summary>
    private void CameraTilt()
    {
        if (!InputCtrl.IsAlt) return;

        var speed = 0f;
        if (InputCtrl.BorderMouse.x > 0) speed = -this.tiltSpeed * Time.unscaledDeltaTime;
        if (InputCtrl.BorderMouse.y > 0) speed = this.tiltSpeed * Time.unscaledDeltaTime;
        this.transform.Rotate(Vector3.right, speed, Space.Self);

        var x = Mathf.Clamp(this.transform.eulerAngles.x, this.lookUpAngleLimit, this.lookDownAngleLimit);
        this.transform.eulerAngles = new Vector3(x, this.transform.eulerAngles.y, 0);
    }
    /// <summary>
    /// 近摄拉摄
    /// </summary>
    private void CameraDolly()
    {
        var move = this.transform.forward * (InputCtrl.Wheel * this.dollySpeed);
        var delta = (this.transform.position + move).y;
        if (delta < this.minHeightLimit || delta > this.maxHeightLimit)
            return;
        this.transform.position += move;
    }

    private void Start()
    {
        if (this.transform.position.y < this.minHeightLimit)
            this.transform.position = this.transform.position.SetValue(null, this.minHeightLimit, null);
        if (this.transform.position.y > this.minHeightLimit)
            this.transform.position = this.transform.position.SetValue(null, this.maxHeightLimit, null);
    }
    private void Update()
    {
        if (InputCtrl.IsRKeyDown || InputCtrl.IsSpaceKeyDown)
        {
            //var ray = Camera.main.ScreenPointToRay(new Vector2(Camera.main.pixelWidth * 0.5f, Camera.main.pixelHeight * 0.5f));
            //RaycastHit hit;
            //Physics.Raycast(ray, out hit, float.MaxValue, LayerMask.GetMask("Ground"));
            //var move = this.CenterPosition.position.SetValue(null, 0f, null) - hit.point.SetValue(null, 0f, null);
            //this.transform.position += move;
            this.transform.position = this.ResetPosition;
            this.transform.eulerAngles = this.ResetRotation;
        }

        CameraTrack();
        CameraPan();
        CameraTilt();
        CameraDolly();
    }
}

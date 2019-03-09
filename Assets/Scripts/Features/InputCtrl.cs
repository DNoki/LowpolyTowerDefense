using UnityEngine;

/// <summary>
/// 入力システムの管理
/// </summary>
public static class InputCtrl
{
    public static Vector2 MousePosition { get { return Input.mousePosition; } }
    /// <summary>
    /// 主摄像机的鼠标射线
    /// </summary>
    public static Ray MainMouseRay => Camera.main.ScreenPointToRay(MousePosition);
    /// <summary>
    /// 鼠标射向地面的射线反馈
    /// </summary>
    public static RaycastHit MouseGroundPosition
    {
        get
        {
            RaycastHit hit;
            Physics.Raycast(MainMouseRay, out hit, float.MaxValue, LayerMask.GetMask("Ground"));
            return hit;
        }
    }
    public static float MouseX { get { return (Input.GetAxisRaw("Mouse X")); } }
    public static float MouseY { get { return (Input.GetAxisRaw("Mouse Y")); } }
    public static float Wheel { get { return Input.GetAxisRaw("Mouse ScrollWheel"); } }

    /// <summary>
    /// 鼠标于边界位置上 （上,下,左,右）
    /// </summary>
    public static Vector4 BorderMouse
    {
        get
        {
            var pos = MousePosition;
            var top = pos.y >= Camera.main.pixelHeight - 2 ? 1f : 0f;
            var bottom = pos.y <= 0 + 2 ? 1f : 0f;
            var left = pos.x <= 0 + 2 ? 1f : 0f;
            var right = pos.x >= Camera.main.pixelWidth - 2 ? 1f : 0f;
            return new Vector4(top, bottom, left, right);
        }
    }

    public static bool IsLeftMouse { get { return Input.GetMouseButton(0); } }
    public static bool IsLeftMouseDown { get { return Input.GetMouseButtonDown(0); } }
    public static bool IsLeftMouseUp { get { return Input.GetMouseButtonUp(0); } }
    public static bool IsRightMouse { get { return Input.GetMouseButton(1); } }
    public static bool IsRightMouseDown { get { return Input.GetMouseButtonDown(1); } }
    public static bool IsRightMouseUp { get { return Input.GetMouseButtonUp(1); } }


    public static float WS { get { return Input.GetAxisRaw("Vertical"); } }
    public static float AD { get { return Input.GetAxisRaw("Horizontal"); } }
    public static bool IsRKeyDown => Input.GetKeyDown(KeyCode.R);
    public static bool IsSpaceKeyDown => Input.GetKeyDown(KeyCode.Space);
    public static bool IsESCKeyDown => Input.GetKeyDown(KeyCode.Escape);
    //public static bool IsDeleteDown => Input.GetKeyDown(KeyCode.Delete);

    public static bool IsAlt { get { return Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt); } }
}
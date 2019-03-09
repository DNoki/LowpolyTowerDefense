using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiDebug : MonoBehaviour
{
    public static GuiDebug Instance { get; private set; } = null;

    public bool Enable = true;
    public Rect BoxRect = new Rect(0, 0, 100, 50);
    public Rect TextRect = new Rect(2, 10, 80, 100);

    private bool isRegist = false;
    private string logText = "";

    /// <summary>
    /// 上一次更新帧率的时间
    /// </summary>
    private float m_LastUpdateShowTime = 0f;
    /// <summary>
    /// 更新帧率的时间间隔
    /// </summary>
    public float UpdateShowDeltaTime = 0.5f;
    /// <summary>
    /// 帧数
    /// </summary>
    private int m_FrameUpdate = 0;
    private float m_FPS = 0;

    /// <summary>
    /// 当命令行输出信息时调用
    /// </summary>
    /// <param name="condition">条件</param>
    /// <param name="stackTrace">堆栈跟踪</param>
    /// <param name="type">类型</param>
    private void OnLogPrint(string condition, string stackTrace, LogType type)
    {
        this.logText = $"{type.ToString()} {condition}";
        //this.logText = $"{type.ToString()} {condition}{System.Environment.NewLine}{System.Environment.NewLine}{this.logText}";
        //if (this.logText.Length > 400)
        //    this.logText = $"{type.ToString()} {condition}";
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);

        if (!this.isRegist)
        {
            this.isRegist = true;
            Application.logMessageReceived += new Application.LogCallback(OnLogPrint);
        }
    }
    void Start()
    {
        this.m_LastUpdateShowTime = Time.realtimeSinceStartup;
    }
    void Update()
    {
        this.m_FrameUpdate++;
        if (Time.realtimeSinceStartup - this.m_LastUpdateShowTime >= this.UpdateShowDeltaTime)
        {
            this.m_FPS = this.m_FrameUpdate / (Time.realtimeSinceStartup - this.m_LastUpdateShowTime);
            this.m_FrameUpdate = 0;
            this.m_LastUpdateShowTime = Time.realtimeSinceStartup;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("StartMenu");
        }

        if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            Time.timeScale = Time.timeScale + 0.2f;
        }
        if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            var value = Time.timeScale - 0.2f;
            Time.timeScale = Mathf.Max(0f, value);
        }
        if (Input.GetKeyDown(KeyCode.Asterisk) || Input.GetKeyDown(KeyCode.KeypadMultiply))
        {
            Time.timeScale = 1f;
        }
    }
    private void OnGUI()
    {
        if (this.Enable)
        {
            //GUI.Box(this.BoxRect, $"Console TS:{Time.timeScale}  CurentFPS: {this.m_FPS.ToString("0.00")}");
            //GUI.TextArea(this.TextRect, this.logText);
            var screenSize = new Vector2(Screen.width, Screen.height);
            var boxRect = new Rect(this.BoxRect.position * screenSize, this.BoxRect.size * screenSize);
            var textRect = new Rect(this.TextRect.position * screenSize, this.TextRect.size * screenSize);
            GUI.Label(boxRect, $"TS:{Time.timeScale}  FPS: {this.m_FPS.ToString("0.00")}");
            GUI.Label(textRect, this.logText);
        }
    }
}

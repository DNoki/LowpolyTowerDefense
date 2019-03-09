using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static LayerMask UI_LAYER_MASK = 1 << 5;

    public Camera UiCamera = null;
    public MoneyUI Money = null;
    public BuildingListUI BuildingList = null;
    public OverUI OverWin = null;
    public OverUI OverLose = null;

    [Space(20f), SerializeField] private TipsUI tipsUiPrefab = null;
    [SerializeField] private TextBox textBoxPrefab = null;


    #region ObjectPool
    public static TipsPool TipsPoolObject { get; private set; }
    public class TipsPool : ObjectPool<TipsUI>
    {
        private TipsUI prefab = null;

        public List<TipsUI> DisplayList = new List<TipsUI>();
        protected override TipsUI CreateObject() => Instantiate(this.prefab);
        protected override void DestroyObject(TipsUI obj) => Destroy(obj.gameObject);

        /// <summary>
        /// 添加提示
        /// </summary>
        /// <param name="tips"></param>
        public void AddTips(TipsUI tips)
        {
            if (!this.DisplayList.Contains(tips))
                this.DisplayList.Add(tips);
        }
        /// <summary>
        /// 移除提示
        /// </summary>
        /// <param name="tips"></param>
        public void RemoveTips(TipsUI tips)
        {
            if (this.DisplayList.Contains(tips))
                this.DisplayList.Remove(tips);
        }
        public void ClearTips()
        {
            foreach (var t in this.DisplayList)
            {
                if (t != null)
                    this.Release(t);
            }
            this.DisplayList.Clear();
        }
        public override void ClearAllObj()
        {
            ClearTips();
            base.ClearAllObj();
        }

        public TipsPool(TipsUI prefab) : base(true, 1000) { this.prefab = prefab; }
    }
    public static TextBoxPool TextBoxPoolObject { get; private set; }
    public class TextBoxPool : ObjectPool<TextBox>
    {
        private TextBox prefab = null;
        protected override TextBox CreateObject() => Instantiate(this.prefab);
        protected override void DestroyObject(TextBox obj) => Destroy(obj.gameObject);
        public TextBoxPool(TextBox prefab) : base(true, 1000) { this.prefab = prefab; }
    }
    #endregion

    /// <summary>
    /// 创建提示
    /// </summary>
    /// <param name="lable"></param>
    /// <param name="title"></param>
    /// <param name="text"></param>
    public void CreateTips(string lable, string title, string text)
    {
        var tipsPool = TipsPoolObject;
        var tips = tipsPool.Take().SetText(lable, title, text);
        tips.transform.SetParent(this.transform, false);
        tips.RectTransform.anchoredPosition = tips.Position;

        if (tipsPool.DisplayList.Count > 0)
        {
            var y = tipsPool.DisplayList[tipsPool.DisplayList.Count - 1].HeightEndPosition + tips.PositionOffsetY;
            tips.RectTransform.anchoredPosition = new Vector2(tips.RectTransform.anchoredPosition.x, y);

            if (tips.RectTransform.anchoredPosition.y < tips.PositionMinLimitY)
            {
                tipsPool.ClearTips();
                tips.RectTransform.anchoredPosition = tips.Position;
            }
        }
        tipsPool.AddTips(tips);
    }

    public void Initialize()
    {
        UI_LAYER_MASK = LayerMask.GetMask("UI");

        if (TipsPoolObject == null) TipsPoolObject = new TipsPool(this.tipsUiPrefab);
        if (TextBoxPoolObject == null) TextBoxPoolObject = new TextBoxPool(this.textBoxPrefab);

        ClearPool();
    }
    public void ClearPool()
    {
        TipsPoolObject.ClearAllObj();
        TextBoxPoolObject.ClearAllObj();
    }

    private void Awake()
    {
        if (this.Money == null) this.Money = GetComponentInChildren<MoneyUI>();
        if (this.BuildingList == null) this.BuildingList = GetComponentInChildren<BuildingListUI>();
    }

}

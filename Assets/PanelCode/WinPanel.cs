using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WinPanel : PanelBase
{
    private Image winImage;
    private Image failImage;
    private Text text;
    private Button closeBtn;
    private bool isWin;

    #region 生命周期
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "WinPanel";
        layer = PanelLayer.Panel;
        //参数 args[1]代表获胜的阵营
        if (args.Length == 1)
        {
            int camp = (int)args[0];
            isWin = (camp == 1);
        }
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        //关闭按钮
        closeBtn = skinTrans.Find("CloseBtn").GetComponent<Button>();
        closeBtn.onClick.AddListener(OnCloseClick);
        //图片和文字
        winImage = skinTrans.Find("WinImage").GetComponent<Image>();
        failImage = skinTrans.Find("FailImage").GetComponent<Image>();
        text = skinTrans.Find("Text").GetComponent<Text>();
        //根据参数显示图片和文字
        if (isWin)
        {
            failImage.enabled = false;
            text.text = "孺子可教也！";
        }
        else
        {
            winImage.enabled = false;
            text.text = "朽木不可雕！";
        }
    }
    #endregion

    public void OnCloseClick()
    {
        PanelMgr.instance.OpenPanel<RoomPanel>("");
        Close();
    }
}



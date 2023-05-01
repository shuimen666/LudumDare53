using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager INSTANCE;
    public static UIManager get() { return INSTANCE; }

    #region ============距离============
    Transform distancePart;
    UIProgressBar distanceProgressBar;
    UILabel distanceLabel;
    void InitDistancePart()
    {
        distancePart = transform.Find("distance");
        distanceProgressBar = distancePart.GetComponent<UIProgressBar>("progress_bar");
        distanceLabel = distancePart.GetComponent<UILabel>("label");
    }
    #endregion

    #region ============时间============
    Transform timePart;
    UILabel timeLabel;
    void InitTimePart()
    {
        timePart = transform.Find("time");
        timeLabel = timePart.GetComponent<UILabel>("label");
    }
    #endregion

    #region ============快递============
    Transform boxPart;
    UILabel boxLabel;
    void InitBoxPart()
    {
        boxPart = transform.Find("box");
        boxLabel = boxPart.GetComponent<UILabel>("label");
    }
    #endregion

    #region ============中部信息框============
    Transform notePart;
    GameObject noteBtn;
    UILabel noteLabel, btnLabel;
    void InitNotePart()
    {
        notePart = transform.Find("note");
        noteLabel = notePart.GetComponent<UILabel>("label");
        noteBtn = notePart.Find("btn").gameObject;
        btnLabel = noteBtn.GetComponent<UILabel>("label");
        notePart.SetActive(false);
    }
    public void ShowNote(string mes, string btnMes, Action callback)
    {
        noteLabel.text = mes;
        btnLabel.text = btnMes;
        UIEventListener.Get(noteBtn).onClick = (x)=> { HideNote(); callback(); };
        notePart.SetActive(true);
    }
    public void HideNote()
    {
        notePart.SetActive(false);
    }
    #endregion

    #region ============语言============
    Transform languagePart;
    UILabel languageLabel;
    void InitLanguagePart()
    {
        languagePart = transform.Find("language");
        languageLabel = languagePart.GetComponent<UILabel>("btn/label");
        languageLabel.text = sm.__("language");
        UIEventListener.Get(languagePart.Find("btn").gameObject).onClick = (x) =>
        {
            sm.isEng = !sm.isEng;
            languageLabel.text = sm.__("language");
        };
    }
    public void HideLanguageBtn()
    {
        languagePart.Find("btn").gameObject.SetActive(false);
    }
    #endregion

    private void Awake()
    {
        INSTANCE = this;
        InitDistancePart();
        InitTimePart();
        InitBoxPart();
        InitNotePart();
        InitLanguagePart();
        RegisterEvents();
    }
    private void RegisterEvents()
    {
        EventControl.get().AddEventListener(sm.even.TURN_UPDATE, OnTurnUpdate);
        EventControl.get().AddEventListener(sm.even.BOX_UPDATE, OnBoxUpdate);
    }
    private void OnTurnUpdate(object sender, EventArgs e)
    {
        var nowDistance = GameManager.get().nowTurn.nowDistance;
        var maxDistance = GameManager.get().nowTurn.totalDistance;
        distanceProgressBar.value = nowDistance / maxDistance;
        distanceLabel.text = String.Format("[b]{0:F1}/{1:F1} km", nowDistance / 1000, maxDistance / 1000);

        if (GameManager.get().nowTurn.nowTime <= 0)
            timeLabel.text = "TIME OUT";
        else
            timeLabel.text = String.Format("{0:F1}" ,GameManager.get().nowTurn.nowTime);
    }
    private void OnBoxUpdate(object sender, EventArgs e)
    {
        var nowBox = GameManager.get().nowTurn.nowBoxNum;
        var maxBox = GameManager.get().nowTurn.needBoxNum;
        if(nowBox>=maxBox)
            boxLabel.text = String.Format("[b][00ff00]{0}[-][000000]/{1}[-]", nowBox, maxBox);
        else
            boxLabel.text = String.Format("[b][ff0000]{0}[-][000000]/{1}[-]", nowBox, maxBox);
    }
}

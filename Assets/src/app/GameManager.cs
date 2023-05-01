using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager INSTANCE;
    private void Awake() { INSTANCE = this; gameObject.AddComponent<AssetLoader>(); }
    public static GameManager get() { return INSTANCE; }

    [SerializeField]
    private SpriteRenderer roadSprite;
    [SerializeField]
    public Transform NGUIRoot;

    float mainSpeed_ = 10f;
    public float mainSpeed { get { return mainSpeed_; } set { mainSpeed_ = value; } }

    public Turn nowTurn;
    public int turnIdx = 1;
    private bool isGaming_ = false;
    [HideInInspector]
    public Car myCar = null;

    public SkillPack skillPack;

    private void Start()
    {
        RegisterEvents();
        //InitMyCar();
        skillPack = new SkillPack(UIManager.get().transform.Find("skill"));
        InitGuide();
    }
    private void InitMyCar()
    {
        if(myCar == null)
        {
            var go = CarFactory.get().GetCar(true);
            go.name = "my_car";
            myCar = go.GetComponent<Car>();
        }
        myCar.roadIdx = 4;
        myCar.speed = sm.MyNormalSpeed;
        myCar.InitPosition(sm.MyCarZ);
        mainSpeed = sm.MyNormalSpeed;
    }
    private void InitGuide()
    {
        turnIdx = 1;
        UIManager.get().ShowNote(sm.__("guide1"), sm.__("next"), () => {
            UIManager.get().ShowNote(sm.__("guide2"), sm.__("next"), () => {
                UIManager.get().ShowNote(sm.__("guide3"), sm.__("next"), () => {
                    UIManager.get().ShowNote(sm.__("guide4"), sm.__("next"), () =>
                    {
                        nowTurn = Turn.GetTurn(turnIdx);
                        nowTurn.Init();
                        InitMyCar();
                        isGaming_ = true;
                        UIManager.get().HideLanguageBtn();
                    });
                });
            });
        });
    }
    private void RegisterEvents()
    {
        EventControl.get().AddEventListener(sm.even.CHECK_FINISH, OnCheckFinish);
    }

    void Update()
    {
        if(isGaming_)
        {
            BgScroll();
            nowTurn.Update();
            AllMoveUpdate();
        }
    }
    void BgScroll()
    {
        roadSprite.material.mainTextureOffset += new Vector2(0, mainSpeed / sm.Offset2Pixel * Time.deltaTime);
    }

    #region ============整体移动============
    float deltaMainZ_ = 0f;
    float allMoveTime_ = 0f;
    public void DoAllMoveZ(float deltaZ, float time)
    {
        deltaMainZ_ = deltaZ / time;
        allMoveTime_ = time;
    }
    void AllMoveUpdate()
    {
        if(allMoveTime_>0)
        {
            allMoveTime_ -= Time.deltaTime;
            var list = CarFactory.get().GetCarsList();
            foreach(var car in list)
            {
                car.transform.localPosition += new Vector3(0, 0, deltaMainZ_ * Time.deltaTime);
            }
        }
    }
    #endregion

    #region ============检测结束============
    private void OnCheckFinish(object sender, EventArgs e)
    {
        if(isGaming_)
        {
            if(nowTurn.nowDistance>=nowTurn.totalDistance && nowTurn.nowBoxNum>=nowTurn.needBoxNum)
            {
                isGaming_ = false;
                WinFinish();
                return;
            }
            if(nowTurn.nowTime<=0)
            {
                UIManager.get().ShowNote(sm.__("you_lose"), sm.__("quit"),()=> {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();//打包编译后退出
#endif
                });
                isGaming_ = false;
                return;
            }
        }
    }
    public void WinFinish()
    {
        CarFactory.get().ClearAllCarsExceptMine();
        myCar.DoWinShow();
    }
    public void DoBetweenGame()
    {
        UIManager.get().ShowNote(sm.__("mid_note"), sm.__("next"), () => {
            //新动作
            List<int> ids = sm.tables.skill.getPool();
            for(var i=0;i<2;i++)
            {
                int idx = UnityEngine.Random.Range(0, ids.Count);
                Skill skill = new Skill(skillPack.getItemPrefab(), skillPack);
                skill.id = ids[idx];
                skillPack.bag.Add(skill);
            }

            skillPack.ShowMid();
        });
    }
    public void FinishSetSkill()
    {
        turnIdx++;
        if (turnIdx > sm.tables.turn.getMaxTurnID())
        {
            UIManager.get().ShowNote(sm.__("real_win"), sm.__("quit"), () => {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();//打包编译后退出
#endif
            });
        }
        else
        {
            nowTurn = Turn.GetTurn(turnIdx);
            nowTurn.Init();
            InitMyCar();
            isGaming_ = true;
        }
    }
    #endregion

    public float GetMyCarSpeed()
    {
        return myCar.speed;
    }
    public bool IsGaming()
    {
        return isGaming_;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Car : MonoBehaviour
{
    static float moveTime = 1f;

    int roadIdx_ = 3;
    public int roadIdx { get { return roadIdx_; } set { roadIdx_ = value; } }
    public float speed = 10f;
    private float accelerate = 0f;
    private float accTime = 0f;

    public bool myCar = true;
    public bool side = true;
    public bool colli = false;
    public bool race = false;
    public List<float> cds;

    Transform moveTrans, scaleTrans, rotateTrans;
    Camera finishCamera;
    private void Init()
    {
        moveTrans = transform.Find("move");
        scaleTrans = transform.Find("move/offset");
        rotateTrans = transform.Find("move/offset/rotate");
        boxsTrans = transform.Find("move/offset/rotate/boxes");
        if(myCar) finishCamera = transform.GetComponent<Camera>("camera");
        RefreshBoxes();
        cds = new List<float>();
        for(int i = 0; i < 4; i++) { cds.Add(0); }
    }
    public void InitPosition(float z = 0)
    {
        transform.localPosition = new Vector3(GetRoadX(), 0, z);
        if (!rotateTrans)
            rotateTrans = transform.Find("move/offset/rotate");
        rotateTrans.localEulerAngles = new Vector3(0, side ? 0 : 180, 0);
    }

    void TurnTo(int idx)
    {
        if (roadIdx > idx)
        {
            int num = roadIdx - idx;
            roadIdx = idx;
            TurnLeft_(num);
        }
        else if (roadIdx < idx)
        {
            int num = idx - roadIdx;
            roadIdx = idx;
            TurnRight_(num);
        }
        else
        {
            roadIdx = idx;
        }
    }
    void TurnLeft(int num = 1)
    {
        if(roadIdx > sm.MinRoadIdx + num - 1) {
            TurnTo(roadIdx - num);
        }
    }
    void TurnRight(int num = 1)
    {
        if (roadIdx < sm.MaxRoadIdx - num + 1)
        {
            TurnTo(roadIdx + num);
        }
    }
    DOTween moveXTween = null;
    void TurnLeft_(int num = 1)
    {
        if(myCar)
        {
            DOTween.Kill("moveX");
            transform.DOMoveX(GetRoadX(), num * moveTime).SetId("moveX");
        }
        else
            transform.DOMoveX(GetRoadX(), num * moveTime);
        rotateTrans.DORotate(new Vector3(0, side ? -30 : 180 + 30, 0), 0.4f * moveTime).SetEase(Ease.Linear).SetId("rotate");
        rotateTrans.DORotate(Vector3.zero, 0.6f * moveTime).SetDelay(0.4f * moveTime + (num-1)* moveTime).SetEase(Ease.Linear).SetId("rotate");
    }
    void TurnRight_(int num = 1)
    {
        if (myCar)
        {
            DOTween.Kill("moveX");
            transform.DOMoveX(GetRoadX(), num * moveTime).SetId("moveX");
        }
        else
            transform.DOMoveX(GetRoadX(), num * moveTime);
        rotateTrans.DORotate(new Vector3(0, side ? 30 : 180 - 30, 0), 0.4f * moveTime).SetEase(Ease.Linear).SetId("rotate");
        rotateTrans.DORotate(Vector3.zero, 0.6f * moveTime).SetDelay(0.4f * moveTime + (num - 1) * moveTime).SetEase(Ease.Linear).SetId("rotate");
    }
    void SpeedTo(float target, float time)
    {
        accTime = time;
        accelerate = (target - speed)/ time;
    }
    void Attack()
    {
        var go = CarFactory.get().GetMissile();
        var missile = go.GetComponent<Missile>();
        missile.speed = speed + 10;
        go.transform.localPosition = transform.localPosition;
    }

    public float GetRoadX(int idx = -1)
    {
        if (idx == -1) idx = roadIdx;
        return (idx - 3) * sm.OneRoadWidth - 3;
    }
    public Vector3 GetSpeedVector()
    {
        return rotateTrans.localEulerAngles.normalized * speed;
    }


    private void Start()
    {
        Init();
    }
    private void Update()
    {
        MoveUpdate();
        if (myCar)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                SettleActionList(0);
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                SettleActionList(1);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                SettleActionList(2);
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                SettleActionList(3);
            }
            var goList = GameManager.get().skillPack.getGoList();
            for(int i=0;i<cds.Count;i++)
            {
                if (cds[i] > 0) cds[i] -= Time.deltaTime;
                goList[i].GetComponent<UISprite>("cd").fillAmount = cds[i] / (0.8f * (i + 1));
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Box") && myCar)
        {
            other.transform.GetComponent<Box>().onCar = true;
            return;
        }
        if(other.CompareTag("Missile"))
        {
            if(!myCar)
            {
                Destroy(other.gameObject);
                CarFactory.get().Recycle(gameObject);
            }
            return;
        }

        colli = true;
        if (myCar) {
            GameManager.get().DoAllMoveZ(-0.9f, 0.8f);
            BombBox();
        }
        transform.DOScale(Vector3.one, 1f).OnComplete(()=> {
            colli = false;
            if (myCar)
            {
                SpeedTo(sm.MyNormalSpeed, 2f);
            }
            else
            {
                transform.GetComponent<Collider>().enabled = false;
            }
        });
        SpeedTo(0, 0.6f);
    }
    private void MoveUpdate()
    {
        if(accTime > 0)
        {
            speed += accelerate * Time.deltaTime;
            accTime -= Time.deltaTime;
        }
        if (speed != GameManager.get().mainSpeed)
        {
            if(side)
                transform.localPosition += new Vector3(0, 0, (speed - GameManager.get().mainSpeed) * sm.Speed2Pixel * Time.deltaTime * sm.CarSmooth);
            else
                transform.localPosition -= new Vector3(0, 0, (speed + GameManager.get().mainSpeed) * sm.Speed2Pixel * Time.deltaTime * sm.CarSmooth);
            if (myCar)
            {
                GameManager.get().mainSpeed = Mathf.Lerp(GameManager.get().mainSpeed, speed, Time.deltaTime * sm.BgSmooth);
            }
        }
        if(transform.localPosition.z < sm.DeadCarZ)
        {
            CarFactory.get().Recycle(gameObject);
        }
    }
    public void DoWinShow()
    {
        finishCamera.gameObject.SetActive(true);
        SpeedTo(0f, 0.6f);
        DOTween.Kill("rotate");
        rotateTrans.DORotate(new Vector3(0, -75, 0), 0.6f);
        DOTweenToTest(2f, ()=>
        {
            foreach (var box in boxes) { CarFactory.get().RecycleBox(box.go); }
            boxes.Clear();
            RefreshBoxes();
            finishCamera.gameObject.SetActive(false);
            GameManager.get().DoBetweenGame();
        });
    }
    public void InitRace()
    {
        if (!race || colli) return;
        DOTweenToTest(0.2f, () =>
        {
            if (roadIdx == 6) { TurnLeft(); return; }
            if (roadIdx == 1) { TurnRight(); return; }
            int n = Random.Range(1, 3);
            if (n == 1) TurnLeft();
            else TurnRight();
            DOTweenToLoopTest(1f, () =>
            {
                if (roadIdx == 6) { TurnLeft(); return; }
                if (roadIdx == 1) { TurnRight(); return; }
                int n = Random.Range(1, 3);
                if (n == 1) TurnLeft();
                else TurnRight();
            }, 5);
        });
    }

    #region ============Box============
    Transform boxsTrans;
    List<Box> boxes = new List<Box>();
    public void RefreshBoxes()
    {
        var size = 0f;
        for (int i = 0; i < boxes.Count; i++)
        {
            boxes[i].go.transform.SetParent(boxsTrans);
            boxes[i].go.transform.localPosition = new Vector3(0, size, 0);
            size += boxes[i].size;
        }
        if(myCar)
        {
            GameManager.get().nowTurn.nowBoxNum = boxes.Count;
            sm.EventInvoke(sm.even.BOX_UPDATE, this, System.EventArgs.Empty);
            sm.EventInvoke(sm.even.CHECK_FINISH, this, System.EventArgs.Empty);
        }
    }
    public void AddBox(Box box)
    {
        if(!boxes.Contains(box)) boxes.Add(box);
        RefreshBoxes();
    }
    private void BombBox()
    {
        foreach(var box in boxes)
        {
            box.roadIdx = Random.Range(1, 7);
            box.onCar = false;
            var collider = box.go.transform.GetComponent<Collider>();
            collider.enabled = false;
            var nowPos = box.go.transform.localPosition;
            var newPos = new Vector3(box.GetRoadX(), 1, Random.Range(10, 31));
            DOTween.To(setter: value => {
                box.go.transform.localPosition = sm.Parabola(nowPos, newPos, 6, value);
            }, startValue: 0, endValue: 1, duration: 1.5f).OnComplete(()=> {
                var collider = box.go.transform.GetComponent<Collider>();
                collider.enabled = true;
            });
        }
        boxes.Clear();
        RefreshBoxes();
    }
    #endregion

    #region ============处理技能============
    public void DOTweenToTest(float delayedTimer, System.Action callback)
    {
        float timer = 0;
        Tween t = DOTween.To(() => timer, x => timer = x, 1, delayedTimer)
                      .OnStepComplete(() =>
                      {
                          callback();
                      });
    }
    public void DOTweenToLoopTest(float delayedTimer, System.Action callback, int loopTimes)
    {
        float timer = 0;
        Tween t = DOTween.To(() => timer, x => timer = x, 1, delayedTimer)
                      .OnStepComplete(() =>
                      {
                          callback();
                      }).SetLoops(loopTimes);
    }
    public void SettleActionList(int idx)
    {
        if (colli) return;
        if (cds[idx] > 0) return;
        List<Skill> list = GameManager.get().skillPack.sList[idx];
        cds[idx] = list.Count * 0.8f;
        for(int i=0;i<list.Count;i++)
        {
            var skill = list[i];
            DOTweenToTest(0.8f*i,() =>
                      {
                          SettleAction(skill.id);
                      });
        }
    }
    public void SettleAction(int id)
    {
        if (id == -1) return;
        switch(id)
        {
            case 1:
                TurnLeft(1);
                break;
            case 2:
                TurnRight(1);
                break;
            case 3:
                TurnLeft(2);
                break;
            case 4:
                TurnRight(2);
                break;
            case 5:
                SpeedTo(Mathf.Max(0, speed - 2), 0.5f);
                break;
            case 6:
                SpeedTo(speed + 2, 0.5f);
                break;
            case 7:
                Attack();
                break;
        }
    }
    #endregion
}

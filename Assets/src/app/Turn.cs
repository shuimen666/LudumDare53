using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn
{
    public float levelTime;
    public float totalDistance;
    public int needBoxNum;
    public float leftP;
    public float leftPTime;
    public float rightP;
    public float rightPTime;
    public float boxPTime;
    public float policeP;
    public float policePTime;
    public float raceP;
    public float racePTime;

    public float nowTime;
    public float nowDistance;
    public float nowBoxNum;
    public float leftNowTime;
    public float rightNowTime;
    public float boxNowTime;
    public float policeNowTime;
    public float raceNowTime;

    private bool isStart = false;

    public static Turn GetTurn(int id)
    {
        var turnConf = sm.tables.turn.getTurn(id);
        return new Turn()
        {
            levelTime = System.Convert.ToSingle(turnConf["levelTime"]),
            totalDistance = System.Convert.ToInt32(turnConf["totalDistance"]),
            needBoxNum = System.Convert.ToInt32(turnConf["needBoxNum"]),
            leftP = System.Convert.ToSingle(turnConf["leftP"]),
            leftPTime = System.Convert.ToSingle(turnConf["leftPTime"]),
            rightP = System.Convert.ToSingle(turnConf["rightP"]),
            rightPTime = System.Convert.ToSingle(turnConf["rightPTime"]),
            boxPTime = System.Convert.ToSingle(turnConf["boxPTime"]),
            policeP = System.Convert.ToSingle(turnConf["policeP"]),
            policePTime = System.Convert.ToSingle(turnConf["policePTime"]),
            raceP = System.Convert.ToSingle(turnConf["raceP"]),
            racePTime = System.Convert.ToSingle(turnConf["racePTime"]),
        };
    }

    public Turn()
    {
        Init();
    }
    public void Init()
    {
        nowTime = levelTime;
        nowDistance = 0;
        nowBoxNum = 0;
        leftNowTime = 0;
        rightNowTime = 0;
        boxNowTime = 0;
        policeNowTime = 0;
        raceNowTime = 0;
        isStart = true;
    }

    public void Update()
    {
        if (!GameManager.get().IsGaming()) return;
        nowTime -= Time.deltaTime;
        nowDistance += Time.deltaTime * GameManager.get().GetMyCarSpeed();
        sm.EventInvoke(sm.even.TURN_UPDATE, this, System.EventArgs.Empty);
        if(nowTime<=0 || nowDistance >= totalDistance)
            sm.EventInvoke(sm.even.CHECK_FINISH, this, System.EventArgs.Empty);
        leftNowTime += Time.deltaTime;
        CheckP(true);
        rightNowTime += Time.deltaTime;
        CheckP(false);
        boxNowTime += Time.deltaTime;
        CheckBox();
        policeNowTime += Time.deltaTime;
        CheckPolice();
        raceNowTime += Time.deltaTime;
        CheckRace();
    }
    private void CheckP(bool left = true)
    {
        var time = left ? leftNowTime : rightNowTime;
        var pTime = left ? leftPTime : rightPTime;
        if (time>=pTime)
        {
            if (left) leftNowTime = 0;
            else rightNowTime = 0;

            var p = left ? leftP : rightP;
            var rn = Random.Range(0f, 1f);
            if (rn <= p)
            {
                var go = CarFactory.get().GetCar();
                var car = go.GetComponent<Car>();
                car.roadIdx = left ? Random.Range(1, 4) : Random.Range(4, 7);
                car.side = !left;
                car.speed = left ? Random.Range(1, 3) : Random.Range(4, 12);
                car.InitPosition(sm.CreateCarZ);
            }
        }
    }
    private void CheckBox()
    {
        if (boxNowTime >= boxPTime)
        {
            boxNowTime = 0;
            var go = CarFactory.get().GetBox();
            var box = go.GetComponent<Box>();
            box.roadIdx = Random.Range(1, 7);
            box.onCar = false;
            box.InitPosition(sm.CreateCarZ);
        }
    }
    private void CheckPolice()
    {
        if (policeNowTime >= policePTime)
        {
            policeNowTime = 0;

            var p = policeP;
            var rn = Random.Range(0f, 1f);
            if (rn <= p)
            {
                var roadIdx = Random.Range(2, 6);
                var left = roadIdx >= 3;
                var speed = left ? Random.Range(1, 3) : Random.Range(4, 12);

                var go = CarFactory.get().GetPolice();
                var car = go.GetComponent<Car>();
                car.roadIdx = roadIdx;
                car.side = left;
                car.speed = speed;
                car.InitPosition(sm.CreateCarZ);
                var go2 = CarFactory.get().GetPolice();
                var car2 = go2.GetComponent<Car>();
                car2.roadIdx = roadIdx + 1;
                car2.side = left;
                car2.speed = speed;
                car2.InitPosition(sm.CreateCarZ);
            }
        }
    }
    private void CheckRace()
    {
        if (raceNowTime >= racePTime)
        {
            raceNowTime = 0;

            var p = raceP;
            var rn = Random.Range(0f, 1f);
            if (rn <= p)
            {
                var roadIdx = Random.Range(4, 7);
                var left = roadIdx >= 4;
                var speed = left ? Random.Range(1, 3) : Random.Range(13, 16);

                var go = CarFactory.get().GetRace();
                var car = go.GetComponent<Car>();
                car.roadIdx = roadIdx;
                car.side = left;
                car.speed = speed;
                car.InitPosition(sm.CreateCarZ);
            }
        }
    }


    public bool IsStart() { return isStart; }
}

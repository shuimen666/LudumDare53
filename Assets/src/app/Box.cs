using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    static float BoxSpeedScale = 0.2f;
    static float BoxRotateTime = 2f;

    Transform offsetTrans, rotateTrans;

    public GameObject go;
    public float size;
    public int roadIdx;

    private bool onCar_ = false;
    public bool onCar {
        get { return onCar_; }
        set {
            onCar_ = value;
            if (onCar_)
            {
                go.transform.localScale = Vector3.one * 2;
                GameManager.get().myCar.AddBox(this);
                var collider = go.transform.GetComponent<Collider>();
                collider.enabled = false;
                offsetTrans.localScale = Vector3.one * size;
                offsetTrans.localPosition = new Vector3(0, (size - 1) / 2, 0);
            }
            else
            {
                go.transform.localScale = Vector3.one * 2;
                go.transform.SetParent(CarFactory.get().transform);
                var collider = go.transform.GetComponent<Collider>();
                collider.enabled = true;
                offsetTrans.localScale = Vector3.one * size;
                offsetTrans.localPosition = Vector3.zero;
            }
        }
    }

    public void Init(float size)
    {
        go = gameObject;
        this.size = size;
        offsetTrans = go.transform.Find("offset");
        rotateTrans = go.transform.Find("offset/rotate");
    }
    public void InitPosition(float z = 0)
    {
        transform.localPosition = new Vector3(GetRoadX(), 1, z);
    }
    public float GetRoadX(int idx = -1)
    {
        if (idx == -1) idx = roadIdx;
        return (idx - 3) * sm.OneRoadWidth - 3;
    }
    private void Update()
    {
        if (!onCar) {
            transform.localPosition -= new Vector3(0, 0, BoxSpeedScale * GameManager.get().mainSpeed * sm.Speed2Pixel * Time.deltaTime);
            rotateTrans.localEulerAngles += new Vector3(0, 360f / BoxRotateTime * Time.deltaTime, 0);
        }
        if (transform.localPosition.z < sm.DeadCarZ)
        {
            CarFactory.get().RecycleBox(gameObject);
        }
    }
}

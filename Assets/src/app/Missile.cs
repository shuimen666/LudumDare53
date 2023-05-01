using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public float speed = 20f;

    void Update()
    {
        transform.localPosition += new Vector3(0, 0, (speed - GameManager.get().mainSpeed) * sm.Speed2Pixel * Time.deltaTime * sm.CarSmooth);
        if (transform.localPosition.z >= sm.CreateCarZ)
        {
            Destroy(gameObject);
        }
    }
}

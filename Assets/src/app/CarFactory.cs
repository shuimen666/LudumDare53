using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarFactory : MonoBehaviour
{
    private static CarFactory INSTANCE;
    private void Awake() { INSTANCE = this; }
    public static CarFactory get() { return INSTANCE; }

    [SerializeField]
    List<GameObject> carPrefabs;
    [SerializeField]
    GameObject myCarPrefab;
    [SerializeField]
    GameObject missilePrefab;
    [SerializeField]
    GameObject boxPrefab;
    [SerializeField]
    GameObject policePrefab, racePrefab;

    List<Car> cars = new List<Car>();

    public GameObject GetCar(bool mine = false)
    {
        int n = Random.Range(0, carPrefabs.Count);
        var go = Instantiate(mine ? myCarPrefab : carPrefabs[n], transform);
        var car = go.AddComponent<Car>();
        car.myCar = mine;
        cars.Add(car);
        return go;
    }
    public GameObject GetPolice(bool mine = false)
    {
        var go = Instantiate(policePrefab, transform);
        var car = go.AddComponent<Car>();
        car.myCar = mine;
        cars.Add(car);
        return go;
    }
    public GameObject GetRace(bool mine = false)
    {
        var go = Instantiate(racePrefab, transform);
        var car = go.AddComponent<Car>();
        car.myCar = mine;
        car.race = true;
        car.InitRace();
        cars.Add(car);
        return go;
    }
    public GameObject GetBox(float size = -1)
    {
        if (size == -1) size = Random.Range(0.3f, 0.8f);
        var go = Instantiate(boxPrefab, transform);
        var box = go.AddComponent<Box>();
        box.Init(size);
        box.onCar = false;
        return go;
    }
    public GameObject GetMissile()
    {
        var go = Instantiate(missilePrefab, transform);
        var missile = go.AddComponent<Missile>();
        return go;
    }
    public void Recycle(GameObject go)
    {
        var car = go.GetComponent<Car>();
        if(car)
        {
            cars.Remove(car);
        }
        Destroy(go);
    }
    public void RecycleBox(GameObject go)
    {
        Destroy(go);
    }

    public List<Car> GetCarsList()
    {
        return cars;
    }
    public void ClearAllCarsExceptMine()
    {
        for (var i = 0; i < cars.Count; i++)
        {
            if (!cars[i].myCar)
            {
                Recycle(cars[i].gameObject);
                i--;
            }
        }
    }

}

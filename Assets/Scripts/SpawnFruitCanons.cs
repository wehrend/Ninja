using UnityEngine;
using Random = UnityEngine.Random;
using System;
using System.Collections;

public class SpawnFruitCanons : MonoBehaviour {

    public int fruitCanonsNumber = 5;

    public FruitCanon canonPrefab;
    void Awake()
    {
        CreateFruitCanons();
    }


    void CreateFruitCanons()
    {
        for (int i = 0; i < fruitCanonsNumber; i++) {
            Transform rotater = new GameObject("Rotater").transform;
            rotater.SetParent(transform, false);
            new WaitForSeconds(Random.Range(0,1));
            FruitCanon canonSpawner = Instantiate<FruitCanon>(canonPrefab);
            canonSpawner.transform.SetParent(rotater, false);
            //Vector2 pos_vec2 = Random.insideUnitCircle.normalized * 2;
            Vector3 position = new Vector3(0, 2,3);
            Debug.Log(position.ToString());
            canonSpawner.transform.localPosition = position;
        }
    }


    // Update is called once per frame
    void Update()
    {

    }
}

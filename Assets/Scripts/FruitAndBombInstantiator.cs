using UnityEngine;
using System.Collections;
using Random= UnityEngine.Random;
using System.Collections;
using System.Collections.Generic;

public class FruitAndBombInstantiator : MonoBehaviour {

    public float pausetime=5;
    public int angle = 90;
    public int maxInstances=3;
    public Transform target;
    public int fruitsLength = 3;
    public Fruit[] fruits;
    // public Bomb[] bombs;
	//public Trojan[] trojan;
    private Fruit[] activeFruits;
    //public Vector3 startPosition = new Vector3(-0.5f,0.5f,-0.5f);
    //public Vector3 endPosition = new Vector3(-0.1f,2f,-0.1f);


    void Start()
    {

        StartCoroutine(FireDelay());

    }



    IEnumerator FireDelay() {
        // As long as the Game is playing, later conditional
        while (true)
        {
            StartCoroutine(FireFruit());
            yield return new WaitForSeconds(pausetime);
        }
    }



    IEnumerator FireFruit()
    {

        //Debug.Log("Fire from "+ myTransform.position);
        //choose randomly from fruit prefabs and instantiate canon
        Fruit prefab = fruits[Random.Range(0, fruitsLength)];
        prefab.target = target;
        List<float> used_x = new List<float>();
        float x_max = 1.5f*Mathf.Sin(Mathf.Deg2Rad* (angle / 2)) + 1.5f; //1.5 offset of the vive-cube
        float x = Random.Range(-x_max, x_max);
        while(used_x.Contains(x))
        {
            x = Random.Range(-x_max, x_max);
            used_x.Add(x);
        }

        float z = Random.Range(8f, 10f);
        Vector3 position = new Vector3(x, 2f, z);

        // wait some small time
        yield return new WaitForSeconds(1.0f);
        // get amount of instantiated fruits
        activeFruits = FindObjectsOfType(typeof(Fruit)) as Fruit[];
        int count = activeFruits.Length;
        if (prefab != null  && (count < maxInstances))
        {
            Instantiate(prefab, position, Quaternion.identity);
            Debug.Log("Fruit Instantiated");

        }
    }
}

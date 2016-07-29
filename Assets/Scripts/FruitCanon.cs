using UnityEngine;
using System.Collections;

public class FruitCanon : MonoBehaviour {

    public float pausetime=5;
    public Transform target;
    public Fruit[] fruits;
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
        Fruit prefab = fruits[Random.Range(0, fruits.Length)];
        prefab.target = target;
        Vector3 position = new Vector3(Random.Range(-10f, 10f), 2f, Random.Range(8f, 10f));
        // wait some small time
        yield return new WaitForSeconds(1.0f);
        Instantiate(prefab, position, Quaternion.identity);
        Debug.Log("Fruit Instantiated");

    }
}

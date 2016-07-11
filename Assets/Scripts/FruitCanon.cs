using UnityEngine;
using System.Collections;

public class FruitCanon : MonoBehaviour {

    public float pausetime=1;
    //public float velocity = 1.5f;
    public float fireAngle = 42f;
    public float gravity = 9.8f;
    public float velocity = 1.2f;


    public Fruit[] fruits;
    //public Vector3 startPosition = new Vector3(-0.5f,0.5f,-0.5f);
    //public Vector3 endPosition = new Vector3(-0.1f,2f,-0.1f);
    public Transform target;
    private Transform throwValue;
    private Transform myTransform;

    void Awake()
    {
        myTransform = transform;
    }

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
        // wait some small time
        yield return new WaitForSeconds(1.0f);
        Fruit canon = Instantiate<Fruit>(prefab);
        // throwValue.position = myTransform.position; //+ some offset
        float targetDist = Vector3.Distance(myTransform.position, target.position);

        //calculate common velocity
        //float velocity = targetDist / (Mathf.Sin(2*fireAngle*Mathf.Deg2Rad)/gravity);

        //calculate 2D angle between target and x-axis
        Vector3 targetDir= target.position - myTransform.position;
        float phi = Vector3.Angle(targetDir, Vector3.right );

        //calculate all velocity components
        float vx = Mathf.Sqrt(velocity) * Mathf.Cos(phi*Mathf.Deg2Rad);
        float vz = Mathf.Sqrt(velocity) * Mathf.Sin(phi*Mathf.Deg2Rad);
        float vy = Mathf.Sqrt(velocity) * Mathf.Cos(fireAngle*Mathf.Deg2Rad);

        float flightTime = targetDist / vx;
        //face fruit forward to the face ;)
        canon.Body.rotation = Quaternion.LookRotation(target.position - myTransform.position);

        float elapse_time = 0;

        while(elapse_time < flightTime )
        {
            canon.Body.velocity = new Vector3(vx, (vy - (gravity * elapse_time)), vz);
            //canon.Body.AddForce(new Vector3(vx, vy,vz), ForceMode.Impulse);
            //canon.Body.AddForce(throwValue, ForceMode.Impulse);
            elapse_time += Time.deltaTime;
            yield return null;
        }

    }
}

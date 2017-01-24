using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class PresentRandomizedPoints : MonoBehaviour {

    public KeyCode startCode = KeyCode.Q;
    private bool showStimulus = false;

    [SerializeField]
    float startAfter = 8f;

    [SerializeField]
    float durationForEachPoint = 2f;

    [SerializeField]
    uint pointsType = 0;

    float timeFromStart;

    List<GameObject> points;

    int currentID;
    float currentElaplsed = 0f;

    bool bSimulationEnded = false;

    // Use this for initialization
    void Start () {
        timeFromStart = 0f;

        if (pointsType == 0)
        {
            points = GameObject.FindGameObjectsWithTag("CalibrationValidationPoint").ToList<GameObject>();//GetComponentsInChildren<Renderer>().ToList<Renderer>();
            Debug.Log("points: " + points.ToString());
            foreach (GameObject p in points)
            {
                p.SetActive(true);
            }
            Debug.Log(points.Count);
            return;
        }
        if (pointsType == 1)
        {
            List<GameObject> points_cal = GameObject.FindGameObjectsWithTag("CalibrationPoint").ToList<GameObject>();//GetComponentsInChildren<Renderer>().ToList<Renderer>();
            Debug.Log("pointscal: " + points_cal.ToString());
            points = GameObject.FindGameObjectsWithTag("CalibrationPoint").ToList<GameObject>();//GetComponentsInChildren<Renderer>().ToList<Renderer>();
            points.Clear();

            //Quaternion.FromToRotation(camera.transform.position,)
            //points_val[0].transformrotation(1, 1, 1);

            for (int i = 0; i < points_cal.Count; i++)
            {
                int ind = i + 1;
                string name = "StimulusPoint " + "(" + ind + ")";
                //Debug.Log(name);
                //Debug.Log(points[i].name);

                for (int j = 0; j < points_cal.Count; j++)
                {
                    bool result = name.Equals(points_cal[j].name, StringComparison.Ordinal);
                    if (result)
                    {
                        points.Add(points_cal[j]);
                        //Debug.Log(name);
                        //Debug.Log(points[i].name);
                    }
                }

            }
        }

        else if (pointsType == 2)
        {
            List<GameObject> points_val = GameObject.FindGameObjectsWithTag("ValidationPoint").ToList<GameObject>();//GetComponentsInChildren<Renderer>().ToList<Renderer>();
            Debug.Log("pontsval: " + points_val[0].ToString());
            points = GameObject.FindGameObjectsWithTag("ValidationPoint").ToList<GameObject>();//GetComponentsInChildren<Renderer>().ToList<Renderer>();
            points.Clear();

            //Quaternion.FromToRotation(camera.transform.position,)
            //points_val[0].transformrotation(1, 1, 1);

            for (int i = 0; i < points_val.Count; i++)
            {
                int ind = i + 7;
                string name = "StimulusPoint " + "(" + ind + ")";
                //Debug.Log(name);
                //Debug.Log(points[i].name);

                for (int j = 0; j < points_val.Count; j++)
                {
                    bool result = name.Equals(points_val[j].name, StringComparison.Ordinal);
                    if (result)
                    {
                        points.Add(points_val[j]);
                        //Debug.Log(name);
                        //Debug.Log(points[i].name);
                    }
                }

            }

            //for (int i = 0; i < points.Count; i++)
            //{
            //    Debug.Log(points[i].name);
            //}
        }

        // Shuffle points (Randomization)
        if (pointsType == 2)
        {
            System.Random rnd = new System.Random(-1);
            List<GameObject> preShuffled = points.OrderBy(x => rnd.Next()).ToList<GameObject>();

            points.Clear();
            points.Add(preShuffled[0]);
            preShuffled.RemoveAt(0);
            while (preShuffled.Count > 2)
            {
                double d1 = (preShuffled[0].transform.position - points[points.Count - 1].transform.position).magnitude; //distance(m_validationPoints[0], validationPointsNew[validationPointsNew.size() - 1]);
                double d2 = (preShuffled[1].transform.position - points[points.Count - 1].transform.position).magnitude; //distance(m_validationPoints[1], validationPointsNew[validationPointsNew.size() - 1]);
                double d3 = (preShuffled[2].transform.position - points[points.Count - 1].transform.position).magnitude; //distance(m_validationPoints[2], validationPointsNew[validationPointsNew.size() - 1]);
                if (d1 > d2 && d1 > d3)
                {
                    points.Add(preShuffled[0]); //validationPointsNew.push_back(m_validationPoints[0]);

                    preShuffled.RemoveAt(0); //m_validationPoints.erase(m_validationPoints.begin());
                }
                else if (d2 > d3)
                {
                    points.Add(preShuffled[1]); //validationPointsNew.push_back(m_validationPoints[1]);
                    preShuffled.RemoveAt(1); //m_validationPoints.erase(m_validationPoints.begin() + 1);
                }
                else
                {
                    points.Add(preShuffled[2]); //validationPointsNew.push_back(m_validationPoints[2]);
                    preShuffled.RemoveAt(2); //m_validationPoints.erase(m_validationPoints.begin() + 2);
                }

            }
            //validationPointsNew.push_back(m_validationPoints[0]);
            //validationPointsNew.push_back(m_validationPoints[1]);

            points.Add(preShuffled[0]);
            points.Add(preShuffled[1]);
        }

        currentID = 0;

        //Debug.Log(points.Count);
        foreach (GameObject p in points)
        {
            p.SetActive(false);
        }


    }

    void NewUpdate()
    {
        currentElaplsed += Time.deltaTime;

        if (currentElaplsed > durationForEachPoint)
        {
            bSimulationEnded = true;

            Debug.Log("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            Debug.Log(points.Count);
            for (int i = 0; i < points.Count; i++)
            {
                points[i].SetActive(false);
            }
        }

    }
    // Update is called once per frame
    void Update () {
        //return;
        if (Input.GetKeyDown(startCode))
        {
            showStimulus = true;
        }

        if (pointsType == 0)
        {
            if (!bSimulationEnded)
                NewUpdate();
            return;
        }

        timeFromStart += Time.deltaTime;
        if(showStimulus) //timeFromStart > startAfter
        {
            Debug.Log("show stimulus");
            currentElaplsed += Time.deltaTime;

            //if (currentID == 0)
                //Debug.Log("Activate a point: " + points[currentID].name);

            for (int i =0; i< points.Count; i++)
            {
                if (i == currentID)
                {
                    Debug.Log("set point (" + points[i].transform.name + ") active");
                    points[i].SetActive(true);
                }
                else
                {
                    Debug.Log("set point (" + points[i].transform.name + ") inactive");
                    points[i].SetActive(false);
                }
            }
            if(currentElaplsed > durationForEachPoint)
            {
                currentElaplsed = 0f;
                currentID++;
                if(currentID > points.Count)
                {
                    Debug.Log("grid end");
                    showStimulus = false;
                }
                //if(currentID<points.Count)
                    //Debug.Log("Activate a point: " + points[currentID].name);
            }
        }
	}
}


using UnityEngine;
using System.Collections;

/// <summary>
/// Not in use yet.
/// </summary>

public class FadingEffect : MonoBehaviour {

    public float fadeTime = 2f;
    public Color startColor;
    public Color endColor; 


    void Awake()
    {
        startColor = GetComponent<Renderer>().material.color;
        endColor = startColor;
        endColor.a = 0.0f;
        Debug.Log("startcolor:" + startColor + "endcolor:" + endColor);
    }

	void Update()
    {
        StartCoroutine(Fading());
	}


    IEnumerator Fading()
    {
        for (float t = 0.0f; t < fadeTime; t += Time.deltaTime)
        {
            startColor = Color.Lerp(startColor, endColor, t / fadeTime);
            GetComponent<Renderer>().material.SetColor("_Color", startColor);
            yield return null;
        }
    }
}

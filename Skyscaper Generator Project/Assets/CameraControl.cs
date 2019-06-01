using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    public GameObject skyscraper;
    public GameObject target2;
    public GameObject p0;
    public GameObject cp0a;
    public GameObject cp0b;
    public GameObject p1;
    public GameObject cp1a;
    public GameObject cp1b;
    public GameObject p2;

    public List<GameObject> points = new List<GameObject>();
    public float distanceFromSckyscraper = 1f;
    public float yLift = 0f;
    public float xDistance = 0f;
    // Use this for initialization
    void Start ()
    {
        xDistance = p0.transform.position.x;//highest x out of cp's

        points.Add(p0);
        points.Add(cp0a);
        points.Add(cp0b);
        points.Add(p1);
        points.Add(cp1a);
        points.Add(cp1b);
        points.Add(p2);
    }
	
	// Update is called once per frame
	void Update ()
    {

        xDistance = p0.transform.position.x;//highest x out of cp's
        foreach (GameObject go in points)
            if (go.transform.position.x > xDistance)
                xDistance = go.transform.position.x;
        Vector3 yHigh = new Vector3(0f, p2.transform.position.y, 0f);
         Vector3 halfWay = (Vector3.Lerp(skyscraper.transform.position, yHigh, .55f));
        Vector3 t = gameObject.transform.position;
        float distance = target2.transform.position.y;
        Vector3 toScraper = (gameObject.transform.position - p2.transform.position).normalized;
        gameObject.transform.position = new Vector3(0f, p2.transform.position.y + yLift, (distance + distanceFromSckyscraper + xDistance));

        gameObject.transform.LookAt(halfWay);

        //add p0.x, use, cp2.y
	}
}

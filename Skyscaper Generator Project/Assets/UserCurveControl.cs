using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserCurveControl : MonoBehaviour {
    public GameObject a;
    public GameObject a1;
    public GameObject a2;
    public GameObject b;
    public GameObject b1;
    public GameObject b2;
    public GameObject c;

    public GameObject x0;
    public GameObject x1;
    public GameObject x2;
    public GameObject x3;
    public GameObject x4;
    public GameObject x5;
    public GameObject x6;
    

    public GameObject cam;


    List<GameObject> aObj;
    List<GameObject> xObj;

    public float x = 5f;

    public LineRenderer lR;
    // Use this for initialization
    void Start ()
    {
         aObj = new List<GameObject>() { a, a1, a2, b, b1,b2, c };
        xObj = new List<GameObject>() { x0, x1, x2, x3, x4, x5, x6 };
    }
	
	// Update is called once per frame
	void Update ()
    {
        for (int i = 0; i < aObj.Count; i++)
        {
            aObj[i].transform.position = xObj[i].transform.position + x*Vector3.right;
        }

        
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public int amount = 10;
    public int space = 100;
    // Use this for initialization
    void Start()
    {
        for (int i = -amount/2; i < amount/2; i++)
        {
            for (int j = -amount/2; j < amount/2; j++)
            {
                GameObject g = new GameObject();
                g.transform.position = new Vector3(i*space, 0f, j*space);
                g.AddComponent<RoundedPolygonMaker>();
                g.AddComponent<MeshRenderer>();
                g.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Post0") as Material;
                g.AddComponent<MeshFilter>();
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampPostMaker : MonoBehaviour {

    public float lampPostHeight = 4f;

    //how many areas the lamp post shaft is split in to, differect sections/patterns running up the shaft
    [Range(1, 20)]
    public int sections = 10;

    [Range(1, 20)]
    public int shaftOverallDetail = 20;//max circle detail
    //public float segHeight = .5f;
    public float radius = .3f;
    public float tipModX = 2f;
    public int stability = 3;//higher is less stable

    public int shaft0Detail = 3;
    public int shaft1Detail = 6;

    void Start()
    {
        Mesh mesh = MakeMesh();


        gameObject.GetComponent<MeshFilter>().mesh = mesh;
    }
    
    Mesh MakeMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        List<Vector3> verticeList = new List<Vector3>();

        Vector3 lastCentre = gameObject.transform.position;

        float[] ringWidths = new float[sections+1];
        int[] ringSections = new int[sections + 1];
        for (int i = 0; i <= sections; i++)
        {
            ringWidths[i] = 1f -( i * .2f);

            
        }

        ringSections[0] = 4;
        ringSections[1] = 8;
        //ringSections[0] = 4;

        float tolerance = 0.0001f;
        int ringsSoFar = 0; //for tris
        float sectionHeight = lampPostHeight / sections;
        for (float y = 0; y < lampPostHeight + tolerance; y+=sectionHeight)        
        {
            //choose how detailed this part of the shaft is
            
            int shaftDetail = ringSections[ringsSoFar];

            //spin around axis (target direction)
            Vector3 targetDirection = Vector3.up;
            //add to current height
            Vector3 newCentre = lastCentre + targetDirection * sectionHeight;
            if (y == 0)
                newCentre = gameObject.transform.position;

            float step = (Mathf.PI * 2) / shaftDetail;
            //this loop creates points arounda  circle. The half step starting point is to create a an equal pattern throughout the rings as they go upwards
            for (float j = step/2; j < Mathf.PI * 2 + step/2 + tolerance;  j += step) //a circle split in to segments ( + tolerance if pi is divided by ten, it needs this tolerance
            {
                //spin
                Vector3 point = GetPerpendicularAtAngle(targetDirection, j);
                float rThis = radius;
                rThis = ringWidths[ringsSoFar];
                point *= rThis;
                point += newCentre;

                //to create a flattened circle shape (rounded rectangle), clamp the distance of the point
                
                vertices.Add(newCentre + point);
                
                GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                c.transform.position = newCentre + point;
                c.transform.name = (vertices.Count-1).ToString();
                c.transform.localScale *= 0.1f;


                
            }

            if (ringsSoFar == 1)
            {
                //if we are stepping up in detail
                if (shaftDetail > ringSections[ringsSoFar - 1] - 1)
                {
                    //how to get point for ring underneath.. get nearest int to upper ring count / lower count
                    float f = (float)(ringSections[ringsSoFar - 1]) / shaftDetail;
                    for (int i = 0; i < shaftDetail; i++)
                    {                        
                        
                        //round to nearest int( nearest vertices on ring below)
                        int c = Mathf.RoundToInt(f * i);
                        //add to ring starting point (ring below that is), working back from vertices count using ring details
                        int ringStartingPoint = vertices.Count - shaftDetail - 1 - ringSections[ringsSoFar - 1] - 1;

                        triangles.Add(vertices.Count - shaftDetail - 1 + i);
                        triangles.Add(ringStartingPoint + c);
                        triangles.Add(vertices.Count - shaftDetail + i);

                        
                    }
                    f = (float)(shaftDetail) / (ringSections[ringsSoFar - 1]);
                    //int ringStartingPoint = vertices.Count - shaftDetail - 1 - ringSections[ringsSoFar - 1] - 1;
                    
                    

                    for (float i = 0; i < shaftDetail; i+=f)
                    {
                        int c = Mathf.RoundToInt(i);
                        
                        Debug.Log(c);
                        //bottom up
                      //  triangles.Add(ringStartingPoint + i);
                       // triangles.Add(ringStartingPoint + i + 1);
                        //triangles.Add(vertices.Count - 1 - shaftDetail -2 + (i * c));
                    }
                }    
                else
                {
                    //ring below start
                    int ringStartingPoint = vertices.Count - shaftDetail - 1 - ringSections[ringsSoFar - 1] - 1;
                    float f = (float)(shaftDetail) / (ringSections[ringsSoFar - 1]);
                 
                    for (int i = 0; i < ringSections[ringsSoFar - 1] ; i++)
                    {
                        //if we are making the segemtsn simpler/less, then we need flip the fraction and work out the triangles upside down

                                               
                        //round to nearest int( nearest vertices on ring above)
                        int c = Mathf.RoundToInt(f * i);

                         

                         triangles.Add(ringStartingPoint + i);
                         triangles.Add(ringStartingPoint + i + 1);
                         triangles.Add(vertices.Count -1 - shaftDetail + c);

                        
                        //    triangles.Add(vertices.Count - 1 - shaftDetail + c);
                          //  triangles.Add(ringStartingPoint + i + 1);
                           // triangles.Add(vertices.Count - 1 - shaftDetail + c + 1);
                        
                        GameObject ca = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        ca.transform.position = vertices[ringStartingPoint];
                        
                        ca.transform.localScale *= 0.2f;

                    }

                    //f = (float)(ringSections[ringsSoFar - 1]) / shaftDetail;
                    Debug.Log(f);
                    for (int i = 0; i < shaftDetail; i++)
                    {
                        int c = Mathf.RoundToInt(f * i);
                        
                    }

                }
            }

            lastCentre = newCentre;
            //lastSegment++;
            ringsSoFar++;
        }





        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();


        //seperate verts for flat shading
        bool flatShading = true;
        if (flatShading)
        {
            Vector3[] oldVerts = mesh.vertices;
            int[] triangles2 = mesh.triangles;
            Vector3[] vertices2 = new Vector3[triangles2.Length];
            for (int i = 0; i < triangles2.Length; i++)
            {
                vertices2[i] = oldVerts[triangles2[i]];
                triangles2[i] = i;
            }

            mesh.vertices = vertices2;
            mesh.triangles = triangles2;
        }

        mesh.RecalculateNormals();

        return mesh;
    }


    Vector3 GetPerpendicularAtAngle(Vector3 v, float angle)
    {
        //slighlty altered from https://gamedev.stackexchange.com/questions/120980/get-perpendicular-vector-from-another-vector

        // Generate a uniformly-distributed unit vector in the XY plane.
        Vector3 inPlane = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);

        // Rotate the vector into the plane perpendicular to v and return it.
        return Quaternion.LookRotation(v) * inPlane;
    }


}

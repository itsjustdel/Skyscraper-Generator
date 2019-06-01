using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundedPolygonMakerBranchedBackup : MonoBehaviour {

    //http://mathworld.wolfram.com/RoundedRectangle.html //from this idea

    [Range(0f, 5f)]
    public float size = .3f;
    public int segments = 8;
    public float segmentHeight = 0f;

    [Range(1, 20)]
    public int sides = 4;
    [Range(1, 6)]
    public int roundnessSize = 1;
    [Range(1, 20)]
    public int roundnessDetail = 2;

    private float tolerance = 0.001f;

    public float respawnTime = 1f;
    private float respawnCounter = 0f;
    // Use this for initialization
    void Start()
    {

        MakePost();
    }



    void MakePost()
    {


        

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        Vector3 targetDirection = Vector3.up;

        //shape defining randoms
        int[] weightedSides = new int[] { 2, 2, 3, 3, 3, 4,4, 4,5, 5, 5,6, 6, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
        sides = weightedSides[Random.Range(0, weightedSides.Length)];
        

        //inner radius limit
        float totalStepSize = size / 4;// Random.Range(1,1); //random?
        float baseHeight = 1f;//var needed
                              //set static change on step (standard step height
                              //float segmentHeightStatic = baseHeight / Random.Range(2, 10);
                              //segmentHeightStatic = .5f;
        float segmentSizeStatic = totalStepSize/ Random.Range(2f,6f);
        //changes if steps get bigger or smaller
        float segmentHeightAdd = baseHeight / Random.Range(20,100);//?
        float segmentHeightStatic = baseHeight / 20;
            
        
        //stretch or squash ascension
        bool stretch = (Random.value > 0.5f);
        //steps or smooth
        bool smooth = (Random.value > 0.5f);
        int baseType = Random.Range(0, 4);
       // baseType = 2;
        // 0 is smooth slope with exponentials, creating curved ascensions
        // 1 is stepped with exponentials
        // 2 is stepped but uniform
        // 3 is sloped but uniform

        
        //smooth = true;
        //stretch = true;
        //float segmentSizeAdd = Random.Range(-.1f, .1f);
        roundnessSize = Random.Range(1, 7);//1 is a circle, anything beyond 6,7 is too small and detailed for what we need. the higher the number, the further  it pushes the cuver in to the corner, making it smaller
             
        float lastHeight = 0f;
                    
        for (int q = 0; q < 100; q++)
        {
            //we can make steps get bigger, or smaller, or stay the same
            //create a step every two rings
            //we either change the size/raduis, or we change the height to make straight flat steps
            if (baseType == 0)
            {
              //  Debug.Log("smooth");
                if (q > 0)
                {
                    size -= segmentSizeStatic;
                    if (stretch)
                    {
                        segmentHeight += (segmentHeightAdd * q);
                     //   Debug.Log("here 0");
                    }
                    else
                    {
                        segmentHeight += (segmentHeightAdd/q)*10;//?
                    //    Debug.Log("here 1");
                    }
                }

            }
            else if (baseType == 1)
            {
                //Debug.Log("not smooth");
                if (q % 2 == 0 )
                {                    
                    size -= segmentSizeStatic;
                }
                else
                {
                    if (stretch)
                    {
                        segmentHeight += (segmentHeightAdd * q);
                    //    Debug.Log("here 2");
                    }
                    else
                    {
                        segmentHeight += (segmentHeightAdd / q)*10;//?
                   //     Debug.Log("here 3");
                    }
                }
            }
            else if (baseType == 2)
            {
                //uniform steps
               // Debug.Log("uniform steps");
                if (q % 2 == 0)
                {
                    size -= segmentSizeStatic;
                }
                else
                {
                    if (!stretch)
                        //this gives straight slope
                        segmentHeight += segmentHeightStatic;
                    else
                        //this stretches it up a little
                        segmentHeight += segmentSizeStatic + segmentHeightAdd;
                }
            }
            else if(baseType == 3)
            {
               // Debug.Log("uniform slope");
                size -= segmentSizeStatic;
                if (q > 0)
                {
                    if(!stretch)
                        //this gives straight slope
                        segmentHeight += segmentHeightStatic;
                    else
                        //this stretches it up a little
                        segmentHeight += segmentSizeStatic + segmentHeightAdd;
                }

            }

            //we need to alter the roundness depedning on size, use the variable to control how it looks still
            float tempRoundnessSize = size/roundnessSize;

            if ( segmentHeight >= baseHeight || size <= totalStepSize)
            {
                //  vertices = AddRing(out triangles, vertices, triangles, targetDirection, sides, size, segmentHeight, tempRoundnessSize, roundnessDetail, tolerance, q);
                /*
                vertices = AddRing(out triangles, vertices, triangles, targetDirection, sides, 0f, lastHeight, tempRoundnessSize, roundnessDetail, tolerance, q);
                  */
                size = 0.0f;
                tempRoundnessSize = size;/// roundnessSize;

                vertices = AddRing(out triangles, vertices, triangles, targetDirection, sides, size, segmentHeight, tempRoundnessSize, roundnessDetail, tolerance, q);

                break;
            }
            lastHeight = segmentHeight;

            vertices = AddRing(out triangles, vertices, triangles, targetDirection, sides, size, segmentHeight, tempRoundnessSize, roundnessDetail, tolerance, q);

        }

        //we have created the base. Now to the main stem of the post
        //choose shape
        

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        if(triangles.Count == 0)
        {
            Debug.Log("no tris");
            Debug.Log("smooth = " + smooth);
            Debug.Log("stretch = " + stretch);
        }

    }

    public static List<Vector3> AddRing(out List<int> trianglesReturned, List<Vector3> vertices,List<int> triangles, Vector3 targetDirection,int sides, float size, float segmentHeight,float roundnessSize,float roundnessDetail, float tolerance, int ringsSofar)
    {
        int y = ringsSofar;
        //how many corners our shape has
        float step = (Mathf.PI * 2) / sides;

        //this loop creates points arounda  circle. The half step starting point is to create a an equal pattern throughout the rings as they go upwards

        //keep a count of vertices for a ring- i could work this out but this is easier
        int verticesThisRing = 0;
        for (float i = 0, j = 0; i <= Mathf.PI * 2 + tolerance; i += step, j++) //a circle split in to segments ( + tolerance if pi is divided by ten, it needs this tolerance
        {

            Vector3 point = GetPerpendicularAtAngle(targetDirection, i);
            Vector3 innerPoint = point * (size - roundnessSize);


            //add segment height
            point += targetDirection * segmentHeight;
            innerPoint += targetDirection * segmentHeight;

            /*
            GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.position = point * size;
            c.transform.localScale *= 0.2f;

            c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.position = innerPoint;
            c.transform.localScale *= 0.1f;
            */
            float roundnessStep = (Mathf.PI * 2) / (roundnessDetail * 10);

            float endAt = ((Mathf.PI * 2) / sides) / 2;
            //a fraction multiplied by what corner we are on math.pi*2 is a full circle, split it by sides, and multiply by j (keeps track of what cornder we are on)
            float spin = ((Mathf.PI * 2) / sides) * j;

            for (float k = -endAt + spin; k <= endAt + spin; k += roundnessStep)
            {
                Vector3 circlePoint = GetPerpendicularAtAngle(targetDirection, k);
                circlePoint *= roundnessSize;

                /*
                GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                c.transform.position = innerPoint + circlePoint;
                c.transform.localScale *= 0.05f;
                */
                vertices.Add(circlePoint + innerPoint);

                verticesThisRing++;
            }


        }
        //triangles, build back the way, from second ring to first
        if (y > 0)
        {
            for (int a = 0; a < verticesThisRing - 1; a++)
            {
                triangles.Add(a + (y * verticesThisRing));
                triangles.Add((a + 1) + ((y - 1) * verticesThisRing));
                triangles.Add(a + 1 + (y * verticesThisRing));

                triangles.Add(a + (y * verticesThisRing));
                triangles.Add((a) + ((y - 1) * verticesThisRing));
                triangles.Add((a + 1) + ((y - 1) * verticesThisRing));
            }
        }
        trianglesReturned = triangles;
        return vertices;
    }

   public static Vector3 GetPerpendicularAtAngle(Vector3 v, float angle)
    {
        //slighlty altered from https://gamedev.stackexchange.com/questions/120980/get-perpendicular-vector-from-another-vector

        // Generate a uniformly-distributed unit vector in the XY plane.
        Vector3 inPlane = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);

        // Rotate the vector into the plane perpendicular to v and return it.
        return Quaternion.LookRotation(v) * inPlane;
    }
}

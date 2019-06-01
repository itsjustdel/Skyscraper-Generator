using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshCollider))]

public class GizmosController : MonoBehaviour
{
    public bool first = false;
    void OnMouseDrag()
    {
        float distance_to_screen = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        Vector3 pos_move = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance_to_screen));

        //if (pos_move.y > 100f)
        //    pos_move.y = 100f;

        if (pos_move.y < 0f)
            pos_move.y = 0f;

        if (pos_move.x < 15f)
            pos_move.x = 15f;


        if (!first)
        transform.position = new Vector3(pos_move.x, pos_move.y, 0f);
        else if (first)
            transform.position = new Vector3(pos_move.x, 0f, 0f);
    }

}
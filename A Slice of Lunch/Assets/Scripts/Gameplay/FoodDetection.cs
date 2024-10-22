using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FoodDetection : MonoBehaviour
{
    private Collider2D container;
    // Start is called before the first frame update
    void Awake()
    {
        container = GetComponent<Collider2D>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerStay2D(Collider2D col)
    {
        Debug.Log(col.transform.parent.name + " is fully in box: " + IsFoodInBox(col));
        //IsFoodInBox(col);
    }

    bool IsFoodInBox(Collider2D col)
    {
        //if (col is not PolygonCollider2D) {return false;}

        //Debug.Log("bounds: "+container.bounds);
        PolygonCollider2D col_p = (PolygonCollider2D) col;
        foreach (Vector2 point in col_p.points)
        {
            Vector3 col_global_point = col.transform.parent.position + (Vector3) point /transform.lossyScale.x;
            //Debug.Log(point);
            if (!container.bounds.Contains(col_global_point))
            {
                return false;
            }
        }
        return true;
    }
}

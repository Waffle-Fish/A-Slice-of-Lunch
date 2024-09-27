using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    private BoxCollider2D myCollider; //collider of the lid
    public BoxCollider2D baseCollider; //collider of the base
    // private int opacity = 255;
    private bool dragging = false; //true if left click is held down flase otherwise
    private Vector3 offset; //mouse position - object position

    // Update is called once per frame
    void Start()
    {
        myCollider = GetComponent<BoxCollider2D>();
    }
    void Update()
    {
        if (dragging) //If mouse down
        {
            //Apply change in mouse location to object's location ignoring difference in location
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset; 
        }

        if (myCollider.IsTouching(baseCollider)) //If the lid is not touching the base
        {
            Debug.Log("The object is touching");
            //lower opacity value
            //apply opacity value
            //if the opacity is less than , delete lid

        }
    }

    private void OnMouseDown()
    {
        //Send true if left click pressed
        dragging = true;
        //calculate difference in location from mouse position to object position
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        //send false when mouse up
        dragging = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    [SerializeField] private float fadeOutRadius;
    [SerializeField] private float fadeTime;
    private bool dragging = false; //true if left click is held down flase otherwise
    private Vector3 offset; //mouse position - object position
    private SpriteRenderer spriteRenderer;

    IEnumerator Fade()
    {
        Color c = spriteRenderer.color;
        for (float alpha = 1f; alpha >= 0; alpha -= 0.05f)
        {
            c.a = alpha;
            spriteRenderer.color = c;
            yield return new WaitForSeconds(fadeTime);
        }
        gameObject.SetActive(false);
    }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    // Update is called once per frame
    void Update()
    {
        if (dragging) //If mouse down
        {
            //Apply change in mouse location to object's location ignoring difference in location
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset; 
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

        //if the position is fadeOutRadius away from (0,0)
        if(Vector2.Distance((Vector2)transform.position, Vector2.zero) >= fadeOutRadius)
        {
            //fade object (dcrease alpha channel)
            StartCoroutine(Fade());
        }
    }
}

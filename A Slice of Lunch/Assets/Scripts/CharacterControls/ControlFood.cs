using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class ControlFood : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Ray clickRay;


    public void OnPointerDown(PointerEventData eventData) {
        Debug.Log("OnPointDown");        
    }
    public void OnEndDrag(PointerEventData eventData) {
        Debug.Log("OnEndDrag");
    }
    public void OnDrag(PointerEventData eventData) {
        Vector2 foodPosition = Vector2.zero;
        foodPosition += eventData.delta;
        Debug.Log("OnDrag");
    }
    public void OnBeginDrag(PointerEventData eventData) {
        Debug.Log("OnBeginDrag");
    }


    private string DetectObject() {
            clickRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(clickRay.origin, clickRay.direction * 100, Color.green, 5f);
            RaycastHit2D hit2D =  Physics2D.GetRayIntersection(clickRay);
            if (hit2D.collider != null) {
                Debug.Log(hit2D.collider.gameObject.name);
                return hit2D.collider.tag;
            }
            return "None";
        }

}

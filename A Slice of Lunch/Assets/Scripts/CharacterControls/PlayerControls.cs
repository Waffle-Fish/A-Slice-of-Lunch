using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlayerControls : MonoBehaviour
{
    [Header("Detect Object")]
    private Vector3 mouseWorldPosition;
    Ray clickRay;

    [Header("Slice Variables")]
    [SerializeField]
    private bool isHoldingKnife = false;
    [SerializeField]
    private GameObject spriteMask;
    private Vector3[] slicePoints = new Vector3[2];
    readonly private Vector3 checkVector = new Vector3(999999, 999999, 999999);
    List<RaycastHit2D> slicedObjects;

    [Header("Drag and Drop")]
    GameObject lastFoodSelected;

    private void Awake() {
        mouseWorldPosition = new();
        lastFoodSelected = null;
    }

    private void Start()
    {
        ResetSlicePoints();
    }

    // This function gets called every frame
    void Update()
    {
        mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        DetectLeftClick();
    }

    // Returns the tag of the first clicked object
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

    #region  Drag and Drop
    // ==================================
    // To Do: For Felix
    // Implement drag and drop food
    // * Click and hold food to pick up
    // * When food is picked up, have it follow the mouse 
    // * Let go of left click to drop food (it stops following mouse)
    // ================================== 


    // You can rename this function
    private void DetectClickAndHold() {
        // if mouse is pressed
        // if DetectObject() is food
        // {
        //    Do Drag and Drop
        // }
    }

    #endregion

    #region Slice Zone
    private void DetectLeftClick()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (isHoldingKnife) {
                Slice();
            }
            // Debug.Log(DetectObject());
        }
    }

    private void ResetSlicePoints()
    {
        slicePoints[0] = checkVector;
        slicePoints[1] = checkVector;
    }

    private void Slice() {
        // Debug.Log("Am Slicing");
        if (slicePoints[0] == checkVector) {
            slicePoints[0] = mouseWorldPosition;
        } else if (slicePoints[1] == checkVector){
            slicePoints[1] = mouseWorldPosition;
            slicedObjects = Physics2D.LinecastAll(slicePoints[0], slicePoints[1]).ToList();
            foreach (var maskCollider in slicedObjects)
            {
                // ignores not food in slicedObjects
                if (!maskCollider.transform.CompareTag("Food") ) {
                    continue;
                }

                // Slice fruit
                Transform parentFruit = maskCollider.transform.parent;

                // Gets spawn point
                Vector2 sliceEdgePoint_0 = Physics2D.Raycast(slicePoints[0], (slicePoints[1] - slicePoints[0]).normalized, 100).point;
                Vector2 sliceEdgePoint_1 = Physics2D.Raycast(slicePoints[1], (slicePoints[0] - slicePoints[1]).normalized, 100).point;
                Debug.DrawLine(slicePoints[0], sliceEdgePoint_0, Color.blue, 100f);
                Debug.DrawLine(slicePoints[1], sliceEdgePoint_1, Color.blue, 100f);
                Vector2 sliceCenter = (sliceEdgePoint_0 + sliceEdgePoint_1) / 2f;

                // Rotate mask to be parallel to slice
                bool rotateFromYAxis = (slicePoints[0].x < slicePoints[1].x && slicePoints[0].y > slicePoints[1].y) || (slicePoints[1].x < slicePoints[0].x && slicePoints[1].y > slicePoints[0].y);
                float opposite = (rotateFromYAxis) ? Mathf.Abs(slicePoints[1].x - slicePoints[0].x) : Mathf.Abs(slicePoints[1].y - slicePoints[0].y);
                float hypotenuse = Vector2.Distance(slicePoints[0], slicePoints[1]);
                float rotAng = Mathf.Asin(opposite / hypotenuse) * Mathf.Rad2Deg;

                // Picks one side of the slice for the mask to go to
                Vector2 perpendicularSlice = Vector2.Perpendicular(slicePoints[0]-slicePoints[1]).normalized;
                // float sliceDistance = Vector2.Distance(sliceEdgePoint_0, sliceEdgePoint_1);
                // Debug.Log(sliceDistance);

                // Spawn Mask
                Vector2 spawnPos = sliceCenter + parentFruit.GetChild(1).localScale.x / 2f * perpendicularSlice;
                Transform currentSpriteMask = null;
                // foreach (Transform sm in parentFruit.GetChild(1))
                // {
                //     if (!sm.gameObject.activeSelf) {
                //         currentSpriteMask = sm;
                //         currentSpriteMask.gameObject.SetActive(true);
                //         currentSpriteMask.transform.SetPositionAndRotation(spawnPos, Quaternion.Euler(0,0,rotAng));
                //         break;
                //     }
                // }
                // if (currentSpriteMask == null) {
                    currentSpriteMask = Instantiate(spriteMask, spawnPos, Quaternion.Euler(0,0,rotAng), parentFruit.GetChild(1)).transform;
                // }

                // Create other side slice
                GameObject otherSlice = null;
                // foreach (Transform food in parentFruit.parent)
                // {
                //     if (!food.gameObject.activeSelf) {
                //         otherSlice = food.gameObject;
                //         Debug.Log(otherSlice.name);
                //         otherSlice.SetActive(true);
                //         otherSlice.transform.SetPositionAndRotation(parentFruit.position, parentFruit.rotation);
                //         otherSlice.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
                //         break;
                //     }
                // }
                // if (otherSlice == null) {
                    otherSlice = Instantiate(parentFruit.gameObject, parentFruit.position, parentFruit.rotation, parentFruit.parent);
                // }
                
                float separationSpace = 0.05f;
                otherSlice.transform.GetChild(1).GetChild(otherSlice.transform.GetChild(1).childCount-1).transform.position = sliceCenter - parentFruit.GetChild(1).localScale.x / 2f * perpendicularSlice;

                parentFruit.Translate(-perpendicularSlice * separationSpace);
                otherSlice.transform.Translate(perpendicularSlice * separationSpace);
            }

            Debug.DrawLine(slicePoints[0], slicePoints[1], Color.black, 10f);
            // process slice
            ResetSlicePoints();
        }
    }
    #endregion
}

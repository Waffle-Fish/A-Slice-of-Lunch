using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    [Header("Slice Variables")]
    [SerializeField]
    private bool isHoldingKnife = false;
    private Vector3[] slicePoints = new Vector3[2];
    readonly private Vector3 checkVector = new Vector3(999999, 999999, 999999);
    List<RaycastHit2D> slicedObjects;

    private Vector3 mouseWorldPosition;
    Ray clickRay;

    private void Awake() {
        mouseWorldPosition = new();
    }

    private void Start()
    {
        ResetSlicePoints();
    }

    void Update()
    {
        mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        DetectLeftClick();
    }

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

    #region Slice Zone
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
                if (!maskCollider.transform.CompareTag("Food")) {
                    continue;
                }

                Transform maskT = maskCollider.transform;
                // Rotate mask to be parallel to slice
                // if slice goes from top left to bottom right, or vice-versa, rotate away from y-axis instead of x-axis
                bool rotateFromYAxis = (slicePoints[0].x < slicePoints[1].x && slicePoints[0].y > slicePoints[1].y) || (slicePoints[1].x < slicePoints[0].x && slicePoints[1].y > slicePoints[0].y);
                float opposite = (rotateFromYAxis) ? Mathf.Abs(slicePoints[1].x - slicePoints[0].x) : Mathf.Abs(slicePoints[1].y - slicePoints[0].y);
                float hypotenuse = Vector2.Distance(slicePoints[0], slicePoints[1]);
                float rotAng = Mathf.Asin(opposite / hypotenuse) * Mathf.Rad2Deg;
                maskT.rotation = Quaternion.identity * Quaternion.Euler(0,0,rotAng);

                // Moves mask away from slice line
                Vector2 perpendicularSlice = Vector2.Perpendicular(slicePoints[0]-slicePoints[1]).normalized;
                Debug.DrawLine(maskT.position, maskT.position+5*(Vector3)perpendicularSlice, Color.green, 10f);
                Vector3 maskHalfYScale = new Vector3 (maskT.localScale.x, maskT.localScale.y * 0.5f, maskT.localScale.z);
                Vector3 maskHalfXScale = new Vector3 (maskT.localScale.x * 0.5f, maskT.localScale.y, maskT.localScale.z);
                maskT.localScale = 
                maskT.position += (Vector3)perpendicularSlice * 0.5f;
            }



            Debug.DrawLine(slicePoints[0], slicePoints[1], Color.black, 10f);
            // process slice
            ResetSlicePoints();
        }
    }
    #endregion
}

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
        Debug.Log("Am Slicing");
        if (slicePoints[0] == checkVector) {
            slicePoints[0] = mouseWorldPosition;
        } else if (slicePoints[1] == checkVector){
            slicePoints[1] = mouseWorldPosition;
            slicedObjects = Physics2D.LinecastAll(slicePoints[0], slicePoints[1]).ToList();
            // ignores not food in slicedObjects
            foreach (var foodCollider in slicedObjects)
            {
                if (!foodCollider.transform.CompareTag("Food")) {
                    continue;
                }
                Transform maskT = foodCollider.transform.GetChild(0);
                float rotAng = Vector2.Angle(slicePoints[0], slicePoints[1]);
                maskT.Rotate(Vector3.forward, rotAng);
            }



            Debug.DrawLine(slicePoints[0], slicePoints[1], Color.black, 100f);
            // process slice
            ResetSlicePoints();
        }
    }
    #endregion
}

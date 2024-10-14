using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

// Rename this to SliceControls
public class PlayerControls : MonoBehaviour
{
    [Header("Detect Object")]
    private Vector3 mouseWorldPosition;

    [Header("Slice Variables")]
    // [SerializeField]
    // private GameObject spriteMask;
    private Vector3[] slicePoints = new Vector3[2];
    public bool IsHoldingKnife /*{ get; private set; }*/ = false;
    readonly private Vector3 CHECK_VECTOR = new Vector3(999999, 999999, 999999);
    List<RaycastHit2D> slicedObjects;

    [Header("Slice Indicators")]
    [SerializeField]
    private GameObject endPointObj;
    [SerializeField]
    private float endPointPosZ;
    private List<GameObject> endPoints;
    private LineRenderer sliceMarking;

    [Header("Object Pooling")]
    private ObjectPooler maskPool;
    private ObjectPooler foodPool;

    private void Awake() {
        sliceMarking = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        mouseWorldPosition = new();
        endPoints = new();

        for (int i = 0; i < 2; i++) {
            endPoints.Add(Instantiate(endPointObj, transform.position, transform.rotation, transform));
        }
        ResetSlicePoints();
    }

    void Update()
    {
        mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        DetectLeftClick();
        DisplaySliceMarkings();
    }

    private void DisplaySliceMarkings()
    {
        if (slicePoints[0] == CHECK_VECTOR) return;

        Vector3[] smPos = new Vector3[2];
        smPos[0] = slicePoints[0];
        smPos[1] = mouseWorldPosition;
        // Making it behind the circle of the slice pos
        smPos[0].z = endPoints[0].transform.position.z + 1;
        smPos[1].z = endPoints[1].transform.position.z + 1;
        sliceMarking.SetPositions(smPos);

        Vector3 endPointPos = mouseWorldPosition;
        endPointPos.z = endPointPosZ;
        endPoints[1].transform.position = endPointPos;
    }

    private void DetectLeftClick()
    {
        if (!IsHoldingKnife) return;
        if (Mouse.current.leftButton.wasPressedThisFrame) BeginSlice();
        if (Mouse.current.leftButton.wasReleasedThisFrame) FinalizeSlice();
    }

    private void ResetSlicePoints()
    {
        slicePoints[0] = CHECK_VECTOR;
        slicePoints[1] = CHECK_VECTOR;

        foreach (var ep in endPoints) ep.SetActive(false);
        sliceMarking.SetPositions(slicePoints);
    }

    private void BeginSlice() {
        // return if slice starts on food
        Ray clickRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit2D =  Physics2D.GetRayIntersection(clickRay);
        if (hit2D.collider != null && hit2D.collider.CompareTag("Food")) return;

        slicePoints[0] = mouseWorldPosition;
        foreach (var ep in endPoints) ep.SetActive(true);
        endPoints[0].transform.position = new (slicePoints[0].x, slicePoints[0].y, endPointPosZ);
    }

    private void FinalizeSlice() {
        if (slicePoints[0] == CHECK_VECTOR) return;

        slicePoints[1] = mouseWorldPosition;
        slicedObjects = Physics2D.LinecastAll(slicePoints[0], slicePoints[1]).ToList();
        foreach (var foodCollider in slicedObjects) {
            // ignores not food in slicedObjects
            if (!foodCollider.transform.CompareTag("Food") ) {
                continue;
            }

            // Slice food
            Transform parentFood = foodCollider.transform.parent;

            // Initialize Pools
            maskPool = foodCollider.collider.GetComponent<ObjectPooler>();
            foodPool = parentFood.parent.GetComponent<ObjectPooler>();

            // Gets spawn point
            Vector2 sliceEdgePoint_0 = Physics2D.Raycast(slicePoints[0], (slicePoints[1] - slicePoints[0]).normalized, 100).point;
            Vector2 sliceEdgePoint_1 = Physics2D.Raycast(slicePoints[1], (slicePoints[0] - slicePoints[1]).normalized, 100).point;
            Vector2 sliceCenter = (sliceEdgePoint_0 + sliceEdgePoint_1) / 2f;

            // Rotate mask to be parallel to slice
            bool rotateFromYAxis = (slicePoints[0].x < slicePoints[1].x && slicePoints[0].y > slicePoints[1].y) || (slicePoints[1].x < slicePoints[0].x && slicePoints[1].y > slicePoints[0].y);
            float opposite = (rotateFromYAxis) ? Mathf.Abs(slicePoints[1].x - slicePoints[0].x) : Mathf.Abs(slicePoints[1].y - slicePoints[0].y);
            float hypotenuse = Vector2.Distance(slicePoints[0], slicePoints[1]);
            float rotAng = Mathf.Asin(opposite / hypotenuse) * Mathf.Rad2Deg;

            // Picks one side of the slice for the mask to go to
            Vector2 perpendicularSlice = Vector2.Perpendicular(slicePoints[0]-slicePoints[1]).normalized;

            // Spawn Mask
            GameObject spriteMask = maskPool.GetPooledObject();
            Vector2 spawnPos = sliceCenter + spriteMask.transform.localScale.x /2f * perpendicularSlice;
            spriteMask.SetActive(true);
            spriteMask.transform.SetPositionAndRotation(spawnPos, Quaternion.Euler(0,0,rotAng));

            // Create other side slice
            GameObject otherSlice = foodPool.GetPooledObject();
            otherSlice.transform.SetPositionAndRotation(parentFood.position,parentFood.rotation);
            
            // Set up other slice sprite masks
            float separationSpace = 0.05f;
            int maskIndex = 0;
            ObjectPooler otherSliceMaskPool = otherSlice.transform.GetChild(0).GetComponent<ObjectPooler>();
            GameObject currentMask = maskPool.GetPooledObjectAtIndex(maskIndex);
            GameObject otherSliceMask;
            // Copy all sprite masks of this slice to other slice
            int numMasks = maskPool.GetNumObjectsActive();
            while (maskIndex < numMasks-1) {
                otherSliceMask = otherSliceMaskPool.GetPooledObject();
                // otherSliceMask.transform.SetPositionAndRotation(currentMask.transform.position, currentMask.transform.rotation);
                otherSliceMask.SetActive(true);
                maskIndex++;
                // currentMask = maskPool.GetPooledObjectAtIndex(maskIndex);
            }

            otherSliceMask = otherSliceMaskPool.GetPooledObject();
            otherSliceMask.transform.SetPositionAndRotation(sliceCenter - spriteMask.transform.localScale.x / 2f * perpendicularSlice,Quaternion.Euler(0,0,rotAng));
            otherSliceMask.SetActive(true);
            
            parentFood.Translate(-perpendicularSlice * separationSpace);
            otherSlice.transform.Translate(perpendicularSlice * separationSpace);
            otherSlice.SetActive(true);
        }
        ResetSlicePoints();
    }

    public void HoldKnife() {
        IsHoldingKnife = true;
        ResetSlicePoints();
    }

    public void DropKnife() {
        IsHoldingKnife = false;
        ResetSlicePoints();
    }
}

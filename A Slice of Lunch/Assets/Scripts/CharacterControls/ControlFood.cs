using UnityEngine;

public class ControlFood : MonoBehaviour
{
    bool dragging = false;
    PlayerControls playerControls;
    
    private void Awake() {
        playerControls = GameObject.FindWithTag("Player").GetComponent<PlayerControls>();
    }

    private void OnMouseDown() {
        if (playerControls.IsHoldingKnife) return;
        // Ray clickRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        // RaycastHit2D hit2D = Physics2D.GetRayIntersection(clickRay);
        // Debug.Log("Hit name:" + hit2D.collider.name);
        // if (hit2D.collider != null && !hit2D.collider.CompareTag("Food")) return;

        dragging = true;
    }

    private void Update() {
        if (dragging) {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0f;
            transform.parent.position = pos;
        }
    }

    private void OnMouseUp() {
        dragging = false;
    }
}

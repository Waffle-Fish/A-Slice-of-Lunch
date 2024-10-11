using UnityEngine;

public class ControlFood : MonoBehaviour
{
    bool dragging = false;
    
    private void OnMouseDown() {
        dragging = true;
    }
    private void Update() {
        if (dragging) {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0f;
            transform.position = pos;
        }
    }
    private void OnMouseUp() {
        dragging = false;
    }
}

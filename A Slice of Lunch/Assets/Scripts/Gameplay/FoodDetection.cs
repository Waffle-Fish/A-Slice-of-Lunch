using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void OnTriggeerEnter2D(Collider2D col)
    {
        Debug.Log("Caca Mega DOO DOO");
    }
}

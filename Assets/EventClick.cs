using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class EventClick : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Check for EventClick");
    }


    // Update is called once per frame
    void Update()
    {
        TestingKeys();
    }

    public void OnMouseDown()
    {
        Debug.Log("Right Mouse Button Clicked");
    }

    public void TestingKeys()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("Key B pressed");
        }
    
        if(Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log("Key N pressed");
        }

        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            Debug.Log("Mouse 1 pressed");
        }

    }
}

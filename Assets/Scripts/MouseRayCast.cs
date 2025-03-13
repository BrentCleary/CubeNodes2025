using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseRayCast : MonoBehaviour
{

    public Camera mainCamera;
    public LayerMask layerMask;
    public bool rayCastMouseSelect;

    private GameObject _hitObject;
    public GameObject hitObject 
    { 
        get { return _hitObject; }
        set { 
            if( _hitObject != hitObject) {
                _hitObject = hitObject;
                OnValueChanged();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        DrawRay();
    
        _hitObject = GetRaycastHitObject();

    }

    // DrawRay
    public void DrawRay()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
        mousePos = mainCamera.ScreenToWorldPoint(mousePos);
        Debug.DrawRay(transform.position, mousePos-transform.position, Color.blue);
    }

    public void OnValueChanged()
    {
        Debug.Log("Raycast hit: " + hitObject.name);
    }

    public GameObject GetRaycastHitObject()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100, layerMask))
        {
            return hit.transform.gameObject; // Return the GameObject that the ray hits
        }

        return null; // Return null if no object is hit
    }


    //* Returns True if an object is hit
    // bool IsRayPointingAtObject()
    // {
    //     Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
    //     RaycastHit hit;

    //     // Perform the raycast and return true if an object is hit, otherwise return false
    //     return Physics.Raycast(ray, out hit, 100, layerMask);
    // }
}

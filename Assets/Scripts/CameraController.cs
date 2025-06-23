using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraController : MonoBehaviour
{
  public  float moveSpeed = 50f;       // Movement speed
  public  float rotationSpeed = 20f;   // Rotation speed
  private float pitch = 0f;          // Vertical rotation (X-axis)
  private float yaw = 0f;            // Horizontal rotation (Y-axis)

  private Dictionary<string, CameraView> views;
  
  [System.Serializable]
  private class CameraView {
      public Vector3 position;
      public Quaternion rotation;
      public CameraView(Vector3 pos, Quaternion rot) {
          position = pos;
          rotation = rot;
      }
      public void ApplyTo(Transform t) {
          t.position = position;
          t.rotation = rotation;
      }
  }

	void Start()
  {
    // Initialize rotation from current transform
    Vector3 angles = transform.eulerAngles;
    yaw = angles.y;
    pitch = angles.x;

		InitializeCameraPresets();
  
  }

  void Update()
  {
    HandleRotation();
    HandleMovement();
		ApplyCameraPresets();
	}

  void HandleRotation() {
    if (Input.GetMouseButton(2)) {                                              // Rotate when middle mouse button is held
      float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
      float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;
      yaw   += mouseX;
      pitch -= mouseY;
      pitch  = Mathf.Clamp(pitch, -89f, 89f);                                   // Prevent flipping
      transform.rotation = Quaternion.Euler(pitch, yaw, 0f);                    // Apply rotation
    }
  }

  void HandleMovement() {
    Vector3 moveDirection = Vector3.zero;
    
    // Move relative to camera orientation
    if (Input.GetKey(KeyCode.W)) moveDirection += Vector3.up;                   // Up
    if (Input.GetKey(KeyCode.S)) moveDirection -= Vector3.up;                   // Down
    if (Input.GetKey(KeyCode.A)) moveDirection -= transform.right;              // Left
    if (Input.GetKey(KeyCode.D)) moveDirection += transform.right;              // Right

    float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
    if (mouseWheel > 0) moveDirection += transform.forward;                     // Forward
    if (mouseWheel < 0) moveDirection -= transform.forward;                     // Back
    moveDirection = moveDirection.normalized * moveSpeed * Time.deltaTime;      // Normalize prevents faster diagonal movement, then apply speed

    transform.position += moveDirection;                                        // Apply movement
  }

  public void SetView(string viewName) {
      if (views.TryGetValue(viewName, out var view)) {
          view.ApplyTo(transform);
      }
  }

  void ApplyCameraPresets() {
    if(Input.GetKey(KeyCode.C)) {
      if      (Input.GetKeyDown(KeyCode.X)) {	SetView("TopDown");	}             // Top View
      else if (Input.GetKeyDown(KeyCode.Z)) { SetView("StartView"); }           // Start View
    }
  }

  void InitializeCameraPresets() {
    views = new Dictionary<string, CameraView>();

    views["StartView"] = new CameraView(
      new Vector3(5f, 8f, -2f),
      new Quaternion(0.4980974f, -0.07547912f, 0.04357789f, 0.8627299f)
    );
    views["TopDown"] = new CameraView(
      new Vector3(4f, 15f, 4f),
      new Quaternion(0.7071068f, 0f, 0f, 0.7071068f)
    );
    views["SideView"] = new CameraView(
      new Vector3(0f, 5f, -10f),
      Quaternion.Euler(30f, 90f, 0f)
    );
  }
}

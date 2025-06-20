using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraController : MonoBehaviour
{
	public float moveSpeed = 50f;       // Movement speed
	public float rotationSpeed = 20f;   // Rotation speed

	private float pitch = 0f;          // Vertical rotation (X-axis)
	private float yaw = 0f;            // Horizontal rotation (Y-axis)

	void Start()
	{
		// Initialize rotation from current transform
		Vector3 angles = transform.eulerAngles;
		yaw = angles.y;
		pitch = angles.x;
	}

	void Update()
	{
		HandleRotation();
		HandleMovement();
	}

	void HandleRotation()
	{
		// Rotate when middle mouse button is held
		if (Input.GetMouseButton(2))
		{
			float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
			float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

			yaw += mouseX;
			pitch -= mouseY;
			pitch = Mathf.Clamp(pitch, -89f, 89f); // Prevent flipping

			// Apply rotation
			transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
		}
	}

	void HandleMovement()
	{
		float time = Time.deltaTime;
		Vector3 moveDirection = Vector3.zero;

		// Move relative to camera orientation
		if (Input.GetKey(KeyCode.W)) moveDirection += Vector3.up;         // Up
		if (Input.GetKey(KeyCode.S)) moveDirection -= Vector3.up;         // Down
		if (Input.GetKey(KeyCode.A)) moveDirection -= transform.right;      // Left
		if (Input.GetKey(KeyCode.D)) moveDirection += transform.right;      // Right

		float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
		if (mouseWheel > 0) moveDirection += transform.forward;             // Forward
		if (mouseWheel < 0) moveDirection -= transform.forward;             // Back

		// Normalize to prevent faster diagonal movement, then apply speed
		moveDirection = moveDirection.normalized * moveSpeed * time;

		// Apply movement
		transform.position += moveDirection;
	}
}





// public class CameraControllerScript : MonoBehaviour
// {
//     // Brent Cleary
//     // 2025/02/25.2040
//     // Script written to control camera behavior during play mode for testing. To potentially be implemented further in game mode.
//     // Script Assistance by ChatGPT: https://chatgpt.com/share/67be9af1-057c-8008-93e1-f126771b1564

//     public float camSpeed = 40f;
//     public float scrollSpeed = 40f;
//     public float shiftMultiplier = 2f;
//     public Vector3 positionDisplay;

//     public float rotationSpeed = 5f;  // Adjust sensitivity
//     private float pitch = 0f;         // Up/down rotation (X-axis)
//     private float yaw = 0f;           // Left/right rotation (Y-axis)

//     void Start()
//     {
//         // Initialize rotation angles from current rotation
//         Vector3 angles = transform.eulerAngles;
//         yaw = angles.y;
//         pitch = angles.x;
//     }

//     void Update()
//     {
//         CameraTransform();
//         CameraRotation();

//     }

//     void CameraTransform()
//     {
//         float time = Time.deltaTime;
//         float speed = Input.GetKey(KeyCode.LeftShift) ? camSpeed * shiftMultiplier : camSpeed;

//         // Get current position
//         Vector3 newCameraPos = transform.position;

//         // Left movement
//         if (Input.GetKey(KeyCode.A)){
//             newCameraPos.x -= speed * time;
//             Debug.Log("KeyCode A");
//         }
//         // Right movement
//         if (Input.GetKey(KeyCode.D)){
//             newCameraPos.x += speed * time;
//             Debug.Log("KeyCode D");
//         }
//         // Forward movement
//         if (Input.GetKey(KeyCode.W)){
//             Debug.Log("KeyCode W");
//             newCameraPos.z += speed * time;
//         }
//         // Backward movement
//         if (Input.GetKey(KeyCode.S)){
//             Debug.Log("KeyCode S");
//             newCameraPos.z -= speed * time;
//         } 

//         // Mouse scroll (forward = Up, backward = Down)
//         float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
//         if (mouseWheel > 0) newCameraPos.y -= scrollSpeed * time;
//         if (mouseWheel < 0) newCameraPos.y += scrollSpeed * time;

//         // Apply new position
//         transform.position = newCameraPos;
//     }

//         void CameraRotation()
//     {
//         // Rotate camera when middle mouse button is held
//         if (Input.GetMouseButton(2)) // Middle mouse button
//         {
//             float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
//             float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

//             // Adjust yaw (Y-axis rotation)
//             yaw += mouseX;

//             // Adjust pitch (X-axis rotation), but clamp it to avoid flipping
//             pitch -= mouseY;
//             pitch = Mathf.Clamp(pitch, -89f, 89f); // Prevent camera flipping

//             // Apply rotation using Quaternion
//             transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
//         }
//     }

// }



// 

//     public Transform cameraTransform;
//     public Vector3 cameraPos;

//     public int camSpeed;
//     public int multipier;

//     /*  Generate Input

//     int speed variable - initally +5 * Time.deltaTime : Random choice, tweak by results

//     a - left horizontal - x negative - Keycode.A
//     d - right horizontal - x positive - Keycode.D
//     w - up vertical - y positive - Keycode.W
//     s - down vertical - y negative - Keycode.S
//     mouse-scroll forward  -  z positive - Keycode.MouseScrollWheel
//     mouse-scroll backward -  z negative - Keycode.MouseScrollWheel
//         float scroll = Input.GetAxis("Mouse ScrollWheel");
//         if (scroll > 0)
//         {
//             Scroll forward (+Z)
//         }
//         else if (scroll < 0)
//         {
//             Scroll backward (-Z)
//         }
//     Shift - ChangeSpeed - KeyCode.LeftShift
//     */

//     // Start is called before the first frame update
//     void Start()
//     {
//         cameraTransform = gameObject.GetComponent<Transform>();
//         cameraPos = cameraTransform.position;

//         camSpeed = 10;
//         multipier = 1;
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         cameraPos = CameraControls();
//     }

//     public Vector3 CameraControls()
//     {
//         Vector3 newCameraPos = new Vector3();

//         float time = Time.deltaTime;
//         float mouseWheel = Input.GetAxis("Mouse ScrollWheel");

//         if(Input.GetKey(KeyCode.A)) {
//             newCameraPos = new Vector3(newCameraPos.x + 5, newCameraPos.y, newCameraPos.z) * time;
//         }
//         if(Input.GetKey(KeyCode.D)) {
//             newCameraPos = new Vector3(newCameraPos.x - 5, newCameraPos.y, newCameraPos.z) * time;
//         }
//         if(Input.GetKey(KeyCode.A)) {
//             newCameraPos = new Vector3(newCameraPos.x, newCameraPos.y + 5, newCameraPos.z) * time;
//         }
//         if(Input.GetKey(KeyCode.A)) {
//             newCameraPos = new Vector3(newCameraPos.x, newCameraPos.y + 5, newCameraPos.z) * time;
//         }

//         if (mouseWheel > 0) {
//             newCameraPos = new Vector3(newCameraPos.x, newCameraPos.y, newCameraPos.z + 5) * time;
//             Debug.Log("Scrolling forward (+Z)");
//         }
//         else if (mouseWheel < 0) {
//             newCameraPos = new Vector3(newCameraPos.x, newCameraPos.y, newCameraPos.z - 5) * time;
//             Debug.Log("Scrolling backward (-Z)");
//         }

//         return newCameraPos;

//     }
// }

using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof (ThirdPersonCharacter))]
public class PlayerMovement : MonoBehaviour
{
	[SerializeField] float destinationRadius = 0.2f;

	ThirdPersonCharacter thirdPersonCharacter;   // A reference to the ThirdPersonCharacter on the object
	CameraRaycaster cameraRaycaster;
    Vector3 currentClickTarget;

	bool isDirectMode = false; 
        
    private void Start()
    {
        cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
        thirdPersonCharacter = GetComponent<ThirdPersonCharacter>();
        currentClickTarget = transform.position;
    }

    // Fixed update is called in sync with physics
    private void FixedUpdate()
    {
		if (Input.GetKeyDown (KeyCode.G)) {
			isDirectMode = !isDirectMode; 	// Toggle direct mode
			currentClickTarget = transform.position;	// Prevent player from running back to ori position when changing from direct to mouse
		}

		if (isDirectMode) {
			ProcessDirectMovement ();
		} else {
			ProcessMouseMovement ();
		}
    }
		
	void ProcessDirectMovement(){
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");

		// calculate camera relative direction to move:
		Vector3 camForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
		Vector3 move = v*camForward + h*Camera.main.transform.right;

		thirdPersonCharacter.Move (move, false, false);
	}

	void ProcessMouseMovement ()
	{
		if (Input.GetMouseButton (0)) {
			switch (cameraRaycaster.currentLayerHit) {
			case Layer.Walkable:
				currentClickTarget = cameraRaycaster.hit.point;
				break;
			case Layer.Enemy:
				Debug.Log ("Enemy detected!");
				break;
			default:
				Debug.Log ("Unregistered Layer.");
				return;
			}
		}
		var currentMove = currentClickTarget - transform.position;
		if (currentMove.magnitude >= destinationRadius) {
			thirdPersonCharacter.Move (currentClickTarget - transform.position, false, false);
		}
		else {
			thirdPersonCharacter.Move (Vector3.zero, false, false);
		}
	}
}


using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof (ThirdPersonCharacter))]
public class PlayerMovement : MonoBehaviour
{
	[SerializeField] float destinationRadius = 0.2f;

    ThirdPersonCharacter m_Character;   // A reference to the ThirdPersonCharacter on the object
	CameraRaycaster cameraRaycaster;
    Vector3 currentClickTarget;

	bool isDirectMode = false; 
        
    private void Start()
    {
        cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
        m_Character = GetComponent<ThirdPersonCharacter>();
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
		Vector3 m_CamForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
		Vector3 m_Move = v*m_CamForward + h*Camera.main.transform.right;

		m_Character.Move (m_Move, false, false);
	}

	void ProcessMouseMovement ()
	{
		if (Input.GetMouseButton (0)) {
			switch (cameraRaycaster.layerHit) {
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
			m_Character.Move (currentClickTarget - transform.position, false, false);
		}
		else {
			m_Character.Move (Vector3.zero, false, false);
		}
	}
}


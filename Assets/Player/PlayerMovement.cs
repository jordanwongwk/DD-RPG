using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof (ThirdPersonCharacter))]
public class PlayerMovement : MonoBehaviour
{
	[SerializeField] float walkMoveStopRadius = 0.2f;
	[SerializeField] float attackMoveStopRadius = 5.0f;

	ThirdPersonCharacter thirdPersonCharacter;   // A reference to the ThirdPersonCharacter on the object
	CameraRaycaster cameraRaycaster;
    Vector3 currentDestination, clickPoint;

	bool isDirectMode = false; 
        
    private void Start()
    {
        cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
        thirdPersonCharacter = GetComponent<ThirdPersonCharacter>();
        currentDestination = transform.position;
    }

    // Fixed update is called in sync with physics
    private void FixedUpdate()
    {
		if (Input.GetKeyDown (KeyCode.G)) {
			isDirectMode = !isDirectMode; 	// Toggle direct mode
			currentDestination = transform.position;	// Prevent player from running back to ori position when changing from direct to mouse
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
		clickPoint = cameraRaycaster.hit.point;
		if (Input.GetMouseButton (0)) {
			switch (cameraRaycaster.currentLayerHit) {
			case Layer.Walkable:
				currentDestination = ShortDestination(clickPoint, walkMoveStopRadius);
				break;
			case Layer.Enemy:
				currentDestination = ShortDestination(clickPoint, attackMoveStopRadius);
				break;
			default:
				Debug.Log ("Unregistered Layer.");
				return;
			}
		}
		MoveToDestination ();
	}

	void MoveToDestination ()
	{
		var currentMove = currentDestination - transform.position;
		if (currentMove.magnitude >= 0) {
			thirdPersonCharacter.Move (currentDestination - transform.position, false, false);
		}
		else {
			thirdPersonCharacter.Move (Vector3.zero, false, false);
		}
	}

	Vector3 ShortDestination (Vector3 destination, float shortenRate){
		Vector3 distanceShorten = (destination - transform.position).normalized * shortenRate;
		return destination - distanceShorten;
	}

	void OnDrawGizmos(){
		// Move line gizmos
		Gizmos.color = Color.black;
		Gizmos.DrawLine (transform.position, clickPoint);
		Gizmos.DrawSphere (currentDestination, 0.2f);
		Gizmos.DrawSphere (clickPoint, 0.1f);

		// Attack sphere gizmos
		Gizmos.color = new Color(255, 0, 0, 0.5f);
		Gizmos.DrawWireSphere (transform.position, attackMoveStopRadius);
	}
}


using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityEngine.AI;

using RPG.CameraUI;

namespace RPG.Characters {
	[RequireComponent(typeof (NavMeshAgent))]
	[RequireComponent(typeof (AICharacterControl))]
	[RequireComponent(typeof (ThirdPersonCharacter))]
	public class PlayerMovement : MonoBehaviour
	{
		ThirdPersonCharacter thirdPersonCharacter;   // A reference to the ThirdPersonCharacter on the object
		CameraRaycaster cameraRaycaster;
		AICharacterControl aiCharacterControl;
		GameObject walkTarget;

		[SerializeField] const int walkableLayerNumber = 8;
		[SerializeField] const int enemyLayerNumber = 9;

	    private void Start()
	    {
	        cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
	        thirdPersonCharacter = GetComponent<ThirdPersonCharacter>();
			aiCharacterControl = GetComponent<AICharacterControl> ();

			walkTarget = new GameObject ("WalkTarget");
			cameraRaycaster.notifyMouseClickObservers += MovePlayer;
	    }
			
		void MovePlayer (RaycastHit raycastHit, int layerHit){
			switch (layerHit) {
				case walkableLayerNumber:
					walkTarget.transform.position = raycastHit.point;
					aiCharacterControl.SetTarget (walkTarget.transform);
					break;
				case enemyLayerNumber:
					GameObject enemy = raycastHit.collider.gameObject;
					aiCharacterControl.SetTarget (enemy.transform);
					break;
				default:
					Debug.LogWarning ("Not sure how to move to this place");
					return;
			}
		}

	//	TODO Make this work again
		void ProcessDirectMovement(){
			float h = Input.GetAxis("Horizontal");
			float v = Input.GetAxis("Vertical");

			// calculate camera relative direction to move:
			Vector3 camForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
			Vector3 move = v*camForward + h*Camera.main.transform.right;

			thirdPersonCharacter.Move (move, false, false);
		}
	}
}


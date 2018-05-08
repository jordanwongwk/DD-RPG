using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using System.Collections.Generic;
using RPG.Characters;

namespace RPG.CameraUI {
	public class CameraRaycaster : MonoBehaviour
	{
		[SerializeField] Texture2D moveCursor = null;
		[SerializeField] Texture2D enemyCursor = null;
		[SerializeField] Vector2 cursorHotspot = new Vector2 (0,0);

		const int WALKABLE_LAYER = 8;
	    float maxRaycastDepth = 100f; // Hard coded value

		Rect screenRectAtStartPlay = new Rect (0, 0, Screen.width, Screen.height);

		public delegate void OnMouseOverEnemy (Enemy enemy);
		public event OnMouseOverEnemy onMouseOverEnemy; 

		public delegate void OnMouseOverTerrain (Vector3 destination);
		public event OnMouseOverTerrain onMouseOverWalkable; 

		void Update()
		{
			// Check if pointer is over an interactable UI element
			if (EventSystem.current.IsPointerOverGameObject ())
			{
				// To Implement UI interaction
			} 
			else 
			{
				PerformRaycasts ();
			}
		}

		void PerformRaycasts ()
		{
			if (screenRectAtStartPlay.Contains (Input.mousePosition)) {
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				// Setting raycast priorities, the order matters cause return will exit the function
				if (RaycastForEnemy (ray)) { return; }
				if (RaycastForWalkable (ray)) { return;	}
			}
		}

		bool RaycastForEnemy (Ray ray) {
			RaycastHit hitInfo;
			Physics.Raycast (ray, out hitInfo, maxRaycastDepth);
			if (hitInfo.transform != null) {
				var gameObjectHit = hitInfo.collider.gameObject;
				var enemyHit = gameObjectHit.GetComponent<Enemy> ();
				if (enemyHit) {
					Cursor.SetCursor (enemyCursor, cursorHotspot, CursorMode.Auto);
					onMouseOverEnemy (enemyHit);
					return true;
				}
			}
			return false;
		}

		bool RaycastForWalkable (Ray ray) {
			RaycastHit hitInfo;
			LayerMask walkableLayer = 1 << WALKABLE_LAYER;
			bool walkableHit = Physics.Raycast (ray, out hitInfo, maxRaycastDepth, walkableLayer);
			if (walkableHit) {
				Cursor.SetCursor (moveCursor, cursorHotspot, CursorMode.Auto);
				onMouseOverWalkable (hitInfo.point);
				return true;
			}
			return false;
		}
	}
}
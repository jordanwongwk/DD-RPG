using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.CameraUI{
	[RequireComponent(typeof(CameraRaycaster))]
	public class CursorAffordance : MonoBehaviour {

		[SerializeField] Texture2D moveCursor = null;
		[SerializeField] Texture2D enemyCursor = null;
		[SerializeField] Texture2D unknownCursor = null;
		[SerializeField] Vector2 cursorHotspot = new Vector2 (0,0);

		//TODO Change const and serializefield conflict
		[SerializeField] const int walkableLayerNumber = 8;
		[SerializeField] const int enemyLayerNumber = 9;

		CameraRaycaster cameraRaycaster;

		// Use this for initialization
		void Awake () {
			cameraRaycaster = GetComponent<CameraRaycaster> ();
			cameraRaycaster.notifyLayerChangeObservers += CursorChange;
		}
		
		// Update is called once per frame
		void CursorChange (int newLayer) {
			switch (newLayer) {
				case walkableLayerNumber:
					Cursor.SetCursor (moveCursor, cursorHotspot, CursorMode.Auto);
					break;
				case enemyLayerNumber:
					Cursor.SetCursor (enemyCursor, cursorHotspot, CursorMode.Auto);
					break;
				default:
					Cursor.SetCursor (unknownCursor, cursorHotspot, CursorMode.Auto);
					break;
			}
		}

		//TODO Think about wheter should de-register the CursorChange
	}
}

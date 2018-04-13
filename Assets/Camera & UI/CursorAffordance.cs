using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CameraRaycaster))]
public class CursorAffordance : MonoBehaviour {

	[SerializeField] Texture2D moveCursor = null;
	[SerializeField] Texture2D enemyCursor = null;
	[SerializeField] Texture2D unknownCursor = null;
	[SerializeField] Vector2 cursorHotspot = new Vector2 (0,0);

	CameraRaycaster cameraRaycaster;

	// Use this for initialization
	void Awake () {
		cameraRaycaster = GetComponent<CameraRaycaster> ();
		cameraRaycaster.layerChangeObservers += CursorChange;
	}
	
	// Update is called once per frame
	void CursorChange (Layer newLayer) {
		switch (newLayer) {
			case Layer.Walkable:
				Cursor.SetCursor (moveCursor, cursorHotspot, CursorMode.Auto);
				break;
			case Layer.Enemy:
				Cursor.SetCursor (enemyCursor, cursorHotspot, CursorMode.Auto);
				break;
			case Layer.RaycastEndStop:
				Cursor.SetCursor (unknownCursor, cursorHotspot, CursorMode.Auto);
				break;
			default:
				Debug.LogError ("No cursor assigned for this layer.");
				return;
		}
	}

	//TODO Think about wheter should de-register the CursorChange
}

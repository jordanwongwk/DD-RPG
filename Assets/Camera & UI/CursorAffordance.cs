using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorAffordance : MonoBehaviour {

	[SerializeField] Texture2D moveCursor = null;
	[SerializeField] Texture2D enemyCursor = null;
	[SerializeField] Texture2D unknownCursor = null;
	[SerializeField] Vector2 cursorHotspot = new Vector2 (96, 96);

	CameraRaycaster cameraRaycaster;

	// Use this for initialization
	void Start () {
		cameraRaycaster = GetComponent<CameraRaycaster> ();
	}
	
	// Update is called once per frame
	void LateUpdate () {
		switch (cameraRaycaster.layerHit) {
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
}

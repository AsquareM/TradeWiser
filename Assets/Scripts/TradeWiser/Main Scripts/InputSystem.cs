using UnityEngine;
using UnityEngine.EventSystems;

public class InputSystem : MonoBehaviour
{
	private Camera mainCam;
	private Touch firstTouch, secondTouch;

	private Vector2 firstTouchPreviousPosition, secondTouchPreviousPosition;
	private Vector3 touchStart;

	private float touchPreviousPositionDifference, touchCurrentPositionDifference, zoomModifier;
	private float zoomSpeed;

	private readonly float maxY = 5f;
	private readonly float maxX = 10f;
	private readonly float minX = -10f;
	private readonly float minY = -5f;

	private static bool cannotPan;

	public static bool CannotPan { get => cannotPan; set => cannotPan = value; }

	// Start is called before the first frame update
	private void Start()
	{
		mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
		zoomSpeed = 0.02f;
		CannotPan = false;
	}

	// Update is called once per frame
	private void Update()
	{
		// Store position where mouse initially touched
		if (Input.GetMouseButtonDown(0))
			touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		if (!cannotPan)
		{
			// If two fingers are detected, calculate zoom in/zoom out level
			if (Input.touchCount == 2)
				Zoom();
			else if (Input.GetMouseButton(0)) // If only one finger detected, pan the camera around
				Pan();

			mainCam.orthographicSize = Mathf.Clamp(mainCam.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * 4, 6f, 12f);
		}

		// Code for OnMouseDown in the Android devices
		for (int i = 0; i < Input.touchCount; ++i)
		{
			if (Input.GetTouch(i).phase.Equals(TouchPhase.Began))
			{
				// Construct a ray from the current touch coordinates
				Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
				if (Physics.Raycast(ray, out RaycastHit hit))
				{
					hit.transform.gameObject.SendMessage("OnMouseDown");
				}
			}
		}
	}

	private void Zoom()
	{
		firstTouch = Input.GetTouch(0);
		secondTouch = Input.GetTouch(1);

		firstTouchPreviousPosition = firstTouch.position - firstTouch.deltaPosition;
		secondTouchPreviousPosition = secondTouch.position - secondTouch.deltaPosition;

		touchPreviousPositionDifference = (firstTouchPreviousPosition - secondTouchPreviousPosition).magnitude;
		touchCurrentPositionDifference = (firstTouch.position - secondTouch.position).magnitude;

		zoomModifier = (firstTouch.deltaPosition - secondTouch.deltaPosition).magnitude * zoomSpeed;

		// if distance between initial touches > distance between moved finger touches, zoom out
		if (touchPreviousPositionDifference > touchCurrentPositionDifference)
			mainCam.orthographicSize += zoomModifier;
		else if (touchPreviousPositionDifference < touchCurrentPositionDifference) // else zoom in
			mainCam.orthographicSize -= zoomModifier;
	}

	private void Pan()
	{
		Vector3 panDistance = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mainCam.transform.position = new Vector3(Mathf.Clamp(mainCam.transform.position.x + panDistance.x, minX, maxX),
										Mathf.Clamp(mainCam.transform.position.y + panDistance.y, minY, maxY), -10);
		//Debug.Log("Panning", mainCam.transform);
	}

}
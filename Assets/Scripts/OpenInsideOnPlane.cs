using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

/// <summary>Instantiate given prefab on touched position</summary>
public class PlaceOnTouch : MonoBehaviour
{
    [SerializeField] private TouchRaycast touchRaycast;
    [SerializeField] private GameObject windowPrefab; // An window that renders render-texture of "insideWall"

    // Start is called before the first frame update
    void Start()
    {
	if (touchRaycast == null) {
	    touchRaycast = GetComponent<TouchRaycast>();
	}

	if (touchRaycast == null) {
	    Debug.Log("Could not find TouchRaycast component. Cancel registering event handler...");
	} else if (windowPrefab == null) {
	    Debug.Log("WindowPrefab is not given. Cancel registering event handler...");
	} else {
	    touchRaycast.OnTouchedOnPlane+=OnTouchedOnPlane;
	}
    }

    void OnTouchedOnPlane(ARPlane plane, Pose pose) {
	// Instantiate "Window"
	var instance = Instantiate(windowPrefab);
	instance.transform.position = pose.position;
	instance.transform.rotation = Quaternion.LookRotation(plane.normal);

	var cpy = instance.GetComponentInChildren<CopyTransformRelation>();
	if (cpy != null) {
	    cpy.windowCenter = instance.transform;
	}
    }
}

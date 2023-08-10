using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

public delegate void OnTouchedOnPlaneHandler(ARPlane plane, Pose pose);

public class TouchRaycast : MonoBehaviour
{
    [SerializeField] private ARRaycastManager m_RaycastManager;
    [SerializeField] private ARAnchorManager m_AnchorManager;
    public GameObject anchorPrefab;
    public InputActionAsset actionAsset;

    public event OnTouchedOnPlaneHandler OnTouchedOnPlane;

    List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();
    
    void OnEnable()
    {
	var tapAction = actionAsset.FindActionMap("Screen Interaction").FindAction("Tap");
	if (tapAction == null) {
	    Debug.LogWarning("Couldn't find necessary input action. This component will do nothing in this scene.");
	    return;
	} else {
	    tapAction.performed += OnPosition;
	    Debug.Log("OnPosition handler is registered.");
	}
    }


    // https://github.com/Unity-Technologies/arfoundation-samples/blob/4ed26bb78e43aca10ce5e365dd8681d24cf63b6c/Assets/Scripts/Runtime/Events/PoseEventController.cs#L75
    void OnPosition(InputAction.CallbackContext ctx) {
	Debug.Log("OnPosition is called");
	var pointer = ctx.control.device as Pointer;
	if (pointer == null) {
	    Debug.Log($"Couldn't transform given input (type of '{ctx.control.device.GetType()}') to Pointer");
	    return;
	}

	var tapPosition = pointer.position.ReadValue();
	if (m_RaycastManager.Raycast(tapPosition, m_Hits, TrackableType.PlaneWithinPolygon)) {
	    var hit = m_Hits[0];
	    Debug.Log($"Touched!");
	    
	    GameObject prefab = Instantiate(anchorPrefab);
		
	    if (hit.trackable is ARPlane plane) {
		Debug.Log($"plane is touched");
		OnTouchedOnPlane(plane, hit.pose);
	    } else {
		prefab.transform.SetPositionAndRotation(hit.pose.position, hit.pose.rotation);
		prefab.AddComponent<ARAnchor>();
	    }
	}
    }

    // Update is called once per frame
    void Update()
    {
    }
}

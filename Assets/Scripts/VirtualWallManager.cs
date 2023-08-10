using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// 各 <code>ARPlane</code> に対応する <code>VirtualWall</code> の生成・管理をする。
/// </summary>
public class VirtualWallManager : MonoBehaviour
{
    // Required to subscribe plane update events
    [SerializeField]
    private ARPlaneManager m_ARPlaneManager;
    [SerializeField]
    private GameObject wallPrefab;


    [SerializeField]
    private Dictionary<TrackableId, GameObject> planeWallMapping = new Dictionary<TrackableId, GameObject>();

    void Start()
    {
	m_ARPlaneManager.planesChanged += OnPlanesChanged;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>Spawn, place, and register Virtual wall for given <code>plane</code></summary>
    void registerPlane(ARPlane plane) {
	if (planeWallMapping.ContainsKey(plane.trackableId)) {
	    return;
	}

	GameObject wall = Instantiate(wallPrefab);
	wall.transform.position = plane.center;
	transform.rotation = Quaternion.LookRotation(plane.normal);
	Debug.Log($"plane's normal was: {plane.normal}, set rotation to Quaternion.LookRotation(plane.normal)");

	planeWallMapping.Add(plane.trackableId, wall);
    }

    /// <summary>Remove virtual wall if given <code>plane</code> have one, and unregister them</summary>
    void unregisterPlane(ARPlane plane) {
	if (planeWallMapping.ContainsKey(plane.trackableId)) {
	    Destroy(planeWallMapping[plane.trackableId]);
	    planeWallMapping.Remove(plane.trackableId);
	}
    }

    void OnPlanesChanged(ARPlanesChangedEventArgs args) {
	foreach (var plane in args.added) {
	    if (plane.subsumedBy == null && plane.alignment == PlaneAlignment.Vertical) {
		registerPlane(plane);
	    }
	}

	foreach (var plane in args.updated) {
	    if (plane.subsumedBy != null) {
		unregisterPlane(plane);
	    }
	}

	foreach (var plane in args.removed) {
	    unregisterPlane(plane);
	}
    }
}

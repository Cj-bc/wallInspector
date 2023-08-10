using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

/// <summary> Drive Transform to parent
/// </summary>
public class CopyTransformRelation : MonoBehaviour
{
    public Transform realCamera;
    public Transform windowCenter;

    void Start() {
	// I assign this directly temporary. It should not be done in this way.
	// It reduces reusability
	realCamera = Camera.main.gameObject.transform;
    }
    void Update()
    {
	if (realCamera == null || windowCenter == null) {
	    return;
	}
        transform.localPosition = windowCenter.InverseTransformPoint(realCamera.position);
	transform.localRotation = Quaternion.Inverse(windowCenter.rotation) * realCamera.rotation;
    }
}

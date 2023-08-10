using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

/// <summary>Provides some Gizmos for ARPlane debugging.</summary>
[RequireComponent(typeof(ARPlane))]
public class ARPlaneDebugGizmos : MonoBehaviour
{
    private ARPlane m_ARPlane;
    [SerializeField] private float normalLength = 3.0f;
    [SerializeField] private Color normalColor = Color.green;

    // Start is called before the first frame update
    void Start()
    {
        m_ARPlane = GetComponent<ARPlane>();
    }

    void OnDrawGizmosSelected() {
	Debug.Log($"Writting ARPlane gizmo...");
	Gizmos.color = normalColor;
	Gizmos.DrawLine(m_ARPlane.center, m_ARPlane.normal * normalLength);

	Gizmos.color = Color.red;
	Gizmos.DrawLine(m_ARPlane.center, transform.forward * normalLength);

	Gizmos.DrawSphere(m_ARPlane.center, 1);
	Debug.Log($"Finished writting ARPlane gizmo; normal is {m_ARPlane.normal}");
    }
}

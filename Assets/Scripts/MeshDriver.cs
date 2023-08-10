using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshDriver : MonoBehaviour
{
    [SerializeField] private MeshFilter filter;
    [SerializeField] private ParticleSystem system;
    private ParticleSystem.ShapeModule shape;
    // Start is called before the first frame update
    void Start()
    {
        shape = system.shape;
	shape.shapeType = ParticleSystemShapeType.Mesh;
    }

    // Update is called once per frame
    void Update()
    {
        if (filter.mesh != null && filter.mesh != shape.mesh) {
	    shape.mesh = filter.mesh;
	}
    }
}

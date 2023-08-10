using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class InsideWallMesh : MonoBehaviour
{
    [Tooltip("背面の距離")] public float depthOfWall = 1.0f;
    
    public ARPlane m_ARPlane;
    public MeshFilter m_MeshFilter;
    private Mesh _mesh;
    public Mesh mesh { get; }
    // Start is called before the first frame update
    void Start()
    {
	if (m_MeshFilter == null) {
	    GetComponent<MeshFilter>();
	}

	if (m_ARPlane == null) {
	    Debug.Log("No ARPlane is given. This wont visualize any InsideWall");
	} else {
	    m_ARPlane.boundaryChanged += OnBoundaryChanged;
	}

	_mesh = new Mesh();
	m_MeshFilter.mesh = _mesh;
    }

    /// <summary>
    /// Calculate wall mesh's vertices positions based on given plane boundary
    /// </summary>
    void UpdateMesh(ARPlane plane) {
	var vertices = new List<Vector3>();


	// Convert each plane-space points into world-space.
	var transformationMatrix = Matrix4x4.identity;
	transformationMatrix.SetColumn(0, transform.right);
	transformationMatrix.SetColumn(1, transform.up);
	transformationMatrix.SetColumn(2, transform.forward);
	transformationMatrix.SetColumn(3, new Vector4(transform.position.x, transform.position.y, transform.position.z, 1));

	/*
	       3 o------o 1
	       .`:    .`| 
	     .`  :  .`  |
	    o------o 0  |
	    | 2  : |    |
	    |    o.|....o 7
	    |  .`5 |  .`
	    |,`    | .
	    o------o'
	  4         6

	  Verticesは上記のような順番で格納されている。手前の面のvertex、奥の面のvertexの順番。
	 */
	foreach (Vector2 point in plane.boundary) {
	    // Vertex that is at the same location of <code>point</code>
	    var frontVertex = transformationMatrix.MultiplyPoint3x4(new Vector3(point.x, 0, point.y));
	    var backVertex = frontVertex + -(plane.normal) * depthOfWall;

	    vertices.Add(frontVertex);
	    vertices.Add(backVertex);
	}

	// 壁面のtriangles:
	// Triangleは、以下の組合せのvertexで生成する。
	// 0, 1, 2; 1, 2, 3; 2, 3, 4 ... n-2, n-1, n; n-1, n, 0; n, 0, 1 (n = vertiecs.Count - 1)
	//
	// n-1, n, 0 と n, 0, 1 だけはループで回せないので別に作る
	var triangles = new List<int>();
	for (int i = 1; i < vertices.Count - 1; i++) {
	    triangles.Add(i-1);
	    triangles.Add(i);
	    triangles.Add(i+1);
	}
	triangles.Add(vertices.Count - 2);
	triangles.Add(vertices.Count - 1);
	triangles.Add(0);
	triangles.Add(vertices.Count - 1);
	triangles.Add(0);
	triangles.Add(1);

	// Apply them to actual mesh
	_mesh.SetTriangles(new List<int>(), 0); // Make sure no vertices are required
	_mesh.SetVertices(vertices);
	_mesh.SetTriangles(triangles, 0);
    }

    // 
    void OnBoundaryChanged(ARPlaneBoundaryChangedEventArgs args) {
	UpdateMesh(args.plane);
    }

    void OnDrawGizmos() {
	foreach (Vector3 p in _mesh.vertices) {
	    Gizmos.color = Color.green;
	    Gizmos.DrawSphere(p, 10f);
	}
    }
}

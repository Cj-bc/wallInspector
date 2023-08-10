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
	_mesh.name = "InsideWall Mesh";
	m_MeshFilter.mesh = _mesh;
    }

    /// <summary>
    /// Calculate wall mesh's vertices positions based on given plane boundary
    /// </summary>
    void UpdateMesh(ARPlane plane) {
	var vertices = new List<Vector3>();


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
	    var frontVertex = new Vector3(point.x, 0, point.y);
	    var backVertex = frontVertex + -(plane.normal) * depthOfWall;

	    vertices.Add(frontVertex);
	    vertices.Add(backVertex);
	}

	var maxIndex = vertices.Count - 1;

	// 壁面のtriangles:
	// Triangleは、以下の組合せのvertexで生成する。これは、Quadを構成する2つのTriangleの法線が同じ方向を向くようにするため。
	// 0, 1, 2; 2, 1, 3; 2, 3, 4; 4, 3, 5 ... x-1, x, x+1; x, x-1, x+1; ... n-1, n, n+1; n, n-1, 0; n, 0, 1 (n = vertiecs.Count - 1)
	//
	// n-1, n, 0 と n, 0, 1 だけはループで回せないので別に作る
	var triangles = new List<int>();
	for (int i = 0; i < vertices.Count - 1; i+=2) {
	    // Clockwise
	    triangles.Add(i);
	    triangles.Add((i+1) % maxIndex);
	    triangles.Add((i+2) % maxIndex);

	    // Counter Clockwise
	    triangles.Add((i+2) % maxIndex);
	    triangles.Add((i+1) % maxIndex);
	    triangles.Add((i+3) % maxIndex);
	}

	// Apply them to actual mesh
	_mesh.Clear();
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

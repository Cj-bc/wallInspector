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


	Vector3 backSurfaceCenter = Vector3.zero;
	/*
	     4,5 o------o 1,2
	       .`:    .`| 
	     .`  :  .`  |
	    o------o 0  |
	    | 3  : |    |
	    |    o.|....o 10,11
	    |  .`78|  .`
	    |,`    | .
	    o------o'      (78 = 7,8)
	  6         9

	  Verticesは上記のような順番で格納されている。手前の面のvertex、奥の面のvertex、backSurface用のvertexの順番。
	 */
	foreach (Vector2 point in plane.boundary) {
	    // Meshを作成する際のvertexはMeshのローカル座標になり、グローバル座標系や <code>plane.normal</code> 等を考慮しなくていい。
	    // なのでZ軸を奥行方向とし、奥行を足すときには -forward する。
	    var frontVertex = new Vector3(point.x, point.y, 0);
	    var backVertex = frontVertex - Vector3.forward * depthOfWall;

	    backSurfaceCenter += backVertex;

	    // 側面のtriangles用のvertex
	    vertices.Add(frontVertex);
	    vertices.Add(backVertex);
	    // backSurface用のvertex。flatに描画するためにvertexを分ける
	    vertices.Add(backVertex);
	}

	// backSurfaceCenter now represents center position of backSurface.
	backSurfaceCenter /= plane.boundary.Length;

	// Be aware that backSurfaceCenter isn't in vertices yet intentionally.
	int backSurfaceCenterIdx = vertices.Count;

	/*
	         o------o
	       .`:####.`| Backface/背面
	     .`  :##.`##|
	    o------o####|
	    |    :#|####|
	    |    o.|....o
	    |  .`  |  .`
	    |,`    | .
	    o------o'


	         o------o
	       .`#####.`| 壁面(手前側は貼らない)
	     .`#####.`##|
	    o------o####|
	    |####: |####|
	    |####o.|####o
	    |##.`##|##.`
	    |,#####|#.
	    o------o'

	Triangleは、以下の組合せのvertexで生成する。これは、Quadを構成する2つのTriangleの法線が同じ方向を向くようにするため。
	0, 1, 3; 3, 1, 4; 5, 2, backSurfaceCenterIdx; 3, 4, 6; ... x, x+1, x+3; x+3, x+1, x+4; x+5, x+2, backSurfaceCenterIdx; ...
	 */
	var triangles = new List<int>();
	for (int i = 0; i < vertices.Count - 1; i+=3) {
	    // 壁面のtriangle 1
	    triangles.Add(i);
	    triangles.Add((i+1) % vertices.Count);
	    triangles.Add((i+3) % vertices.Count);

	    // 壁面のtriangle 2
	    triangles.Add((i+3) % vertices.Count);
	    triangles.Add((i+1) % vertices.Count);
	    triangles.Add((i+4) % vertices.Count);

	    // 背面のtriangles
	    triangles.Add((i+5) % vertices.Count);
	    triangles.Add(i+2);
	    triangles.Add(backSurfaceCenterIdx);
	}

	vertices.Add(backSurfaceCenter); // 壁面を作る際に考慮したくなかったのでここで追加

	// Apply them to actual mesh
	_mesh.Clear();
	_mesh.SetVertices(vertices);
	_mesh.SetTriangles(triangles, 0);
	_mesh.RecalculateNormals();
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

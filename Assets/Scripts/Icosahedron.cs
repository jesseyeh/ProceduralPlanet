using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Icosahedron : MonoBehaviour {

  // 12 vertices in an icosahedron
  private Vector3[] vertices = new Vector3[12];
  private Mesh mesh;

  public int scale = 1;

  private int triangleIndex;

  public bool autoUpdate;

  private void Awake() {
    Generate();
  }

  public void Generate() {

    mesh = new Mesh();
    mesh.name = "Procedural Icosahedron";
    this.GetComponent<MeshFilter>().mesh = mesh;

    float t = 1 + Mathf.Sqrt(5) / 2;
    CreateVertices(t);
    CreateTriangles();
  }

  // create the 12 vertices
  private void CreateVertices(float t) {

    vertices[0]  = new Vector3(-1,  t,  0) * scale;
    vertices[1]  = new Vector3( 1,  t,  0) * scale;
    vertices[2]  = new Vector3(-1, -t,  0) * scale;
    vertices[3]  = new Vector3( 1, -t,  0) * scale;

    vertices[4]  = new Vector3( 0, -1, -t) * scale;
    vertices[5]  = new Vector3( 0,  1, -t) * scale;
    vertices[6]  = new Vector3( 0, -1,  t) * scale;
    vertices[7]  = new Vector3( 0,  1,  t) * scale;

    vertices[8]  = new Vector3( t,  0,  1) * scale;
    vertices[9]  = new Vector3( t,  0, -1) * scale;
    vertices[10] = new Vector3(-t,  0,  1) * scale;
    vertices[11] = new Vector3(-t,  0, -1) * scale;

    mesh.vertices = vertices;
  }

  // create the 20 faces
  private void CreateTriangles() {

    triangleIndex = 0;
    int[] triangles = new int[20 * 3];

    // order vertices clockwise so that the face faces the right direction

    // five faces around point 0
    AddTriangle(triangles, 0, 5, 11);
    AddTriangle(triangles, 0, 1, 5);
    AddTriangle(triangles, 0, 7, 1);
    AddTriangle(triangles, 0, 10, 7);
    AddTriangle(triangles, 0, 11, 10);

    // five adjacent faces
    AddTriangle(triangles, 1, 9, 5);
    AddTriangle(triangles, 5, 4, 11);
    AddTriangle(triangles, 11, 2, 10);
    AddTriangle(triangles, 10, 6, 7);
    AddTriangle(triangles, 7, 8, 1);

    // five face around point 3 (polar opposite of point 0)
    AddTriangle(triangles, 3, 4, 9);
    AddTriangle(triangles, 3, 2, 4);
    AddTriangle(triangles, 3, 6, 2);
    AddTriangle(triangles, 3, 8, 6);
    AddTriangle(triangles, 3, 9, 8);

    // five adjacent faces
    AddTriangle(triangles, 4, 5, 9);
    AddTriangle(triangles, 2, 11, 4);
    AddTriangle(triangles, 6, 10, 2);
    AddTriangle(triangles, 8, 7, 6);
    AddTriangle(triangles, 9, 1, 8);

    mesh.triangles = triangles;
  }

  // creates a single triangle face
  private void AddTriangle(int[] triangles, int v1, int v2, int v3) {

    triangles[triangleIndex] = v1;
    triangles[triangleIndex + 1] = v2;
    triangles[triangleIndex + 2] = v3;

    // increment triangleIndex to start at the next set of vertices
    triangleIndex += 3;
  }

  // draw gizmo at each vertex
  private void OnDrawGizmos() {

    Gizmos.color = Color.black;
    for(int i = 0; i < vertices.Length; i++) {
      Gizmos.DrawSphere(vertices[i], 0.1f);
    }
  }
}

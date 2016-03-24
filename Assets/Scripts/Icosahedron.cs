using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Icosahedron : MonoBehaviour {

  // 12 vertices in an icosahedron
  private Vector3[] vertices = new Vector3[12];

  private void Awake() {
    Generate();
  }

  private void Generate() {

    float t = 1 + Mathf.Sqrt(5) / 2;
    AddVertices(vertices, t);
  }

  // create the 12 vertices
  private void AddVertices(Vector3[] vertices, float t) {

    vertices[0]  = new Vector3(-1,  t,  0);
    vertices[1]  = new Vector3( 1,  t,  0);
    vertices[2]  = new Vector3(-1, -t,  0);
    vertices[3]  = new Vector3( 1, -t,  0);

    vertices[4]  = new Vector3( 0, -1,  t);
    vertices[5]  = new Vector3( 0,  1,  t);
    vertices[6]  = new Vector3( 0, -1, -t);
    vertices[7]  = new Vector3( 0,  1, -t);

    vertices[8]  = new Vector3( t,  0, -1);
    vertices[9]  = new Vector3( t,  0,  1);
    vertices[10] = new Vector3(-t,  0, -1);
    vertices[11] = new Vector3(-t,  0,  1);
  }

  // draw gizmo at each vertex
  private void OnDrawGizmos() {
    Gizmos.color = Color.black;
    for(int i = 0; i < vertices.Length; i++) {
      Gizmos.DrawSphere(vertices[i], 0.1f);
    }
  }
}

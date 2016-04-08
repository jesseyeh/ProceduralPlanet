using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class IcoMesh : MonoBehaviour {

  #region VARIABLES
  [Header("Appearance")]
  public int scale = 1;
  [Range(0, 3)]
  public int subdivisions = 0;
  public int seed;
  [Range(0, 1)]
  public float oceanTilesPercentage;
  [Range(0, 1)]
  public float treesPercentage;
  public Color oceanColor;
  public Color desertColor;
  public Color plainsColor;
  public Color forestColor;
  public Color mountainsColor;
  public Color snowColor;
  [Range(1, 10)]
  public int bumpiness;
  public int planetaryRotationSpeed = 1;

  [Header("Props")]
  public List<Transform> trees;

  [Header("Misc")]
  public IcoTile tilePrefab;
  public bool autoUpdate;
  public CloudSpawner cloudSpawner;

  private Mesh mesh;
  private List<Vector3> vertices;
  private List<int> triangles;
  private List<Color> colors;
  private List<IcoTile> tiles;
  private Dictionary<Vector3, List<IcoTile>> tilesByVertices;
  #endregion

  private void Awake() {

    Generate();
  }

  public void Generate() {

    // initialize variables
    vertices = new List<Vector3>();
    triangles = new List<int>();
    colors = new List<Color>();
    tiles = new List<IcoTile>();
    tilesByVertices = new Dictionary<Vector3, List<IcoTile>>();
    mesh = new Mesh();
    mesh.name = "Procedural Icosahedron";
    this.GetComponent<MeshFilter>().mesh = mesh;

    cloudSpawner.Generate();

    AddVertices();
    AddTriangles();
    AddTiles();
    tiles = Utility.Shuffle(seed, tiles);
    AddNeighbors();
    AddElevations();
    AddColors();
    SpawnTrees();

    AddCollider(mesh);
  }

  private void AddVertices() {

    float t = (1 + Mathf.Sqrt(5)) / 2;

    // five faces around point 0
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3(-1,  t,  0)) * scale); // 0
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3( 0,  1, -t)) * scale); // 5
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3(-t,  0, -1)) * scale); // 11

    vertices.Add(Utility.AdjustForUnitSphere(new Vector3(-1,  t,  0)) * scale); // 0
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3( 1,  t,  0)) * scale); // 1
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3( 0,  1, -t)) * scale); // 5

    vertices.Add(Utility.AdjustForUnitSphere(new Vector3(-1,  t,  0)) * scale); // 0
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3( 0,  1,  t)) * scale); // 7
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3( 1,  t,  0)) * scale); // 1

    vertices.Add(Utility.AdjustForUnitSphere(new Vector3(-1,  t,  0)) * scale); // 0
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3(-t,  0,  1)) * scale); // 10
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3( 0,  1,  t)) * scale); // 7

    vertices.Add(Utility.AdjustForUnitSphere(new Vector3(-1,  t,  0)) * scale); // 0
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3(-t,  0, -1)) * scale); // 11
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3(-t,  0,  1)) * scale); // 10

    // five adjacent faces
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3( 1,  t,  0)) * scale); // 1
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3( t,  0, -1)) * scale); // 9
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3( 0,  1, -t)) * scale); // 5

    vertices.Add(Utility.AdjustForUnitSphere(new Vector3( 0,  1, -t)) * scale); // 5
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3( 0, -1, -t)) * scale); // 4
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3(-t,  0, -1)) * scale); // 11

    vertices.Add(Utility.AdjustForUnitSphere(new Vector3(-t,  0, -1)) * scale); // 11
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3(-1, -t,  0)) * scale); // 2
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3(-t,  0,  1)) * scale); // 10

    vertices.Add(Utility.AdjustForUnitSphere(new Vector3(-t,  0,  1)) * scale); // 10
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3( 0, -1,  t)) * scale); // 6
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3( 0,  1,  t)) * scale); // 7

    vertices.Add(Utility.AdjustForUnitSphere(new Vector3( 0,  1,  t)) * scale); // 7
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3( t,  0,  1)) * scale); // 8
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3( 1,  t,  0)) * scale); // 1

    // five face around point 3 (polar opposite of point 0)
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3( 1, -t,  0)) * scale); // 3
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3( 0, -1, -t)) * scale); // 4
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3( t,  0, -1)) * scale); // 9

    vertices.Add(Utility.AdjustForUnitSphere(new Vector3( 1, -t,  0)) * scale); // 3
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3(-1, -t,  0)) * scale); // 2
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3( 0, -1, -t)) * scale); // 4

    vertices.Add(Utility.AdjustForUnitSphere(new Vector3( 1, -t,  0)) * scale); // 3
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3( 0, -1,  t)) * scale); // 6
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3(-1, -t,  0)) * scale); // 2

    vertices.Add(Utility.AdjustForUnitSphere(new Vector3( 1, -t,  0)) * scale); // 3
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3( t,  0,  1)) * scale); // 8
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3( 0, -1,  t)) * scale); // 6

    vertices.Add(Utility.AdjustForUnitSphere(new Vector3( 1, -t,  0)) * scale); // 3
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3( t,  0, -1)) * scale); // 9
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3( t,  0,  1)) * scale); // 8

    // five adjacent faces
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3( 0, -1, -t)) * scale); // 4
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3( 0,  1, -t)) * scale); // 5
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3( t,  0, -1)) * scale); // 9

    vertices.Add(Utility.AdjustForUnitSphere(new Vector3(-1, -t,  0)) * scale); // 2
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3(-t,  0, -1)) * scale); // 11
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3( 0, -1, -t)) * scale); // 4

    vertices.Add(Utility.AdjustForUnitSphere(new Vector3( 0, -1,  t)) * scale); // 6
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3(-t,  0,  1)) * scale); // 10
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3(-1, -t,  0)) * scale); // 2

    vertices.Add(Utility.AdjustForUnitSphere(new Vector3( t,  0,  1)) * scale); // 8
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3( 0,  1,  t)) * scale); // 7
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3( 0, -1,  t)) * scale); // 6

    vertices.Add(Utility.AdjustForUnitSphere(new Vector3( t,  0, -1)) * scale); // 9
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3( 1,  t,  0)) * scale); // 1
    vertices.Add(Utility.AdjustForUnitSphere(new Vector3( t,  0,  1)) * scale); // 8
  }

  private void AddTriangles() {

    // the 20 faces of the non-subdivided icosahedron
    for (int i = 0; i < vertices.Count; i += 3) {
      triangles.Add(i);
      triangles.Add(i + 1);
      triangles.Add(i + 2);
    }
    
    // used to help add subdivisions into the trianglesSubdivisions List
    int start;

    List<int> trianglesSubdivisions = new List<int>();

    // copy of the previous iteration's vertices
    List<Vector3> oldVertices;

    for(int i = 0; i < subdivisions; i++) {
      // save a copy of the previous iteration's vertices
      oldVertices = new List<Vector3>(vertices);
      // reset variables from the previous iteration
      vertices.Clear();
      trianglesSubdivisions.Clear();
      start = 0;

      for(int j = 0; j < oldVertices.Count; j += 3) {
        Vector3 p0 = oldVertices[j];
        Vector3 p1 = oldVertices[j + 1];
        Vector3 p2 = oldVertices[j + 2];

        // calculate midpoints
        Vector3 mp01 = new Vector3((p0.x + p1.x) / 2f,
                                   (p0.y + p1.y) / 2f,
                                   (p0.z + p1.z) / 2f);
        Vector3 mp12 = new Vector3((p1.x + p2.x) / 2f,
                                   (p1.y + p2.y) / 2f,
                                   (p1.z + p2.z) / 2f);
        Vector3 mp02 = new Vector3((p0.x + p2.x) / 2f,
                                   (p0.y + p2.y) / 2f,
                                   (p0.z + p2.z) / 2f);
        // first subdivided face
        vertices.Add(Utility.AdjustForUnitSphere(p0)   * scale);
        vertices.Add(Utility.AdjustForUnitSphere(mp01) * scale);
        vertices.Add(Utility.AdjustForUnitSphere(mp02) * scale);

        // second subdivided face
        vertices.Add(Utility.AdjustForUnitSphere(mp01) * scale);
        vertices.Add(Utility.AdjustForUnitSphere(mp12) * scale);
        vertices.Add(Utility.AdjustForUnitSphere(mp02) * scale);

        // third subdivided face
        vertices.Add(Utility.AdjustForUnitSphere(mp01) * scale);
        vertices.Add(Utility.AdjustForUnitSphere(p1)   * scale);
        vertices.Add(Utility.AdjustForUnitSphere(mp12) * scale);

        // fourth subdivided face
        vertices.Add(Utility.AdjustForUnitSphere(mp02) * scale);
        vertices.Add(Utility.AdjustForUnitSphere(mp12) * scale);
        vertices.Add(Utility.AdjustForUnitSphere(p2)   * scale);

        int currVerticesCount = vertices.Count;

        // add subdivisions
        for(int k = start; k < currVerticesCount; k += 3) {
          trianglesSubdivisions.Add(k);
          trianglesSubdivisions.Add(k + 1);
          trianglesSubdivisions.Add(k + 2);
        }
        // update start
        start = currVerticesCount;
      }
    }

    /* -- set vertices and triangles for the mesh -- */
    mesh.vertices = vertices.ToArray();
    if(subdivisions == 0) {
      // use base triangles List when there are no subdivisions
      mesh.triangles = triangles.ToArray();
    } else {
      // otherwise, use trianglesSubdivisions List
      mesh.triangles = trianglesSubdivisions.ToArray();
      triangles.Clear();
      triangles = trianglesSubdivisions;
    }
    mesh.RecalculateNormals();
  }

  private void AddTiles() {

    // prevent duplicate instantiations of tiles when working in the Editor
    foreach(IcoTile tile in this.GetComponentsInChildren<IcoTile>()) {
      DestroyImmediate(tile.gameObject);
    }

    for (int i = 0; i < vertices.Count; i += 3) {
      // create a new IcoTile from the vertices List
      IcoTile tile = Instantiate<IcoTile>(tilePrefab);
      tile.v0 = vertices[i];
      tile.v1 = vertices[i + 1];
      tile.v2 = vertices[i + 2];
      tile.transform.SetParent(this.transform, false);
      tiles.Add(tile);

      // set up the tilesByVertices Dictionary
      // vertices (keys) are mapped to a List of IcoTiles (values) containing them
      if(tilesByVertices.ContainsKey(vertices[i])) {
        tilesByVertices[vertices[i]].Add(tile);
      } else {
        tilesByVertices.Add(vertices[i], new List<IcoTile>());
        tilesByVertices[vertices[i]].Add(tile);
      }
      // do the same for vertices[i + 1] and vertices[i + 2]
      if(tilesByVertices.ContainsKey(vertices[i + 1])) {
        tilesByVertices[vertices[i + 1]].Add(tile);
      } else {
        tilesByVertices.Add(vertices[i + 1], new List<IcoTile>());
        tilesByVertices[vertices[i + 1]].Add(tile);
      }
      if(tilesByVertices.ContainsKey(vertices[i + 2])) {
        tilesByVertices[vertices[i + 2]].Add(tile);
      } else {
        tilesByVertices.Add(vertices[i + 2], new List<IcoTile>());
        tilesByVertices[vertices[i + 2]].Add(tile);
      }
    }
  }

  private void AddNeighbors() {

    for(int i = 0; i < tiles.Count; i++) {
      // check for common vertices between tiles
      // each intersection will have two tiles each
      var intersection0 = tilesByVertices[tiles[i].v0].Intersect(tilesByVertices[tiles[i].v1]);
      var intersection1 = tilesByVertices[tiles[i].v1].Intersect(tilesByVertices[tiles[i].v2]);
      var intersection2 = tilesByVertices[tiles[i].v0].Intersect(tilesByVertices[tiles[i].v2]);

      // a size of two matching vertices between tiles indicates that they are neighbors

      // set the neighbor for intersection0
      if(intersection0.Count() > 1 && tiles[i].GetNeighbor(0) == null) {
        if(intersection0.ToArray()[0] != tiles[i]) {
          tiles[i].SetNeighbor(0, intersection0.ToArray()[0]);
        } else {
          tiles[i].SetNeighbor(0, intersection0.ToArray()[1]);
        }
      }
      // do the same for intersection1 and intersection2
      if(intersection1.ToArray()[0] != tiles[i]) {
        tiles[i].SetNeighbor(1, intersection1.ToArray()[0]);
      } else {
        tiles[i].SetNeighbor(1, intersection1.ToArray()[1]);
      }
      if(intersection2.ToArray()[0] != tiles[i]) {
        tiles[i].SetNeighbor(2, intersection2.ToArray()[0]);
      } else {
        tiles[i].SetNeighbor(2, intersection2.ToArray()[1]);
      }
    }
  }

  private void AddElevations() {

    // naive implementation
    System.Random prng = new System.Random();

    float elevation;
    foreach(Vector3 vertex in tilesByVertices.Keys) {
      elevation = (1 + (prng.Next(0, bumpiness)) / 100f);
      for(int i = 0; i < vertices.Count; i++) {
        if(vertex == vertices[i]) {
          vertices[i] *= elevation;
        }
      }
    }

    mesh.vertices = vertices.ToArray();
  }

  private void AddColors() {

    for(int i = 0; i < tiles.Count; i++) {
      tiles[i].color = plainsColor;
      for(int j = 0; j < 3; j++) {
        colors.Add(tiles[i].color);
      }
    }

    mesh.colors = colors.ToArray();
  }

  private void SpawnTrees() {

    int numTrees = (int)(tiles.Count * treesPercentage);
    System.Random prng = new System.Random();
    for(int i = 0; i < numTrees; i++) {
      int randomTreeIndex = prng.Next(0, trees.Count);
      Vector3 gravityUp = (tiles[i].v0 - this.transform.position).normalized;
      Transform tree = Instantiate(trees[randomTreeIndex], tiles[i].v0, Quaternion.FromToRotation(trees[0].transform.up, gravityUp)) as Transform;
      tree.localScale *= scale;
      tree.SetParent(this.transform, false);
    }
  }

  private void AddCollider(Mesh mesh) {

    // add a mesh collider to the generated mesh
    if (this.GetComponent<MeshCollider>() == null) {
      MeshCollider meshc = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
      meshc.sharedMesh = mesh;
    } else {
      // destroy all existing colliders
      foreach(MeshCollider meshc in this.GetComponents<MeshCollider>()) {
        DestroyImmediate(meshc);
      }
      // create new mesh collider
      MeshCollider newMeshc = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
      newMeshc.sharedMesh = mesh;
    }
  }

  private void Update() {

    this.transform.RotateAround(Vector3.zero, Vector3.up, planetaryRotationSpeed * Time.deltaTime);
  }

  #region GIZMOS
  [Header("Debug")]
  public int debugGizmoIndex;
  public float gizmoSize = 0.1f;

  // draw gizmos at specified vertices (used for debugging)
  private void OnDrawGizmos() {

    // draw gizmos for a tile's and its neighbors' vertices
    if (tiles != null) {
      Gizmos.color = Color.green;

      // draw gizmos for tiles[0]
      Gizmos.DrawSphere(tiles[debugGizmoIndex].v0, gizmoSize);
      Gizmos.DrawSphere(tiles[debugGizmoIndex].v1, gizmoSize);
      Gizmos.DrawSphere(tiles[debugGizmoIndex].v2, gizmoSize);

      Gizmos.color = Color.red;
      // draw gizmos for tiles[debugGizmoIndex]'s neighbors
      for (int i = 0; i < 3; i++) {
        if (i == 1) Gizmos.color = Color.blue;
        if (i == 2) Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(tiles[debugGizmoIndex].GetNeighbor(i).v0, gizmoSize / (i + 2));
        Gizmos.DrawSphere(tiles[debugGizmoIndex].GetNeighbor(i).v1, gizmoSize / (i + 2));
        Gizmos.DrawSphere(tiles[debugGizmoIndex].GetNeighbor(i).v2, gizmoSize / (i + 2));
      }
    }

    // draw gizmos for cloud spawn positions
    if (cloudSpawner != null) {
      if(cloudSpawner.spawnPoints != null) {
        Gizmos.color = Color.black;
        for(int i = 0; i < cloudSpawner.spawnPoints.Count; i++) {
          Gizmos.DrawSphere(cloudSpawner.spawnPoints[i], 0.1f);
        }
      }
    }
  }
  #endregion
}

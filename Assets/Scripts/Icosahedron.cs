using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Icosahedron : MonoBehaviour {

  #region VARIABLES
  private List<Vector3> vertices;
  private List<int> triangles;
  private Dictionary<long, int> midpointCache;
  Dictionary<int, Tile> tiles;
  Dictionary<Vector3, List<Tile>> verticesUsedByTiles;
  Dictionary<int, int[]> neighbors;
  List<Color> colors;
  private Mesh mesh;

  public int scale = 1;
  [Range(0, 3)]
  public int subdivisions = 0;

  public bool autoUpdate;
  public bool isFlatShaded;
  public int seed;
  public Color landColor;
  public Color oceanColor;
  
  [Range(0, 1)]
  public float oceanTilesPercentage;
  #endregion

  private void Awake() {
    Generate();
  }

  public void Generate() {
    
    // initialize variables
    vertices = new List<Vector3>();
    triangles = new List<int>();
    colors = new List<Color>();
    tiles = new Dictionary<int, Tile>();
    verticesUsedByTiles = new Dictionary<Vector3, List<Tile>>();
    midpointCache = new Dictionary<long, int>();
    mesh = new Mesh();
    mesh.name = "Procedural Icosahedron";
    this.GetComponent<MeshFilter>().mesh = mesh;

    float t = (1 + Mathf.Sqrt(5)) / 2;
    if(!isFlatShaded) {
      CreateVertices(t);
      CreateTriangles();
    } else {
      CreateFlatVertices(t);
      CreateFlatTriangles();
    }

    // organize vertices into tiles
    SetTiles();
    ShuffleTiles(seed);

    // AddCollider(mesh);
    
    SetVertexColors();
  }

  // create separate vertices for flat shading
  private void CreateFlatVertices(float t) {

    // five faces around point 0
    vertices.Add(adjustForUnitSphere(new Vector3(-1,  t,  0)) * scale); // 0
    vertices.Add(adjustForUnitSphere(new Vector3( 0,  1, -t)) * scale); // 5
    vertices.Add(adjustForUnitSphere(new Vector3(-t,  0, -1)) * scale); // 11

    vertices.Add(adjustForUnitSphere(new Vector3(-1,  t,  0)) * scale); // 0
    vertices.Add(adjustForUnitSphere(new Vector3( 1,  t,  0)) * scale); // 1
    vertices.Add(adjustForUnitSphere(new Vector3( 0,  1, -t)) * scale); // 5

    vertices.Add(adjustForUnitSphere(new Vector3(-1,  t,  0)) * scale); // 0
    vertices.Add(adjustForUnitSphere(new Vector3( 0,  1,  t)) * scale); // 7
    vertices.Add(adjustForUnitSphere(new Vector3( 1,  t,  0)) * scale); // 1

    vertices.Add(adjustForUnitSphere(new Vector3(-1,  t,  0)) * scale); // 0
    vertices.Add(adjustForUnitSphere(new Vector3(-t,  0,  1)) * scale); // 10
    vertices.Add(adjustForUnitSphere(new Vector3( 0,  1,  t)) * scale); // 7

    vertices.Add(adjustForUnitSphere(new Vector3(-1,  t,  0)) * scale); // 0
    vertices.Add(adjustForUnitSphere(new Vector3(-t,  0, -1)) * scale); // 11
    vertices.Add(adjustForUnitSphere(new Vector3(-t,  0,  1)) * scale); // 10

    // five adjacent faces
    vertices.Add(adjustForUnitSphere(new Vector3( 1,  t,  0)) * scale); // 1
    vertices.Add(adjustForUnitSphere(new Vector3( t,  0, -1)) * scale); // 9
    vertices.Add(adjustForUnitSphere(new Vector3( 0,  1, -t)) * scale); // 5

    vertices.Add(adjustForUnitSphere(new Vector3( 0,  1, -t)) * scale); // 5
    vertices.Add(adjustForUnitSphere(new Vector3( 0, -1, -t)) * scale); // 4
    vertices.Add(adjustForUnitSphere(new Vector3(-t,  0, -1)) * scale); // 11

    vertices.Add(adjustForUnitSphere(new Vector3(-t,  0, -1)) * scale); // 11
    vertices.Add(adjustForUnitSphere(new Vector3(-1, -t,  0)) * scale); // 2
    vertices.Add(adjustForUnitSphere(new Vector3(-t,  0,  1)) * scale); // 10

    vertices.Add(adjustForUnitSphere(new Vector3(-t,  0,  1)) * scale); // 10
    vertices.Add(adjustForUnitSphere(new Vector3( 0, -1,  t)) * scale); // 6
    vertices.Add(adjustForUnitSphere(new Vector3( 0,  1,  t)) * scale); // 7

    vertices.Add(adjustForUnitSphere(new Vector3( 0,  1,  t)) * scale); // 7
    vertices.Add(adjustForUnitSphere(new Vector3( t,  0,  1)) * scale); // 8
    vertices.Add(adjustForUnitSphere(new Vector3( 1,  t,  0)) * scale); // 1

    // five face around point 3 (polar opposite of point 0)
    vertices.Add(adjustForUnitSphere(new Vector3( 1, -t,  0)) * scale); // 3
    vertices.Add(adjustForUnitSphere(new Vector3( 0, -1, -t)) * scale); // 4
    vertices.Add(adjustForUnitSphere(new Vector3( t,  0, -1)) * scale); // 9

    vertices.Add(adjustForUnitSphere(new Vector3( 1, -t,  0)) * scale); // 3
    vertices.Add(adjustForUnitSphere(new Vector3(-1, -t,  0)) * scale); // 2
    vertices.Add(adjustForUnitSphere(new Vector3( 0, -1, -t)) * scale); // 4

    vertices.Add(adjustForUnitSphere(new Vector3( 1, -t,  0)) * scale); // 3
    vertices.Add(adjustForUnitSphere(new Vector3( 0, -1,  t)) * scale); // 6
    vertices.Add(adjustForUnitSphere(new Vector3(-1, -t,  0)) * scale); // 2

    vertices.Add(adjustForUnitSphere(new Vector3( 1, -t,  0)) * scale); // 3
    vertices.Add(adjustForUnitSphere(new Vector3( t,  0,  1)) * scale); // 8
    vertices.Add(adjustForUnitSphere(new Vector3( 0, -1,  t)) * scale); // 6

    vertices.Add(adjustForUnitSphere(new Vector3( 1, -t,  0)) * scale); // 3
    vertices.Add(adjustForUnitSphere(new Vector3( t,  0, -1)) * scale); // 9
    vertices.Add(adjustForUnitSphere(new Vector3( t,  0,  1)) * scale); // 8

    // five adjacent faces
    vertices.Add(adjustForUnitSphere(new Vector3( 0, -1, -t)) * scale); // 4
    vertices.Add(adjustForUnitSphere(new Vector3( 0,  1, -t)) * scale); // 5
    vertices.Add(adjustForUnitSphere(new Vector3( t,  0, -1)) * scale); // 9

    vertices.Add(adjustForUnitSphere(new Vector3(-1, -t,  0)) * scale); // 2
    vertices.Add(adjustForUnitSphere(new Vector3(-t,  0, -1)) * scale); // 11
    vertices.Add(adjustForUnitSphere(new Vector3( 0, -1, -t)) * scale); // 4

    vertices.Add(adjustForUnitSphere(new Vector3( 0, -1,  t)) * scale); // 6
    vertices.Add(adjustForUnitSphere(new Vector3(-t,  0,  1)) * scale); // 10
    vertices.Add(adjustForUnitSphere(new Vector3(-1, -t,  0)) * scale); // 2

    vertices.Add(adjustForUnitSphere(new Vector3( t,  0,  1)) * scale); // 8
    vertices.Add(adjustForUnitSphere(new Vector3( 0,  1,  t)) * scale); // 7
    vertices.Add(adjustForUnitSphere(new Vector3( 0, -1,  t)) * scale); // 6

    vertices.Add(adjustForUnitSphere(new Vector3( t,  0, -1)) * scale); // 9
    vertices.Add(adjustForUnitSphere(new Vector3( 1,  t,  0)) * scale); // 1
    vertices.Add(adjustForUnitSphere(new Vector3( t,  0,  1)) * scale); // 8
  }

  private Vector3 adjustForUnitSphere(Vector3 point) {

    float length = Mathf.Sqrt(point.x * point.x + point.y * point.y + point.z * point.z);
    Vector3 newPoint = new Vector3(point.x / length, point.y / length, point.z / length);
    return newPoint;
  }

  private void CreateFlatTriangles() {

    // the 20 faces of the non-subdivided icosahedron
    for (int i = 0; i < vertices.Count; i += 3) {
      AddTriangle(i, i + 1, i + 2);
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
        vertices.Add(adjustForUnitSphere(p0)   * scale);
        vertices.Add(adjustForUnitSphere(mp01) * scale);
        vertices.Add(adjustForUnitSphere(mp02) * scale);

        // second subdivided face
        vertices.Add(adjustForUnitSphere(mp01) * scale);
        vertices.Add(adjustForUnitSphere(mp12) * scale);
        vertices.Add(adjustForUnitSphere(mp02) * scale);

        // third subdivided face
        vertices.Add(adjustForUnitSphere(mp01) * scale);
        vertices.Add(adjustForUnitSphere(p1)   * scale);
        vertices.Add(adjustForUnitSphere(mp12) * scale);

        // fourth subdivided face
        vertices.Add(adjustForUnitSphere(mp02) * scale);
        vertices.Add(adjustForUnitSphere(mp12) * scale);
        vertices.Add(adjustForUnitSphere(p2)   * scale);

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

  // creates a single triangle face
  private void AddTriangle(int v1, int v2, int v3) {

    triangles.Add(v1);
    triangles.Add(v2);
    triangles.Add(v3);
  }

  // same as GetMidpoint, but does not account for shared vertices
  private int GetFlatMidpoint(int v1, int v2) {
    // first point
    Vector3 p1 = vertices[v1];
    // second point
    Vector3 p2 = vertices[v2];
    // midpoint between first and second points
    Vector3 mp = new Vector3((p1.x + p2.x) / 2f,
                             (p1.y + p2.y) / 2f,
                             (p1.z + p2.z) / 2f);
    vertices.Add(adjustForUnitSphere(p1) * scale);
    vertices.Add(adjustForUnitSphere(p2) * scale);
    vertices.Add(adjustForUnitSphere(mp) * scale);
    return vertices.Count - 1;
  }

  private void SetTiles() {

    // set up tiles dictionary
    int index = 0;
    for(int i = 0; i < vertices.Count; i += 3) {
      Tile tile = new Tile(vertices[i], vertices[i + 1], vertices[i + 2],
                           triangles[i], triangles[i + 1], triangles[i + 2]);

      // Dictionary used to map vertices (keys) to the tiles (values) that contain them
      // first check if the key already exists
      if(verticesUsedByTiles.ContainsKey(vertices[i])) {
        verticesUsedByTiles[vertices[i]].Add(tile);
      } else {
        verticesUsedByTiles.Add(vertices[i], new List<Tile>());
        verticesUsedByTiles[vertices[i]].Add(tile);
      }
      // do the same for vertices[i + 1] and vertices[i + 2]
      if(verticesUsedByTiles.ContainsKey(vertices[i + 1])) {
        verticesUsedByTiles[vertices[i + 1]].Add(tile);
      } else {
        verticesUsedByTiles.Add(vertices[i + 1], new List<Tile>());
        verticesUsedByTiles[vertices[i + 1]].Add(tile);
      }
      if(verticesUsedByTiles.ContainsKey(vertices[i + 2])) {
        verticesUsedByTiles[vertices[i + 2]].Add(tile);
      } else {
        verticesUsedByTiles.Add(vertices[i + 2], new List<Tile>());
        verticesUsedByTiles[vertices[i + 2]].Add(tile);
      }

      tiles.Add(index, tile);
      index++;
    }
  }

  // Fisher-Yates shuffle algorithm
  private void ShuffleTiles(int seed) {

    System.Random prng = new System.Random(seed);
    
    // we can ignore the last iteration of the algorithm
    for(int i = 0; i < tiles.Count - 1; i++) {
      int randomIndex = prng.Next(i, tiles.Count);

      // swap
      Tile tempTile = tiles[randomIndex];
      tiles[randomIndex] = tiles[i];
      tiles[i] = tempTile;
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

  private void SetVertexColors() {
    
    // initially, set all tiles to be land tiles
    for(int i = 0; i < vertices.Count; i++) {
      colors.Add(landColor);
    }
    
    // get the keys from the dictionary
    // take the first numOceanTiles elements and set those to be ocean tiles
    int numOceanTiles = (int)(tiles.Count * oceanTilesPercentage);
    for(int i = 0; i < numOceanTiles; i++) {
      tiles[i].isOceanTile = true;
      
      // the int values for the triangles of each tile can be used to designate the
      // appropriate indices in the colors List
      colors[tiles[i].t0] = oceanColor;
      colors[tiles[i].t1] = oceanColor;
      colors[tiles[i].t2] = oceanColor;
    }

    mesh.colors = colors.ToArray();
  }

  private void SetNeighbors() {

    // TODO

  }

  #region SHARED_VERTICES_METHODS
  // create the 12 vertices
  private void CreateVertices(float t) {
    
    vertices.Add(adjustForUnitSphere(new Vector3(-1,  t,  0)) * scale); // 0
    vertices.Add(adjustForUnitSphere(new Vector3( 1,  t,  0)) * scale); // 1
    vertices.Add(adjustForUnitSphere(new Vector3(-1, -t,  0)) * scale); // 2
    vertices.Add(adjustForUnitSphere(new Vector3( 1, -t,  0)) * scale); // 3

    vertices.Add(adjustForUnitSphere(new Vector3( 0, -1, -t)) * scale); // 4
    vertices.Add(adjustForUnitSphere(new Vector3( 0,  1, -t)) * scale); // 5
    vertices.Add(adjustForUnitSphere(new Vector3( 0, -1,  t)) * scale); // 6
    vertices.Add(adjustForUnitSphere(new Vector3( 0,  1,  t)) * scale); // 7

    vertices.Add(adjustForUnitSphere(new Vector3( t,  0,  1)) * scale); // 8
    vertices.Add(adjustForUnitSphere(new Vector3( t,  0, -1)) * scale); // 9
    vertices.Add(adjustForUnitSphere(new Vector3(-t,  0,  1)) * scale); // 10
    vertices.Add(adjustForUnitSphere(new Vector3(-t,  0, -1)) * scale); // 11
    
    mesh.vertices = vertices.ToArray();
  }

  private void CreateTriangles() {

    // order vertices clockwise so that the face faces the right direction

    // create the 20 faces
    // five faces around point 0
    AddTriangle(0, 5, 11);
    AddTriangle(0, 1, 5);
    AddTriangle(0, 7, 1);
    AddTriangle(0, 10, 7);
    AddTriangle(0, 11, 10);

    // five adjacent faces
    AddTriangle(1, 9, 5);
    AddTriangle(5, 4, 11);
    AddTriangle(11, 2, 10);
    AddTriangle(10, 6, 7);
    AddTriangle(7, 8, 1);

    // five face around point 3 (polar opposite of point 0)
    AddTriangle(3, 4, 9);
    AddTriangle(3, 2, 4);
    AddTriangle(3, 6, 2);
    AddTriangle(3, 8, 6);
    AddTriangle(3, 9, 8);

    // five adjacent faces
    AddTriangle(4, 5, 9);
    AddTriangle(2, 11, 4);
    AddTriangle(6, 10, 2);
    AddTriangle(8, 7, 6);
    AddTriangle(9, 1, 8);

    // subdivisions
    List<int> trianglesSubdivisions = new List<int>();

    // refine triangles
    for(int i = 0; i < subdivisions; i++) {
      for(int j = 0; j < triangles.Count; j += 3) {
        // find midpoints for each triangle
        int mp1 = GetMidpoint(triangles[j], triangles[j + 1]);
        int mp2 = GetMidpoint(triangles[j + 1], triangles[j + 2]);
        int mp3 = GetMidpoint(triangles[j], triangles[j + 2]);

        // first subdivision
        trianglesSubdivisions.Add(triangles[j]);
        trianglesSubdivisions.Add(mp1);
        trianglesSubdivisions.Add(mp3);
        
        // second subdivision
        trianglesSubdivisions.Add(mp1);
        trianglesSubdivisions.Add(triangles[j + 1]);
        trianglesSubdivisions.Add(mp2);

        // third subdivision
        trianglesSubdivisions.Add(mp3);
        trianglesSubdivisions.Add(mp2);
        trianglesSubdivisions.Add(triangles[j + 2]);

        // middle subdivision
        trianglesSubdivisions.Add(mp1);
        trianglesSubdivisions.Add(mp2);
        trianglesSubdivisions.Add(mp3);
      }

      // update triangles List
      triangles.AddRange(trianglesSubdivisions);
    }

    // update mesh
    mesh.vertices = vertices.ToArray();
    mesh.triangles = triangles.ToArray();
  }

  // gets the midpoint between two points of a triangle's edge
  private int GetMidpoint(int v1, int v2) {

    long smallerIndex;
    long largerIndex;
    if(v1 >= v2) {
      smallerIndex = v2;
      largerIndex = v1;
    } else {
      smallerIndex = v1;
      largerIndex = v2;
    }

    // the key is unique to any two pair of points
    long key = (smallerIndex << 32) + largerIndex;

    int ret;
    if(midpointCache.TryGetValue(key, out ret)) {
      return ret; 
    }

    /* -- continue past this point if the key does not exist -- */

    // key does not exist, calculate midpoint
    // first point
    Vector3 p1 = vertices[v1];
    // second point
    Vector3 p2 = vertices[v2];
    // midpoint between first and second points
    Vector3 mp = new Vector3((p1.x + p2.x) / 2f,
                             (p1.y + p2.y) / 2f,
                             (p1.z + p2.z) / 2f);
    vertices.Add(adjustForUnitSphere(mp) * scale); 

    // add key and midpoint vertex index (value) to cache
    midpointCache.Add(key, vertices.Count - 1);
    return vertices.Count - 1;
  }
  #endregion

  #region GIZMOS
  /* VERTICES
  private void OnDrawGizmos() {

    if (vertices != null) {
      Gizmos.color = Color.black;
      for (int i = 0; i < vertices.Count; i++) {
        if (i < 1) {
          Gizmos.color = Color.green;
        }
        else {
          Gizmos.color = Color.clear;
        }
        Gizmos.DrawSphere(vertices[i], 0.1f);
      }
    }
  }
  */

  /* TILES
  // draw gizmo at each vertex
  private void OnDrawGizmos() {

    if(tiles != null) {
      Gizmos.color = Color.black;
      for (int i = 0; i < tiles.Count; i++) {
        if(i < 1) {
          Gizmos.color = Color.green;
        } else {
          Gizmos.color = Color.clear;
        }
        Gizmos.DrawSphere(tiles[i].v0, 0.1f);
        Gizmos.DrawSphere(tiles[i].v1, 0.1f);
        Gizmos.DrawSphere(tiles[i].v2, 0.1f);
      }
    }
  }
  */
  #endregion

  private class Tile {
    public Vector3 v0, v1, v2;
    public int t0, t1, t2;
    // default to false
    public bool isOceanTile = false;

    public Tile(Vector3 v0, Vector3 v1, Vector3 v2, int t0, int t1, int t2) {
      this.v0 = v0;
      this.v1 = v1;
      this.v2 = v2;
      this.t0 = t0;
      this.t1 = t1;
      this.t2 = t2;
    }
  }
}

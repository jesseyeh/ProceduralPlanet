using UnityEngine;
using System.Collections;

public class IcoTile : MonoBehaviour {

  public Vector3 v0, v1, v2;
  public Color color;

  [SerializeField]
  public IcoTile[] neighbors;

  public IcoTile GetNeighbor(int index) {

    return this.neighbors[index];
  }

  public void SetNeighbor(int index, IcoTile neighborTile) {

    this.neighbors[index] = neighborTile;
  }

  public IcoTile(Vector3 v0, Vector3 v1, Vector3 v2) {

    this.v0 = v0;
    this.v1 = v1;
    this.v2 = v2;
  }
}

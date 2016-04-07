using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// collection of generally useful methods
public static class Utility {

  // used to turn icosahedron into icosphere
  public static Vector3 AdjustForUnitSphere(Vector3 point) {

    float length = Mathf.Sqrt(point.x * point.x + point.y * point.y + point.z * point.z);
    Vector3 newPoint = new Vector3(point.x / length, point.y / length, point.z / length);
    return newPoint;
  }

  // Fisher-Yates shuffle algorithm
  public static List<T> Shuffle<T>(int seed, List<T> list) {

    System.Random prng = new System.Random(seed);

    // ignore the last iteration of the algorithm
    for (int i = 0; i < list.Count - 1; i++) {
      int randomIndex = prng.Next(i, list.Count);

      // swap the tiles
      T tempItem = list[randomIndex];
      list[randomIndex] = list[i];
      list[i] = tempItem;
    }

    return list;
  }
}
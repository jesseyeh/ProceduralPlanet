using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(IcoMesh))]
public class IcoMeshEditor : Editor {

  public override void OnInspectorGUI() {

    IcoMesh im = (IcoMesh)target;

    if (DrawDefaultInspector()) {
      if (im.autoUpdate) {
        im.Generate();
      }
    }

    if (GUILayout.Button("Generate")) {
      im.Generate();
    }
  }
}
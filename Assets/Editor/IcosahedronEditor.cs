using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Icosahedron))]
public class MapGeneratorEditor : Editor {

  public override void OnInspectorGUI() {

    Icosahedron ico = (Icosahedron)target;

    if (DrawDefaultInspector()) {
      if (ico.autoUpdate) {
        ico.Generate();
      }
    }

    if (GUILayout.Button("Generate")) {
      ico.Generate();
    }
  }
}
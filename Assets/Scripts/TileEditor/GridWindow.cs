using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GridWindow : EditorWindow {
    GridScript grid;

    public void Init() {
        grid = (GridScript)FindObjectOfType(typeof(GridScript));
    }

    void OnGUI() {
        // TODO: Check if this can update grid without switching focus to scene view.
        grid.color = EditorGUILayout.ColorField(grid.color, GUILayout.Width(200));
    }
}

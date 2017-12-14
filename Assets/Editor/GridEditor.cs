using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridScript))]
public class GridEditor : Editor {
    GridScript grid;

    void OnEnable() {
        grid = (GridScript)target;
    }

    [MenuItem("Assets/Create/Tileset")]
    static void CreateTileSet() {
        TileSet asset = ScriptableObject.CreateInstance<TileSet>();
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);

        if (string.IsNullOrEmpty(path)) {
            path = "Assets";
        }
        else if (System.IO.Path.GetExtension(path) != "") {
            path = path.Replace(System.IO.Path.GetFileName(path), "");
        }
        else {
            path += "/";
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "Tileset.asset");
        AssetDatabase.CreateAsset(asset, assetPathAndName);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    public override void OnInspectorGUI() {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Grid Width");
        grid.width = EditorGUILayout.IntSlider(grid.width, 1, 128);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Grid Height");
        grid.height = EditorGUILayout.IntSlider(grid.height, 1, 128);
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Open Grid Window")) {
            GridWindow window = (GridWindow)EditorWindow.GetWindow(typeof(GridWindow));
            window.Init();
        }

        // Tile Prefab
        EditorGUI.BeginChangeCheck();
        Transform newTilePrefab = (Transform)EditorGUILayout.ObjectField("Tile Prefab", grid.tilePrefab, typeof(Transform), false);

        if (EditorGUI.EndChangeCheck()) {
            grid.tilePrefab = newTilePrefab;
            Undo.RecordObject(target, "Grid Changed");
        }

        // Tile Map
        EditorGUI.BeginChangeCheck();
        TileSet newTileSet = (TileSet)EditorGUILayout.ObjectField("Tileset", grid.tileSet, typeof(TileSet), false);

        if (EditorGUI.EndChangeCheck()) {
            grid.tileSet = newTileSet;
            Undo.RecordObject(target, "Grid Changed");
        }
    }
}

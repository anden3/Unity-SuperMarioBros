using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridScript))]
public class GridEditor : Editor {
    GridScript grid;

    private const int TILES_PER_ROW = 9;

    private int selectedTile = 0;
    private float tileSize;

    void OnEnable() {
        grid = (GridScript)target;
        SelectTile(selectedTile);
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
        asset.hideFlags = HideFlags.DontSave;
    }

    public override void OnInspectorGUI() {
        tileSize = EditorGUIUtility.currentViewWidth / TILES_PER_ROW;

        GUILayout.BeginHorizontal();
        GUILayout.Label("Grid Width");
        grid.width = EditorGUILayout.IntSlider(grid.width, 1, 128);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Grid Height");
        grid.height = EditorGUILayout.IntSlider(grid.height, 1, 128);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Grid Color");
        grid.color = EditorGUILayout.ColorField(grid.color);
        GUILayout.EndHorizontal();

        // Tile Prefab
        /*
        EditorGUI.BeginChangeCheck();
        GameObject newTilePrefab = (GameObject)EditorGUILayout.ObjectField("Tile Prefab", grid.tilePrefab, typeof(Transform), false);

        if (EditorGUI.EndChangeCheck()) {
            grid.tilePrefab = newTilePrefab;
        }
        */

        // Draggable
        EditorGUI.BeginChangeCheck();
        bool draggable = EditorGUILayout.Toggle("Toggle Dragging", grid.draggable);

        if (EditorGUI.EndChangeCheck()) {
            grid.draggable = draggable;
        }

        // Tile Map
        EditorGUI.BeginChangeCheck();
        TileSet newTileSet = (TileSet)EditorGUILayout.ObjectField("Tileset", grid.tileSet, typeof(TileSet), false);

        if (EditorGUI.EndChangeCheck()) {
            grid.tileSet = newTileSet;
        }

        // Tile List
        /*
        if (grid.tileSet != null) {
            EditorGUI.BeginChangeCheck();

            grid.tileSet.tiles.Sort((x, y) => x.name.CompareTo(y.name));

            string[] names = new string[grid.tileSet.tiles.Count];
            int[] values = new int[grid.tileSet.tiles.Count];

            for (int i = 0; i < names.Length; i++) {
                GameObject tile = grid.tileSet.tiles[i];
                names[i] = (tile != null) ? tile.name : "";
                values[i] = i;

                if (tile == grid.tilePrefab) {
                    selectedTile = i;
                }
            }

            int newSelectedTile = EditorGUILayout.IntPopup("Select Tile", selectedTile, names, values);

            if (EditorGUI.EndChangeCheck()) {
                SelectTile(newSelectedTile);
            }
        }
        */

        /*
        Texture[] tileTextures = new Texture[grid.tileSet.tiles.Count];

        for (int i = 0; i < grid.tileSet.tiles.Count; i++) {
            GameObject tile = grid.tileSet.tiles[i];
            tileTextures[i] = AssetPreview.GetAssetPreview(tile);

            if (tile == grid.tilePrefab) {
                selectedTile = i;
            }
        }

        selectedTile = GUILayout.SelectionGrid(
            selectedTile, tileTextures, TILES_PER_ROW, GUI.skin.button,
            GUILayout.Width(tileSize * TILES_PER_ROW)
        );
        */

        for (int row = 0; row < Mathf.CeilToInt((float)grid.tileSet.tiles.Count / (TILES_PER_ROW - 1)); row++) {
            EditorGUILayout.BeginHorizontal();

            for (int column = 0; column < (TILES_PER_ROW - 1); column++) {
                int index = row * (TILES_PER_ROW - 1) + column;

                if (index >= grid.tileSet.tiles.Count) {
                    break;
                }

                GameObject tile = grid.tileSet.tiles[index];

                bool isPressed = GUILayout.Button(
                    new GUIContent(AssetPreview.GetAssetPreview(tile), tile.name),
                    GUILayout.Width(tileSize), GUILayout.Height(tileSize)
                );

                if (isPressed) {
                    SelectTile(index);
                }
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    void OnSceneGUI() {
        // TODO: Figure out why you cannot drag camera while this game object is selected.

        Event e = Event.current;

        // Ignore any events which aren't mouse events.
        if (!e.isMouse) {
            return;
        }

        bool mouseDown = e.type == EventType.MouseDown;

        if (grid.draggable) {
            mouseDown = mouseDown || e.type == EventType.MouseDrag;
        }

        if (mouseDown) {
            // Prevent other controls from getting mouse events.
            // Stops object from losing focus when clicking in scene view.
            GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
            e.Use();

            Ray ray = Camera.current.ScreenPointToRay(new Vector3(
                e.mousePosition.x,
                -e.mousePosition.y + Camera.current.pixelHeight
            ));
            Vector3 mousePos = ray.origin;
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

            // Create tile if left mouse button is down.
            if (e.button == 0) {
                if (grid.tilePrefab == null) {
                    return;
                }

                GameObject prefab = grid.tilePrefab;

                if (hit.collider != null) {
                    // Don't replace tile if it's the same type.
                    if (hit.collider.name == prefab.name) {
                        return;
                    }

                    Undo.DestroyObjectImmediate(hit.collider.gameObject);
                }

                GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab.gameObject);

                Vector3 gridPos = new Vector3(
                    (Mathf.Floor(mousePos.x / grid.width) + 0.5f) * grid.width,
                    (Mathf.Floor(mousePos.y / grid.height) + 0.5f) * grid.height
                );
                obj.transform.position = gridPos;
                obj.transform.parent = grid.transform;

                Undo.RegisterCreatedObjectUndo(obj, "Create " + obj.name);
            }
            // Remove tile if right mouse button is down.
            else if (e.button == 1) {
                if (hit.collider != null) {
                    Undo.DestroyObjectImmediate(hit.collider.gameObject);
                }
            }
        }
        else if (e.type == EventType.MouseUp) {
            GUIUtility.hotControl = 0;
        }
    }

    private void SelectTile(int tileIndex) {
        if (tileIndex == selectedTile && grid.tilePrefab != null) {
            return;
        }

        if (tileIndex >= grid.tileSet.tiles.Count) {
            return;
        }

        selectedTile = tileIndex;
        grid.tilePrefab = grid.tileSet.tiles[tileIndex];

        // Update grid to match tile size.
        Vector2 tileSize = grid.tilePrefab.GetComponent<Renderer>().bounds.size;
        grid.width = (int)tileSize.x;
        grid.height = (int)tileSize.y;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileSet))]
public class TileSetEditor : Editor {
    TileSet tileSet;

    private int selectedTile;
    private GUIStyle gridStyle;

    private int tilesPerRow = 9;
    private float tileSize;

    private Dictionary<string, Texture> tileTextures = new Dictionary<string, Texture>();

    private void OnEnable() {
        tileSet = (TileSet)target;
    }

    public override void OnInspectorGUI() {
        tileSize = EditorGUIUtility.currentViewWidth / tilesPerRow;

        CheckForDroppedTiles();
        CreateTileGrid();
    }

    private void CheckForDroppedTiles() {
        Event e = Event.current;

        switch (e.type) {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                bool allObjectsOk = true;

                foreach (Object obj in DragAndDrop.objectReferences) {
                    if (!IsObjectValid(obj)) {
                        allObjectsOk = false;
                        break;
                    }
                }

                DragAndDrop.visualMode = allObjectsOk ? DragAndDropVisualMode.Link : DragAndDropVisualMode.Rejected;

                if (allObjectsOk && e.type == EventType.DragPerform) {
                    DragAndDrop.AcceptDrag();
                    Undo.RecordObject(tileSet, "Tileset Changed");
                    
                    foreach (GameObject obj in DragAndDrop.objectReferences) {
                        AddTile(obj);
                    }
                }

                e.Use();
                break;
        }
    }

    private void CreateTileGrid() {
        int indexToDelete = -1;

        for (int row = 0; row < Mathf.CeilToInt((float)tileSet.tiles.Count / (tilesPerRow - 1)); row++) {
            EditorGUILayout.BeginHorizontal();

            for (int column = 0; column < (tilesPerRow - 1); column++) {
                int index = row * (tilesPerRow - 1) + column;

                if (index >= tileSet.tiles.Count) {
                    break;
                }

                GameObject tile = tileSet.tiles[index];

                bool isPressed = GUILayout.Button(
                    new GUIContent(GetTexture(tile), tile.name), 
                    GUILayout.Width(tileSize), GUILayout.Height(tileSize)
                );

                if (isPressed) {
                    indexToDelete = index;
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        if (indexToDelete > -1) {
            Undo.RecordObject(tileSet, "Tileset Changed");
            RemoveTile(tileSet.tiles[indexToDelete]);
        }
    }

    private bool IsObjectValid(Object obj) {
        if (obj == null || obj as GameObject == null) {
            return false;
        }

        return ((GameObject)obj).GetComponent<SpriteRenderer>() != null;
    }

    private Texture GetTexture(GameObject tile) {
        if (!tileTextures.ContainsKey(tile.name)) {
            StoreTexture(tile);
        }

        return tileTextures[tile.name];
    }

    private void AddTile(GameObject tile) {
        if (tileSet.tiles.Contains(tile)) {
            return;
        }

        tileSet.tiles.Add(tile);
        StoreTexture(tile);

        // Sort list alphabetically.
        tileSet.tiles.Sort((x, y) => x.name.CompareTo(y.name));
    }

    private void RemoveTile(GameObject tile) {
        if (!tileSet.tiles.Contains(tile)) {
            return;
        }

        tileSet.tiles.Remove(tile);
        tileTextures.Remove(tile.name);
    }

    private void StoreTexture(GameObject tile) {
        Texture2D preview = AssetPreview.GetAssetPreview(tile);
        Debug.Assert(preview != null);

        if (tileTextures.ContainsKey(tile.name)) {
            tileTextures.Remove(tile.name);
        }

        tileTextures.Add(tile.name, new Texture2D(preview.width, preview.height, TextureFormat.RGB24, false));
        Graphics.CopyTexture(preview, tileTextures[tile.name]);
    }
}

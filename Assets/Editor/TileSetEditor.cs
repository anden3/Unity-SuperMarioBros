using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileSet))]
public class TileSetEditor : Editor {
    TileSet tileSet;

    private int selectedTile;
    private GUIStyle gridStyle;

    private int tilesPerRow = 10;
    private float tileWidth;

    private void OnEnable() {
        tileSet = (TileSet)target;
    }

    public override void OnInspectorGUI() {
        tileWidth = EditorGUIUtility.currentViewWidth / tilesPerRow;

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
                        if (!tileSet.tiles.Contains(obj)) {
                            tileSet.tiles.Add(obj);
                        }
                    }
                }

                e.Use();
                break;
        }
    }

    private void CreateTileGrid() {
        Texture[] tileTextures = new Texture[tileSet.tiles.Count];

        for (int i = 0; i < tileSet.tiles.Count; i++) {
            tileTextures[i] = AssetPreview.GetAssetPreview(tileSet.tiles[i]);
        }

        EditorGUILayout.BeginVertical();

        selectedTile = GUILayout.SelectionGrid(
            selectedTile, tileTextures, tilesPerRow, GUI.skin.button, GUILayout.Height(tileWidth)
        );

        EditorGUILayout.EndVertical();
    }

    private bool IsObjectValid(Object obj) {
        if (obj == null || obj as GameObject == null) {
            return false;
        }

        return ((GameObject)obj).GetComponent<SpriteRenderer>() != null;
    }
}

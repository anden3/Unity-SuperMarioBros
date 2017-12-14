using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridScript : MonoBehaviour {
    public int width = 16;
    public int height = 16;

    public bool draggable = true;

    public Color color = Color.white;
    public Transform tilePrefab;
    public TileSet tileSet;

    void OnDrawGizmos() {
        Camera cam = Camera.current;

        // Prevent invalid values.
        this.width = Mathf.Max(this.width, 1);
        this.height = Mathf.Max(this.height, 1);

        Vector3 pos = cam.transform.position;
        Gizmos.color = this.color;

        Vector2 cameraSize = new Vector2(
            cam.orthographicSize * cam.aspect,
            cam.orthographicSize
        );

        // Vertical lines.
        for (float x = pos.x - cameraSize.x; x < pos.x + cameraSize.x; x += this.width) {
            Gizmos.DrawLine(
                new Vector3(
                    Mathf.Floor(x / width) * width,
                    Mathf.Floor((pos.y - cameraSize.y) / height) * height
                ),
                new Vector3(
                    Mathf.Floor(x / width) * width,
                    Mathf.Ceil((pos.y + cameraSize.y) / height) * height
                )
            );
        }

        // Horizontal lines.
        for (float y = pos.y - cameraSize.y; y < pos.y + cameraSize.y; y += this.height) {
            Gizmos.DrawLine(
                new Vector3(
                    Mathf.Floor((pos.x - cameraSize.x) / width) * width,
                    Mathf.Floor(y / height) * height
                ),
                new Vector3(
                    Mathf.Ceil((pos.x + cameraSize.x) / width) * width,
                    Mathf.Floor(y / height) * height
                )
            );
        }
    }
}

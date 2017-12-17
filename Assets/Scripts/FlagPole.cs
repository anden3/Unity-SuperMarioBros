using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagPole : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D other) {
        BoxCollider2D col = gameObject.GetComponent<BoxCollider2D>();

        float hitHeight = other.transform.position.y - col.bounds.min.y;
    }
}

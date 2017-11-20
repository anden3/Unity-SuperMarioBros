using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollower : MonoBehaviour {
    public Transform playerTransform;

    private float lastX;

    void Awake() {
        lastX = transform.position.x;
    }
	
	void LateUpdate() {
        if (playerTransform.position.x > lastX) {
            transform.position = new Vector3(
                playerTransform.position.x,
                transform.position.y,
                transform.position.z
            );

            lastX = transform.position.x;
        }
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lootbox : MonoBehaviour {
    public Sprite inactiveSprite;

    public float pushDistance;
    public float pushTime;

    private bool isActive = true;

    void OnCollisionEnter2D(Collision2D col) {
        if (!isActive || !col.gameObject.CompareTag("Player")) {
            return;
        }

        ContactPoint2D[] contacts = new ContactPoint2D[5];
        int count = col.GetContacts(contacts);

        for (int i = 0; i < count; i++) {
            // Check if hit came from underneath.
            if (contacts[i].normal == new Vector2(0, 1)) {
                isActive = false;
                StartCoroutine(Hit());
            }
        }
    }

    IEnumerator Hit() {
        gameObject.GetComponent<Animator>().StopPlayback();
        Destroy(gameObject.GetComponent<Animator>());
        gameObject.GetComponent<SpriteRenderer>().sprite = inactiveSprite;

        transform.position += new Vector3(0, pushDistance, 0);
        yield return new WaitForSeconds(pushTime);
        transform.position -= new Vector3(0, pushDistance, 0);
    }
}

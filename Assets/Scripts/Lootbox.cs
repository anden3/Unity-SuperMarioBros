using System.Reflection;
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
        int count;

        MethodInfo method = typeof(Collision2D).GetMethod("GetContacts");

        // Check if method exists.
        // This has to be done because Collision2D.GetContacts() exists in a newer version of Unity, but not in the version on the school computers.
        if (method != null) {
            count = (int)method.Invoke(col, new object[] { contacts });
        }
        else {
            PropertyInfo property = typeof(Collision2D).GetProperty("contacts");
            contacts = (ContactPoint2D[])property.GetValue(col, null);
            count = contacts.Length;
        }

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

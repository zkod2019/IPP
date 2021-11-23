using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public string collectableName;
    public AudioSource collectSound;
    public static float xRotation = 0f;
    public float rotationSpeed = 0.00001f;

    void FixedUpdate() {
        gameObject.transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.Self);
    }

    void OnTriggerEnter(Collider other) {
        collectSound.Play();
        other.gameObject.GetComponent<PlayerInventory>().Add(this);
        Destroy(gameObject);
    }
}

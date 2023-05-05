using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private const string PLAYER_TAG = "Player";
    private bool used = false;
    private BoxCollider2D gate;

    private void Awake()
    {
        //gate = transform.GetComponentsInChildren<BoxCollider2D>()[1]; // Also gets parent's component
        Transform barrier = transform.GetChild(0);
        gate = barrier.GetComponent<BoxCollider2D>();
        gate.enabled = false;
    }

    public bool Used
    {
        get { return used; }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == PLAYER_TAG)
        {
            if (used) { return; }

            // Saves and checks if both characters are to the left, if true wait till they are both passed
            if (GameManager.Instance.SaveCheckpoint(this)) { 
                used = true;
                gate.enabled = true;
            }
        }
    }
}

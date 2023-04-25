using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private const string PLAYER_TAG = "Player";
    private bool used = false;

    public bool Used
    {
        get { return used; }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == PLAYER_TAG)
        {
            used = true;
            GameManager.Instance.Save(this);
        }
    }
}

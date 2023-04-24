using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject respawnPoint;

    private void Start()
    {
        respawnPoint = transform.GetChild(0).gameObject;
    }

    public Vector3 GetRespawn()
    {
        respawnPoint.transform.parent = null;
        Vector3 getRespawn = respawnPoint.transform.position;
        respawnPoint.transform.parent = transform;
        return getRespawn;

    }
}

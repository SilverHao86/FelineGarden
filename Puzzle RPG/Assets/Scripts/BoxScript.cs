using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxScript : MonoBehaviour
{
    public PhysicsMaterial2D frictionMat;
    protected virtual void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "PushableBox")
        {
            this.gameObject.GetComponent<Rigidbody2D>().sharedMaterial = frictionMat; 
        }
    }
}

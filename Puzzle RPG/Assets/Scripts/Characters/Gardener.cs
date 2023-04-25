using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Gardener : Character
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        base.Awake();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        base.FixedUpdate();
    }

    void OnTriggerStay2D(UnityEngine.Collider2D collision)
    {

        base.OnTriggerStay2D(collision);

    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (collision.gameObject.tag == "GardenerPickup" && active)
        {
            InventoryController.instance.Add(collision.gameObject.GetComponent<ItemController>().item);
            collision.gameObject.GetComponent<ItemController>().Equipped();
            Destroy(collision.gameObject);
        }
    }
}

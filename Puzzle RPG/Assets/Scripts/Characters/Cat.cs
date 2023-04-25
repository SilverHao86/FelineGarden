using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cat : Character
{
    // Start is called before the first frame update
    private void Awake()
    {
        base.Awake();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        base.FixedUpdate();
    }

    // Make Sure the cant plant
    void OnTriggerStay2D(UnityEngine.Collider2D collision)
    {

        base.OnTriggerStay2D(collision);


    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        // Pick up items excusive to cat
        if (collision.gameObject.tag == "CatPickup" && active)
        {
            InventoryController.instance.Add(collision.gameObject.GetComponent<ItemController>().item);
            //collision.gameObject.GetComponent<ItemController>().Equipped();
            Destroy(collision.gameObject);
        }
    }
}

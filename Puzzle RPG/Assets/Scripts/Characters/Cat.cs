using System;
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
        
        // Distroy A Dirt Block
        if (collision.gameObject.tag == "Dirt" && active && plantPlant.IsPressed())
        {
            int index = InventoryController.instance.equippedIndex[1];
            Item tempItem;
            try
            {
                tempItem = InventoryController.instance.catItems[index];
                if (!tempItem.itemName.Equals("Garden Shovel"))
                {
                    Debug.Log(index);
                    return;
                }
            }
            catch (Exception e)
            {
                return;
            }


            
                collision.gameObject.transform.parent.gameObject.GetComponent<DirtBlock>().StartDistroyingDirt();
            
        }

    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        // Pick up items excusive to cat
        if (collision.gameObject.tag == "CatPickup" && active)
        {
            InventoryController.instance.Add(collision.gameObject.GetComponent<ItemController>().item);
            collision.gameObject.GetComponent<ItemController>().Equipped();
            Destroy(collision.gameObject);
        }
    }
}

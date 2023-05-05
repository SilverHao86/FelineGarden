using System;
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
        if (collision.gameObject.tag == "Player" && active && plantPlant.IsPressed())
        {
            bool withTranslator = false;
            
            int index = InventoryController.instance.equippedIndex[1];
            Item tempItem;
            try
            {
                tempItem = InventoryController.instance.catItems[index];
                if (tempItem.itemName.Equals("Meow Translator"))
                {
                    withTranslator= true;
                }
            }
            catch (Exception e)
            {
                return;
            }
            if (withTranslator)
            {
                Interactable?.Interact(this, data.catFlavorDialogues[UnityEngine.Random.Range(0, data.catFlavorDialogues.Count)]);
            }
            else
            {
                Interactable?.Interact(this, data.dialogues[0]);
            }

        }
        
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (collision.gameObject.tag == "GardenerPickup" && active)
        {
            InventoryController.instance.Add(collision.gameObject.GetComponent<ItemController>().item);
            collision.gameObject.GetComponent<ItemController>().Equipped();
            Interactable?.Interact(this, collision.gameObject.GetComponent<ItemController>().pickUpDialogue);
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "CatPickup" && active)
        {
            Interactable?.Interact(this, data.dialogues[2]);
        }
    }
}

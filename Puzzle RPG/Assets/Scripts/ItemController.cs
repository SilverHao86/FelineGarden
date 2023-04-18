using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public Item item;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Equipped()
    {
        Debug.Log("clicked");
        int index = transform.GetSiblingIndex();
        Debug.Log(transform.parent.gameObject);
        InventoryController.instance.EquipItem(item, index);
        InventoryController.instance.ListItems();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlantPlot;

public class ItemController : MonoBehaviour
{
    [field: SerializeField] public PlantType seedType { get; private set; }
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
        // Change the equip item image rather than refreshing the entire list
        int index = transform.GetSiblingIndex();
        InventoryController.instance.EquipItem(item, index);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryController : MonoBehaviour
{
    public static InventoryController instance;
    public List<Item> witchItems = new List<Item>();
    public List<Item> catItems = new List<Item>();
    public int[] equippedIndex;

    public Transform witchContent;
    public Transform catContent;
    public GameObject itemPrefab;
    private Color equippedColor;
    private Color emptyColor;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        equippedIndex = new int[2];
        equippedIndex[0] = 0;
        equippedIndex[1] = 0;
        equippedColor = new Color(1, 1, 1, 0.05f);
        emptyColor = new Color(1, 1, 1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Add(Item item)
    {
        // Add code for duplicate items
        // If they share id, add to amount rather than adding new item
        // Add override to use name and amount only rather than the item object
        int sharedIndex = -1;
        if (item.type == "Witch")
        {
            sharedIndex = ListHasItem(witchItems, item);
            if (sharedIndex != -1)
            {
                witchItems[sharedIndex].amount += item.amount;
            }
            else
            {
                witchItems.Add(Object.Instantiate(item));
            }
            
        }
        else if(item.type == "Cat")
        {
            sharedIndex = ListHasItem(witchItems, item);
            if(sharedIndex != -1)
            {
                catItems[sharedIndex].amount += item.amount;
            }
            else
            {
                catItems.Add(Object.Instantiate(item));
            }
            
        }
        ListItems();
    }

    public void Remove(Item item)
    {
        witchItems.Remove(item);
        catItems.Remove(item);
    }

    public void ListItems()
    {
        // Before displaying, clear content
        foreach(Transform obj in witchContent)
        {
            Destroy(obj.gameObject);
        }
        foreach (Transform obj in catContent)
        {
            Destroy(obj.gameObject);
        }
        for (int i = 0; i < witchItems.Count; i++)
        {
            GameObject obj = Instantiate(itemPrefab, witchContent);
            FillInfo(obj, witchItems[i]);

            // If the index is an equipped item, designate that it is equipped
            obj.transform.GetChild(3).GetComponent<Image>().color = (equippedIndex[1] == i ? equippedColor : emptyColor);

        }
        for (int i = 0; i < catItems.Count; i++)
        {
            GameObject obj = Instantiate(itemPrefab, catContent);
            FillInfo(obj, catItems[i]);

            // If the index is an equipped item, designate that it is equipped
            obj.transform.GetChild(3).GetComponent<Image>().color = (equippedIndex[1] == i ? equippedColor : emptyColor);
        }
    }

    public void FillInfo(GameObject itemObj, Item item)
    {
        itemObj.GetComponent<ItemController>().item = item;
        itemObj.transform.GetChild(0).GetComponent<TMP_Text>().text = item.itemName;
        itemObj.transform.GetChild(1).GetComponent<Image>().sprite = item.icon;
        itemObj.transform.GetChild(2).GetComponent<TMP_Text>().text = "" + item.amount;
    }

    public void EquipItem(Item item, int index = -1)
    { 
        int witchIndex = ListHasItem(witchItems, item);

        if (witchIndex != -1)
        {
            if (equippedIndex[0] != -1)
            {
                witchContent.GetChild(equippedIndex[0]).GetChild(3).GetComponent<Image>().color = emptyColor;
            }
            equippedIndex[0] = index != -1 && index < witchItems.Count ? index : witchIndex;
            witchContent.GetChild(equippedIndex[0]).GetChild(3).GetComponent<Image>().color = equippedColor;
        }
        else
        {
            if (equippedIndex[1] != -1)
            {
                catContent.GetChild(equippedIndex[1]).GetChild(3).GetComponent<Image>().color = emptyColor;
            }
            equippedIndex[1] = index != -1 && index < catItems.Count ? index : ListHasItem(catItems, item);
            catContent.GetChild(equippedIndex[1]).GetChild(3).GetComponent<Image>().color = equippedColor;
        }



        //if (index == -1)
        //{
        //    int witchIndex = ListHasItem(witchItems, item);
        //    if (witchIndex != -1)
        //    {
        //        if(equippedIndex[0] != -1)
        //        {
        //            witchContent.GetChild(equippedIndex[0]).GetChild(3).GetComponent<Image>().color = emptyColor;
        //        }
        //        equippedIndex[0] = witchIndex;
        //        witchContent.GetChild(equippedIndex[0]).GetChild(3).GetComponent<Image>().color = equippedColor;
        //    }
        //    else
        //    {
        //        catContent.GetChild(equippedIndex[1]).GetChild(3).GetComponent<Image>().color = emptyColor;
        //        equippedIndex[1] = ListHasItem(witchItems, item);
        //        catContent.GetChild(equippedIndex[1]).GetChild(3).GetComponent<Image>().color = equippedColor;
        //    }
        //}
        //else
        //{
        //    if(ListHasItem(witchItems, item) != -1)
        //    {
        //        if (equippedIndex[0] != -1)
        //        {
        //            witchContent.GetChild(equippedIndex[0]).GetChild(3).GetComponent<Image>().color = emptyColor;
        //        }
        //        equippedIndex[0] = index;
        //        witchContent.GetChild(equippedIndex[0]).GetChild(3).GetComponent<Image>().color = equippedColor;
        //    }
        //    else
        //    {
        //        catContent.GetChild(equippedIndex[1]).GetChild(3).GetComponent<Image>().color = emptyColor;
        //        equippedIndex[1] = index;
        //        catContent.GetChild(equippedIndex[1]).GetChild(3).GetComponent<Image>().color = equippedColor;
        //    }
        //}
    }

    private int ListHasItem(List<Item> list, Item item)
    {
        int index = -1;
        for(int i = 0; i < list.Count; i++)
        {
            if (list[i].itemName == item.itemName)
            {
                index = i;
            }
        }
        return index;
    }
}

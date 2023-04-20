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
        equippedIndex[0] = Random.Range(0, 10);
        equippedIndex[1] = Random.Range(0, 10);
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
                witchItems.Add(item);
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
                catItems.Add(item);
            }
            
        }
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
        for (int i = 0; i < witchItems.Count; i++)
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
        itemObj.transform.GetChild(0).GetComponent<TMP_Text>().text = item.name;
        itemObj.transform.GetChild(1).GetComponent<Image>().sprite = item.icon;
        itemObj.transform.GetChild(2).GetComponent<TMP_Text>().text = "" + item.amount;
    }

    public void EquipItem(Item item, int index = -1)
    { 
        if(index == -1)
        {
            if (witchItems.Contains(item))
            {
                witchContent.GetChild(equippedIndex[0]).GetChild(3).GetComponent<Image>().color = emptyColor;
                equippedIndex[0] = witchItems.IndexOf(item);
                witchContent.GetChild(equippedIndex[0]).GetChild(3).GetComponent<Image>().color = equippedColor;
            }
            else
            {
                catContent.GetChild(equippedIndex[1]).GetChild(3).GetComponent<Image>().color = emptyColor;
                equippedIndex[1] = catItems.IndexOf(item);
                catContent.GetChild(equippedIndex[1]).GetChild(3).GetComponent<Image>().color = equippedColor;
            }
        }
        else
        {
            if(witchItems.Contains(item))
            {
                witchContent.GetChild(equippedIndex[0]).GetChild(3).GetComponent<Image>().color = emptyColor;
                equippedIndex[0] = witchItems.IndexOf(item);
                witchContent.GetChild(equippedIndex[0]).GetChild(3).GetComponent<Image>().color = equippedColor;
            }
            else
            {
                catContent.GetChild(equippedIndex[1]).GetChild(3).GetComponent<Image>().color = emptyColor;
                equippedIndex[1] = index;
                catContent.GetChild(equippedIndex[1]).GetChild(3).GetComponent<Image>().color = equippedColor;
            }
        }
    }

    private int ListHasItem(List<Item> list, Item item)
    {
        int index = -1;
        for(int i = 0; i < list.Count; i++)
        {
            if (list[i].name == item.name)
            {
                index = i;
            }
        }
        return index;
    }
}

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
        if(item.type == "Witch")
        {
            witchItems.Add(item);
        }
        else if(item.type == "Cat")
        {
            catItems.Add(item);
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
            obj.transform.GetChild(3).GetComponent<Image>().color = new Color(1,1,1, equippedIndex[0] == i ? 0.05f : 0);

        }
        for (int i = 0; i < witchItems.Count; i++)
        {
            GameObject obj = Instantiate(itemPrefab, catContent);
            FillInfo(obj, catItems[i]);

            // If the index is an equipped item, designate that it is equipped
            obj.transform.GetChild(3).GetComponent<Image>().color = new Color(1, 1, 1, equippedIndex[1] == i ? 0.05f : 0);
        }
        Debug.Log(equippedIndex[0]);
        Debug.Log(equippedIndex[1]);
    }

    public void FillInfo(GameObject itemObj, Item item)
    {
        itemObj.GetComponent<ItemController>().item = item;
        itemObj.transform.GetChild(0).GetComponent<TMP_Text>().text = item.name;
        itemObj.transform.GetChild(1).GetComponent<Image>().sprite = item.icon;
        itemObj.transform.GetChild(2).GetComponent<TMP_Text>().text = "" + Random.Range(0, 12);
        // itemObj.transform.GetChild(2).GetComponent<TMP_Text>().text = "" + item.amount;
    }

    public void EquipItem(Item item, int index = -1)
    {
        if(index == -1)
        {
            if (witchItems.Contains(item))
            {
                instance.equippedIndex[0] = witchItems.IndexOf(item);
            }
            else
            {
                instance.equippedIndex[1] = catItems.IndexOf(item);
            }
        }
        else
        {
            if(witchItems.Contains(item))
            {
                instance.equippedIndex[0] = index;
            }
            else
            {
                instance.equippedIndex[1] = index;
            }
        }
    }
}

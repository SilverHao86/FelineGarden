using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryController : MonoBehaviour
{
    // Static instance so that it can be used universally
    public static InventoryController instance;

    // List of both items in inventories, as well as which item is equipped
    public List<Item> witchItems = new List<Item>();
    public List<Item> catItems = new List<Item>();
    public int[] equippedIndex;
    private int[] equippedObserver;

    // GameObject values for the creation of the inventory UI
    // Parent for the witch inventory
    public Transform witchContent;
    // Parent for the cat inventory
    public Transform catContent;
    // Prefab to spawn an inventory item from
    public GameObject itemPrefab;
    // Color to use when highlighting equipped item
    private Color equippedColor;
    // Empty color to use when an item is not equipped
    private Color emptyColor;

    // UI indicators for what the witch and cat have equipped
    [SerializeField] private GameObject witchEquipped;
    [SerializeField] private GameObject catEquipped;

    // The amount of time the coroutine takes to animate the displayed swap
    [SerializeField] private float swapTime = 0.5f;

    // Variable to track if the coroutine is running to not duplicate work
    private bool swapRunning = false;
    // Create a parent coroutine to stop the exact coroutine if need be
    Coroutine coInstance = null;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        // Fill the equipped index and colors
        equippedIndex = new int[2];
        equippedIndex[0] = 0;
        equippedIndex[1] = 0;
        equippedObserver = equippedIndex;

        equippedColor = new Color(1, 1, 1, 0.25f);
        emptyColor = new Color(1, 1, 1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        // If the current frame has different equipped items than last frame, update the indicators
        if(equippedIndex[0] != equippedObserver[0] || equippedIndex[1] != equippedObserver[1])
        {
            UpdateEquipDisplay();
        }
    }

    /// <summary>
    /// Adds a given item to the players inventory, based on the tags that it has.
    /// </summary>
    /// <param name="item">The item to add to inventory</param>
    public void Add(Item item)
    {
        // Finds which list the item would belong to
        // If it shares a name with another, then add amounts
        // If it doesn't share a name, add it to the list
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

    /// <summary>
    /// Removes a given object from both lists.
    /// </summary>
    /// <param name="item">The item to remove</param>
    public void Remove(Item item)
    {
        witchItems.Remove(item);
        catItems.Remove(item);
    }

    /// <summary>
    /// Updates the inventory display when needed through destroying and recreating the object tiles.
    /// </summary>
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
        // Display the witch list
        for (int i = 0; i < witchItems.Count; i++)
        {
            GameObject obj = Instantiate(itemPrefab, witchContent);
            FillInfo(obj, witchItems[i]);

            // If the index is an equipped item, designate that it is equipped
            if (obj != null) obj.transform.GetChild(3).GetComponent<Image>().color = (equippedIndex[0] == i ? equippedColor : emptyColor);

        }
        // Display the cat list
        for (int i = 0; i < catItems.Count; i++)
        {
            GameObject obj = Instantiate(itemPrefab, catContent);
            FillInfo(obj, catItems[i]);

            // If the index is an equipped item, designate that it is equipped
            if(obj != null) obj.transform.GetChild(3).GetComponent<Image>().color = (equippedIndex[1] == i ? equippedColor : emptyColor);
        }
    }
    /// <summary>
    /// Fills the item prefab with a given ScriptableObject item.
    /// </summary>
    /// <param name="itemObj">Object to fill</param>
    /// <param name="item">ScriptableObject with information to utilize</param>
    public void FillInfo(GameObject itemObj, Item item)
    {
        // If the amount is less than or equal to 0, equip a different item, list the items, and update equipped display
        if(item.amount <= 0)
        {
            if(itemObj.Equals(witchContent))
            {
                if (equippedIndex[0] == ListHasItem(witchItems, item))
                {
                    EquipIndex(0, equippedIndex[0] - 1);
                }
            }
            else
            {
                if (equippedIndex[1] == ListHasItem(catItems, item))
                {
                    EquipIndex(1, equippedIndex[1] - 1);
                }
            }
            Remove(item);
            ListItems();
            UpdateEquipDisplay();
            return;
        }

        // Otherwise, fill in all of the relevant information
        itemObj.GetComponent<ItemController>().item = item;
        itemObj.transform.GetChild(0).GetComponent<TMP_Text>().text = item.itemName; 
        itemObj.transform.GetChild(1).GetComponent<Image>().sprite = item.icon;
        itemObj.transform.GetChild(2).GetComponent<TMP_Text>().text = "" + item.amount;
    }

    /// <summary>
    /// Fill info only using the given item.
    /// </summary>
    /// <param name="item">Item to fill information based off of</param>
    public void FillInfo(Item item)
    {
        int witchIndex = ListHasItem(witchItems, item);
        GameObject container = (witchIndex != -1 ? witchContent.GetChild(witchIndex).gameObject : catContent.GetChild(ListHasItem(catItems, item)).gameObject);
        FillInfo(container, item);
    }

    /// <summary>
    /// Equip an item to a given character.
    /// </summary>
    /// <param name="item">The item intended to equip</param>
    /// <param name="index">Optional, the index of the item on its list if known</param>
    public void EquipItem(Item item, int index = -1)
    { 
        int witchIndex = ListHasItem(witchItems, item);

        if (witchIndex != -1)
        {
            // Clear the old equipped item if there was one
            if (equippedIndex[0] != -1 && equippedIndex[0] < witchItems.Count)
            {
                witchContent.GetChild(equippedIndex[0]).GetChild(3).GetComponent<Image>().color = emptyColor;
            }
            // Set the new index and set the equipped color
            equippedIndex[0] = index != -1 && index < witchItems.Count ? index : witchIndex;
            witchContent.GetChild(equippedIndex[0]).GetChild(3).GetComponent<Image>().color = equippedColor;
        }
        else
        {
            // Clear the old equipped item if there was one
            if (equippedIndex[1] != -1 && equippedIndex[1] < catItems.Count)
            {
                catContent.GetChild(equippedIndex[1]).GetChild(3).GetComponent<Image>().color = emptyColor;
            }
            // Set the new index and set the equipped color
            equippedIndex[1] = index != -1 && index < catItems.Count ? index : ListHasItem(catItems, item);
            catContent.GetChild(equippedIndex[1]).GetChild(3).GetComponent<Image>().color = equippedColor;
        }

        UpdateEquipDisplay();

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

    /// <summary>
    /// Equip an item based off of the index of the character and the index of the list it belongs to.
    /// </summary>
    /// <param name="equippedChar">The index of the character, 0 = Witch and 1 = Cat</param>
    /// <param name="index">The index of the item on the character's inventory list</param>
    public void EquipIndex(int equippedChar, int index)
    {
        // Find the count and if the index >= count, clamp the index to the end of the list
        int count = (equippedChar == 0 ? witchItems.Count : catItems.Count);
        if (index >= count)
        {
            index = count - 1;
        }

        switch (equippedChar)
        {
            case 0:
                if (equippedIndex[0] != -1 && equippedIndex[0] < witchItems.Count)
                {
                    witchContent.GetChild(equippedIndex[0]).GetChild(3).GetComponent<Image>().color = emptyColor;
                }
                equippedIndex[0] = index != -1 && index < witchItems.Count ? index : equippedIndex[0];
                witchContent.GetChild(equippedIndex[0]).GetChild(3).GetComponent<Image>().color = equippedColor;
                break;
            case 1:
                if (equippedIndex[1] != -1 && equippedIndex[1] < catItems.Count)
                {
                    catContent.GetChild(equippedIndex[1]).GetChild(3).GetComponent<Image>().color = emptyColor;
                }
                equippedIndex[1] = index != -1 && index < catItems.Count ? index : equippedIndex[1];
                catContent.GetChild(equippedIndex[1]).GetChild(3).GetComponent<Image>().color = equippedColor;
                break;
            default:
                break;
        }

        UpdateEquipDisplay();
    }

    /// <summary>
    /// Get the index of the item given the character index and the string name.
    /// </summary>
    /// <param name="charIndex">Witch = 0, Cat = 1</param>
    /// <param name="name">The itemName of a given item</param>
    /// <returns>-1 if the list doesn't contain the item, otherwise returns the index of the item.</returns>
    public int ListHasItem(int charIndex, string name)
    {
        return ListHasItem(charIndex == 0 ? witchItems : catItems, name);
    }

    /// <summary>
    /// Get the index of the item given the list and the item.
    /// </summary>
    /// <param name="list">The list to search through</param>
    /// <param name="item">The item being searched for</param>
    /// <returns>-1 if the list doesn't contain the item, otherwise returns the index of the item.</returns>
    public int ListHasItem(List<Item> list, Item item)
    {
        return ListHasItem(list, item.itemName);
    }

    /// <summary>
    /// Get the index of an item given the list to search and the string name.
    /// </summary>
    /// <param name="list">The list to search through</param>
    /// <param name="name">The name of the item to search for</param>
    /// <returns>-1 if the list doesn't contain the item, otherwise returns the index of the item.</returns>
    public int ListHasItem(List<Item> list, string name)
    {
        int index = -1;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].itemName == name)
            {
                index = i;
            }
        }
        return index;
    }

    /// <summary>
    /// Updates the main equipped item display for the player when called.
    /// </summary>
    public void UpdateEquipDisplay()
    {   
        // If there isn't an item equipped, clear the display
        if(equippedIndex[0] >= witchItems.Count || equippedIndex[0] == -1)
        {
            witchEquipped.transform.GetChild(0).GetComponent<TMP_Text>().text = "";
            witchEquipped.transform.GetChild(1).gameObject.SetActive(false);
            witchEquipped.transform.GetChild(2).GetComponent<TMP_Text>().text = "";
        }
        // Otherwise, add the relevant information to the display
        else
        {
            Debug.Log(equippedIndex[0]);
            FillInfo(witchEquipped, witchItems[equippedIndex[0]]);
            witchEquipped.transform.GetChild(1).gameObject.SetActive(true);
        }
        // If there isn't an item equipped, clear the display
        if (equippedIndex[1] >= catItems.Count || equippedIndex[1] == -1)
        {
            catEquipped.transform.GetChild(0).GetComponent<TMP_Text>().text = "";
            catEquipped.transform.GetChild(1).gameObject.SetActive(false);
            catEquipped.transform.GetChild(2).GetComponent<TMP_Text>().text = "";
        }
        // Otherwise, add the relevant information to the display
        else
        {
            FillInfo(catEquipped, catItems[equippedIndex[1]]);
            catEquipped.transform.GetChild(1).gameObject.SetActive(true);
        }
        
        
    }

    /// <summary>
    /// Head function to swap the display of equipped items. <br/>
    /// Disables a previous coroutine if active and then starts a new one.
    /// </summary>
    /// <param name="activeChar"></param>
    public void SwapEquippedDisplay(int activeChar)
    {
        if(swapRunning)
        {
            // https://answers.unity.com/questions/891122/how-to-stop-coroutine-with-parameters.html
            // Using the instance created before, stop the exact coroutine that we desire from running
            StopCoroutine(coInstance);
        }
        // Create an instance of the coroutine so the exact copy can be stopped if need be
        coInstance = StartCoroutine(MoveEquipped(swapTime, activeChar));
    }

    /// <summary>
    /// PRAISE THE UNIT CIRCLE. <br/>
    /// Moves the two equipped items to swap their positions and scales in a circular manner.
    /// </summary>
    /// <param name="time">The total time for the objects to interpolate between</param>
    /// <param name="activeChar">The index of the active character, 0 = witch and 1 = cat</param>
    /// <returns>IEnumerator return value</returns>
    IEnumerator MoveEquipped(float time, int activeChar)
    {
        // Boolean to check the state of the coroutine in the class
        swapRunning = true;

        // Positions that felt good for the equipped indicators
        Vector3 firstPos = new Vector3(47.5f, -47.5f, 0f);
        Vector3 secondPos = new Vector3(115f, -115f, 0f);

        // Scales that felt good for the equipped indicators
        Vector3 firstScale = Vector3.one;
        Vector3 secondScale = Vector3.one * 0.6f;

        // Calculate the center and radius of the circle for positioning
        Vector3 center = new Vector3((firstPos.x + secondPos.x) / 2f, (firstPos.y + secondPos.y) / 2f, 0f);
        float radius = Vector3.Distance(firstPos, center);

        // Create filler gameobjects for the switch statement
        // Have to be set to a value even if changed because of the while loop (thinks there will be a local scope breakdown)
        GameObject bigToSmall = witchEquipped;
        GameObject smallToBig = witchEquipped;

        // Theta used to calculate their posiitons on the circle
        float smallToBigTheta = 0f;
        float bigToSmallTheta = 0f;

        // Time that passes for the interpolation formula
        float elapsedTime = 0f;

        // Set the objects properly for the given index
        switch(activeChar)
        {
            case 0:
                bigToSmall = catEquipped;
                smallToBig = witchEquipped;
                break;
            case 1:
                bigToSmall = witchEquipped;
                smallToBig = catEquipped;
                break;
            default:
                break;
        }

        // While the given time duration hasn't passed
        while(elapsedTime < time)
        {
            // Interpolate the angles of each object using the ratio of elapsedTime to duration
            // Calculate the small object to move upwards and to the left from its position
            smallToBigTheta = Mathf.Lerp(Mathf.PI / -4f, Mathf.PI * 3f / 4f, elapsedTime / time);
            // Calculate the big object moving downwards and to the right from its position
            bigToSmallTheta = Mathf.Lerp(Mathf.PI * 3f / 4f, Mathf.PI * 7f / 4f, elapsedTime / time);

            // Calculate the position from the angle using trigonometry
            smallToBig.GetComponent<RectTransform>().anchoredPosition = new Vector3(Mathf.Cos(smallToBigTheta), Mathf.Sin(smallToBigTheta), 0f) * radius + center;
            bigToSmall.GetComponent<RectTransform>().anchoredPosition = new Vector3(Mathf.Cos(bigToSmallTheta), Mathf.Sin(bigToSmallTheta), 0f) * radius + center;

            // Interpolate scale using the same ratio as before
            smallToBig.transform.localScale = Vector3.Lerp(secondScale, firstScale, elapsedTime / time);
            bigToSmall.transform.localScale = Vector3.Lerp(firstScale, secondScale, elapsedTime / time);


            bigToSmall.transform.GetChild(0).GetComponent<TMP_Text>().fontSize = Mathf.Lerp(7, 9, elapsedTime / time);
            smallToBig.transform.GetChild(0).GetComponent<TMP_Text>().fontSize = Mathf.Lerp(9, 7, elapsedTime / time);

            // Add to the total time passed
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // In case the interpolating fails, set each object's position to their final position and scale
        smallToBig.GetComponent<RectTransform>().anchoredPosition = firstPos;
        bigToSmall.GetComponent<RectTransform>().anchoredPosition = secondPos;

        smallToBig.transform.localScale = firstScale;
        bigToSmall.transform.localScale = secondScale;

        bigToSmall.transform.GetChild(0).GetComponent<TMP_Text>().fontSize = 9;
        smallToBig.transform.GetChild(0).GetComponent<TMP_Text>().fontSize = 7;

        // Make sure the class knows the coroutine is no longer running
        swapRunning = false;

        yield return null;
    }
}

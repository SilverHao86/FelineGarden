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
    private int[] equippedObserver;


    public Transform witchContent;
    public Transform catContent;
    public GameObject itemPrefab;
    private Color equippedColor;
    private Color emptyColor;


    [SerializeField] private GameObject witchEquipped;
    [SerializeField] private GameObject catEquipped;
    [SerializeField] private float swapTime = 0.5f;

    private bool swapRunning = false;

    Coroutine coInstance = null;

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
        equippedObserver = equippedIndex;

        equippedColor = new Color(1, 1, 1, 0.25f);
        emptyColor = new Color(1, 1, 1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(equippedIndex[0] != equippedObserver[0] || equippedIndex[1] != equippedObserver[1])
        {
            UpdateEquipDisplay();
        }
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
            if (obj != null) obj.transform.GetChild(3).GetComponent<Image>().color = (equippedIndex[0] == i ? equippedColor : emptyColor);

        }
        for (int i = 0; i < catItems.Count; i++)
        {
            GameObject obj = Instantiate(itemPrefab, catContent);
            FillInfo(obj, catItems[i]);

            // If the index is an equipped item, designate that it is equipped
            if(obj != null) obj.transform.GetChild(3).GetComponent<Image>().color = (equippedIndex[1] == i ? equippedColor : emptyColor);
        }
    }

    public void FillInfo(GameObject itemObj, Item item)
    {
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
        itemObj.GetComponent<ItemController>().item = item;
        itemObj.transform.GetChild(0).GetComponent<TMP_Text>().text = item.itemName; 
        itemObj.transform.GetChild(1).GetComponent<Image>().sprite = item.icon;
        itemObj.transform.GetChild(2).GetComponent<TMP_Text>().text = "" + item.amount;
    }

    public void FillInfo(Item item)
    {
        int witchIndex = ListHasItem(witchItems, item);
        GameObject container = (witchIndex != -1 ? witchContent.GetChild(witchIndex).gameObject : catContent.GetChild(ListHasItem(catItems, item)).gameObject);
        FillInfo(container, item);
    }

    public void EquipItem(Item item, int index = -1)
    { 
        int witchIndex = ListHasItem(witchItems, item);

        if (witchIndex != -1)
        {
            if (equippedIndex[0] != -1 && equippedIndex[0] < witchItems.Count)
            {
                witchContent.GetChild(equippedIndex[0]).GetChild(3).GetComponent<Image>().color = emptyColor;
            }
            equippedIndex[0] = index != -1 && index < witchItems.Count ? index : witchIndex;
            witchContent.GetChild(equippedIndex[0]).GetChild(3).GetComponent<Image>().color = equippedColor;
        }
        else
        {
            if (equippedIndex[1] != -1 && equippedIndex[1] < catItems.Count)
            {
                catContent.GetChild(equippedIndex[1]).GetChild(3).GetComponent<Image>().color = emptyColor;
            }
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

    public void EquipIndex(int equippedChar, int index)
    {
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

    public int ListHasItem(int charIndex, string name)
    {
        return ListHasItem(charIndex == 0 ? witchItems : catItems, name);
    }

    public int ListHasItem(List<Item> list, Item item)
    {
        return ListHasItem(list, item.itemName);
    }

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

    public void UpdateEquipDisplay()
    {        
        if(equippedIndex[0] >= witchItems.Count || equippedIndex[0] == -1)
        {
            witchEquipped.transform.GetChild(0).GetComponent<TMP_Text>().text = "";
            witchEquipped.transform.GetChild(1).gameObject.SetActive(false);
            witchEquipped.transform.GetChild(2).GetComponent<TMP_Text>().text = "";
        }
        else
        {
            Debug.Log(equippedIndex[0]);
            FillInfo(witchEquipped, witchItems[equippedIndex[0]]);
            witchEquipped.transform.GetChild(1).gameObject.SetActive(true);
        }

        if(equippedIndex[1] >= catItems.Count || equippedIndex[1] == -1)
        {
            catEquipped.transform.GetChild(0).GetComponent<TMP_Text>().text = "";
            catEquipped.transform.GetChild(1).gameObject.SetActive(false);
            catEquipped.transform.GetChild(2).GetComponent<TMP_Text>().text = "";
        }
        else
        {
            FillInfo(catEquipped, catItems[equippedIndex[1]]);
            catEquipped.transform.GetChild(1).gameObject.SetActive(true);
        }
        
        
    }

    /// <summary>
    /// Head function to swap the display of equipped items
    /// Disables a previous coroutine if active and then starts a new one
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
    /// PRAISE THE UNIT CIRCLE
    /// Moves the two equipped items to swap their positions and scales in a circular manner
    /// </summary>
    /// <param name="time">The total time for the objects to interpolate between</param>
    /// <param name="activeChar">The index of the active character, 0 = witch and 1 = cat</param>
    /// <returns></returns>
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

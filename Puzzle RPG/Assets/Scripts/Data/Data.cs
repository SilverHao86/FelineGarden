using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Data : ScriptableObject
{
    private const char SEPARATOR_1 = '+'; // inventory, characters
    private const char SEPARATOR_2 = '~'; // inventory (cat/gardener), characters (cat/gardener)
    private const char SEPARATOR_3 = '|'; // cat/gardener (items), cat (position), gardener (position)
    private const char SEPARATOR_4 = '?'; // 
    private const string FILE_NAME = "Data";
    private const string FILE_EXT = ".txt";
    private const string FILE = FILE_NAME + FILE_EXT;
    private string SAVE_FOLDER = Application.dataPath + "/Saves/";
    private readonly string FILE_PATH;
    private string inventoryBuffer = string.Empty;
    private string characterBuffer = string.Empty;
    private string checkpointBuffer = string.Empty;
    private string buffer = string.Empty;
    private InventoryController inventory = InventoryController.instance;

    private uint checkpointIndex = 0;
    private Cat cat;
    private Gardener gardener;
    //public float timePlayed = 0;

    public uint CheckpointIndex
    {
        get { return checkpointIndex; }
    }

    public Data() { }
    public Data(
        Cat cat,
        Gardener gardener)
    {
        this.cat = cat;
        this.gardener = gardener;
        FILE_PATH = SAVE_FOLDER + FILE;
    }

    private void ExtractInventory()
    {
        inventoryBuffer = string.Empty;

        // Witch Items
        for (int i = 0; i < inventory.witchItems.Count; i++)
        {
            inventoryBuffer += JsonUtility.ToJson(inventory.witchItems[i], true);
            inventoryBuffer += i+1 != inventory.witchItems.Count ? SEPARATOR_3 : ""; // Separator + Spacing
        }
        inventoryBuffer += SEPARATOR_2;
        // Cat Items
        for (int i = 0; i < inventory.catItems.Count; i++)
        {
            inventoryBuffer += JsonUtility.ToJson(inventory.catItems[i], true);
            inventoryBuffer += i + 1 != inventory.catItems.Count ? SEPARATOR_3 : ""; // Separator + Spacing
        }
        inventoryBuffer += SEPARATOR_2;
        // Equipped Index
        inventoryBuffer += inventory.equippedIndex.Length;
        inventoryBuffer += SEPARATOR_3;
        inventoryBuffer += JsonUtility.ToJson(inventory.equippedIndex, true);
        inventoryBuffer += SEPARATOR_1; // end of inventory

        //Debug.Log("There are " + inventory.witchItems.Count + " witch item(s) in the inventory");
        //Debug.Log("There are " + inventory.catItems.Count + " cat item(s) in the inventory");
        //Debug.Log(buffer);
    }
    private void ExtractCharacters()
    {
        characterBuffer = string.Empty;
        characterBuffer += JsonUtility.ToJson(cat, true);
        characterBuffer += SEPARATOR_3;
        Vector3 catLocation = cat.gameObject.transform.position;
        characterBuffer += JsonUtility.ToJson(catLocation, true);

        characterBuffer += SEPARATOR_2; // remove if nothing is next

        characterBuffer += JsonUtility.ToJson(gardener, true);
        characterBuffer += SEPARATOR_3;
        Vector3 gardenerLocation = gardener.gameObject.transform.position;
        characterBuffer += JsonUtility.ToJson(gardenerLocation, true);

        characterBuffer += SEPARATOR_1;
    }
    private void ExtractCheckpoint()
    {
        // Save checkpoint data
        checkpointBuffer = string.Empty;
        checkpointBuffer += checkpointIndex;
    }
    public void Extract()
    {
        ExtractInventory();
        ExtractCharacters();
        ExtractCheckpoint();
    }

    private void FillInventory()
    {
        if(inventoryBuffer.Length <= 0) { return; }

        string[] characterInventories = inventoryBuffer.Split(SEPARATOR_2);
        string[] witchItems = characterInventories[0].Split(SEPARATOR_3);
        string[] catItems = characterInventories[1].Split(SEPARATOR_3);
        string[] equippedIndexes = characterInventories[2].Split(SEPARATOR_3);

        List <Item> witchItems2 = new List<Item>(witchItems.Length);
        List<Item> catItems2 = new List<Item>(catItems.Length);

        for (int i = 0; i < witchItems.Length; i++)
        {
            Item item = new Item();
            JsonUtility.FromJsonOverwrite(witchItems[i], item);
            witchItems2.Add(item);
        }
        for (int i = 0; i < catItems.Length; i++)
        {
            Item item = new Item();
            JsonUtility.FromJsonOverwrite(catItems[i], item);
            catItems2.Add(item);
        }

        int indexSize = int.Parse(equippedIndexes[0]);
        int[] equippedIndex = new int[indexSize];
        JsonUtility.FromJsonOverwrite(equippedIndexes[1], equippedIndex);

        inventory.witchItems = witchItems2;
        inventory.catItems = catItems2;
        inventory.equippedIndex = equippedIndex;
    }
    private void FillCharacters()
    {
        if (characterBuffer.Length <= 0) { return; }

        string[] characterData = characterBuffer.Split(SEPARATOR_2);
        string[] catData = characterData[0].Split(SEPARATOR_3);
        string[] gardenerData = characterData[0].Split(SEPARATOR_3);

        string cData = catData[0];
        string cLocation = catData[1];
        string gData = gardenerData[0];
        string gLocation = gardenerData[1];

        JsonUtility.FromJsonOverwrite(cData, cat);
        Vector3 catLoc = JsonUtility.FromJson<Vector3>(cLocation);
        catLoc.y += 0.5f;
        cat.gameObject.transform.position = catLoc;
        JsonUtility.FromJsonOverwrite(gData, cat);
        Vector3 gardenerLoc = JsonUtility.FromJson<Vector3>(gLocation);
        gardenerLoc.y += 0.5f;
        gardener.gameObject.transform.position = gardenerLoc;

        // Set active player!!!!
    }
    private void FillCheckpoint()
    {
        if (checkpointBuffer.Length <= 0) { return; }

        checkpointIndex = uint.Parse(checkpointBuffer);
    }
    private void Fill()
    {
        FillInventory();
        FillCharacters();
        FillCheckpoint();
    }

    private void Load()
    {
        buffer = System.IO.File.ReadAllText(FILE_PATH);
        if (buffer.Length <= 0) { return; }

        string[] core = buffer.Split(SEPARATOR_1);
        inventoryBuffer = core[0];
        characterBuffer = core[1];
        checkpointBuffer = core[2];
    }
    private void Save()
    {
        buffer = inventoryBuffer + characterBuffer + checkpointBuffer;
        System.IO.File.WriteAllText(FILE_PATH, buffer);
    }
    public void SaveCheckpoint(uint checkpointIndex)
    {
        this.checkpointIndex = checkpointIndex;
    }
    public void ExtractAndSave()
    {
        Extract();
        Save();
    }
    public void LoadAndFill()
    {
        Load();
        Fill();
    }

    public void SaveInventoryTest()
    {
        TestItem testItem = new TestItem();
        testItem.id = 0; 
        testItem.itemName = "Name";
        testItem.amount = 2;
        //Debug.Log(JsonUtility.ToJson(testItem, true));

        List<TestItem> testItems = new List<TestItem>();
        testItems.Add(testItem);
        //Debug.Log(JsonUtility.ToJson(testItems, true));

        SerializableList<TestItem> sTestItems = new SerializableList<TestItem>();
        sTestItems.list.Add(testItem);
        //Debug.Log(JsonUtility.ToJson(sTestItems, true));

        testItems.Add(testItem);
        testItems.Add(testItem);
        testItems.Add(testItem);
        string buffer = string.Empty;
        const char ITEM_SEPARATOR = '~';

        buffer += "ITEMS\n";
        foreach (TestItem item in testItems)
        {
            buffer += JsonUtility.ToJson(item, true);
            buffer += "\n" + ITEM_SEPARATOR + "\n"; // Separator + Spacing
        }
        //Debug.Log(buffer);

        Item realItem = new Item();
        realItem.id = 0;
        realItem.itemName = "Name";
        realItem.amount = 2;
        //Debug.Log(JsonUtility.ToJson(realItem, true));

        InventoryController inventory = InventoryController.instance;
        Item realItem2 = inventory.witchItems[0];
        //Debug.Log(JsonUtility.ToJson(realItem2, true));

        buffer = string.Empty;
        buffer += "WITCH-ITEMS\n";
        foreach (Item item in inventory.witchItems)
        {
            buffer += JsonUtility.ToJson(item, true);
            buffer += "\n" + ITEM_SEPARATOR + "\n"; // Separator + Spacing
        }
        Debug.Log("There are "+ inventory.witchItems.Count + " witch item(s) in the inventory");
        Debug.Log(buffer);
    }
    public void Test()
    {
        Extract();
        Save();
        Load();
        Fill();
    }
}


[System.Serializable] public class SerializableList<T>
{
    public List<T> list;

    public SerializableList()
    {
        list = new List<T>();
    }
}
public class TestItem : ScriptableObject //[System.Serializable]
{
    public int id;
    public string itemName;
    public uint amount;
    public Sprite icon;
    public string type;
    public bool isInventory;

    public void ResetData()
    {
        amount = 1;
    }
}
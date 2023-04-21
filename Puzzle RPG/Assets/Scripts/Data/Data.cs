using UnityEngine;

[System.Serializable]
public class Data : ScriptableObject
{
    public int score = 0;
    public float timePlayed = 0;

    // 2 characters and 2 cameras
    // Player locations
    public Cat cat;
    public Gardener gardener;

    // Inventory
    InventoryController inventory; // might need to break apart

    public Data() { }
    public Data(
        Cat cat,
        Gardener gardener)
    {
        this.cat = cat;
        this.gardener = gardener;
        inventory = InventoryController.instance;
    }
}
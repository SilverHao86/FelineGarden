using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantPlot : MonoBehaviour
{
    public enum PlantType { beanStalk, lily, pollenPuff, notAPlant };
    [field: SerializeField] public PlantType PlotType { get; private set; }
    [HideInInspector] public bool PlantActive { get; set; }

    [SerializeField, Header("Beanstalk Specific")] private GameObject beanstalkBase; 
    [SerializeField, Range(1, 10)] private int beanStalkSize; 
    [SerializeField] Sprite beanStalkTop;
    [SerializeField] List<Sprite>  beanStalkMiddle;
    [SerializeField] Item beanStalkSeed;

    [SerializeField, Header("Lily Specific")] private List<GameObject> lilyBases;
    [SerializeField] List<Sprite> lilyVariants;
    [SerializeField] Item lilySeed;


    private List<GameObject> plantMakeUp = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
        PlantActive = false;
        //foreach (GameObject p in beanstalkBase) { p.gameObject.SetActive(PlantActive); }
        if(beanstalkBase != null) { beanstalkBase.gameObject.SetActive(PlantActive); }
        foreach (GameObject l in lilyBases) { l.SetActive(PlantActive); }; 
        

        //switch (PlotType)
        //{
        //    case PlantType.beanStalk:
                
        //        break;
        //    case PlantType.lily:
        //        break;
        //    case PlantType.pollenPuff:
        //        break;
        //    case PlantType.notAPlant:
        //        break;
        //}
    }

    public void PlantPlant()
    {
        int index = InventoryController.instance.equippedIndex[0];
        Item tempItem;
        try
        {
            tempItem = InventoryController.instance.witchItems[index];
            Debug.Log(index);
            if (!(tempItem.amount > 0) || tempItem.id > 1 || tempItem.itemName.Equals("Ring of Strength")) return; // Greater than 1 id == not a plant
            PlotType = (PlantType)tempItem.id;
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
            return;
               
        }

        PlantActive = true;
        // Remove Plant from inventory
        

        switch (PlotType)
        {
            case PlantType.beanStalk:
                InventoryController.instance.witchItems[index].amount--;
                InventoryController.instance.FillInfo(InventoryController.instance.witchItems[index]);
                PopulateBeanStalk();   
                break;

            case PlantType.lily:
                int lilies = 0;
                bool allertOnce = false;
                bool stopPlanting = false;
                for (int i = 0; i < lilyBases.Count; i++)
                {
                    if (InventoryController.instance.witchItems.Count != 0 && !stopPlanting)
                    {
                        InventoryController.instance.witchItems[index].amount--;
                        InventoryController.instance.FillInfo(InventoryController.instance.witchItems[index]);
                        lilies++;

                        if (!(tempItem.amount > 0) || tempItem.id != 1) stopPlanting = true;
                    }
                    if (stopPlanting)
                    {
                        if (!allertOnce)
                        {
                            allertOnce = true;
                            Debug.Log("Not enough Lilies");
                        }
                    }

                }
                PopulateLilys(lilies);
                break;

            case PlantType.pollenPuff:
                break;

            case PlantType.notAPlant:
                break;
        }

        foreach (GameObject p in plantMakeUp)
        {
            p.gameObject.SetActive(PlantActive);
        }
    }

    public void CutPlant()
    {
        // Dont Cut the Plant if the knife isn't equiped
        int index = InventoryController.instance.equippedIndex[1];
        Item tempItem;
        try
        {
            tempItem = InventoryController.instance.catItems[index];
            if (!tempItem.itemName.Equals("Plant Cutter")) return;
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
            return;
        }
        
        
        PlantActive = false;
        
        
        switch (PlotType)
        {
            case PlantType.beanStalk:
                InventoryController.instance.Add(beanStalkSeed);
                for (int i = 0; i < plantMakeUp.Count; i++)
                {
                    if (i == 0)
                    {
                        plantMakeUp[i].SetActive(PlantActive);
                    }
                    else
                    {
                        Destroy(plantMakeUp[i]);
                    }
                }
                break;

            case PlantType.lily:
                for (int i = 0; i < plantMakeUp.Count; i++)
                {
                    InventoryController.instance.Add(lilySeed);
                    plantMakeUp[i].SetActive(PlantActive);
                }
                break;

            case PlantType.pollenPuff:
                break;
            default:
                break;
        }
        
        plantMakeUp.Clear();
    }


    public void PopulateLilys(int lilyAmmount)
    {
        Debug.Log("Lily Amm:" + lilyAmmount);
        for (int i = 0; i < lilyAmmount; i++)
        {
            plantMakeUp.Add(lilyBases[i]);
        }
        

        // Change their sprite randomly
        foreach (GameObject lily in plantMakeUp)
        {
            lily.GetComponent<SpriteRenderer>().sprite = lilyVariants[UnityEngine.Random.Range(0, lilyVariants.Count)];
        }
    }

    public void PopulateBeanStalk()
    {
        
        if (beanStalkSize <= 1) return;
        plantMakeUp.Add(beanstalkBase);
        for (int i = 1; i < beanStalkSize; i++)
        {
            GameObject addedPlant = Instantiate(beanstalkBase, transform.parent);
            addedPlant.transform.position = new Vector3(beanstalkBase.transform.position.x, beanstalkBase.transform.position.y + (0.8f * i), beanstalkBase.transform.position.z);

            // top of the beanstalk

            if (i == beanStalkSize-1)
            {
                addedPlant.GetComponent<SpriteRenderer>().sprite = beanStalkTop;
            }
            else
            {
                addedPlant.GetComponent<SpriteRenderer>().sprite = beanStalkMiddle[UnityEngine.Random.Range(0, (beanStalkMiddle.Count))];
            }
            plantMakeUp.Add(addedPlant);
        }

    }
}

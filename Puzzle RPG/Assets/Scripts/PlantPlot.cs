using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantPlot : MonoBehaviour
{
    public enum PlantType { beanStalk, lily, pollenPuff, notAPlant };
    [field: SerializeField] public PlantType PlotType { get; private set; }
    [SerializeField] private GameObject plant;
    [SerializeField]
    [Range(1, 10)]
    private int beanStalkSize;
    [SerializeField] Sprite beanStalkTop;
    [SerializeField] List<Sprite>  beanStalkMiddle;


    [HideInInspector] public bool PlantActive { get; set; }
    private List<GameObject> plantMakeUp = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
        PlantActive = false;
        plant.gameObject.SetActive(PlantActive);

        switch (PlotType)
        {
            case PlantType.beanStalk:
                PopulateBeanStalk();
                break;
            case PlantType.lily:
                break;
            case PlantType.pollenPuff:
                break;
            case PlantType.notAPlant:
                break;
        }
    }

    public void PlantPlant()
    {
        PlantActive = true;
        foreach (GameObject p in plantMakeUp)
        {
            p.gameObject.SetActive(PlantActive);
        }
    }

    public void CutPlant()
    {
        PlantActive = false;
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

        plantMakeUp.Clear();

        switch (PlotType)
        {
            case PlantType.beanStalk:
                PopulateBeanStalk();
                break;
            case PlantType.lily:
                break;
            case PlantType.pollenPuff:
                break;
            default:
                break;
        }
    }

    public void PopulateBeanStalk()
    {
        plantMakeUp.Add(plant);
        if (beanStalkSize <= 1) return;
        for (int i = 1; i < beanStalkSize; i++)
        {
            GameObject addedPlant = Instantiate(plant, transform.parent);
            addedPlant.transform.position = new Vector3(plant.transform.position.x, plant.transform.position.y + (0.8f * i), plant.transform.position.z);

            // top of the beanstalk

            if (i == beanStalkSize-1)
            {
                addedPlant.GetComponent<SpriteRenderer>().sprite = beanStalkTop;
            }
            else
            {
                addedPlant.GetComponent<SpriteRenderer>().sprite = beanStalkMiddle[Random.Range(0, (beanStalkMiddle.Count))];
            }
            plantMakeUp.Add(addedPlant);
        }
        foreach (GameObject p in plantMakeUp)
        {
            p.gameObject.SetActive(PlantActive);
        }
    }
}

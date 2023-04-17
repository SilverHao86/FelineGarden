using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantPlot : MonoBehaviour
{
    public enum PlantType { beanStalk, lily, pollenPuff };
    [field: SerializeField] public PlantType PlotType { get; private set; }
    [SerializeField] private GameObject plant;
    [HideInInspector] public bool PlantActive { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        PlantActive = false;
        plant.gameObject.SetActive(PlantActive);
    }

    public void PlantPlant()
    {
        PlantActive = true;
        plant.gameObject.SetActive(PlantActive);
    }
}

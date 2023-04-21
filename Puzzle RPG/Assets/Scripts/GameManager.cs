using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Camera catCam;
    [SerializeField] private Camera gardenCam;
    [SerializeField] private Cat catChar;
    [SerializeField] private Gardener gardenerChar;
    public KeyCode SwapCamKey;
    private UIController controller;
    private DataManager dataManager;

    void Start()
    {
        catChar.Cam = catCam;
        gardenerChar.Cam = gardenCam;
        controller = GetComponent<UIController>();
        Physics2D.IgnoreCollision(catChar.GetComponent<BoxCollider2D>(), gardenerChar.GetComponent<BoxCollider2D>());
        SetActiveGardner(true);
        SetActiveCat(false);
        
        InitData(); // only needs to be triggered when gameplay starts
    }

    void Update()
    {
        if (Input.GetKeyDown(controller.PauseKey))
        {
            controller.Paused = !controller.Paused;
            Time.timeScale = controller.Paused ? 0f : 1f; // If paused is true, stop time scale, if it is false, set the timescale to normal values
            controller.PauseMenu.SetActive(controller.Paused);
        }

        if (!controller.Paused)
        {
            if (Input.GetKeyDown(SwapCamKey))
            {
                Debug.Log("Collision Check Gardener: " + gardenerChar.isOnLayerMask(gardenerChar.data.groundLayer) + " "  + gardenerChar.isOnLayerMask(gardenerChar.data.beanstalkLayer) + " " + gardenerChar.isOnLayerMask(gardenerChar.data.boxLayer));
                Debug.Log("Collision Check Cat: " + catChar.isOnLayerMask(catChar.data.groundLayer) + " " + catChar.isOnLayerMask(catChar.data.beanstalkLayer) + " " + catChar.isOnLayerMask(catChar.data.boxLayer));
                if ((gardenerChar.isOnLayerMask(gardenerChar.data.groundLayer) || gardenerChar.isOnLayerMask(gardenerChar.data.beanstalkLayer) || gardenerChar.isOnLayerMask(gardenerChar.data.boxLayer))
                    && (catChar.isOnLayerMask(catChar.data.groundLayer) || catChar.isOnLayerMask(catChar.data.beanstalkLayer) || catChar.isOnLayerMask(catChar.data.boxLayer)))
                {
                    gardenerChar.Cam.enabled = !gardenerChar.Cam.enabled;
                    gardenerChar.active = gardenerChar.Cam.enabled;
                    catChar.Cam.enabled = !catChar.Cam.enabled;
                    catChar.active = catChar.Cam.enabled;
                    catChar.ToggleMovement();
                    gardenerChar.ToggleMovement();
                }
               
            }
        }

    }

    void SetActiveGardner(bool active)
    {
        gardenerChar.active = active;
        gardenerChar.Cam.enabled = active;
        gardenerChar.ToggleMovement();
    }

    void SetActiveCat(bool active)
    {
        catChar.active = active;
        catChar.Cam.enabled = active;
        catChar.ToggleMovement();
    }

    private void InitData()
    {
        dataManager = new DataManager();
    }
}

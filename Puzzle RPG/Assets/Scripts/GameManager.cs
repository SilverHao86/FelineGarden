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
    // Start is called before the first frame update
    void Start()
    {
        catChar.Cam = catCam;
        gardenerChar.Cam = gardenCam;
        controller = GetComponent<UIController>();
        Physics2D.IgnoreCollision(catChar.GetComponent<BoxCollider2D>(), gardenerChar.GetComponent<BoxCollider2D>());
        SetActiveGardner(true);
        SetActiveCat(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(controller.PauseKey))
        {
            controller.Paused = !controller.Paused;
            Time.timeScale = controller.Paused ? 0f : 1f; // If paused is true, stop time scale, if it is false, set the timescale to normal values
            controller.pauseMenu.SetActive(controller.Paused);
        }

        if (!controller.Paused)
        {
            if (Input.GetKeyDown(SwapCamKey))
            {
                if (gardenerChar.isOnGround() && catChar.isOnGround())
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
}

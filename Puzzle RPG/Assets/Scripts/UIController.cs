using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private KeyCode pauseKey;
    private bool paused = false;
    [SerializeField] private GameObject pauseMenu;

    [SerializeField] private List<CanvasGroup> canvasGroups;
    [SerializeField] private float fadeRate = 1.0f;

    [SerializeField] private List<Image> checklist;
    [SerializeField] private Sprite checkSprite;
    [SerializeField] private Sprite crossSprite;
    //private List<bool> solvedList = new List<bool>();


    private bool fadeIn = false;
    private bool fadeOut = false;
    [SerializeField] private Vector2 mousePos;
    [SerializeField] private int currentStep = 0;

    // Swap between gardener and cat
    [SerializeField] private KeyCode swapCamKey;
    [SerializeField] private Camera gardenerCam;
    [SerializeField] private Camera catCam;
    public int activeCharIndex;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 1; i < canvasGroups.Count; i++)
        {
            canvasGroups[i].alpha = 0;
        }
        //foreach(Image image in checklist)
        //{
        //    solvedList.Add(true);
        //}
    }

    public void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            paused = !paused;
            Time.timeScale = paused ? 0f : 1f; // If paused is true, stop time scale, if it is false, set the timescale to normal values
            pauseMenu.SetActive(paused);
        }

        if(!paused)
        {
            if(Input.GetKeyDown(swapCamKey))
            {
                gardenerCam.enabled = !gardenerCam.enabled;
                catCam.enabled = !catCam.enabled;
                activeCharIndex = (gardenerCam.enabled ? 0 : 1);
            }
        }
        
        //for (int i = 0; i < checklist.Count; i++)
        //{
        //    checklist[i].sprite = (solvedList[i] ? checkSprite : crossSprite);
        //}
        //if (currentStep < 6)
        //{
        //    switch(currentStep)
        //    {
        //        case 0:
        //            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)) fadeOut = true;
        //            break;
        //        case 1:
        //            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) fadeOut = true;
        //            break;
        //        case 2:
        //            if (GetScrollWheelInput()) fadeOut = true;
        //            break;
        //        case 3:
        //            if (GetMouseClickMove()) fadeOut = true;
        //            break;
        //        case 4:
        //            if (Input.GetMouseButton(0) && GetScrollWheelInput()) fadeOut = true;
        //            break;
        //        default:
        //            break;
        //    }
        //    if (fadeOut)
        //    {
        //        canvasGroups[currentStep].alpha -= fadeRate * Time.deltaTime;
        //        fadeIn = false;
        //    }
        //    if (fadeIn)
        //    {
        //        canvasGroups[currentStep].alpha += fadeRate * Time.deltaTime;
        //    }
        //    if (currentStep+1 < canvasGroups.Count && canvasGroups[currentStep].alpha <= 0)
        //    {
        //        fadeOut = false;
        //        canvasGroups[currentStep].gameObject.SetActive(false);
        //        currentStep++;
        //        if(currentStep == 3 || currentStep == 4)
        //        {
        //            mousePos = Input.mousePosition;
        //        }
        //        canvasGroups[currentStep].gameObject.SetActive(true);
        //        fadeIn = true;
        //    }
        //    if (canvasGroups[currentStep].alpha == 1)
        //    {
        //        fadeIn = false;
        //    }
        //}
    }

    bool GetScrollWheelInput()
    {
        return Input.mouseScrollDelta != Vector2.zero;
    }

    bool GetMouseClickMove()
    {
        bool mouseMove = (!mousePos.Equals(Input.mousePosition));
        //Debug.Log("Mouse Move: " + mouseMove);
        mousePos = Input.mousePosition;
        return Input.GetMouseButton(0) && mouseMove;
    }

    void SetSprite()
    {

    }
}

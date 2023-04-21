using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [field: SerializeField] public KeyCode PauseKey { get; private set; }
    public bool Paused { get; set; }
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

    [SerializeField] private KeyCode inventoryKey;
    [SerializeField] private GameObject witchInventory;
    [SerializeField] private GameObject catInventory;

    public GameObject PauseMenu { get { return pauseMenu; } }

    // Start is called before the first frame update
    void Start()
    {
        Paused = false;
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
        if(Input.GetKeyDown(inventoryKey))
        {
            witchInventory.SetActive(!witchInventory.activeSelf);
            catInventory.SetActive(!catInventory.activeSelf);
            if (witchInventory.activeSelf || catInventory.activeSelf)
            {
                InventoryController.instance.ListItems();
            }
        }

        if (Input.GetKeyDown(KeyCode.Minus))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

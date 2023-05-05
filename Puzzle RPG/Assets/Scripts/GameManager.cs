using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    enum GameState
    {
        Init,
        Play,
        Pause
    };
    [SerializeField] private Camera catCam;
    [SerializeField] private Camera gardenCam;
    [SerializeField] private Cat catChar;
    [SerializeField] private Gardener gardenerChar;
    public KeyCode SwapCamKey;
    private UIController controller;
    private static GameManager instance;
    public GameObject checkpointContainer;
    //private Checkpoint[] checkpoints;
    private List<Checkpoint> checkpoints;

    private DataManager dataManager;
    private Data playerData;
    private float saveTimer = 0.0f;
    private const int SAVE_INTERVAL = 2; // 1min
    GameState state = GameState.Init;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>(); // Looks for existing
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(GameManager).Name;
                    instance = obj.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        if(checkpointContainer != null)
        {
            //checkpoints = new Checkpoint[checkpointContainer.transform.childCount];
            checkpoints = new List<Checkpoint>(checkpointContainer.transform.childCount);
            foreach (Checkpoint cpScript in checkpointContainer.transform.GetComponentsInChildren<Checkpoint>())
            {
                checkpoints.Add(cpScript);
            }
        }
    }

    void Start()
    {
        catChar.Cam = catCam;
        gardenerChar.Cam = gardenCam;
        controller = GetComponent<UIController>();
        Physics2D.IgnoreCollision(catChar.GetComponent<BoxCollider2D>(), gardenerChar.GetComponent<BoxCollider2D>());
        SetActiveGardner(true);
        SetActiveCat(false);
        
        state = GameState.Play;
        InitData(); // only needs to be triggered when gameplay starts
    }

    void Update()
    {
        if (Input.GetKeyDown(controller.PauseKey))
        {
            controller.Paused = !controller.Paused;
            Time.timeScale = controller.Paused ? 0f : 1f; // If paused is true, stop time scale, if it is false, set the timescale to normal values
            state = controller.Paused ? GameState.Pause : GameState.Play;
            controller.PauseMenu.SetActive(controller.Paused);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SpawnAtLastCheckpoint();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            Save();
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Load();
        }

        if (state == GameState.Play)
        {
            if (Input.GetKeyDown(SwapCamKey))
            {
                //Debug.Log("Collision Check Gardener: " + gardenerChar.isOnLayerMask(gardenerChar.data.groundLayer) + " "  + gardenerChar.isOnLayerMask(gardenerChar.data.beanstalkLayer) + " " + gardenerChar.isOnLayerMask(gardenerChar.data.boxLayer));
                //Debug.Log("Collision Check Cat: " + catChar.isOnLayerMask(catChar.data.groundLayer) + " " + catChar.isOnLayerMask(catChar.data.beanstalkLayer) + " " + catChar.isOnLayerMask(catChar.data.boxLayer));
                if ((gardenerChar.isOnLayerMask(gardenerChar.data.groundLayer) || gardenerChar.isOnLayerMask(gardenerChar.data.beanstalkLayer) || gardenerChar.isOnLayerMask(gardenerChar.data.boxLayer))
                    && (catChar.isOnLayerMask(catChar.data.groundLayer) || catChar.isOnLayerMask(catChar.data.beanstalkLayer) || catChar.isOnLayerMask(catChar.data.boxLayer)))
                {
                    gardenerChar.Cam.enabled = !gardenerChar.Cam.enabled;
                    gardenerChar.active = gardenerChar.Cam.enabled;
                    catChar.Cam.enabled = !catChar.Cam.enabled;
                    catChar.active = catChar.Cam.enabled;
                    catChar.ToggleMovement();
                    gardenerChar.ToggleMovement();

                    InventoryController.instance.SwapEquippedDisplay(gardenerChar.active ? 0 : 1);
                }
               
            }

            saveTimer += Time.deltaTime;
            if(saveTimer > SAVE_INTERVAL)
            {
                //SaveGame();
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

    public void SaveCheckpoint(Checkpoint cp)
    {
        //Debug.Log("Saved Checkpoint");
        uint cpIndex = (uint)checkpoints.IndexOf(cp);
        playerData.SaveCheckpoint(cpIndex);

        // determine which character is inactive
        Character character = gardenerChar.active ? catChar : gardenerChar;
        // check if character is behind the wall
        if(character.transform.position.x < cp.transform.position.x)
        { // if it is move it
            Vector3 newPos = cp.transform.position;
            newPos.x -= 9.0f;
            newPos.y += 1.0f;
            character.transform.position = newPos;
        }
    }
    public void SpawnAtLastCheckpoint()
    {
        //Debug.Log("Moving to last checkpoint");
        int cpIndex = (int)playerData.CheckpointIndex;
        MoveTo(checkpoints[cpIndex]);
    }
    public void Save()
    {
        saveTimer = 0.0f;
        playerData.ExtractAndSave();
        //dataManager.Save(playerData, "Test");
    }
    public void Load()
    {
        playerData.LoadAndFill();
    }
    private void InitData()
    {
        saveTimer = 0.0f;
        dataManager = new DataManager();
        playerData = new Data(catChar, gardenerChar);
        //playerData.Test();
    }
    private void MoveTo(Checkpoint cp)
    {
        Vector3 cpPosition = cp.gameObject.transform.position;
        catChar.gameObject.transform.position = cpPosition;
        gardenerChar.gameObject.transform.position = cpPosition;
    }
}

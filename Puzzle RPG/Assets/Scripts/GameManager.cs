using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<Character> characters;
    private UIController controller;
    private DataManager dataManager;

    void Start()
    {
        controller = GetComponent<UIController>();
        InitData(); // only needs to be triggered when gameplay starts
    }

    void Update()
    {
        for(int i = 0; i < characters.Count; i++)
        {
            characters[i].active = (i == controller.activeCharIndex);
        }
    }

    private void InitData()
    {
        dataManager = new DataManager();
    }
}

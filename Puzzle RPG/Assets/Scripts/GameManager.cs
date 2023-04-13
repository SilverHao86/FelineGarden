using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<Character> characters;
    private UIController controller;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<UIController>();   
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < characters.Count; i++)
        {
            characters[i].active = (i == controller.activeCharIndex);
        }
    }
}

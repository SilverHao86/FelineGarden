using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CharacterCustomization : MonoBehaviour
{
    //actual prefab (should persist in level)
    [SerializeField] Gardener player;
    //gardener that displays on the customization screen to update in real time
    [SerializeField] Gardener displayGardener;

    [SerializeField] CharacterData data;
    int counter = 0;

    public void ChangeCharacter()
    {
        Debug.Log(counter);
        if(counter < data.characters.Length - 1)
        {
            data.currentCharacter = data.characters[counter += 1];
            player.GetComponent<Animator>().runtimeAnimatorController = data.currentCharacter;
            displayGardener.GetComponent<Animator>().runtimeAnimatorController = data.currentCharacter;
            Debug.Log(counter);
        }
        else
        {
            counter = 0;
            data.currentCharacter = data.characters[counter];
            player.GetComponent<Animator>().runtimeAnimatorController = data.currentCharacter;
            displayGardener.GetComponent<Animator>().runtimeAnimatorController = data.currentCharacter;
            Debug.Log(counter);
        }

    }

    public void ChangeCharacterLeft()
    {
        Debug.Log(counter);
        if (counter > 0)
        {
            data.currentCharacter = data.characters[counter -= 1];
            player.GetComponent<Animator>().runtimeAnimatorController = data.currentCharacter;
            displayGardener.GetComponent<Animator>().runtimeAnimatorController = data.currentCharacter;
            Debug.Log(counter);
        }
        else
        {
            counter = 2;
            data.currentCharacter = data.characters[counter];
            player.GetComponent<Animator>().runtimeAnimatorController = data.currentCharacter;
            displayGardener.GetComponent<Animator>().runtimeAnimatorController = data.currentCharacter;
            Debug.Log(counter);
        }

    }

    public void Play()
    {
        SceneManager.LoadScene("Graybox");
    }

    public void Menu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

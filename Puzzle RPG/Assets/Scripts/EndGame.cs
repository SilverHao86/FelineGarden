using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    [SerializeField] GameObject endScreen;
    [SerializeField] GameObject endScreenBackground;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnEndGame()
    {
        Time.timeScale = 0;
        endScreenBackground.SetActive(true);
        endScreen.SetActive(true);
    }

    public void ContinueGame()
    {
        Time.timeScale = 1;
        endScreenBackground.SetActive(false);
        endScreen.SetActive(false);
    }

    public void ReturnToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}

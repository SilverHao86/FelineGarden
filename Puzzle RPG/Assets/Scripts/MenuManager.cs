using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    #region Fields
    /// <summary>
    /// First slider shown on the options menu
    /// </summary>
    [SerializeField] private Scrollbar optionOneSlider;
    /// <summary>
    /// Landing Page Parent Object
    /// </summary>
    [SerializeField] private GameObject mainMenu;
    /// <summary>
    /// Options Page Parent Object
    /// </summary>
    [SerializeField] private GameObject optionMenu;
    /// <summary>
    /// Splash Image with a warning to the user
    /// </summary>
    //[SerializeField] private GameObject forewarning;
    /// <summary>
    /// Controls how fast the screen fades to transparency
    /// </summary>
    [SerializeField] private float fadeSpeed;
    /// <summary>
    /// SFX played when the play button is clicked
    /// </summary>
    [SerializeField] private AudioClip playSFX;
    /// <summary>
    /// SFX played when the exit button is pressed
    /// </summary>
    [SerializeField] private AudioClip exitSFX;
    /// <summary>
    /// SFX played when the options button is pressed
    /// </summary>
    [SerializeField] private AudioClip optionSFX;

    /// <summary>
    /// CanvasGroup of the initial warning screen
    /// </summary>
    private CanvasGroup canvas;
    /// <summary>
    /// Is the warning fading out
    /// </summary>
    private bool fadeOut;

    /// <summary>
    /// Audio player component on the manager object
    /// </summary>
    private AudioSource audioSource;
    #endregion Fields
    // Start is called before the first frame update
    void Start()
    {
        audioSource = this.gameObject.GetComponent<AudioSource>();
        //canvas = forewarning.GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        //if(fadeOut)
        //{
        //    canvas.alpha -= fadeSpeed * Time.deltaTime;
        //}
        //if(canvas.alpha <= 0)
        //{
        //    forewarning.SetActive(false);
        //}
    }

    /// <summary>
    /// Executes a change in value based on the first option slider being changed
    /// Current implementation changes the value of volume from the AudioListener
    /// </summary>
    public void OnOptionOneChange()
    {
        AudioListener.volume = optionOneSlider.value;
    }

    /// <summary>
    /// Event whenever the player clicks the Play button
    /// Plays a corresponding audio clip and goes to the desired level scene
    /// </summary>
    public void PlayButtonClicked()
    {
        audioSource.clip = playSFX;
        audioSource.Play();
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }

    /// <summary>
    /// Event whenever the player clicks the Exit button
    /// Plays a corresponding audio clip and then exits application
    /// </summary>
    public void ExitButtonClicked()
    {
        audioSource.clip = exitSFX;
        audioSource.Play();
        Application.Quit();
    }

    /// <summary>
    /// Event whenever the player clicks the Option or Back button
    /// Plays a corresponding audio clip and then swaps between the options page and the landing page
    /// </summary>
    public void OptionsButtonClicked()
    {
        audioSource.clip = optionSFX;
        audioSource.Play();
        mainMenu.SetActive(!mainMenu.activeInHierarchy);
        optionMenu.SetActive(!optionMenu.activeInHierarchy);
    }

    /// <summary>
    /// Fades out the warning screen to the main menu
    /// </summary>
    public void PreloadButtonClicked()
    {
        mainMenu.SetActive(true);
        audioSource.clip = optionSFX;
        audioSource.Play();
        fadeOut = true;
    }
}

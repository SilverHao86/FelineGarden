using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtBlock : MonoBehaviour
{
    public GameObject CrackAnimation;
    public GameObject TriggerBox;

    // Start is called before the first frame update
    void Start()
    {
        CrackAnimation.GetComponent<Animator>().enabled = false;
        TriggerBox.GetComponent<SpriteRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartDistroyingDirt()
    {
        CrackAnimation.GetComponent<Animator>().enabled = true;
    }
}

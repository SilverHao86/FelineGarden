using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cat : Character
{
    // Start is called before the first frame update
    private void Awake()
    {
        base.Awake();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        base.FixedUpdate();
    }

    // Make Sure the cant plant
    protected override void OnTriggerStay2D(UnityEngine.Collider2D collision)
    {
        
        

    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }
}

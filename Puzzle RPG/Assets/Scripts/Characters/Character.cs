using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] protected Camera cam;
    public bool active;
    public bool paused = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    protected void Update()
    {
        cam.transform.position = new Vector3(transform.position.x, transform.position.y, cam.transform.position.z);
        if(active && !paused)
        {
            // Tester Character Code
            if (Input.GetKey(KeyCode.D))
            {
                Debug.Log("movement");
                transform.position = new Vector3(transform.position.x + 0.01f, transform.position.y, transform.position.z);
            }
        }   
    }
}

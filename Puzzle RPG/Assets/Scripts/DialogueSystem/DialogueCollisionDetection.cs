using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueCollisionDetection : MonoBehaviour
{
    [SerializeField] bool triggerOnce = true;
    public bool alreadyTriggered = false;
    [SerializeField] DialogueObject dialogueToTrigger;
    
    public DialogueObject TriggerDialogue()
    {
        if (triggerOnce)
        {
            alreadyTriggered = true;
        }
        return dialogueToTrigger;
    }
    
}

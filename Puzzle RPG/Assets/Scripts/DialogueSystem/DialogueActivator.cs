using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueActivator : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueObject dialogueObject;

    protected void Awake()
    {
        this.gameObject.GetComponent<Character>().Interactable = this;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && collision.TryGetComponent(out Character player))
        {
            //player.Interactable = this;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.TryGetComponent(out Character player))
        {
            if(player.Interactable is DialogueActivator dialogueActivator && dialogueActivator == this)
            {
                //player.Interactable = null;
            }
        }
    }

    public void Interact(Character player, DialogueObject dialogue)
    {
        player.DialogueUI.ShowDialogue(dialogue);
    }


}

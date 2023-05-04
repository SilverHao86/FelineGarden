using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Character Type", menuName = "Character Data")]
public class CharacterData : ScriptableObject
{

    [field: SerializeField, Header("Movement Fields"), Space(5)] public float movementSpeed { get; private set; }
    [field: SerializeField] public float accelRate { get; private set; }
    [field: SerializeField] public float decelRate { get; private set; }
    [field: SerializeField] public float velocityPower { get; private set; }
    [field: SerializeField] public float frictionAmm { get; private set; }
    [field: SerializeField] public float jumpForce { get; private set; }
    [field: SerializeField] public float jumpCut { get; private set; }
    [field: SerializeField] public float fallMult { get; private set; }
    [field: SerializeField] public float jumpCooldown { get; private set; }

    [field: SerializeField, Header("Layer Fields"), Space(5)] public LayerMask groundLayer { get; private set; }
    [field: SerializeField] public LayerMask plotLayer { get; private set; }
    [field: SerializeField] public LayerMask beanstalkLayer { get; private set; }

    [field: SerializeField] public LayerMask boxLayer { get; private set; }
    [field: SerializeField] public LayerMask dirtLayer { get; private set; }


    [field: SerializeField, HideInInspector] public PlayerInput playerControls { get; set; }
    [field: SerializeField, HideInInspector] public bool isGrounded { get; set; }
    [field: SerializeField, HideInInspector] public bool isClimbing { get; set; }
    [field: SerializeField, HideInInspector] public bool canClimb { get; set; }

    


}

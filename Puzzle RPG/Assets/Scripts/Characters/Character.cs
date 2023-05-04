using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static PlantPlot;
//using static UnityEditor.Experimental.GraphView.GraphView;

[System.Serializable]
public class Character : MonoBehaviour
{
    [HideInInspector] public Camera Cam;
    public bool active;
    public bool paused = false;

    public PhysicsMaterial2D pushMat;
    public PhysicsMaterial2D cantPushMat;

    public CharacterData data;
    public Animator anim;

    private Rigidbody2D rb;

    [SerializeField] private DialogueUI dialogueUI;
    public DialogueUI DialogueUI => dialogueUI;

    public IInteractable Interactable { get; set; }

    private InputAction move;
    private InputAction jump;
    private InputAction swapCharacter;
    private InputAction plantPlant;
    private InputAction inventoryBinds;

    // Player Movement Floats
    // Values editable in Scriptable Object
    private float movementSpeed, accelRate, decelRate, velocityPower, frictionAmm, jumpForce, jumpCut, fallMult, jumpCooldown, currentCooldown;
    private LayerMask groundLayer;
    private LayerMask plotLayer;
    private LayerMask beanStalkLayer;
    private LayerMask boxLayer;
    private BoxCollider2D boxCollider;
    private bool canJump;
    private bool isOnTopOfStalk;

    private float pauseVineCollision;


    // Start is called before the first frame update
    protected void Awake()
    {
        // Setting up scriptable Object
        if (data.playerControls == null)
        {
            data.playerControls = new PlayerInput();
        }

        move = data.playerControls.Player.Move;
        jump = data.playerControls.Player.Jump;
        swapCharacter = data.playerControls.Player.SwapCharater;
        plantPlant = data.playerControls.Player.PlantOnPlot;
        inventoryBinds = data.playerControls.Player.Inventory;

        move.Enable();
        jump.Enable();
        swapCharacter.Enable();
        plantPlant.Enable();
        inventoryBinds.Enable();
        //inventoryBinds.performed += ctx =>
        //{
        //    InventoryController.instance.EquipIndex(this is Gardener ? 0 : 1, Mathf.RoundToInt(inventoryBinds.ReadValue<float>()));
        //};

        rb = this.gameObject.GetComponent<Rigidbody2D>();

        movementSpeed = data.movementSpeed;
        accelRate = data.accelRate;
        decelRate = data.decelRate;
        velocityPower = data.velocityPower;
        frictionAmm = data.frictionAmm;
        jumpForce = data.jumpForce;
        jumpCut = data.jumpCut;
        fallMult = data.fallMult;
        groundLayer = data.groundLayer;
        plotLayer = data.plotLayer;
        beanStalkLayer = data.beanstalkLayer;
        boxLayer = data.boxLayer;
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
        data.isGrounded = true;
        data.canClimb = false;
        data.isClimbing = false;
        canJump = false;
        jumpCooldown = data.jumpCooldown;
        currentCooldown = 0;
    }

    // Update is called once per frame
    protected void FixedUpdate()
    {
        CameraLogic();
        bool isOnGround = isOnLayerMask(groundLayer);
        //bool tryingToPushBox = isPushingBox();
        //Debug.Log(tryingToPushBox);
        bool isOnStalk = isOnLayerMask(beanStalkLayer);
        isOnGround = (isOnGround || isOnLayerMask(boxLayer));

        //if (active && (0.0f <= pauseVineCollision))
        //{
        //    pauseVineCollision -= Time.deltaTime;
        //    isOnStalk = false;
        //}

        HorizontalMovement(isOnGround /*, tryingToPushBox*/);
        VerticalMovement(isOnGround, isOnStalk);
        if (!jump.inProgress && active)
        {
            BeanStalkMovement(isOnStalk);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interactable?.Interact(this);
            Debug.Log("interacted start");
        }



    }

    protected void CameraLogic()
    {
        Cam.transform.position = new Vector3(transform.position.x, transform.position.y, Cam.transform.position.z);
    }

    protected void VerticalMovement(bool onGround, bool onStalk)
    {

        if (active && currentCooldown >= 0 && (onGround || onStalk || isOnTopOfStalk)) { currentCooldown -= Time.deltaTime; }

        if ((jump.inProgress) && (onGround || onStalk || isOnTopOfStalk) && active && rb.velocity.y == 0 && currentCooldown <= 0)
        {

            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            
            data.isGrounded = false;
            anim.SetBool("Running", false);
            //lizardAnimator.SetBool("Jumping", true);
            currentCooldown = jumpCooldown;
            if (onStalk)
            {
                //pauseVineCollision = 0.5f;
            }
        }

        // Apply Jumpcut
        if (rb.velocity.y > 0 && (!onGround && !isOnTopOfStalk)  && !(jump.inProgress) )
        {
            // jump cut should be 1-0
            rb.AddForce(Vector2.down * rb.velocity.y * (1 - jumpCut), ForceMode2D.Impulse);
        }

        // Apply Gravity
        if (!onGround)
        {
            rb.gravityScale = (rb.velocity.y < 0) ? fallMult : 1;
        }
    }

    protected void HorizontalMovement(bool onGround)
    {

        if (active)
        {
            
            if(isHittingSide() && !onGround) {
                //Debug.Log("hitting side");
                return; 
            }

            float speed = move.ReadValue<Vector2>().x * movementSpeed;
            // Speed difference
            float speedDif = speed - rb.velocity.x;

            if (Mathf.Abs(speed) > 0)
            {
                anim.SetBool("Running", true);
                if (speed > 0)
                {
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0f));
                    //this.transform.localScale = new Vector3(-this.transform.localScale.x, this.transform.localScale.y, this.transform.localScale.z);

                }
                else
                {
                    transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                    //this.transform.localScale = new Vector3(this.transform.localScale.x, this.transform.localScale.y, this.transform.localScale.z);

                }
            }
            else
            {
                anim.SetBool("Running", false);

            }
            float acceration = Mathf.Abs(speed);

            // Swtich acceleration based on direction
            acceration = (acceration > 0) ? accelRate : decelRate;

            float horizontalMovement = Mathf.Pow(Mathf.Abs(speedDif) * acceration, velocityPower) * Mathf.Sign(speedDif);

            

            rb.AddForce(horizontalMovement * Vector2.right);

            
        }
        

        // Still apply friction
        if (onGround && Mathf.Abs(move.ReadValue<Vector2>().x) < 0.0f)
        {
            float friction = Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(frictionAmm)) * Mathf.Sign(rb.velocity.x);

            rb.AddForce(Vector2.right * -friction, ForceMode2D.Impulse);
        }
        //Debug.Log(isPushingBox());

        //if (isPushingBox() && !data.canPushBox) { Debug.Log("cant push box"); rb.velocity = new Vector2(0, rb.velocity.y); }
    }

    // https://www.youtube.com/watch?v=Ln7nv-Y2tf4
    public void BeanStalkMovement(bool onStalk)
    {
        if (onStalk)
        {
            data.canClimb = true;
            // Hitting Up
            if(move.ReadValue<Vector2>().y != 0 || isAboveLayerMask(beanStalkLayer)) { data.isClimbing = true; }
            else {
                if(move.ReadValue<Vector2>().x != 0 && !isOnTopOfStalk) { data.isClimbing = false; } 
            }
        }
        else
        {
            data.isClimbing = false;
            data.canClimb = false;
            isOnTopOfStalk = false;
        }

        if(data.isClimbing)
        {
            if(isAboveLayerMask(beanStalkLayer))
            {
 
                rb.velocity = new Vector2(rb.velocity.x, move.ReadValue<Vector2>().y * movementSpeed);
            }
            else
            {
                isOnTopOfStalk = true;
                if(move.ReadValue<Vector2>().y < 0)
                {
                    rb.velocity = new Vector2(rb.velocity.x, move.ReadValue<Vector2>().y * movementSpeed);
                }
            }
            
            if (!jump.inProgress) rb.gravityScale = 0;
        }
        else
        {
            
            if (!jump.inProgress) rb.gravityScale = fallMult;
        }
    }

    public void ToggleMovement()
    {
        if (!active)
        {

            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            //rb.bodyType = RigidbodyType2D.Kinematic;
            //rb.Sleep();
            anim.SetBool("Running", false);
        }
        else
        {
            //rb.WakeUp();
            //rb.bodyType = RigidbodyType2D.Dynamic;
        }

        
    }

    public bool isHittingSide()
    {
        Vector2 direction =  (transform.eulerAngles.y == 180.0f) ? Vector2.left : Vector2.right;
      
        RaycastHit2D wallhit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, direction, 0.1f, boxLayer);
        RaycastHit2D boxhit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, direction, 0.1f, groundLayer);
        return wallhit.collider != null || boxhit.collider != null;

    }

    public bool isAboveLayerMask(LayerMask layer)
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.up, 0.1f, layer);
        return hit.collider != null;

    }

    public bool isOnLayerMask(LayerMask layer)
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, 0.1f, layer) ;
        return hit.collider != null;

    }



    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground")
        {
            // Need to fix this 
            //data.isGrounded = true;
            //currentCooldown = jumpCooldown;
            //anim.SetBool("isJumping", false);
            //anim.SetBool("isAscending", false);

        }
        if (collision.collider.tag == "BeanStalk")
        {
            // Need to fix this 
            //data.isGrounded = true;
            //currentCooldown = jumpCooldown;
            //anim.SetBool("isJumping", false);
            //anim.SetBool("isAscending", false);

        }

        if (collision.collider.tag == "PushableBox" && active)
        {
            Rigidbody2D boxRb = collision.gameObject.GetComponent<Rigidbody2D>();
            TryPushBoxHelper(boxRb);

        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "SeedBag" && active)
        {
            // Get the seed type
            InventoryController.instance.Add(collision.gameObject.GetComponent<ItemController>().item);

            // Need to add, add to inventory
            Destroy(collision.gameObject);




        }

        if(collision.gameObject.tag == "WaterPool")
        {
            Vector2 respawnPoint = collision.gameObject.GetComponent<Water>().GetRespawn();
            this.transform.position = new Vector3(respawnPoint.x,respawnPoint.y, transform.position.z);
        }
    }

    protected virtual void OnTriggerStay2D(UnityEngine.Collider2D collision)
    {
        if (collision.gameObject.tag == "PlantBase" && active)
        {
            //Debug.Log(plantPlant.inProgress);

            if (plantPlant.WasPerformedThisFrame() && !collision.gameObject.GetComponent<PlantPlot>().PlantActive && this is Gardener)
            {
                // Dont Cut the Plant if the knife isn't equiped
                int index = InventoryController.instance.equippedIndex[0];
                Item tempItem;
                try
                {
                    tempItem = InventoryController.instance.witchItems[index];
                    if (tempItem.itemName.Equals("Ring of Strength"))
                    {
                        Debug.Log(index);
                        return;
                    }
                }
                catch (Exception e)
                {
                    return;
                }

                collision.gameObject.GetComponent<PlantPlot>().PlantPlant();
            }

            if (plantPlant.IsPressed() && collision.gameObject.GetComponent<PlantPlot>().PlantActive && this is Cat)
            {
                collision.gameObject.GetComponent<PlantPlot>().CutPlant();
            }
        }

        
    }

    protected virtual void OnCollisionStay2D(UnityEngine.Collision2D collision)
    {
        if (collision.collider.tag == "PushableBox" && active)
        {
            Rigidbody2D boxRb = collision.gameObject.GetComponent<Rigidbody2D>();
            TryPushBoxHelper(boxRb);
        }


    }

    protected virtual void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground")
        {
            data.isGrounded = false;

        }

        if (collision.collider.tag == "PushableBox" && active)
        {
            Rigidbody2D boxRb = collision.gameObject.GetComponent<Rigidbody2D>();
            TryPushBoxHelper(boxRb);
            
        }
    }


    private void TryPushBoxHelper(Rigidbody2D boxRb)
    {
        bool canPush = false;
        if (this is Gardener)
        {
            // Dont Cut the Plant if the knife isn't equiped
            int index = InventoryController.instance.equippedIndex[0];
            Item tempItem;
            try
            {
                tempItem = InventoryController.instance.witchItems[index];
                if (tempItem.itemName.Equals("Ring of Strength"))
                {
                    canPush = true;
                    boxRb.WakeUp();
                    boxRb.bodyType = RigidbodyType2D.Dynamic;
                }
            }
            catch (Exception e)
            {
                boxRb.velocity = Vector2.zero;
                boxRb.angularVelocity = 0f;
                boxRb.bodyType = RigidbodyType2D.Kinematic;
                boxRb.Sleep();
                //return;
            }
        }
        if (!canPush)
        {
            boxRb.velocity = Vector2.zero;
            boxRb.angularVelocity = 0f;
            boxRb.bodyType = RigidbodyType2D.Kinematic;
            boxRb.Sleep();
        }
    }
}

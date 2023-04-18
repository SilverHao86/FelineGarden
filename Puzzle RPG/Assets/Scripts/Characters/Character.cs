using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    [HideInInspector] public Camera Cam;
    public bool active;
    public bool paused = false;


    public CharacterData data;
    public Animator anim;

    private Rigidbody2D rb;
    


    private InputAction move;
    private InputAction jump;
    private InputAction swapCharacter;
    private InputAction plantPlant;

    // Player Movement Floats
    // Values editable in Scriptable Object
    private float movementSpeed, accelRate, decelRate, velocityPower, frictionAmm, jumpForce, jumpCut, fallMult, jumpCooldown, currentCooldown;
    private LayerMask groundLayer;
    private LayerMask plotLayer;
    private LayerMask beanStalkLayer;
    private BoxCollider2D boxCollider;
    private bool canJump;

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

        move.Enable();
        jump.Enable();
        swapCharacter.Enable();
        plantPlant.Enable();

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
        bool isOnStalk = isOnLayerMask(beanStalkLayer);

        if (active && (0.0f <= pauseVineCollision))
        {
            pauseVineCollision -= Time.deltaTime;
            isOnStalk = false;
        }

        HorizontileMovement(isOnGround);
        VerticalMovement(isOnGround, isOnStalk);
        if (!jump.inProgress && active)
        {
            BeanStalkMovement(isOnStalk);
        }
        

        



    }

    protected void CameraLogic()
    {
        Cam.transform.position = new Vector3(transform.position.x, transform.position.y, Cam.transform.position.z);
        /*
        if(swapCharacter.WasReleasedThisFrame())
        {

        }
        */
    }

    protected void VerticalMovement(bool onGround, bool onStalk)
    {

        if (active && currentCooldown >= 0 && (onGround || onStalk)) { currentCooldown -= Time.deltaTime; }

        if ((jump.inProgress) && (onGround || onStalk) && active && rb.velocity.y == 0 && currentCooldown <= 0)
        {

            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            
            data.isGrounded = false;
            anim.SetBool("Running", false);
            //lizardAnimator.SetBool("Jumping", true);
            currentCooldown = jumpCooldown;
            if (onStalk)
            {
                pauseVineCollision = 1f;
            }
        }

        // Apply Jumpcut
        if (rb.velocity.y > 0 && !onGround && !(jump.inProgress) )
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

    protected void HorizontileMovement(bool onGround)
    {
        if (active)
        {
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

            float horizontileMovement = Mathf.Pow(Mathf.Abs(speedDif) * acceration, velocityPower) * Mathf.Sign(speedDif);

            rb.AddForce(horizontileMovement * Vector2.right);
        }

        // Still apply friction
        if (onGround && Mathf.Abs(move.ReadValue<Vector2>().x) < 0.0f)
        {
            float friction = Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(frictionAmm)) * Mathf.Sign(rb.velocity.x);

            rb.AddForce(Vector2.right * -friction, ForceMode2D.Impulse);
        }
    }

    // https://www.youtube.com/watch?v=Ln7nv-Y2tf4
    public void BeanStalkMovement(bool onStalk)
    {
        if (onStalk)
        {
            data.canClimb = true;
            // Hitting Up
            if(move.ReadValue<Vector2>().y > 0) { data.isClimbing = true; }
            else {
                if(move.ReadValue<Vector2>().x != 0) { data.isClimbing = false; } 
            }
        }
        else
        {
            data.isClimbing = false;
            data.canClimb = false;
        }

        if(data.isClimbing)
        {
            rb.velocity = new Vector2(rb.velocity.x, move.ReadValue<Vector2>().y * movementSpeed);
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
            if (isOnLayerMask(groundLayer) || isOnLayerMask(beanStalkLayer))
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.Sleep();

            }
        }
        else
        {
            rb.WakeUp();
            rb.bodyType = RigidbodyType2D.Dynamic;
        }

        
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
        

    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "SeedBag")
        {
            // Get the seed type
            collision.gameObject.GetComponent<SeedPickUp>();

            // Need to add, add to inventory
            Destroy(collision.gameObject);




        }
    }

    protected virtual void OnTriggerStay2D(UnityEngine.Collider2D collision)
    {
        if (collision.gameObject.tag == "PlantBase" && active)
        {
            //Debug.Log(plantPlant.inProgress);
            if (plantPlant.IsPressed() && !collision.gameObject.GetComponent<PlantPlot>().PlantActive)
            {
                collision.gameObject.gameObject.GetComponent<PlantPlot>().PlantPlant();
            }
        }

        
    }

    protected virtual void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground")
        {
            data.isGrounded = false;


        }
    }

}

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    private float movementSpeed, accelRate, decelRate, velocityPower, frictionAmm, jumpForce, jumpCut, fallMult, maxJumpSecs, jumpCoolMax;
    private LayerMask groundLayer;
    private LayerMask plotLayer;
    private BoxCollider2D boxCollider;


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
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
        data.isGrounded = true;
        data.canClimb = false;
        data.isClimbing = false;
    }

    // Update is called once per frame
    protected void FixedUpdate()
    {
        CameraLogic();
        HorizontileMovement();
        VerticalMovement();   
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

    protected void VerticalMovement()
    {
        bool isOnGround = isOnLayerMask(groundLayer);
        if ((jump.inProgress) && isOnGround && active)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            data.isGrounded = false;
            anim.SetBool("Running", false);
            //lizardAnimator.SetBool("Jumping", true);
        }

        // Apply Jumpcut
        if (rb.velocity.y > 0 && !isOnGround && !(jump.inProgress))
        {
            // jump cut should be 1-0
            rb.AddForce(Vector2.down * rb.velocity.y * (1 - jumpCut), ForceMode2D.Impulse);
        }

        // Apply Gravity
        if (!isOnGround)
        {
            rb.gravityScale = (rb.velocity.y < 0) ? fallMult : 1;
        }
    }

    protected void HorizontileMovement()
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
        if (isOnLayerMask(groundLayer) && Mathf.Abs(move.ReadValue<Vector2>().x) < 0.0f)
        {
            float friction = Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(frictionAmm)) * Mathf.Sign(rb.velocity.x);

            rb.AddForce(Vector2.right * -friction, ForceMode2D.Impulse);
        }
    }
    
    public void ToggleMovement()
    {
        if (!active)
        {
            if (isOnLayerMask(groundLayer))
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

            //anim.SetBool("isJumping", false);
            //anim.SetBool("isAscending", false);

        }

    }

    protected virtual void OnTriggerStay2D(UnityEngine.Collider2D collision)
    {
        if (collision.gameObject.tag == "PlantBase" && active)
        {
            Debug.Log(plantPlant.inProgress);
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

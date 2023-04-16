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

    // Player Movement Floats
    // Values editable in Scriptable Object
    private float movementSpeed, accelRate, decelRate, velocityPower, frictionAmm, jumpForce, jumpCut, fallMult, maxJumpSecs, jumpCoolMax;
    private LayerMask groundLayer;
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

        move.Enable();
        jump.Enable();
        swapCharacter.Enable();

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
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
        data.isGrounded = true;
    }

    // Update is called once per frame
    protected void FixedUpdate()
    {
        CameraLogic();
        if(active && !paused)
        {
            HorizontileMovement();
            VerticalMovement();
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

    protected void VerticalMovement()
    {
        if ((jump.inProgress) && isOnGround())
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            data.isGrounded = false;
            //lizardAnimator.SetBool("Running", false);
            //lizardAnimator.SetBool("Jumping", true);
        }

        // Apply Jumpcut
        if (rb.velocity.y > 0 && !isOnGround() && !(jump.inProgress))
        {
            // jump cut should be 1-0
            rb.AddForce(Vector2.down * rb.velocity.y * (1 - jumpCut), ForceMode2D.Impulse);
        }

        // Apply Gravity
        if (!data.isGrounded)
        {
            rb.gravityScale = (rb.velocity.y < 0) ? fallMult : 1;
        }
    }

    protected void HorizontileMovement()
    {
        float speed = move.ReadValue<Vector2>().x * movementSpeed;
        // Speed difference
        float speedDif = speed - rb.velocity.x;

        if (System.Math.Abs(speed) > 0)
        {
            //anim.SetBool("isWalking", true);
            if (speed > 0)
            {
                this.transform.localScale = new Vector3(5, 5, 5);

            }
            else
            {
                this.transform.localScale = new Vector3(-5, 5, 5);

            }
        }
        else
        {
            //anim.SetBool("isWalking", false);

        }
        float acceration = Mathf.Abs(speed);

        // Swtich acceleration based on direction
        acceration = (acceration > 0) ? accelRate : decelRate;

        float horizontileMovement = Mathf.Pow(Mathf.Abs(speedDif) * acceration, velocityPower) * Mathf.Sign(speedDif);

        rb.AddForce(horizontileMovement * Vector2.right);

        if (isOnGround() && Mathf.Abs(move.ReadValue<Vector2>().x) < 0.0f)
        {
            float friction = Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(frictionAmm)) * Mathf.Sign(rb.velocity.x);

            rb.AddForce(Vector2.right * -friction, ForceMode2D.Impulse);
        }
    }
    
    public void ToggleMovement()
    {
        if (!active)
        {
            if (isOnGround())
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.Sleep();

            }
        }
        else
        {
            rb.WakeUp();
        }

        
    }

    public bool isOnGround()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.transform.position, boxCollider.size, 0f, Vector2.down, 0.1f, groundLayer);
        return hit.collider != null;

    }


    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground")
        {
            // Need to fix this 
            data.isGrounded = true;

            //anim.SetBool("isJumping", false);
            //anim.SetBool("isAscending", false);

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

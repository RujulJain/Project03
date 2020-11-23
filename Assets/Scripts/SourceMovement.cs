using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourceMovement : MonoBehaviour
{
    #region //Variables 
    public CharacterController controller;
    public Transform GroundCheck;
    public LayerMask GroundMask;
    public Transform playerView;
    public Transform Player;

    public float moveSpeed = 7.0f;
    public float gravity = -20f;
    public float GroundDistance = 0.4f;
    public float runAcceleration = 14f;   // Ground accel
    public float runDeacceleration = 10f;   // Deacceleration that occurs when running on the ground
    public float airAcceleration = 2.0f;  // Air accel
    public float airDeacceleration = 2.0f;    // Deacceleration experienced when opposite strafing
    public float airControl = 0.3f;  // How precise air control is
    public float sideStrafeAcceleration = 50f;   // How fast acceleration occurs to get up to sideStrafeSpeed when side strafing
    public float sideStrafeSpeed = 1f;    // What the max speed to generate when side strafing
    public float jumpSpeed = 8.0f;
    public float friction = 6f;
    private float playerTopVelocity = 0;
    public float playerFriction = 0f;
    public float x;
    public float z;
    private float wishspeed;
    private float wishspeed2;
    float currentspeed;
    float addspeed;
    float accelspeed;
    float zspeed;
    float speed;
    float dot;
    float k;
    float accel;
    float newspeed;
    float control;
    float drop;

    bool isGrounded;
    public bool jumpBhop = false;
    public bool wishJump = false;

    public Vector3 moveDirection;
    public Vector3 moveDirectionNorm;
    private Vector3 velocity;
    Vector3 wishdir;
    Vector3 vec;
    Vector3 udp;
    #endregion

    /*private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }*/

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, GroundMask);

        BhopJump();

        //Movement part,
        //Checks if the player is on the ground or in air
        //Apply different methods depending on player state
        if (controller.isGrounded)
        {
            GroundMove();
        }
        else if (!controller.isGrounded)
        {
            AirMove();
        }

        //Move the controller
        controller.Move(velocity * Time.deltaTime);

        //Calculating the top velocity
        udp = velocity;
        udp.y = 0;
        if (udp.magnitude > playerTopVelocity)
        {
            playerTopVelocity = udp.magnitude;
        }
    }

    public void SetMovementDir()
    {
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");
    }
    
    //Checks if the player press space while in the air
    //Buffers a jump input for the player and jumps when touching the ground
    public void BhopJump()
    {
        if(jumpBhop)
        {
            wishJump = Input.GetKey(KeyCode.Space);
            return;
        }

        if(Input.GetKey(KeyCode.Space) && !wishJump)
        {
            wishJump = true;
        }
        if(Input.GetKeyUp(KeyCode.Space))
        {
            wishJump = false;
        }

    }

    //Allows the player to gain speed
    public void Accelerate(Vector3 wishdir, float wishspeed, float accel)
    {
        currentspeed = Vector3.Dot(velocity, wishdir);
        addspeed = wishspeed - currentspeed;
        if(addspeed <= 0)
        {
            return;
        }
        accelspeed = accel * Time.deltaTime * wishspeed;
        if (accelspeed > addspeed)
        {
            accelspeed = addspeed;
        }

        velocity.x += accelspeed * wishdir.x;
        velocity.z += accelspeed * wishdir.z;
    }

    //Called in Update()
    public void GroundMove()
    {
        //Check if player is trying to jumpBhop
        //Doesn't apply friction if jumpBhop
        if(!wishJump)
        {
            ApplyFriction(1.0f);
        }
        else
        {
            ApplyFriction(0);
        }

        SetMovementDir();

        wishdir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        wishdir = transform.TransformDirection(wishdir);
        wishdir.Normalize();
        moveDirectionNorm = wishdir;

        wishspeed = wishdir.magnitude;
        wishspeed *= moveSpeed;

        Accelerate(wishdir, wishspeed, runAcceleration);

        //Reset gravity vel
        velocity.y = 0;

        if(wishJump)
        {
            velocity.y = jumpSpeed;
            wishJump = false;
        }

        void ApplyFriction(float t)
        {
            vec = velocity; //copy the vector
            vec.y = 0f;
            speed = vec.magnitude;
            drop = 0f;

            //Checks if player isGrounded to apply friction
            if(controller.isGrounded)
            {
                if(speed < runDeacceleration)
                {
                    control = runDeacceleration;
                }
                else
                {
                    control = speed;
                }

                drop = control * friction * Time.deltaTime * t;
            }

            newspeed = speed - drop;
            playerFriction = newspeed;
            if(newspeed < 0)
            {
                newspeed = 0;
            }

            if(speed > 0)
            {
                newspeed /= speed;
            }

            velocity.x *= newspeed;
            velocity.z *= newspeed;
        }
    }

    public void AirMove()
    {
        SetMovementDir();

        wishdir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        wishdir = transform.TransformDirection(wishdir);

        wishspeed = wishdir.magnitude;

        wishspeed *= 7f;

        wishdir.Normalize();
        moveDirectionNorm = wishdir;

        wishspeed2 = wishspeed;
        if (Vector3.Dot(velocity, wishdir) < 0)
        {
            accel = airDeacceleration;
        }
        else
        {
            accel = airAcceleration;
        }

        //checks if player is strafing left or right only
        if(Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") != 0)
        {
            if(wishspeed > sideStrafeSpeed)
            {
                wishspeed = sideStrafeSpeed;
            }
            accel = sideStrafeAcceleration;
        }

        Accelerate(wishdir, wishspeed, accel);
        AirControl(wishdir, wishspeed2);

        //Apply gravity to player
        velocity.y += gravity * Time.deltaTime;

        void AirControl(Vector3 wishdir, float wishspeed)
        {
            //Player must be moving forward or backwards to air control
            if(Input.GetAxis("Horizontal") == 0 || wishspeed == 0)
            {
                return;
            }

            zspeed = velocity.y;
            velocity.y = 0;
            speed = velocity.magnitude;
            velocity.Normalize();

            dot = Vector3.Dot(velocity, wishdir);
            k = 32;
            k *= airControl * dot * dot * Time.deltaTime;

            if(dot > 0)
            {
                velocity.x = velocity.x * speed + wishdir.x * k;
                velocity.y = velocity.y * speed + wishdir.y * k;
                velocity.z = velocity.z * speed + wishdir.z * k;

                velocity.Normalize();
                moveDirectionNorm = velocity;
            }

            velocity.x *= speed;
            velocity.y = zspeed;
            velocity.z *= speed;
        }
    }
}

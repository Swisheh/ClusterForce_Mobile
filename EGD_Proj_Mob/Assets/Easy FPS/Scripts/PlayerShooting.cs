using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject player;
    public GameObject playerCamera;
    public GameObject canvas;
    public GameObject bulletSpawn;

    public float speed = 6f;            // The speed that the player will move at.
    public float turnSpeed = 100;

    Vector3 movement;                   // The vector to store the direction of the player's movement.
    //Animator anim;                      // Reference to the animator component.
    Rigidbody playerRigidbody;

    public float currentSpeed;
    public float accelerationSpeed = 50000f;
    public float deaccelerationSpeed = 15.0f;
    public int maxSpeed = 5;
    private Vector2 horizontalMovement;
    private Vector3 slowdownV;



    // Start is called before the first frame update
    void Awake()
    {
        //anim = GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void Update()
    {
        float h = 0; 
        float v = 0; 
        if (Input.GetKey(KeyCode.W))
        {
            v = 1f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            v = -1f;
        }

        if (Input.GetKey(KeyCode.A))
        {
            h = -1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            h = 1f;
        }

        // Move the player around the scene.
        Move(h, v);

        // Turn the player to face the mouse cursor.
        Turning();

        // Animate the player.
        //Animating(h, v);
    }

    void Move(float h, float v)
    {
        // Set the movement vector based on the axis input.
        //movement.Set(h, 0f, v);

        // Normalise the movement vector and make it proportional to the speed per second.
        //movement = movement.normalized * speed * Time.deltaTime;

        // Move the player to it's current position plus the movement.
        //playerRigidbody.MovePosition(transform.position + movement);
        //playerRigidbody.AddRelativeForce(h * speed * Time.deltaTime, 0, v * speed * Time.deltaTime);

        currentSpeed = playerRigidbody.velocity.magnitude;
        horizontalMovement = new Vector2(playerRigidbody.velocity.x, playerRigidbody.velocity.z);
        if (horizontalMovement.magnitude > maxSpeed)
        {
            horizontalMovement = horizontalMovement.normalized;
            horizontalMovement *= maxSpeed;
        }
        playerRigidbody.velocity = new Vector3(horizontalMovement.x, playerRigidbody.velocity.y, horizontalMovement.y);
        
        playerRigidbody.velocity = Vector3.SmoothDamp(playerRigidbody.velocity, new Vector3(0, playerRigidbody.velocity.y, 0), ref slowdownV, deaccelerationSpeed);
        playerRigidbody.AddRelativeForce(h * accelerationSpeed * Time.deltaTime, 0, v * accelerationSpeed * Time.deltaTime);

        if (h != 0 || v != 0)
        {
            deaccelerationSpeed = 0.5f;
        }
        else
        {
            deaccelerationSpeed = 0.1f;
        }
    }

    void Turning()
    {

        if (Input.GetKey(KeyCode.G))
        {
            float turn = -1f * (Time.deltaTime * turnSpeed);
            Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
            playerRigidbody.MoveRotation(playerRigidbody.rotation * turnRotation);
        }
        else if (Input.GetKey(KeyCode.H))
        {
            float turn = 1f * (Time.deltaTime * turnSpeed);
            Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
            playerRigidbody.MoveRotation(playerRigidbody.rotation * turnRotation);
        }
    }

    //void Animating(float h, float v)
    //{
    //    // Create a boolean that is true if either of the input axes is non-zero.
    //    bool walking = h != 0f || v != 0f;

    //    // Tell the animator whether or not the player is walking.
    //    anim.SetBool("IsWalking", walking);
    //}
}

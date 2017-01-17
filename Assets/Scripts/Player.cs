using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{

    public float jumpHeight = 4;
    public float timeToJumpApex = .4f;
    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;
    float moveSpeed = 6;

    float gravity;
    float jumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;

    //reference to controller
    Controller2D controller;


    void Start()
    {
        controller = GetComponent<Controller2D>();

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = -gravity * timeToJumpApex;
        print("Gravity:" + gravity + "Jump Velocity:" + jumpVelocity);

    }
    void Update()
    {
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetKeyDown(KeyCode.Space) && (controller.collisions.below || controller.collisions.above) )
        {
            jumpVelocity = -gravity * timeToJumpApex;
            velocity.y = jumpVelocity;
            Debug.Log("jump");
        }       

        //applies the gravity to the velocity on each frame
        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        Debug.Log(controller.collisions.below + "\n" + controller.collisions.above);
    }
    

    public void ReverseGravity()
    {
        gravity = gravity * -1;
        Vector3 newScale = transform.localScale;
        newScale.y *= -1;
        transform.localScale = newScale;
       
       
    }
}

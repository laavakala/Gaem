using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : RaycastController {

    float maxClimbAngle = 80;
    float maxDescentAngle = 80;

    public CollisionInfo collisions;

    //when the start method is called, first call Raycast Controller's start method then continues this start method
    public override void Start()
    {
        base.Start();
        collisions.faceDir = 1;
    }

    public void Move(Vector3 velocity, bool standingOnPlatform = false){
        UpdateRaycastOrigins();
        collisions.Reset();
        collisions.velocityOld = velocity;

        if (velocity.x != 0) {
            collisions.faceDir = (int)Mathf.Sign(velocity.x);
        }

            if (velocity.y < 0) {
            DescentSlope(ref velocity);
        }
        
        HorizontalCollisions(ref velocity);
        
        if (velocity.y != 0){
            VerticalCollisions(ref velocity);
        }
        transform.Translate(velocity);

        if (standingOnPlatform == true) {
            collisions.below = true;
        }
    }

    void HorizontalCollisions(ref Vector3 velocity)
    {
        float directionX = collisions.faceDir;
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        if(Mathf.Abs(velocity.x) < skinWidth) {
            rayLength = 2 * skinWidth;
        }

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 RayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            RayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(RayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(RayOrigin, Vector2.right * directionX * rayLength, Color.red);

            //if raycast hits something set y velocity equal the amount we have to move wtf no comments
            if (hit) {
                if(hit.distance == 0) {
                    continue;
                }
                //if raycast detecs slope on the ground execute void ClimbSlope. Cool stuff no idea
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && slopeAngle<= maxClimbAngle){
                    if (collisions.descendingSlope) {
                        collisions.descendingSlope = false;
                        velocity = collisions.velocityOld;
                    }
                    float distanceToSlopeStart = 0;
                    //When ClimbSlope method is called uses velocity x once player has actually reached he slope 
                    if (slopeAngle != collisions.slopeAngleOld){
                        distanceToSlopeStart = hit.distance-skinWidth;
                        velocity.x -= distanceToSlopeStart * directionX;
                    }
                    ClimbSlope(ref velocity, slopeAngle);
                    velocity.x+=distanceToSlopeStart*directionX;
                }

                if (!collisions.climbingSlope || slopeAngle > maxClimbAngle){
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;

                    //Fixes the funny jiggling movement on y axis
                    if (collisions.climbingSlope){
                        velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                    }

                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }
            }
        }
    }
    void VerticalCollisions(ref Vector3 velocity){
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++){
            Vector2 RayOrigin = (directionY==-1)?raycastOrigins.bottomLeft:raycastOrigins.topLeft;
            RayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(RayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(RayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if (hit){
                velocity.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                //fixes funny jiggling movement on x axis
                if (collisions.climbingSlope){
                    velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }
        }
        if (collisions.climbingSlope){
            float directionX = Mathf.Sign(velocity.x);
            rayLength = Mathf.Abs(velocity.x) + skinWidth;
            Vector2 rayOrigin = ((directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight) + Vector2.up * velocity.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if (hit) {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != collisions.slopeAngle){
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                }
            }
        }
    }
    //Enables the player move along the slopes smoothly. Also calculates new speed new velocity for x and y axis 
    void ClimbSlope(ref Vector3 velocity, float slopeAngle){
        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        //enable jumping on slope
        if(velocity.y <= climbVelocityY){
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
        }
        
    }

    void DescentSlope(ref Vector3 velocity){
        float directionX = Mathf.Sign(velocity.x);
        Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

        if (hit) {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle !=0 && slopeAngle <= maxDescentAngle){
                if (Mathf.Sign(hit.normal.x)== directionX){
                    if(hit.distance - skinWidth <= Mathf.Tan(slopeAngle*Mathf.Deg2Rad)* Mathf.Abs(velocity.x)){
                        float moveDistance = Mathf.Abs(velocity.x);
                        float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
                        velocity.y -= descendVelocityY;

                        collisions.slopeAngle = slopeAngle;
                        collisions.descendingSlope = true;
                        collisions.below = true;
                    }
                }
            }
        }
    }


    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public bool climbingSlope;
        public bool descendingSlope;
        public float slopeAngle, slopeAngleOld;
        public Vector3 velocityOld;

        public int faceDir; 

        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            descendingSlope = false;
           

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }


}

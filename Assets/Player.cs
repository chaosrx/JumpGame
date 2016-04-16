using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public float MaxSpeed = 10;
    public float Acceleration = 15;
    public float JumpSpeed = 8;
    public float JumpDuration;

    public bool EnableDoubleJump = true;

    public bool wallHitDoubleJumpOverride = true;

    //internal Checks
    bool canDoubleJump = true;
    float jmpDuration;
    bool jumpKeyDown = false;
    bool canVariableJump = false;

    public Renderer rend;
    public Rigidbody2D myBody;

	// Update is called once per frame
	void Update ()
    {
        float horizontal = Input.GetAxis("Horizontal");

        if(horizontal < -0.1f)
        {
            if(myBody.velocity.x > -this.MaxSpeed)
            {
                myBody.AddForce(new Vector2(-this.Acceleration, 0.0f));
            }
            else
            {
                myBody.velocity = new Vector2(-this.MaxSpeed, myBody.velocity.y);
            }
        }
        else if(horizontal > 0.1f)
        {
            if (myBody.velocity.x < this.MaxSpeed)
            {
                myBody.AddForce(new Vector2(this.Acceleration, 0.0f));
            }
            else
            {
                myBody.velocity = new Vector2(this.MaxSpeed, myBody.velocity.y);
            }
        }

        bool onTheGround = isOnGround();

        float vertical = Input.GetAxis("Vertical");

        if(onTheGround)
        {
            canDoubleJump = true;
        }

        if(vertical > 0.1f)
        {
            if(!jumpKeyDown)    // 1st frame
            {
                jumpKeyDown = true;
                
                if(onTheGround || (canDoubleJump && EnableDoubleJump || wallHitDoubleJumpOverride))
                {
                    bool wallHit = false;
                    int wallHitDirection = 0;

                    Debug.Log(wallHitDirection);

                    bool leftWallHit = isOnWallLeft();
                    bool rightWallHit = isOnWallRight();

                    if(horizontal != 0)
                    {
                        if(leftWallHit)
                        {
                            wallHit = true;
                            wallHitDirection = 1;
                        }
                        else if(rightWallHit)
                        {
                            wallHit = true;
                            wallHitDirection = -1;
                        }
                    }

                    Debug.Log(wallHitDirection);

                    if (!wallHit)
                    {
                        if(onTheGround || (canDoubleJump && EnableDoubleJump))
                        {
                            myBody.velocity = new Vector2(myBody.velocity.x, this.JumpSpeed);

                            jmpDuration = 0.0f;
                            canVariableJump = true;
                        }
                    }

                    else
                    {
                        myBody.velocity = new Vector2(this.JumpSpeed * wallHitDirection, this.JumpSpeed);

                        jmpDuration = 0.0f;
                        canVariableJump = true;
                    }

                    if(!onTheGround && !wallHit)
                    {
                        canDoubleJump = false;
                    }
                }
            }// 2nd frame
            else if(canVariableJump)
            {
                jmpDuration += Time.deltaTime;

                if (jmpDuration < this.JumpDuration * 0.001f)
                {
                    myBody.velocity = new Vector2(myBody.velocity.x, this.JumpSpeed);
                }
            }
        }
        else
        {
            jumpKeyDown = false;
            canVariableJump = false;
        }
	}

    private bool isOnGround()
    {
        float lenghtToSearch = 0.1f;
        float colliderThreshold = 0.001f;

        Vector2 linestart = new Vector2(this.transform.position.x, this.transform.position.y - rend.bounds.extents.y - colliderThreshold);

        Vector2 vectorToSearch = new Vector2(this.transform.position.x, linestart.y - lenghtToSearch);

        RaycastHit2D hit = Physics2D.Linecast(linestart, vectorToSearch);

        return hit;
    }

    private bool isOnWallLeft()
    {
        bool retVal = false;

        float lengthToSearch = 0.1f;
        float colliderThreshold = 0.01f;

        Vector2 linestart = new Vector2(this.transform.position.x - rend.bounds.extents.x - colliderThreshold, this.transform.position.y);
        Vector2 vectorToStart = new Vector2(linestart.x - lengthToSearch, this.transform.position.y);

        RaycastHit2D hitLeft = Physics2D.Linecast(linestart, vectorToStart);

        retVal = hitLeft;

        if (retVal)
        {
            if (hitLeft.collider.GetComponent<NoSlideJump>())
            {
                retVal = false;
            }
        }

        return retVal;
    }

    private bool isOnWallRight()
    {
        bool retVal = false;

        float lengthToSearch = 0.1f;
        float colliderThreshold = 0.01f;

        Vector2 linestart = new Vector2(this.transform.position.x + rend.bounds.extents.x + colliderThreshold, this.transform.position.y);
        Vector2 vectorToStart = new Vector2(linestart.x + lengthToSearch, this.transform.position.y);

        RaycastHit2D hitRight = Physics2D.Linecast(linestart, vectorToStart);

        retVal = hitRight;

        if (retVal)
        {
            if (hitRight.collider.GetComponent<NoSlideJump>())
            {
                retVal = false;
            }
        }

        return retVal;
    }
}

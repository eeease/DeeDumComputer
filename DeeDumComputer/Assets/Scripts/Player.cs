using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour {
    Rigidbody2D rb;
    public Vector2 velocity;
    public float speedH, maxDownVel, gravityDown, gravityUp, jumpVel, jumpVelMultiHeights, jumpPushCooldown, jumpContinueVelEarly, jumpContinueVelLate; //gravity is actually different when going down (it pulls you down faster than it allows you to go up)
    [SerializeField]
    public bool onTopOfPlatform, jumping, onTopOfPlayer;
    public float onPlatformTimerMax, jumpTimerMax, jumpHeldTimerMax;
    [SerializeField]
    float onPlatformTimer, jumpTimer, jumpHeldTimer;
    public bool multiJumpHeights; //do we want regular jump or fake double float jump
    public string playerName;
    public AudioSource jumpSFX, landSFX;
    public float rayDist, rayLength;
    public LayerMask layerMask, pLayerMask;
    public float slideTimer = 0.1f;
    public float slideTime = 0.1f;
    public bool sliding, buttonMoving, reanimate;
    public float reanimateTimer, reanimateTimerOG;
    ParticleSystem[] myDustParticles;
    public bool check1, check2, check3; //for purple player raycasting down to see if it's hitting something in its midpoint, left, and right sides
    public bool conveying, conveyL, conveyR;
    bool frozen, iceRespawn, antiGrav;
    GameObject iceBlock;
    public float frozenTimer, frozenTimerOG, iceRespawnTime, iceRespawnTimeOG;


    //controller check:
    public string xBoxX;

    [Header("Costume Vars")]
    public SpriteMask myCostumeMask;
    public Sprite[] costumes;


    // Use this for initialization
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        //jumpVelMultiHeights = 12.5f;//~~hard setting it in here because i already have 20+ scenes with the players in them.  this is more efficient (lf less variable)
        if (multiJumpHeights)
        {
            //jumpVel = jumpVelMultiHeights; //multiHeights is going to be a different val.
        }
        myDustParticles = GetComponentsInChildren<ParticleSystem>();
        ControllerManager.CM.ControllerCheck();//players should do a controller check when they come into existence to see what's controlling them.

        myCostumeMask = GetComponentInChildren<SpriteMask>();
        myCostumeMask.sprite = costumes[GameManager.GM.equippedCostume];
    }

    // Update is called once per frame
    void Update()
    {
        onPlatformTimer -= Time.deltaTime;
        jumpTimer -= Time.deltaTime;
        if (!buttonMoving && !GameManager.GM.lvlCompletePAnel.activeSelf && !GameManager.GM.GOPanel.activeSelf && !GameManager.GM.quitPanel.activeSelf)
        {
            velocity.x = Input.GetAxisRaw("Horizontal") * speedH; //move right/left //commenting this out for mobile
            if (xBoxX != string.Empty)
            {
                velocity.x = Input.GetAxisRaw("XHorizontal") * speedH; 

            }
        } //was too annoying to not be able to test with arrows.
        //if (Input.GetButtonDown("MoveLeft")) //setting these in inputmanager as arrows and d-pad
        //{
        //    MoveLeft();
        //}
        //if (Input.GetButtonUp("MoveLeft") && !buttonMoving)
        //{
        //    VelReset();
        //}
        //if (Input.GetButtonDown("MoveRight"))
        //{
        //    MoveRight();
        //}
        //if (Input.GetButtonUp("MoveRight") && !buttonMoving)
        //{
        //    VelReset();
        //}

        if (sliding)
        {
            slideTimer-=Time.deltaTime;
            if (slideTimer >= 0) //while the timer is above 0 (which should only be about a tenth of a second) freeze the x position.
            {
                rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation; //when players touch each other, they start sliding for some reason.  this will stop that for a second.
            }else
            {
                sliding = false;
            }
        }else
        {
            rb.constraints =  RigidbodyConstraints2D.FreezeRotation; //just lock the z axis if it's not 'sliding'

        }
        //add gravity:
        //if (velocity.y > -maxDownVel) velocity.y -= gravityDown*Time.deltaTime;

        if ((onTopOfPlatform || onTopOfPlayer) && (velocity.y <= 0 || antiGrav && velocity.y >= 0)) //hard set the yvel if you're going down. //
        {
            velocity.y = 0;
            onPlatformTimer = onPlatformTimerMax;
        }
        else if ((velocity.y > -maxDownVel) || (antiGrav && velocity.y< -maxDownVel))
        {
            if (velocity.y > 2 || (antiGrav && velocity.y < -2))
            {

                velocity.y -= gravityUp * Time.deltaTime;

                onTopOfPlatform = false; //just covering my bases here...
            }
            else if(velocity.y <=2 || (antiGrav && velocity.y>=-2))
            {
                velocity.y -= gravityDown * Time.deltaTime; //if you're holding J (distance joint, don't apply the gravity force.
            }
        }

        //jump logic:

        if (Input.GetButtonDown(xBoxX+"Jump"))
        {
            if (!jumping && onPlatformTimer > 0)
            {
                //GameManager.GM.LogStats(true, "jumps");
                //GameManager.GM.LogStats(false, "jumps");

                StartJump();
            }
            else
            {
                jumpTimer = jumpTimerMax;
            }
        }

        if (multiJumpHeights)
        {
            //early jump free push section
            if (jumpHeldTimer > jumpHeldTimerMax - jumpPushCooldown)
            {
                velocity.y += jumpContinueVelEarly * Time.deltaTime;
                jumpHeldTimer -= Time.deltaTime;
            }
            else if (Input.GetButton(xBoxX+"Jump") && jumpHeldTimer > 0)
            {
                velocity.y += jumpContinueVelLate * Time.deltaTime; //give another push
                jumpHeldTimer -= Time.deltaTime;
            }
            else
            {
                jumpHeldTimer = 0;
            }
        }
        

        //onTopOfPlatform = false; // I am morally against this and I think I'm going to raycast down instead and if it doesn't hit a platform, set onTop to be false.

        //ray stuff:
        Vector2 rayStart = transform.position;
        Debug.DrawRay(rayStart, Vector2.down * rayDist, Color.red);
        Debug.DrawRay(rayStart + new Vector2(.45f, 0), Vector2.down * rayDist, Color.cyan);
        Debug.DrawRay(rayStart - new Vector2(.45f, 0), Vector2.down * rayDist, Color.cyan);

        //Physics2D.Raycast(rayStart, Vector2.down, rayLength);
        //Debug.Log(Physics2D.Raycast(rayStart, Vector2.down, rayLength, layerMask).collider.tag);

        //THIS RAY WILL ONLY CHECK FOR PLATFORM COLLISIONS:
        //Check middle ray:
        if (Physics2D.Raycast(rayStart, Vector2.down, rayLength, layerMask).collider != null)
        {

            //|| Physics2D.Raycast(rayStart, Vector2.down, rayLength, layerMask).collider.tag == "Player"
            if (Physics2D.Raycast(rayStart, Vector2.down, rayLength, layerMask).collider.tag == "Platform")
            {
                GroundHittingEffects();
                onTopOfPlatform = true;
                if (GetComponent<Animator>() != null)
                {
                    // GetComponent<Animator>().SetBool("Jumping", false);
                }
            }
            else if (Physics2D.Raycast(rayStart, Vector2.down, rayLength, pLayerMask).collider.tag == "ConveyerBelt")
            {
                GroundHittingEffects();
                onTopOfPlatform = true;
                conveying = true;
            }

        }
        //Check right ray (for hanging):
        else if (Physics2D.Raycast(rayStart + new Vector2(.45f, 0), Vector2.down, rayLength, layerMask).collider != null)
        {
            if (Physics2D.Raycast(rayStart + new Vector2(.45f, 0), Vector2.down, rayLength, layerMask).collider.tag == "Platform")
            {
                GroundHittingEffects();
                onTopOfPlatform = true;
            }else if (Physics2D.Raycast(rayStart + new Vector2(.45f, 0), Vector2.down, rayLength, layerMask).collider.tag == "ConveyerBelt")
            {
                GroundHittingEffects();
                onTopOfPlatform = true;
                conveying = true;
            }

        }
        //Check left ray (for hanging):
        else if(Physics2D.Raycast(rayStart - new Vector2(.45f, 0), Vector2.down, rayLength, layerMask).collider != null)
        {
            if (Physics2D.Raycast(rayStart - new Vector2(.45f, 0), Vector2.down, rayLength, layerMask).collider.tag == "Platform")
            {
                GroundHittingEffects();
                onTopOfPlatform = true;
            }
            else if (Physics2D.Raycast(rayStart - new Vector2(.45f, 0), Vector2.down, rayLength, layerMask).collider.tag == "ConveyerBelt")
            {
                GroundHittingEffects();
                onTopOfPlatform = true;
                conveying = true;
            }
        }
        else
        {

            onTopOfPlatform = false;
            if (conveying)
            {
                conveying = false;
            }
        }

        //THIS ONE FOR PLAYER COLLISIONS, IN WHICH CASE ANIMATION SHOULD BE TURNED OFF, ETC.:
        //surprisingly, this mostly works.  The only issue here is that player2(purple) is casting its ray from its midpoint, which, when it's on top of p1, may not be colliding with p1.  need to cast another...?

        if (playerName == "Purple") //purple needs to cast additional rays downward to check if it's hitting on its ends
        {
            Debug.DrawRay((rayStart - new Vector2(0.8f,0)), Vector2.down * rayDist, Color.blue);
            //Debug.DrawRay((rayStart + new Vector2(0.8f,0)), Vector2.down * rayDist, Color.blue);

            if (Physics2D.Raycast((rayStart - new Vector2(0.8f, 0)), Vector2.down, rayLength, pLayerMask).collider != null) //a little to the left
            {
                if (Physics2D.Raycast((rayStart - new Vector2(0.8f, 0)), Vector2.down, rayLength, pLayerMask).collider.tag == "Player")
                {
                    foreach (GameObject player in GameManager.GM.players)
                    {
                        //player.GetComponent<Animator>().enabled = false; //no longer need to disable animator because it's just sprite swapping, not actually changing scale
                        player.GetComponent<Player>().reanimateTimer = reanimateTimerOG; //seems silly that i have to get it this way but it's the easiest way to get them both.
                        onTopOfPlayer = true;
                        check1 = true;
                    }

                }else if(Physics2D.Raycast((rayStart - new Vector2(0.8f, 0)), Vector2.down, rayLength, pLayerMask).collider.tag == "Platform")
                {
                    GroundHittingEffects();

                    onTopOfPlatform = true;

                }else if(Physics2D.Raycast((rayStart - new Vector2(0.8f, 0)), Vector2.down, rayLength, pLayerMask).collider.tag == "ConveyerBelt")
                {
                    GroundHittingEffects();
                    onTopOfPlatform = true;
                    conveying = true;
                }

            }
            else
            {
                check1 = false;
            }
            
            if (Physics2D.Raycast((rayStart + new Vector2(0.8f, 0)), Vector2.down, rayLength, pLayerMask).collider != null) //a little to the right
            {
                if (Physics2D.Raycast((rayStart + new Vector2(0.8f, 0)), Vector2.down, rayLength, pLayerMask).collider.tag == "Player")
                {
                    foreach (GameObject player in GameManager.GM.players)
                    {
                        //player.GetComponent<Animator>().enabled = false;
                        player.GetComponent<Player>().reanimateTimer = reanimateTimerOG; //seems silly that i have to get it this way but it's the easiest way to get them both.
                        onTopOfPlayer = true;
                        check2 = true;
                    }

                }
                else if (Physics2D.Raycast((rayStart + new Vector2(0.8f, 0)), Vector2.down, rayLength, pLayerMask).collider.tag == "Platform")
                {
                    GroundHittingEffects();

                    onTopOfPlatform = true;
                }
                else if (Physics2D.Raycast((rayStart + new Vector2(0.8f, 0)), Vector2.down, rayLength, pLayerMask).collider.tag == "ConveyerBelt")
                {
                    GroundHittingEffects();
                    onTopOfPlatform = true;
                    conveying = true;
                }

            }
            else
            {
                check2 = false;
            }

            if (Physics2D.Raycast(rayStart, Vector2.down, rayLength, pLayerMask).collider != null)
            {
                if (Physics2D.Raycast(rayStart, Vector2.down, rayLength, pLayerMask).collider.tag == "Player")
                {
                    foreach (GameObject player in GameManager.GM.players)
                    {
                        //player.GetComponent<Animator>().enabled = false;
                        player.GetComponent<Player>().reanimateTimer = reanimateTimerOG; //seems silly that i have to get it this way but it's the easiest way to get them both.
                        onTopOfPlayer = true;
                        check3 = true;
                    }

                }

            }
            else
            {
                check3 = false;
            }



            if (!check1 && !check2 && !check3)
            {
                reanimate = true;
                onTopOfPlayer = false;
            }

        }
        else
        {
            if (Physics2D.Raycast(rayStart, Vector2.down, rayLength, pLayerMask).collider != null)
            {
                if (Physics2D.Raycast(rayStart, Vector2.down, rayLength, pLayerMask).collider.tag == "Player")
                {
                    foreach (GameObject player in GameManager.GM.players)
                    {
                       // player.GetComponent<Animator>().enabled = false;
                        player.GetComponent<Player>().reanimateTimer = reanimateTimerOG; //seems silly that i have to get it this way but it's the easiest way to get them both.
                        onTopOfPlayer = true;

                    }

                }

            }
            else
            {
                reanimate = true;
                onTopOfPlayer = false;

            }
        }
        if (reanimate)
        {
            reanimateTimer -= Time.deltaTime;
            if (reanimateTimer <= 0)
            {
                foreach (GameObject player in GameManager.GM.players)
                {
                    player.GetComponent<Animator>().enabled = true;
                }
                reanimate = false;
            }
        }

        //if(Physics2D.Raycast(rayStart, Vector2.down, rayLength, layerMask).collider!= null)
        //{
        //    Debug.Log(Physics2D.Raycast(rayStart, Vector2.down, rayLength, layerMask).collider.tag);

        //}


        //if (Input.GetKey(KeyCode.UpArrow))
        //{
        //    GameManager.GM.ZoomOut(2);
        //}



        if (conveying)
        {
            if (Physics2D.Raycast(rayStart , Vector2.down, rayLength, pLayerMask).collider.gameObject.name == "ConveyerRight")
            {
                velocity.x = 2;
            }else
            {
                velocity.x = -2;
            }
        }
        if (iceRespawn) //do i want ice respawning?  it might be cool (and simple) to just have one ice block in a level.  you have to figure out exactly when to use it.  would cater less to experimentation but maybe more to restarts aka $$
        {
            iceRespawnTime -= Time.deltaTime;
            if (iceRespawnTime <= 0)
            {
                iceBlock.SetActive(true);
                iceRespawn = false;
            }
        }
        ////if (GameManager.GM.oneHandControls && !GameManager.GM.GOPanel.activeSelf) //if onehand controls are true AND if the pause canvas is off:
        ////{
        ////    if(Mathf.Abs(CrossPlatformInputManager.GetAxis("Horizontal"))>0 || Mathf.Abs(CrossPlatformInputManager.GetAxis("Vertical")) > 0)
        ////    {
        ////        buttonMoving = true; //set button moving to true if you're using joystick at all so the camera doesn't pan around.
        ////    }else

        ////    {
        ////        buttonMoving = false;
        ////    }
        ////    //Debug.Log(CrossPlatformInputManager.GetAxis("Horizontal"));
        ////    if (CrossPlatformInputManager.GetAxis("Horizontal") > .5)
        ////    {
        ////        velocity.x = speedH;
        ////    }else if (CrossPlatformInputManager.GetAxis("Horizontal") < -0.5)
        ////    {
        ////        velocity.x = -speedH;
        ////    }

        ////    if(GameObject.Find("MobileJoystick").GetComponent<Joystick>().holdingUp &&CrossPlatformInputManager.GetAxis("Vertical") < 0) //if holding up is true and you drop the joystick below halfway (0)...
        ////    {
        ////        GameObject.Find("MobileJoystick").GetComponent<Joystick>().holdingUpCount--;

        ////        GameObject.Find("MobileJoystick").GetComponent<Joystick>().holdingUp = false; //turn off holding up so that you can jump again without having to let go of the joystick
        ////    }
        ////    //jumping:
        ////    if (GameManager.GM.joystickControl.activeSelf &&!GameObject.Find("MobileJoystick").GetComponent<Joystick>().holdingUp)
        ////    {
        ////        if (CrossPlatformInputManager.GetAxis("Vertical") > .8) //this was .5 but it should probably be more extreme (.8?)
        ////        {
        ////            StartJump();
        ////            GameObject.Find("MobileJoystick").GetComponent<Joystick>().holdingUpCount++;
        ////            if (GameObject.Find("MobileJoystick").GetComponent<Joystick>().holdingUpCount < 2)
        ////            {
        ////                GameManager.GM.LogStats(true, "jumps");
        ////                GameManager.GM.LogStats(false, "jumps"); //!!~ this is probably not the correct way to be counting jumps (in the logstats function instead of the startjump function.
        ////            }

        ////        }
        ////        else if (CrossPlatformInputManager.GetAxis("Vertical") < 0) // <.5 was causing quick jump issues.
        ////        {
        ////            JumpVelReset();
        ////            //GameObject.Find("MobileJoystick").GetComponent<Joystick>().holdingUpCount = 0;
        ////            //GameObject.Find("MobileJoystick").GetComponent<Joystick>().holdingUp = false;
        ////        }
        ////    }
        ////    //velocity.x = CrossPlatformInputManager.GetAxis("Horizontal")*speedH;
        ////}

        if (!frozen)
        {
            rb.MovePosition(rb.position + velocity * Time.deltaTime); // MovePosition() is the best way to move a rb without using unity physics.
        }else
        {
            velocity.y = 0; //otherwise it's going to build up a ton of downward velocity due to not being on a platform
            frozenTimer -= Time.deltaTime;
            if (frozenTimer <= 0)
            {
                iceBlock.SetActive(false);
                frozen = false;
                frozenTimer = frozenTimerOG;
                iceRespawnTime = iceRespawnTimeOG;
                iceRespawn = true;
            }
        }
    }

    public void StartJump()
    {
            if (!jumping && onPlatformTimer > 0 && !GameManager.GM.lvlCompletePAnel.activeSelf && !GameManager.GM.GOPanel.activeSelf && !GameManager.GM.quitPanel.activeSelf)
            {
                if (GetComponent<Animator>() != null)
                {
                    //GetComponent<Animator>().SetBool("Jumping", true);
                }
                jumping = true;
                onTopOfPlatform = false;
                if (conveying)
                {
                    conveying = false;
                }

                velocity.y = jumpVel;
                if (!GameManager.GM.muteSFX)
                {
                    jumpSFX.Play(); //i'm hoping it's this simple
                }
                if (multiJumpHeights)
                {
                    jumpHeldTimer = jumpHeldTimerMax;
                }



            }
            else
            {
                jumpTimer = jumpTimerMax;
            }
        
    }
    //players triggering the goals:
    void OnTriggerEnter2D(Collider2D col)
    {
        //only want to fire the audiosources once, so OnEnter
        if (col.gameObject == GameManager.GM.goals[0] && this.playerName == "Green")
        {
            if (GameManager.GM.greenWarp.enabled) //not a huge deal but it's possibly inactive due to audio settings.  throws a minor error and it's pretty easy to avoid it.
            {
                GameManager.GM.greenWarp.Play();
            }
        }
        if (col.gameObject == GameManager.GM.goals[1] && this.playerName == "Purple")
        {
            if (GameManager.GM.purpleWarp.enabled)
            {
                GameManager.GM.purpleWarp.Play();
            }
        }

        if(col.tag == "IceZone")
        {
            iceBlock = col.gameObject; //set the ice block that you're stuck in (for later destruction)
            frozen = true;
            //also play a freeze sound effect:

        //it shoudl also display a popup sprite for visual cue that you're char is frozen.
        }
        if (col.tag == "AntiGrav")
        {
            AntiGravitate();
            //Debug.Log("AntiGravityyyy");
           
            //StartJump();
        }

        if(col.tag == "Coin")
        {
            Destroy(col.gameObject); //should probably do more than just destroy.
            GameManager.GM.gotCoin = true;
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject == GameManager.GM.goals[0] && this.playerName == "Green")
        {
            GameManager.GM.greenGo = true;
        }
        if (col.gameObject == GameManager.GM.goals[1] && this.playerName == "Purple")
        {
            GameManager.GM.purpleGo = true;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject == GameManager.GM.goals[0] && this.playerName == "Green")
        {
            GameManager.GM.greenGo = false;
            GameManager.GM.winning = false;
            GameManager.GM.greenWarp.Stop();

        }
        if (col.gameObject == GameManager.GM.goals[1] && this.playerName == "Purple")
        {
            GameManager.GM.purpleGo = false;
            GameManager.GM.winning = false;

            GameManager.GM.purpleWarp.Stop();

        }
    }
    
    void OnCollisionEnter2D(Collision2D collisionInfo)
    {
        if(collisionInfo.gameObject.tag == "Platform" || collisionInfo.gameObject.tag == "Player" || collisionInfo.gameObject.tag == "ConveyerBelt")
        {
            foreach(ContactPoint2D contact in collisionInfo.contacts)
            {
                if (Mathf.Abs(contact.normal.y) > Mathf.Abs(contact.normal.x)) //check if you hit the top/bottom of the platform.  if hitting the sides, don't do anything
                {
                    if (contact.normal.y >= 0 || (antiGrav && contact.normal.y <=0)) // if you're coming from the top of the platform, going down.  it's the normal of the platform beneath you
                    {
                        jumping = false;
                        if (!onTopOfPlatform) //only play this once, right before the onTopOfPlatform is set to true;
                        {
                            //landSFX.Play();  //~~this stuff isn't firing consistently because the raycasting is stopping collision.  going to have to try migrating it to that section...
                            //foreach (ParticleSystem part in myDustParticles)
                            //{
                            //    if (part.isEmitting)
                            //    {
                            //        part.Stop();
                            //    }
                            //    part.Emit(20);
                            //    Debug.Log("I'm hitting the ground and emitting my particles");
                            //}
                            //Debug.Log("just hit the ground");
                        }
                        onTopOfPlatform = true;
                        if (GetComponent<Animator>() != null)
                        {
                            //GetComponent<Animator>().SetBool("Jumping", false);
                        }
                        
                        if (jumpTimer > 0) StartJump(); //if you just start touching a platform from above and you pressed the button right before landing.

                    }else
                    {
                        if (!antiGrav)
                        {
                            velocity.y = 0; //if you're hitting from the bottom, hard set y
                        }
                    }
                }
            }
        }
        

        if(collisionInfo.gameObject.tag == "Respawn")
        {
            if (GameManager.GM.heartsRemaining > 0)
            {
                GameManager.GM.ResetSounds();
                //GameManager.GM.LoseALife(); //atm, this only happens when you restart a level, so this is being called in that function instead.

                GameManager.GM.RestartLevel();

            }else
            {
                GameManager.GM.RestartLevel();
                //GameManager.GM.ShowGOCanvas(); //this is happening in LoseALife() (which is called at end of RestartLevel().  don't need it to happen twice.
            }
        }

 
       
    }

    void OnCollisionStay2D(Collision2D collisionInfo)
    {
        if (collisionInfo.gameObject.tag == "Platform" || collisionInfo.gameObject.tag == "Player")
        {
            foreach (ContactPoint2D contact in collisionInfo.contacts)
            {
                if (Mathf.Abs(contact.normal.y) > Mathf.Abs(contact.normal.x)) //check if you hit the top/bottom of the platform.  if hitting the sides, don't do anything
                {
                    if (contact.normal.y >= 0) // if you're coming from the top of the platform, going down.  it's the normal of the platform beneath you
                    {
                        jumping = false;
                        //onTopOfPlatform = true;

                    }
                }
            }
        }

       
    }



    void OnCollisionExit2D(Collision2D collisionInfo)
    {
        //if (collisionInfo.gameObject.tag == "Platform")
        //{
        //    foreach (ContactPoint2D contact in collisionInfo.contacts)
        //    {
        //        if (Mathf.Abs(contact.normal.y) > Mathf.Abs(contact.normal.x)) //check if you hit the top/bottom of the platform.  if hitting the sides, don't do anything
        //        {
        //            if (contact.normal.y >= 0) // if you're coming from the top of the platform, going down.  it's the normal of the platform beneath you
        //            {

        //                onTopOfPlatform = false;
        //            }
        //        }
        //    }
        //}
        if (collisionInfo.gameObject.tag == "Player") //this will trigger when the shapes separate (because of the mysterious sliding...)
        {
            if (onTopOfPlayer)
            {
                onTopOfPlayer = false;
            }
            if (Mathf.Abs(velocity.y) == 0 && onTopOfPlatform && Mathf.Abs(velocity.x)==0) //this is causing an error while jumping so only do it if both players are on the ground.
            {
                slideTimer = slideTime;
                sliding = true;
            }
        }

    }

    public void MoveLeft()
    {
            velocity.x = -speedH;
            if (!buttonMoving)
            {
                buttonMoving = true;
            }
       
    }

    public void MoveRight()
    {
            velocity.x = speedH;
            if (!buttonMoving)
            {
                buttonMoving = true;
            }
       
    }

    public void VelReset()
    {
        velocity.x = 0;
        buttonMoving = false;
    }

    public void JumpVelReset()
    {
        if (velocity.y > 0)
        {
            velocity.y = velocity.y-2; //this is my faux way of making multijump heights with buttons.
            
        }
    }

    public void GroundHittingEffects()
    {
        if (!onTopOfPlatform && (velocity.y<0 || (antiGrav&&velocity.y>0))) //this is going to be called right before onTopOfPlatform is set to true. //only if the yvel is > 0 (it's going down)
        {
            if (!GameManager.GM.muteSFX)
            {
                landSFX.Play();
            }
            foreach (ParticleSystem part in myDustParticles)
            {
                if (part.isEmitting)
                {
                    part.Stop();
                }
                part.Emit(20);
                //Debug.Log("I'm hitting the ground and emitting my particles");
            }

        }
    }

    public void AntiGravitate()
    {
        antiGrav = true;
        velocity.y *= -1;
        jumpVel *= -1;
        jumpVelMultiHeights *= -1;
        gravityDown *= -1;
        gravityUp *= -1;
        rayDist *= -1;
        rayLength *= -1;
        maxDownVel *= -1;
        jumpContinueVelEarly *= -1;
        jumpContinueVelLate *= -1;

    }
}

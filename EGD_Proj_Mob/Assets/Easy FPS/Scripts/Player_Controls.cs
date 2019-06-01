using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using XInputDotNetPure;
using Photon.Pun;
using Photon.Realtime;
using LeoLuz.PlugAndPlayJoystick;


public class Player_Controls : MonoBehaviourPunCallbacks, IPunObservable
{
    public string fire;
    public string horizontal;
    public string vertical;
    public string turning;

    public AnalogicKnob left;
    public AnalogicKnob right;

    public PhotonView photonView;

    //bool playerIndexSet = false;
    //PlayerIndex playerIndex;
    //GamePadState state;
    //GamePadState prevState;

    float horizontalMove;
    float verticalMove;
    float shoot;
    float turningMove;

    public int health = 100;
    public int ammo = 20;

    private Text healthText;
    private Text ammoText;

    public GameObject canvas;
    public GameObject bulletSpawn;
    public GameObject bullet;
    public GameObject pointDisplay;
    public UI ui;
    private GameObject[] bulletUI = new GameObject[0];
    private GameObject bulletDisplay;

    public AudioSource hitSound;
    public AudioSource shootingSound;

    // For the DeathBonus function call
    private Points pointScript;
    private string whoDied;
    public bool gameOver = true;

    public GameObject[] gunFlash;
    private int currentBullet = 0;
    public GameObject gunSmoke;
    private bool fired = false;
    public GameObject blood;

    private float shootSpeed = 0.5f;
    private float maxShootSpeed = 0f;
    private float currentShootSpeed = 0.5f;
    private bool canShoot = true;

    public Animator anim;                      // Reference to the animator component.
    Animator gunBarrel;
    Rigidbody playerRigidbody;

    public bool forwardA = false;       // Animation bools
    public bool leftA = false;
    public bool rightA = false;
    public bool fireA = false;
    public bool deadA = false;

    private float turnSpeed = 100;
    private float currentSpeed;
    private float accelerationSpeed = 50000f;
    private float deaccelerationSpeed = 15.0f;
    private int maxSpeed = 5;
    private Vector2 horizontalMovement;
    private Vector3 slowdownV;

    // Start is called before the first frame update
    void Awake()
    {
        anim = this.transform.Find("character").GetComponent<Animator>();
        gunBarrel = this.transform.Find("Gatling_Gun/Barrels").GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
        healthText = canvas.transform.Find("Health").GetComponent<Text>();
        ammoText = canvas.transform.Find("Ammo").GetComponent<Text>();
        ui = canvas.transform.GetComponent<UI>();
        ui.SetUI();
        bulletDisplay = this.transform.Find("Canvas/Bullet1").gameObject;
        Bullets();
    
        // For the DeathBonus function call
        pointScript = GameObject.Find("Point System").GetComponent<Points>();
        whoDied = gameObject.name.ToString();
        SetPlayerControls();

        turnSpeed = pointScript.turnSpeed;
        shootSpeed = pointScript.startShootSpeed;
        maxShootSpeed = pointScript.incrementShootSpeed;
        accelerationSpeed = pointScript.playerSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver == false)
        {
            healthText.text = "Health: " + health.ToString();
            ammoText.text = "Ammo: " + ammo.ToString();

            // set up controller with XInput   
            //if (!playerIndexSet)
            //{
            //    if (whoDied == "Player 1")
            //    {
            //        PlayerIndex testPlayerIndex = PlayerIndex.One;
            //        GamePadState testState = GamePad.GetState(testPlayerIndex);

            //            Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
            //            playerIndex = testPlayerIndex;
            //            playerIndexSet = true;                        

            //    }
            //    else if (whoDied == "Player 2")
            //    {
            //        PlayerIndex testPlayerIndex = PlayerIndex.Two;
            //        GamePadState testState = GamePad.GetState(testPlayerIndex);

            //            Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
            //            playerIndex = testPlayerIndex;
            //            playerIndexSet = true;

            //    }
            //    else if (whoDied == "Player 3")
            //    {
            //        PlayerIndex testPlayerIndex = PlayerIndex.Three;
            //        GamePadState testState = GamePad.GetState(testPlayerIndex);

            //            Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
            //            playerIndex = testPlayerIndex;
            //            playerIndexSet = true;

            //    }
            //    else if (whoDied == "Player 4")
            //    {
            //        PlayerIndex testPlayerIndex = PlayerIndex.Four;
            //        GamePadState testState = GamePad.GetState(testPlayerIndex);

            //            Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
            //            playerIndex = testPlayerIndex;
            //            playerIndexSet = true;

            //    }
            //}

            //prevState = state;
            //state = GamePad.GetState(playerIndex);
            //if (state.IsConnected)
            //{
            //    horizontalMove = state.ThumbSticks.Left.X;
            //    verticalMove = state.ThumbSticks.Left.Y;
            //    shoot = state.Triggers.Right;
            //    turningMove = state.ThumbSticks.Right.X;
            //}
            //else
            //{
                horizontalMove = Input.GetAxis(horizontal);
                verticalMove = Input.GetAxis(vertical);
                shoot = Input.GetAxis(fire);
                turningMove = Input.GetAxis(turning);
            //}

            
                Move();

                // Turn the player to face the mouse cursor.
                Turning();

                // Shooting
                Shooting();

            Animating();
            // Move the player around the scene.
            
        }
    }

    void Bullets()
    {
        if(bulletUI.Length > 0)
        {
            for (int i = 0; i < bulletUI.Length; i++)
            {
                Destroy(bulletUI[i]);
            }
        }
        
        bulletUI = new GameObject[ammo];
        for (int i = 0; i < bulletUI.Length; i++)
        {
            bulletUI[i] = Instantiate(bulletDisplay);
            bulletUI[i].SetActive(true);
            bulletUI[i].transform.SetParent(canvas.transform);
            bulletUI[i].transform.rotation = bulletDisplay.transform.rotation;
            bulletUI[i].transform.localPosition = new Vector3((86.2f + ((float)i * 12)), -94.4f, 0);
            bulletUI[i].transform.localScale = new Vector3(0.1f, 0.3f, 1);
            bulletUI[i].name = "Bullet " + (i + 1);
            if (bulletUI.Length >= 10)
            {
                bulletUI[i].transform.localPosition = new Vector3(((86.2f + (((float)i * 12) / (bulletUI.Length)) * 9)), -94.4f, 0);
            }
        }
    }

    void Move()
    {
        currentSpeed = playerRigidbody.velocity.magnitude;
        horizontalMovement = new Vector2(playerRigidbody.velocity.x, playerRigidbody.velocity.z);
        if (horizontalMovement.magnitude > maxSpeed)
        {
            horizontalMovement = horizontalMovement.normalized;
            horizontalMovement *= maxSpeed;
        }
        playerRigidbody.velocity = new Vector3(horizontalMovement.x, playerRigidbody.velocity.y, horizontalMovement.y);

        playerRigidbody.velocity = Vector3.SmoothDamp(playerRigidbody.velocity, new Vector3(0, playerRigidbody.velocity.y, 0), ref slowdownV, deaccelerationSpeed);
        playerRigidbody.AddRelativeForce(horizontalMove * accelerationSpeed * Time.deltaTime, 0, verticalMove * accelerationSpeed * Time.deltaTime);
        //Debug.Log(state.ThumbSticks.Left.X);

        if (horizontalMove != 0 || verticalMove != 0)
        {
            deaccelerationSpeed = 0.5f;
        }
        else
        {
            deaccelerationSpeed = 0.1f;
        }

        if (verticalMove != 0)
        {
            //anim.SetBool("forward", true);
            //Debug.Log("Moving");
            forwardA = true;
        }
        else if (horizontalMove > 0)
        {
            //anim.SetTrigger("right");
            rightA = true;
        }
        else if (horizontalMove < 0)
        {
            //anim.SetTrigger("left");
            leftA = true;
        }
        else if (playerRigidbody.velocity == new Vector3(0, 0, 0))
        {
            //if (photonView.IsMine)
            //{
            //    Debug.Log(photonView.ViewID + " " + playerRigidbody.velocity);
            //}
            //anim.ResetTrigger("right");
            //anim.ResetTrigger("left");
            //anim.SetBool("forward", false);
            //photonView.RPC("RPCResetAnim", RpcTarget.All, "right");
            //RPCResetAnim("left");
            //RPCResetAnim("forward");
            forwardA = false;
            rightA = false;
            leftA = false;
        }
    }

    void Turning()
    {

        if (turningMove < -0.25f && turningMove < 0)
        {
            float turn = -1f * (Time.deltaTime * turnSpeed);
            Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
            playerRigidbody.MoveRotation(playerRigidbody.rotation * turnRotation);
        }
        else if (turningMove > 0.25f && turningMove > 0)
        {
            float turn = 1f * (Time.deltaTime * turnSpeed);
            Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
            playerRigidbody.MoveRotation(playerRigidbody.rotation * turnRotation);
        }
    }

    void Shooting()
    {
        if (shoot >= 0.5f && ammo > 0 && canShoot && health > 0)
        {
            StartCoroutine(Shoot(currentShootSpeed));
            gunBarrel.SetTrigger("shoot");
            //RPCAnimate(gunBarrel, "shoot");
            gunBarrel.speed = currentShootSpeed + 1;
            currentShootSpeed += maxShootSpeed;
            ammo = ammo - 1;
            shootingSound.Play();

            Bullets();
        }
        else if (shoot == 0)
        {
            currentShootSpeed = 0f;
            gunBarrel.ResetTrigger("shoot");
            //photonView.RPC("RPCResetAnim", RpcTarget.All, gunBarrel, "shoot");
            gunBarrel.speed = 1;
            if(fired == true)
            {
                Destroy(Instantiate(gunSmoke, bulletSpawn.transform.position, bulletSpawn.transform.rotation, this.transform), 2);
                fired = false;
            }            
        }
    }

    public IEnumerator Shoot(float currentShootSpeed)
    {
        GameObject newBullet = PhotonNetwork.Instantiate(bullet.name, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
        newBullet.name = bullet.name;
        newBullet.GetComponent<Bullet>().pointDisplay = pointDisplay;
        bullet.SetActive(true);
        fired = true;
        Destroy(Instantiate(gunFlash[currentBullet], bulletSpawn.transform.position, bulletSpawn.transform.rotation, this.transform), 0.5f); // cycles through muzzle flashes
        canShoot = false;
        if (shootSpeed - currentShootSpeed >= maxShootSpeed)
        {
            yield return new WaitForSeconds(shootSpeed - currentShootSpeed);
        }
        else
        {
            yield return new WaitForSeconds(maxShootSpeed);
        }
        canShoot = true;
        currentBullet++;
        if (currentBullet == gunFlash.Length)
        {
            currentBullet = 0;
        }
    }

    public void Animating()
    {
        if(forwardA)
        {
            anim.SetBool("forward", true);
        } 
        else if (leftA)
        {
            anim.SetBool("left", true);
        }
        else if (rightA)
        {
            anim.SetBool("right", true);
        }
        else
        {
            
            anim.SetBool("forward", false);
            anim.SetBool("left", false);
            anim.SetBool("right", false);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Bullets" && col.name != bullet.name)
        {
            health = health - pointScript.damagePerHit;
            ammo = ammo + pointScript.ammoPerHit;
            //photonView.RPC("GettingHit", RpcTarget.All);
            HealthBar();
            Destroy(Instantiate(blood, col.transform.position, col.transform.rotation, this.transform), 1);
            Bullets();
            //col.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer);
            Destroy(col.gameObject);
            hitSound.Play();
            HitFeedback();
            if(health == 0)
            {
                pointScript.DeathBonus(whoDied);
                //Debug.Log(whoDied);
                anim.SetTrigger("dead");
                this.GetComponent<Collider>().enabled = false; // so they cant get hit when dead
                this.GetComponent<Rigidbody>().isKinematic = true; // so they don't fall through the floor
                this.transform.Find("Main Camera").parent = this.transform.Find("character/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck"); //sets camera to neck for fall down animation
                this.transform.Find("Gatling_Gun").parent = this.transform.Find("character/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder/mixamorig:LeftArm/mixamorig:LeftForeArm/mixamorig:LeftHand"); // sets gun to the hand 
            }
        }
    }

    void HitFeedback()
    {
        GameObject hit = Instantiate(transform.Find("Canvas/HitFeedback").gameObject);
        hit.SetActive(true);
        hit.transform.SetParent(canvas.transform);
        hit.transform.localPosition = new Vector3(0, 0, 0);
        hit.transform.localRotation = new Quaternion(0, 0, 0, 0);
        hit.transform.localScale = new Vector3(1, 1, 0);
        Destroy(hit, 0.25f);
        StartCoroutine(Rumble());
    }

    public IEnumerator Rumble()
    {
        //if (state.IsConnected)
        //{
        //    GamePad.SetVibration(playerIndex, 0.25f, 0.25f);
            yield return new WaitForSeconds(0.15f);
        //    GamePad.SetVibration(playerIndex, 0, 0);
        //}        
    }

    void HealthBar()
    {
        Image healthBar = transform.Find("Canvas/Health Bar").GetComponent<Image>();
        healthBar.fillAmount = (float)health / 100;
    }

    public void SetPlayerControls()
    {
        Player[] playerList = PhotonNetwork.PlayerList;
        for (int i = 0; i < playerList.Length; i++) 
        {
            if (i+1 == photonView.ViewID)
            {
                //Debug.Log(photonView.ViewID);
                //photonView.TransferOwnership(playerList[i]);
                gameObject.GetComponent<PhotonView>().TransferOwnership(playerList[i]);
                if (playerList[i].IsLocal)
                {
                    left.HorizontalAxis = horizontal;
                    left.VerticalAxis = vertical;
                    right.HorizontalAxis = turning;
                    right.VerticalAxis = fire;
                    Camera cam = transform.Find("Main Camera").GetComponent<Camera>();
                    cam.rect = new Rect(0f, 0f, 1f, 1f);
                    cam = Camera.main;
                    cam.depth = 1;
                    Camera camUI = transform.Find("Main Camera/Camera").GetComponent<Camera>();
                    camUI.rect = new Rect(0f, 0f, 1f, 1f);
                    camUI.depth = 2;
                }
                
                //Debug.Log(horizontal + " " + photonView.ViewID);
                //Debug.Log(vertical + " " + photonView.ViewID);
            }
            else if (i+1 != photonView.ViewID && playerList[i].IsLocal)
            {
                //Debug.Log(playerList[i].ActorNumber + " " + photonView.ViewID);
                transform.Find("Main Camera").GetComponent<Camera>().gameObject.SetActive(false);
                //transform.Find("Main Camera/Camera").GetComponent<Camera>().gameObject.SetActive(false);
            }
        }
    }

    [PunRPC]
    public void RPCAnimate(string animation)
    {
        anim.SetTrigger(animation);
    }

    [PunRPC]
    public void RPCResetAnim(string animation)
    {
        anim.ResetTrigger(animation);
    }

    [PunRPC]
    public void GettingHit()
    {
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(health);
            stream.SendNext(ammo);
            stream.SendNext(forwardA);
            stream.SendNext(leftA);
            stream.SendNext(rightA);
            //Debug.Log("Sending");
        }
        else
        {
            health = (int)stream.ReceiveNext();
            ammo = (int)stream.ReceiveNext();
            forwardA = (bool)stream.ReceiveNext();
            leftA = (bool)stream.ReceiveNext();
            rightA = (bool)stream.ReceiveNext();
            //Debug.Log("Receive");
        }
    }
}

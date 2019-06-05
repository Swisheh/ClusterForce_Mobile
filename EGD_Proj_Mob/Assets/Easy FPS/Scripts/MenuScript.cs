using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

public class MenuScript : MonoBehaviourPunCallbacks, IPunObservable
{
    private AudioSource startSound;
    public GameObject startPanel;
    public Text startingCountdown;
    public Text instructions;
    public GameObject players4;
    public Button slowStart;
    public Button fastStart;
    public Button ready;

    private int timer = 5;
    private bool startCountdown = false;
    
    void Awake()
    {
        players4.transform.Find("Player 1").GetComponent<Player_Controls>().enabled = false;
        players4.transform.Find("Player 2").GetComponent<Player_Controls>().enabled = false;
        players4.transform.Find("Player 3").GetComponent<Player_Controls>().enabled = false;
        players4.transform.Find("Player 4").GetComponent<Player_Controls>().enabled = false;
        startSound = GameObject.Find("Start").GetComponent<AudioSource>();

        if(!PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            slowStart.gameObject.SetActive(false);
            fastStart.gameObject.SetActive(false);
            ready.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(startCountdown == true)
        {
            StartCoroutine(Timer());            
        } 
        
        if(timer <= 0)
        {
            start4players();
        }          
    }

    [PunRPC]
    public void start4players()
    {
        
            //startSound.Play();
            players4.transform.Find("Player 1").GetComponent<Player_Controls>().enabled = true;
            players4.transform.Find("Player 2").GetComponent<Player_Controls>().enabled = true;
            players4.transform.Find("Player 3").GetComponent<Player_Controls>().enabled = true;
            players4.transform.Find("Player 4").GetComponent<Player_Controls>().enabled = true;
            players4.transform.Find("Player 1").GetComponent<Player_Controls>().gameOver = false;
            players4.transform.Find("Player 2").GetComponent<Player_Controls>().gameOver = false;
            players4.transform.Find("Player 3").GetComponent<Player_Controls>().gameOver = false;
            players4.transform.Find("Player 4").GetComponent<Player_Controls>().gameOver = false;
            //mobileJoy.SetActive(true);
            this.gameObject.SetActive(false);
        
    }

    [PunRPC]
    public void gameStarting()
    {
        
            startCountdown = true;
            startPanel.SetActive(false);
            startingCountdown.gameObject.SetActive(true);
            instructions.gameObject.SetActive(true);
        
    }

    public void longStart()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("gameStarting", RpcTarget.All);
        }
    }

    public void shortStart()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("start4players", RpcTarget.All);
        }
    }

    public void ReadyUp()
    {

    }

    private IEnumerator Timer()
    {
        //Debug.Log(timer);
        startingCountdown.text = timer.ToString();
        if(timer == 5)
        {
            instructions.text = "Shoot Walls To Get Points";
        } else if (timer == 4)
        {
            instructions.text = "The Further It Travels, The More Points You Get";
        } else if (timer == 3)
        {
            instructions.text = "Get Shot To Get More Ammo";
        } else if (timer == 2)
        {
            instructions.text = "Dying Multiplies Your Score!";
        } else
        {
            instructions.text = "LETS GO!";
            instructions.fontSize = 100;
            instructions.color = Color.red;
        }

        startCountdown = false;
        yield return new WaitForSeconds(2f);
        timer--;
        startCountdown = true;
    }

    public void resetGame()
    {
        startSound.Play();
        SceneManager.LoadScene(0);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //stream.SendNext();
        }
        else
        {

        }
    }

}

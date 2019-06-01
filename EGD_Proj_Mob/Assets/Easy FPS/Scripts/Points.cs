using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

public class Points : MonoBehaviourPunCallbacks
{
    public GameObject p1;
    public GameObject p2;
    public GameObject p3;
    public GameObject p4;

    private Player_Controls p1Script;
    private Player_Controls p2Script;
    private Player_Controls p3Script;
    private Player_Controls p4Script;

    private float p1Points = 0.0f;
    private float p2Points = 0.0f;
    private float p3Points = 0.0f;
    private float p4Points = 0.0f;

    public float[] list;

    private GameObject first;
    private GameObject second;
    private GameObject third;
    private GameObject fourth;

    private float p1DeathBonus = 1;
    private float p2DeathBonus = 1;
    private float p3DeathBonus = 1;
    private float p4DeathBonus = 1;

    private GameObject gameOverCanvas;
    private int gameOverTimer = 5;
    private bool timer = true;
    public bool gameOver = false;
    public GameObject menuSpinner;

    public Text p1Text;
    public Text p2Text;
    public Text p3Text;
    public Text p4Text;

    // Points Variables
    public float bonus = 1.75f;
    public float decrease = 0.25f;

    public float bulletSpeed = 40f;
    public int pointsTravelled = 10;
    public double multiplier = 1;
    public int damagePerHit = 1;
    public int ammoPerHit = 1;

    public float turnSpeed = 100;
    public float playerSpeed = 400000f;
    public float startShootSpeed = 0.5f;
    public float incrementShootSpeed = 0.05f;

    // The P1_Control scripts sends a message to this script adding +1 to the number of deaths
    public int deaths = 0;

    // Start is called before the first frame update
    void Start()
    {
        p1Script = p1.GetComponent<Player_Controls>();
        p2Script = p2.GetComponent<Player_Controls>();
        p3Script = p3.GetComponent<Player_Controls>();
        p4Script = p4.GetComponent<Player_Controls>();
        gameOverCanvas = this.transform.Find("GameOverCanvas").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (deaths >= 3 || ((p1Script.ammo == 0 || p1Script.health == 0) && (p2Script.ammo == 0 || p2Script.health == 0) && (p3Script.ammo == 0 || p3Script.health == 0) && (p4Script.ammo == 0 || p4Script.health == 0)) || gameOver)
        {            
            // hard-coding the maximum value for now because list.Max() doesn't seem to be a thing
            if(timer == true)
            {
                StartCoroutine(GameOver());
            }
            gameOver = true;
            DisablePlayers();
            p1Script.anim.SetTrigger("gameover");
            p2Script.anim.SetTrigger("gameover");
            p3Script.anim.SetTrigger("gameover");
            p4Script.anim.SetTrigger("gameover");    
        }
    }

    private IEnumerator GameOver()
    {
        if (gameOverTimer <= 5 && gameOverTimer != 0)
        {
            menuSpinner.transform.parent = this.transform;
            timer = false;
            gameOverCanvas.SetActive(true);
            yield return new WaitForSeconds(1f);
            gameOverCanvas.transform.Find("Game Over Timer").GetComponent<Text>().text = gameOverTimer.ToString();
            gameOverCanvas.transform.Find("Game Over Timer (1)").GetComponent<Text>().text = gameOverTimer.ToString();

            gameOverTimer--;
            timer = true;
            if(gameOverTimer <= 2)
            {
                GameObject.Find("Winner").GetComponent<AudioSource>().Play();
            }
        }
        else if (gameOverTimer == 0)
        {
            if (timer == true)
            {                
                p1Points = p1Points * p1DeathBonus;
                p2Points = p2Points * p2DeathBonus;
                p3Points = p3Points * p3DeathBonus;
                p4Points = p4Points * p4DeathBonus;
                list = new float[] {p1Points, p2Points, p3Points, p4Points};
                //Debug.Log(list);
                Array.Sort(list);
                first = GetPlaces(list[3]);
                second = GetPlaces(list[2]);
                third = GetPlaces(list[1]);
                fourth = GetPlaces(list[0]);
                timer = false;
                //Debug.Log(p1Points / p1DeathBonus);
                //Debug.Log(p1Points);
                //Debug.Log(p1DeathBonus);
            }
            yield return new WaitForSeconds(1f);
            gameOverCanvas.SetActive(false);
            VictoryCamera(first);
            gameOverTimer--;
        }       
    }

    private GameObject GetPlaces(float score)
    {
        if (score == p1Points)
        {
            return GameObject.Find("Player 1");
        }
        else if (score == p2Points)
        {
            return GameObject.Find("Player 2");
        }
        else if (score == p3Points)
        {
            return GameObject.Find("Player 3");
        }
        else // p4 wins
        {
            return GameObject.Find("Player 4");
        }
    }

    private void VictoryCamera(GameObject winner)
    {
        // Then activates the victory camera of the player who won and the canvas for the final scores 
        menuSpinner.SetActive(false);   
        GameObject camera = this.transform.Find("Camera Pivot").gameObject;
        camera.SetActive(true);
        camera.transform.position = first.transform.position;  
        GameObject canvas = this.transform.Find("Canvas").gameObject;
        canvas.SetActive(true);
        canvas.transform.Find("First Place Score").GetComponent<Text>().text = PlayerName(first.name) + " " + list[3];
        canvas.transform.Find("First Place Score").GetComponent<Text>().color = first.GetComponent<MeshRenderer>().material.color;
        canvas.transform.Find("Second Place Score").GetComponent<Text>().text = PlayerName(second.name) + " " + list[2];
        canvas.transform.Find("Second Place Score").GetComponent<Text>().color = second.GetComponent<MeshRenderer>().material.color;
        canvas.transform.Find("Third Place Score").GetComponent<Text>().text = PlayerName(third.name) + " " + list[1];
        canvas.transform.Find("Third Place Score").GetComponent<Text>().color = third.GetComponent<MeshRenderer>().material.color;
        canvas.transform.Find("Fourth Place Score").GetComponent<Text>().text = PlayerName(fourth.name) + " " + list[0];
        canvas.transform.Find("Fourth Place Score").GetComponent<Text>().color = fourth.GetComponent<MeshRenderer>().material.color;
    }

    [PunRPC]
    public void GivePoints(int points, string whichPlayer)
    {
        Debug.Log(points + " " + whichPlayer);
        if (whichPlayer == "P1_Bullet(Clone)")
        {
            p1Points = p1Points + points;
            p1Text.text = "" + p1Points.ToString();
        }

        if (whichPlayer == "P2_Bullet(Clone)")
        {
            p2Points = p2Points + points;
            p2Text.text = "" + p2Points.ToString();
        }

        if (whichPlayer == "P3_Bullet(Clone)")
        {
            p3Points = p3Points + points;
            p3Text.text = "" + p3Points.ToString();
        }

        if (whichPlayer == "P4_Bullet(Clone)")
        {
            p4Points = p4Points + points;
            p4Text.text = "" + p4Points.ToString();
        }
    }

    public void DeathBonus(string whichPlayer)
    {
        deaths++;
        if (whichPlayer == "Player 1")
        {
            p1DeathBonus = bonus;
            //Debug.Log(p1Points + "p1");
        }
        else if (whichPlayer == "Player 2")
        {
            p2DeathBonus = bonus;
            //Debug.Log(p2Points + "p2");
        }
        else if (whichPlayer == "Player 3")
        {
            p3DeathBonus = bonus;
            //Debug.Log(p3Points + "p3");
        }
        else if (whichPlayer == "Player 4")
        {
            p4DeathBonus = bonus;
            //Debug.Log(p4Points + "p4");
        }
        bonus = bonus - decrease;
    }
    
    public void DisablePlayers()
    {
        p1Script.gameOver = true;
        p2Script.gameOver = true;
        p3Script.gameOver = true;
        p4Script.gameOver = true;
    }

    public string PlayerName(string name)
    {
        Player[] players = PhotonNetwork.PlayerList;
        switch (name)
        {
            case "Player 1":                
                if (players.Length > 0)
                {
                    if (!string.IsNullOrEmpty(players[0].NickName))
                    {
                        return PhotonNetwork.PlayerList[0].NickName;
                    }
                    else
                    {
                        return "Player 1";
                    }
                }                
                break;
            case "Player 2":
                if (players.Length > 1)
                {
                    if (!string.IsNullOrEmpty(players[1].NickName))
                    {
                        return PhotonNetwork.PlayerList[1].NickName;
                    }
                    else
                    {
                        return "Player 2";
                    }
                }                
                break;
            case "Player 3":
                if (players.Length > 2)
                {
                    if (!string.IsNullOrEmpty(players[2].NickName))
                    {
                        return PhotonNetwork.PlayerList[2].NickName;
                    }
                    else
                    {
                        return "Player 3";
                    }
                }                
                break;
            case "Player 4":
                if (players.Length > 3)
                { 
                    if (!string.IsNullOrEmpty(players[3].NickName))
                    {
                        return PhotonNetwork.PlayerList[3].NickName;
                    }
                    else
                    {
                        return "Player 4";
                    }
                }
                break;
        }
        return name;
    }
}

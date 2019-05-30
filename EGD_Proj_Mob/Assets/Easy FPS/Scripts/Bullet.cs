using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;


public class Bullet : MonoBehaviour
{
    public GameObject pointDisplay;
    public float speed = 40f;

    public int pointsTravelled = 10;
    public double multiplier = 1; // Starting number which is multiplied at the end
    public double increment = 0.1; // increases by this much per 0.1sec

    private Points pointScript;

    private string whoShot;

    private bool timer = false;
    
    void Awake()
    {
        //this.gameObject.SetActive(true);
        
        pointScript = GameObject.Find("Point System").GetComponent<Points>();
        pointsTravelled = pointScript.pointsTravelled;
        increment = pointScript.multiplier;
        speed = pointScript.bulletSpeed;

        whoShot = gameObject.name.ToString();
        //Debug.Log(whoShot);
    }

    // Update is called once per frame
    void Update()
    {
        // a timer would measure how far the bullet has travelled and then set that as pointsTravelled
        if (timer == false)
        {
            StartCoroutine(Travel());
        }

        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        if(pointScript.gameOver == true)
        {
            speed = 22f;
        }
    }

    public IEnumerator Travel()
    {
        timer = true;
        yield return new WaitForSeconds(0.1f);
        multiplier = multiplier + increment;
        timer = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player" && other.tag != "Bullets")
        {
            //Debug.Log(pointsTravelled * multiplier + " " + whoShot);
            PhotonView photonView = PhotonView.Get(this);
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("Points", RpcTarget.All, multiplier);
            }            
        }
    }

    [PunRPC]
    public void Points(double multi)
    {        
        int points = Mathf.RoundToInt((float)pointsTravelled * ((float)multi * (float)multi));
        pointScript.GivePoints(points, whoShot);
        //Debug.Log(whoShot);
        //Debug.Log(other);    
        GameObject pointsDisplay = Instantiate(pointDisplay, this.transform.position, this.transform.rotation);
        Text text = pointsDisplay.transform.Find("PointDisplayCanvas/Text").GetComponent<Text>();
        text.text = "+" + points;
        text.fontSize = 2 * (int)multi;
        text.color = this.GetComponent<MeshRenderer>().material.color;
        Destroy(pointsDisplay.gameObject, 1f);
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    void LoadArena()
    {
        if(!PhotonNetwork.IsMasterClient)
        {

        }


        PhotonNetwork.LoadLevel("_scene");
    }

    public void Ready()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("_scene");
        }
    }


    public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.Log("player connected");

        if (PhotonNetwork.IsMasterClient)
        {
            //Debug.Log("master client");
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("player left");

        if (PhotonNetwork.IsMasterClient)
        {
            //Debug.Log("master client left");
        }
    }
}

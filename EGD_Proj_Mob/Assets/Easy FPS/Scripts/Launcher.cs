using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;


namespace Com.collective.timclanceys
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        [Tooltip("Max Room Players")]
        [SerializeField]
        private byte maxPlayersPerRoom = 4;
        [Tooltip("UI that tells players its connecting")]
        [SerializeField]
        private Text progressLabel;
        [Tooltip("Room #")]
        [SerializeField]
        private Text room;

        private bool isConnecting;
        #region Private Serializable Fields

        #endregion

        #region Private Fields

        string gameVersion = "1";

        #endregion

        #region MonoBehaviour CallBacks

        void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;    
        }
        // Start is called before the first frame update
        void Start()
        {
            progressLabel.gameObject.SetActive(false);
        }

        #endregion

        #region Public Methods

        public void Connect()
        {
            isConnecting = true;
            progressLabel.gameObject.SetActive(true);

            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.JoinRoom("XYZ");
            }
            else
            {
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        #endregion

        #region MonoBehaviourPunCallbacks Callbacks

        public override void OnConnectedToMaster()
        {
            if (isConnecting)
            {
                PhotonNetwork.JoinRoom("XYZ");
            }
            
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            progressLabel.text = "Connecting...";
            progressLabel.gameObject.SetActive(false);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("new room created");
            Debug.Log(message);

            PhotonNetwork.CreateRoom("XYZ", new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log("new XYZ created");
            //Debug.Log(message);

            PhotonNetwork.CreateRoom("XYZ", new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("entered room");
            progressLabel.text = "Connected";
            room.text = "Room: " + PhotonNetwork.CurrentRoom.Name;

            foreach (Player player in PhotonNetwork.PlayerList)
            {
                Debug.Log(player.ActorNumber);
            }
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("is master");
            }
            
        }

        #endregion


        // Update is called once per frame
        //void Update()
        //{

        //}
    }
}

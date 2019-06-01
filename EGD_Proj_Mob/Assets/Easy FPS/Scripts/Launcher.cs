using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using System;


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
        [Tooltip("Room Field")]
        [SerializeField]
        private InputField roomField;
        [Tooltip("Player 1 Field")]
        [SerializeField]
        private Text player1Field;
        [Tooltip("Player 2 Field")]
        [SerializeField]
        private Text player2Field;
        [Tooltip("Player 3 Field")]
        [SerializeField]
        private Text player3Field;
        [Tooltip("Player 4 Field")]
        [SerializeField]
        private Text player4Field;
        [Tooltip("Random Button")]
        [SerializeField]
        private Button randomButton;
        [Tooltip("Connect Button")]
        [SerializeField]
        private Button connectButton;
        [Tooltip("Leave Button")]
        [SerializeField]
        private Button leaveButton;
        [Tooltip("Ready Button")]
        [SerializeField]
        private Button readyButton;

        private bool isConnecting;
        private bool isRandom;
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
            leaveButton.interactable = false;
            readyButton.interactable = false;
            if (PhotonNetwork.InRoom)
            {
                progressLabel.gameObject.SetActive(true);
                progressLabel.text = "Connected";
                room.text = "Room: " + PhotonNetwork.CurrentRoom.Name;
                OnJoinedRoom();
            }
        }

        #endregion

        #region Public Methods

        public void Connect()
        {
            isConnecting = true;
            progressLabel.gameObject.SetActive(true);

            if (PhotonNetwork.IsConnected)
            {
                if (!string.IsNullOrEmpty(roomField.text))
                {
                    if(OnlyLetters(roomField.text))
                    {
                        PhotonNetwork.JoinRoom(roomField.text);
                    }
                }
                else
                {
                    PhotonNetwork.JoinRoom("XYZ");
                }
                leaveButton.interactable = true;
            }
            else
            {
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        public void Random()
        {
            isRandom = true;
            progressLabel.gameObject.SetActive(true);

            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        public string RandomLetters()
        {
            string letters = "";
            System.Random rand = new System.Random();
            for (int i = 0; i < 3; i++)
            {
                char letter = (char)('A' + rand.Next(0, 26));
                letters = letters + letter;
            }
            //Debug.Log(letters);
            return letters;
        }

        public bool OnlyLetters(string letters)
        {
            foreach (char letter in letters)
            {
                if(!char.IsLetter(letter))
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region MonoBehaviourPunCallbacks Callbacks

        public override void OnConnectedToMaster()
        {
            if (isConnecting)
            {
                if (!string.IsNullOrEmpty(roomField.text))
                {
                    if (OnlyLetters(roomField.text))
                    {
                        PhotonNetwork.JoinRoom(roomField.text);
                    }
                }
                else
                {
                    PhotonNetwork.JoinRoom("XYZ");
                }
            }
            else if (isRandom)
            {
                PhotonNetwork.JoinRandomRoom();
            }
           
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            progressLabel.text = "Connecting...";
            progressLabel.gameObject.SetActive(false);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            //Debug.Log("new room created");
            //Debug.Log(message);
            PhotonNetwork.CreateRoom(RandomLetters(), new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            //Debug.Log("new XYZ created");
            //Debug.Log(message);
            if (!string.IsNullOrEmpty(roomField.text))
            {
                if (OnlyLetters(roomField.text))
                {
                    PhotonNetwork.CreateRoom(roomField.text, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
                }
            }
            else
            {
                PhotonNetwork.CreateRoom("XYZ", new RoomOptions { MaxPlayers = maxPlayersPerRoom });
            }
        }

        public override void OnJoinedRoom()
        {
            if (PhotonNetwork.IsConnected)
            {
                randomButton.interactable = false;
                connectButton.interactable = false;
                leaveButton.interactable = true;
                if(PhotonNetwork.IsMasterClient)
                {
                    readyButton.interactable = true;
                }

                //Debug.Log("entered room");
                progressLabel.text = "Connected";
                room.text = "Room: " + PhotonNetwork.CurrentRoom.Name;

                Player[] players = PhotonNetwork.PlayerList;
                for (int i = 0; i < players.Length; i++)
                {
                    switch (i)
                    {
                        case 0:
                            SetName(player1Field, players[i], i + 1);
                            if (players[i].IsLocal)
                            {
                                if(players[i].IsMasterClient)
                                {
                                    player1Field.text = "(HOST) " + player1Field.text;
                                }
                                player1Field.text = ">>>" + player1Field.text;
                            }
                            break;
                        case 1:
                            SetName(player2Field, players[i], i + 1);
                            if (players[i].IsLocal)
                            {
                                player2Field.text = ">>>" + player2Field.text;
                            }
                            break;
                        case 2:
                            SetName(player3Field, players[i], i + 1);
                            if (players[i].IsLocal)
                            {
                                player3Field.text = ">>>" + player3Field.text;
                            }
                            break;
                        case 3:
                            SetName(player4Field, players[i], i + 1);
                            if (players[i].IsLocal)
                            {
                                player4Field.text = ">>>" + player4Field.text;
                            }
                            break;
                        default:
                            break;
                    }

                    
                }                
            }
        }

        public void SetName(Text field, Player player, int i)
        {
            if (!string.IsNullOrEmpty(player.NickName))
            {
                field.text = player.NickName;
            }
            else
            {
                field.text = "Player " + i;
            }
        }

        public override void OnPlayerEnteredRoom(Player other)
        {
            //Debug.Log("player connected");
            OnJoinedRoom();

            if (PhotonNetwork.IsMasterClient)
            {
                //Debug.Log("master client");
            }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            //Debug.Log("player left");
            player1Field.text = "4 players needed to start";
            player2Field.text = "4 players needed to start";
            player3Field.text = "4 players needed to start";
            player4Field.text = "4 players needed to start";
            OnJoinedRoom();

            if (PhotonNetwork.IsMasterClient)
            {
                //Debug.Log("master client left");
            }
        }

        #endregion


        // Update is called once per frame
        //void Update()
        //{

        //}
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Launcher.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking Demos
// </copyright>
// <summary>
//  Used in "PUN Basic tutorial" to handle typical game management requirements
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using UnityEngine.UIElements;

namespace Photon.Pun.Racer
{
#pragma warning disable 649

    /// <summary>
    /// Game manager.
    /// Connects and watch Photon Status, Instantiate Player
    /// Deals with quiting the room and the game
    /// Deals with level loading (outside the in room synchronization)
    /// </summary>
    public class MyGameManager : MonoBehaviourPunCallbacks
    {
        static public MyGameManager Instance;

        private GameObject instance;

        public bool StartGame;

        [SerializeField] private Transform[] bornPoints;

        [SerializeField] private GameObject playerPrefab;

        [SerializeField] private RaceFinish raceFinish;
        
        #region MonoBehaviour CallBacks

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        void Start()
        {
            Instance = this;

            // in case we started this demo with the wrong scene being active, simply load the menu scene
            if (!PhotonNetwork.IsConnected)
            {
                SceneManager.LoadScene("LobbyScene");
                return;
            }

            if (playerPrefab == null)
            {
                // #Tip Never assume public properties of Components are filled up properly, always check and inform the developer of it.

                Debug.LogError(
                    "<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'",
                    this);
            }
            else
            {
                if (MyPlayerManager.LocalPlayerInstance == null)
                {
                    Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);

                    // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                    int carPos = 0;
                    if (!PhotonNetwork.IsMasterClient)
                    {
                        carPos = 1;
                    }

                    var born = bornPoints[carPos];
                    var player =
                        PhotonNetwork.Instantiate(this.playerPrefab.name, born.position, Quaternion.identity, 0);
                    player.GetComponentInChildren<ChkTrigger>().CarPosListNumber = carPos;
                    FindObjectOfType<Continue>().Play();
                }
                else
                {
                    Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                }
            }
        }

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity on every frame.
        /// </summary>
        void Update()
        {
            // "back" button of phone equals "Escape". quit app if that's pressed
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                QuitApplication();
            }
        }

        #endregion

        #region Photon Callbacks

        /// <summary>
        /// Called when a Photon Player got connected. We need to then load a bigger scene.
        /// </summary>
        /// <param name="other">Other.</param>
        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.Log("OnPlayerEnteredRoom() " + other.NickName); // not seen if you're the player connecting

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}",
                    PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
                // LoadArena();
            }
        }

        /// <summary>
        /// Called when a Photon Player got disconnected. We need to load a smaller scene.
        /// </summary>
        /// <param name="other">Other.</param>
        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.Log("OnPlayerLeftRoom() " + other.NickName); // seen when other disconnects

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}",
                    PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
                // LoadArena(); 
            }
        }

        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene("LobbyScene");
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (changedProps != null && changedProps.ContainsKey(MyPunPlayerLaps.PlayerLapsProp))
            {
                CheckEndOfGame();
            }
        }

        #endregion

        private void CheckEndOfGame()
        {
            bool allDestroyed = true;

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object laps;
                if (p.CustomProperties.TryGetValue(MyPunPlayerLaps.PlayerLapsProp, out laps))
                {
                    if ((int)laps > LapSelector.nLaps)
                    {
                        raceFinish.ShowFinish(p.IsLocal);
                    }
                }
            }
        }

        public bool LeaveRoom()
        {
            return PhotonNetwork.LeaveRoom();
        }

        public void QuitApplication()
        {
            Application.Quit();
        }
    }
}
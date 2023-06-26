// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerListEntry.cs" company="Exit Games GmbH">
//   Part of: Asteroid Demo,
// </copyright>
// <summary>
//  Player List Entry
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

namespace Photon.Pun.Racer
{
    public class MyPlayerListEntry : MonoBehaviour
    {
        [Header("UI References")] public Text PlayerNameText;

        public Image PlayerColorImage;
        public Button PlayerRacerButton;
        public Button PlayerReadyButton;
        public Image PlayerReadyImage;

        private int ownerId;
        private bool isPlayerReady;

        #region UNITY

        public void OnEnable()
        {
            MyPlayerNumbering.OnPlayerNumberingChanged += OnPlayerNumberingChanged;
        }

        public void Start()
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber != ownerId)
            {
                PlayerReadyButton.gameObject.SetActive(false);
            }
            else
            {
                Hashtable initialProps = new Hashtable()
                {
                    { RacerGame.PLAYER_READY, isPlayerReady },
                    { RacerGame.PLAYER_LIVES, RacerGame.PLAYER_MAX_LIVES },
                    { RacerGame.PLAYER_RACER, 0 }
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);
                PhotonNetwork.LocalPlayer.SetScore(0);


                PlayerReadyButton.onClick.AddListener(() =>
                {
                    isPlayerReady = !isPlayerReady;
                    SetPlayerReady(isPlayerReady);

                    Hashtable props = new Hashtable() { { RacerGame.PLAYER_READY, isPlayerReady } };
                    PhotonNetwork.LocalPlayer.SetCustomProperties(props);

                    if (PhotonNetwork.IsMasterClient)
                    {
                        FindObjectOfType<MyLobbyMainPanel>().LocalPlayerPropertiesUpdated();
                    }
                });
            }
        }

        public void OnDisable()
        {
            MyPlayerNumbering.OnPlayerNumberingChanged -= OnPlayerNumberingChanged;
        }

        #endregion

        public void Initialize(int playerId, string playerName)
        {
            ownerId = playerId;
            PlayerNameText.text = playerName;
        }

        private void OnPlayerNumberingChanged()
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if (p.ActorNumber == ownerId)
                {
                    PlayerColorImage.color = RacerGame.GetColor(p.GetPlayerNumber());
                    PlayerRacerButton.GetComponentInChildren<Text>().text = RacerGame.GetRacer(p.GetPlayerRacer());
                }
            }
        }

        public void SetPlayerReady(bool playerReady)
        {
            PlayerReadyButton.GetComponentInChildren<Text>().text = playerReady ? "Ready!" : "Ready?";
            PlayerReadyImage.enabled = playerReady;
        }

        public void SetRacer(int racer)
        {
            PlayerRacerButton.GetComponentInChildren<Text>().text = RacerGame.GetRacer(racer);
        }
    }
}
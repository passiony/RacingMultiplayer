// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PunPlayerScores.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Utilities,
// </copyright>
// <summary>
//  Scoring system for PhotonPlayer
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------



using UnityEngine;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Photon.Pun.Racer
{
    /// <summary>
    /// Scoring system for PhotonPlayer
    /// </summary>
    public class MyPunPlayerRacer : MonoBehaviour
    {
        public const string PlayerRacerProp = "racer";
    }

    public static class RaserExtensions
    {
        public static int GetPlayerRacer(this Player player)
        {
            if (PhotonNetwork.OfflineMode)
            {
                return 0;
            }
            object value;
            if (player.CustomProperties.TryGetValue (MyPlayerNumbering.RoomPlayerRacerProp, out value)) {
                return (byte)value;
            }
            return 0;
        }

        public static void SetPlayerRacer(this Player player, int racerNumber)
        {
            if (player == null) {
                return;
            }

            if (PhotonNetwork.OfflineMode)
            {
                return;
            }

            if (racerNumber < 0)
            {
                Debug.LogWarning("Setting invalid racerNumber: " + racerNumber + " for: " + player.ToStringFull());
            }

            if (!PhotonNetwork.IsConnectedAndReady)
            {
                Debug.LogWarning("SetPlayerNumber was called in state: " + PhotonNetwork.NetworkClientState + ". Not IsConnectedAndReady.");
                return;
            }

            int current = player.GetPlayerRacer();
            if (current != racerNumber)
            {
                Debug.Log("PlayerNumbering: Set number "+racerNumber);
                player.SetCustomProperties(new Hashtable() { { MyPlayerNumbering.RoomPlayerRacerProp, (byte)racerNumber } });
            }
        }
    }
}
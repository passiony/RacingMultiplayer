// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PunPlayerScores.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Utilities,
// </copyright>
// <summary>
//  Scoring system for PhotonPlayer
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Photon.Pun.Racer
{
    /// <summary>
    /// Scoring system for PhotonPlayer
    /// </summary>
    public class MyPunPlayerLaps : MonoBehaviour
    {
        public const string PlayerLapsProp = "laps";
    }

    public static class LapsExtensions
    {
        public static void SetLaps(this Player player, int newScore)
        {
            Hashtable score = new Hashtable(); // using PUN's implementation of Hashtable
            score[MyPunPlayerLaps.PlayerLapsProp] = newScore;

            player.SetCustomProperties(score); // this locally sets the score and will sync it in-game asap.
        }

        public static void AddLaps(this Player player, int scoreToAddToCurrent)
        {
            int current = player.GetLaps();
            current += scoreToAddToCurrent;

            Hashtable score = new Hashtable(); // using PUN's implementation of Hashtable
            score[MyPunPlayerLaps.PlayerLapsProp] = current;

            player.SetCustomProperties(score); // this locally sets the score and will sync it in-game asap.
        }

        public static int GetLaps(this Player player)
        {
            object score;
            if (player.CustomProperties.TryGetValue(MyPunPlayerLaps.PlayerLapsProp, out score))
            {
                return (int)score;
            }

            return 0;
        }
    }
}
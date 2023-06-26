// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerNumberingInspector.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Utilities,
// </copyright>
// <summary>
//  Custom inspector for PlayerNumbering
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEditor;

using Photon.Realtime;

namespace Photon.Pun.Racer
{
	[CustomEditor(typeof(MyPlayerNumbering))]
	public class PlayerNumberingInspector : Editor {

	 	int localPlayerIndex;

		void OnEnable () {
			MyPlayerNumbering.OnPlayerNumberingChanged += RefreshData;
		}

		void OnDisable () {
			MyPlayerNumbering.OnPlayerNumberingChanged -= RefreshData;
		}

		public override void OnInspectorGUI()
		{
            DrawDefaultInspector();

            MyPlayerNumbering.OnPlayerNumberingChanged += RefreshData;

			if (PhotonNetwork.InRoom)
			{
				EditorGUILayout.LabelField("Player Index", "Player ID");
				if (MyPlayerNumbering.SortedPlayers != null)
				{
					foreach(Player punPlayer in MyPlayerNumbering.SortedPlayers)
					{
						GUI.enabled = punPlayer.ActorNumber > 0;
						EditorGUILayout.LabelField("Player " +punPlayer.GetPlayerNumber() + (punPlayer.IsLocal?" - You -":""), punPlayer.ActorNumber == 0?"n/a":punPlayer.ToStringFull());
						GUI.enabled = true;
					}
				}
			}else{
				GUILayout.Label("PlayerNumbering only works when localPlayer is inside a room");
			}
		}

		/// <summary>
		/// force repaint fo the inspector, else we would not see the new data in the inspector.
		/// This is better then doing it in OnInspectorGUI too many times per frame for no need
		/// </summary>
		void RefreshData()
		{
			Repaint();
		}

	}
}
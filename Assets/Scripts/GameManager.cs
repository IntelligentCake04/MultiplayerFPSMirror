using System.Collections.Generic;
using UnityEngine;

namespace IntelligentCake
{
    public class GameManager : MonoBehaviour
    {
        private const string PlayerIdPrefix = "Player ";
        
        private static Dictionary<string, Player.Player> _players = new Dictionary<string, Player.Player>();

        public static void RegisterPlayer(string netId, Player.Player player)
        {
            string playerId = PlayerIdPrefix + netId;
            _players.Add(playerId, player);
            player.transform.name = playerId;
        }

        public static void UnRegisterPlayer(string playerId)
        {
            _players.Remove(playerId);
        }

        public static Player.Player GetPlayer(string playerId)
        {
            return _players[playerId];
        }

        /*private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(200, 200, 200, 500));
            GUILayout.BeginVertical();

            foreach (string playerId in _players.Keys)
            {
                 GUILayout.Label(playerId + " - " +_players[playerId].transform.name);
            }
            
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }*/
    }
}
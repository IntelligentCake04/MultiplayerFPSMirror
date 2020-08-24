using UnityEngine;

namespace IntelligentCake.UI
{
    public class Scoreboard : MonoBehaviour
    {
        [SerializeField] private GameObject playerScoreboardItem;
        [SerializeField] private Transform playerScoreboardList;
        
        private void OnEnable()
        {
            Player.Player[] players = GameManager.GetAllPlayers();

            foreach (Player.Player player in players)
            {
                GameObject itemGO = (GameObject)Instantiate(playerScoreboardItem, playerScoreboardList);
                PlayerScoreboardItem item = itemGO.GetComponent<PlayerScoreboardItem>();
                if (item != null)
                {
                    item.Setup(player.username, player.kills, player.deaths);
                }
            }
        }

        private void OnDisable()
        {
            foreach (Transform child in playerScoreboardList)
            {
                Destroy(child.gameObject);
            }
        }
    }
}

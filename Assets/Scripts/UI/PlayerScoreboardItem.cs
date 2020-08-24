using TMPro;
using UnityEngine;

namespace IntelligentCake.UI
{
    public class PlayerScoreboardItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text usernameText;
        [SerializeField] private TMP_Text killsText;
        [SerializeField] private TMP_Text deathsText;

        public void Setup(string username, int kills, int deaths)
        {
            usernameText.text = username;
            killsText.text = "Kills: " + kills;
            deathsText.text = "Deaths: " + deaths;
        }
    }
}
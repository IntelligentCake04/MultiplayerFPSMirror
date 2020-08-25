using TMPro;
using UnityEngine;

namespace IntelligentCake.UI
{
    public class KillfeedItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;

        public void Setup(string player, string source)
        {
            text.text = "<b>" + source + "</b>" + " <color=red>" + "killed" + "</color>" + " <i>" + player + "</i>";
        }
    }
}

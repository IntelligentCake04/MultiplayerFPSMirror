using UnityEngine;

namespace IntelligentCake.UI
{
    public class Killfeed : MonoBehaviour
    {
        [SerializeField] private GameObject killfeedItemPrefab;
        
        // Start is called before the first frame update
        void Start()
        {
            GameManager.Instance.onPlayerKilledCallback += OnKill;
        }

        public void OnKill(string player, string source)
        {
            GameObject go = (GameObject)Instantiate(killfeedItemPrefab, this.transform);
            go.GetComponent<KillfeedItem>().Setup(player, source);
            
            Destroy(go, 4f);
        }
    }
}

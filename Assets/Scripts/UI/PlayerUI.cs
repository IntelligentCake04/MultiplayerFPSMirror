using IntelligentCake.Combat;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace IntelligentCake.UI
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] private RectTransform healthBarFill;
        [SerializeField] private Text healthText;

        [SerializeField] private Text ammoText;
    
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private GameObject scoreboard;
        
        public AudioMixer audioMixer;

        private Player.Player _player;
        private WeaponManager _weaponManager;

        public void SetPlayer(Player.Player player)
        {
            _player = player;
            _weaponManager = player.GetComponent<WeaponManager>();
        }
        
        private void Update()
        {
            healthText.text = _player.GetHealth().ToString();
            SetAmmoAmount(_weaponManager.GetCurrentWeapon().bullets, _weaponManager.GetCurrentWeapon().maxBullets);
            SetHealthAmount(_player.GetHealthPct());
            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePauseMenu();
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                scoreboard.SetActive(true);
            } else if (Input.GetKeyUp(KeyCode.Tab))
            {
                scoreboard.SetActive(false);
            }
        }

        private void Start()
        {
            PauseMenu.IsOn = false;
        }

        void TogglePauseMenu()
        {
            pauseMenu.SetActive(!pauseMenu.activeSelf);
            PauseMenu.IsOn = pauseMenu.activeSelf;
            if (PauseMenu.IsOn)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        void SetHealthAmount(float amount)
        {
            healthBarFill.localScale = new Vector3(1f, amount, 1f);
        }

        void SetAmmoAmount(int currentAmount, int maxAmount)
        {
            ammoText.text = currentAmount + " / " + maxAmount;
        }
        
        public void SetVolume(float volume)
        {
            audioMixer.SetFloat("volume", Mathf.Log10(volume) * 20); 
        }
    }
}

using System;
using UnityEngine;

namespace IntelligentCake.Player
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] private GameObject pauseMenu;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePauseMenu();
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
                Cursor.lockState = CursorLockMode.None;
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
}

using System.Collections;
using UnityEngine;

namespace EpicToonFX
{
    public class ETFXLoopScript : MonoBehaviour
    {
        public GameObject chosenEffect;
        public float loopTimeLimit = 2.0f;

        [Header("Spawn without")] public bool spawnWithoutLight = true;

        public bool spawnWithoutSound = true;

        private void Start()
        {
            PlayEffect();
        }

        public void PlayEffect()
        {
            StartCoroutine("EffectLoop");
        }

        private IEnumerator EffectLoop()
        {
            var effectPlayer = Instantiate(chosenEffect, transform.position, transform.rotation);

            if (spawnWithoutLight = true && effectPlayer.GetComponent<Light>())
                effectPlayer.GetComponent<Light>().enabled = false;
            //Destroy(gameObject.GetComponent<Light>());

            if (spawnWithoutSound = true && effectPlayer.GetComponent<AudioSource>())
                effectPlayer.GetComponent<AudioSource>().enabled = false;
            //Destroy(gameObject.GetComponent<AudioSource>());

            yield return new WaitForSeconds(loopTimeLimit);

            Destroy(effectPlayer);
            PlayEffect();
        }
    }
}
﻿using UnityEngine;

namespace EpicToonFX
{
    public class ETFXLightFade : MonoBehaviour
    {
        private float initIntensity;
        public bool killAfterLife = true;

        private Light li;

        [Header("Seconds to dim the light")] public float life = 0.2f;

        // Use this for initialization
        private void Start()
        {
            if (gameObject.GetComponent<Light>())
            {
                li = gameObject.GetComponent<Light>();
                initIntensity = li.intensity;
            }
            else
            {
                print("No light object found on " + gameObject.name);
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if (gameObject.GetComponent<Light>())
            {
                li.intensity -= initIntensity * (Time.deltaTime / life);
                if (killAfterLife && li.intensity <= 0)
                    //Destroy(gameObject);
                    Destroy(gameObject.GetComponent<Light>());
            }
        }
    }
}
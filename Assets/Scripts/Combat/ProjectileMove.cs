using UnityEngine;

namespace IntelligentCake.Combat
{
    public class ProjectileMove : MonoBehaviour
    {
        public float speed;

        // Update is called once per frame
        private void Update()
        {
            if (speed != 0)
                transform.position += transform.forward * (speed * Time.deltaTime);
            else
                Debug.Log("No speed");
        }

        private void OnParticleCollision(GameObject other)
        {
            Destroy(gameObject);
        }
    }
}
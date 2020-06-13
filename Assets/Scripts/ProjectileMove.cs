using UnityEngine;

namespace IntelligentCake
{
    public class ProjectileMove : MonoBehaviour
    {
        public float speed;

        // Start is called before the first frame update
        private void Start()
        {
        }

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
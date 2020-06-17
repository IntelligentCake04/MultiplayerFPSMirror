using Mirror;

namespace IntelligentCake.Player
{
    public class Player : NetworkBehaviour
    {
        public float health = 100f;

        public void TakeDamage(float amount)
        {
            health -= amount;
            if (health <= 0f) Die();
        }

        private void Die()
        {
            Destroy(gameObject);
        }
    }
}
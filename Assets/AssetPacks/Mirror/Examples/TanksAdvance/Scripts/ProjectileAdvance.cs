using UnityEngine;

namespace Mirror.Examples.Tanks
{
    public class ProjectileAdvance : NetworkBehaviour
    {
        public float destroyAfter = 5;
        public float force = 1000;
        public Rigidbody rigidBody;

        public override void OnStartServer()
        {
            Invoke(nameof(DestroySelf), destroyAfter);
        }

        // set velocity for server and client. this way we don't have to sync the
        // position, because both the server and the client simulate it.
        private void Start()
        {
            rigidBody.AddForce(transform.forward * force);
        }

        // destroy for everyone on the server
        [Server]
        private void DestroySelf()
        {
            NetworkServer.Destroy(gameObject);
        }

        // ServerCallback because we don't want a warning if OnTriggerEnter is
        // called on the client
        [ServerCallback]
        private void OnTriggerEnter(Collider co)
        {
            NetworkServer.Destroy(gameObject);
        }
    }
}
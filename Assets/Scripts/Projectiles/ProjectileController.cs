using UnityEngine;

namespace Projectiles
{
    public class ProjectileController : MonoBehaviour
    {
        [SerializeField] private float _speed;

        public bool AllowMoving { get; internal set; }

        private void Update()
        {
            if (!AllowMoving)
                return;

            this.transform.position += Vector3.forward * (_speed * Time.deltaTime);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.TryGetComponent(out IProjectileHittable projectileHittable))
            {
                projectileHittable.OnProjectileEnter();
            }
            
            Destroy(this.gameObject);
        }
    }
}

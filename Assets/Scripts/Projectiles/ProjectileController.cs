using UnityEngine;

namespace Projectiles
{
    public class ProjectileController : MonoBehaviour
    {
        [SerializeField] private float _speed;

        private void Update()
        {
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

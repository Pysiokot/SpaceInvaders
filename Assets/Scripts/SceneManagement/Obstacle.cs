using Projectiles;
using UnityEngine;

namespace SceneManagement
{
    public class Obstacle : MonoBehaviour, IProjectileHittable
    {
        [SerializeField]
        private int _hitsToDestroy = 5;

        public void OnProjectileEnter()
        {
            if(_hitsToDestroy == 0)
            {
                Destroy(this.gameObject);
                return;
            }

            _hitsToDestroy -= 1;
         
            // TODO: Shader for destroying object?
        }
    }
}
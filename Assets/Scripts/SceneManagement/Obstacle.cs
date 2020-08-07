using Projectiles;
using UnityEngine;

namespace SceneManagement
{
    public class Obstacle : MonoBehaviour, IProjectileHittable
    {
        [SerializeField]
        private int _hitsToDestroy = 5;

        private float _minAlfa = 0.2f;
        private float _decreaseStep;

        private void Start()
        {
            _decreaseStep = (1f - _minAlfa) / (_hitsToDestroy - 1);
        }

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
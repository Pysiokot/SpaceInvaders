using System;
using Projectiles;
using ScriptableObjects;
using UnityEngine;

namespace Enemy
{
    public class EnemyController : MonoBehaviour, IProjectileHittable
    {
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");

        [SerializeField] private EnemyParams _params;
        [SerializeField] private MeshRenderer _mesh;

        internal event EventHandler<EnemyKilledEventArgs> EnemyKilled;
        
        public void InitParams(EnemyParams enemyParams)
        {
            _params = enemyParams;
            
            InitGameObject();
        }
        
        #region Interface Methods

        public void OnProjectileEnter()
        {
            EnemyKilled?.Invoke(this, new EnemyKilledEventArgs() {Points = _params.Points});
            
            Destroy(this.gameObject);
#if UNITY_EDITOR
            Debug.Log("Enemy hit");
#endif
        }
 
        #endregion
        
        private void InitGameObject()
        {
            this.transform.localScale = _params.QuadScale;
            
            _mesh.material.SetTexture(MainTex, _params.EnemyTexture);
        }
    }
}

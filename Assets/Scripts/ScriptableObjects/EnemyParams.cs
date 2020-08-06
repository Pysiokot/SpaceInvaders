using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "EnemyParams", menuName = "ScriptableObjects/EnemyParams", order = 2)]
    public class EnemyParams : ScriptableObject
    {
        public int Points;
        public Vector3 QuadScale;
        public Texture EnemyTexture;
    }
}

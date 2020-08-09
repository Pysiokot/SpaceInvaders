using Enemy;
using TMPro;
using UnityEngine;
using Zenject;

namespace UI
{
    public class ScoreController : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _textMesh;

        private IEnemyLifeController _enemyLifeController;

        private int _score;

        // Start is called before the first frame update
        void Start()
        {
            SetScore(0);
        }

        [Inject]
        private void InitializeDI(IEnemyLifeController enemyLifeController)
        {
            _enemyLifeController = enemyLifeController;

            _enemyLifeController.EnemyKilled += OnEnemyKilled;
        }

        private void OnEnemyKilled(EnemyController enemyController, EnemyKilledEventArgs args)
        {
            SetScore(_score + args.Points);
        }

        private void SetScore(int score)
        {
            _score = score;

            _textMesh.SetText(_score.ToString());
        }

        private void OnDestroy()
        {
            if(_enemyLifeController != null)
            {
                _enemyLifeController.EnemyKilled -= OnEnemyKilled;
            }
        }
    }
}
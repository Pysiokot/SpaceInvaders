using Enemy;
using TMPro;
using UnityEngine;
using Utils;
using Zenject;

namespace UI
{
    public class ScoreController : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _textMesh;

        private IEnemyLifeController _enemyLifeController;
        private IGameStateController _gameStateController;

        private int _score;

        // Start is called before the first frame update
        void Start()
        {
            SetScore(0);
        }

        [Inject]
        private void InitializeDI(IEnemyLifeController enemyLifeController, IGameStateController gameStateController)
        {
            _enemyLifeController = enemyLifeController;
            _gameStateController = gameStateController;

            _enemyLifeController.EnemyKilled += OnEnemyKilled;
            _gameStateController.GameStateChanged += OnGameStateChanged;
        }

        private void OnGameStateChanged(GameState newState)
        {
            if(newState == GameState.Reset)
            {
                SetScore(0);
            }
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

            if(_gameStateController != null)
            {
                _gameStateController.GameStateChanged -= OnGameStateChanged;
            }
        }
    }
}
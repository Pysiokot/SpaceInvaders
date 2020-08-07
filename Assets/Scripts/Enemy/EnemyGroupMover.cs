using System.Collections;
using SceneManagement;
using UnityEngine;

namespace Enemy
{
    public class EnemyGroupMover : MonoBehaviour
    {
        [SerializeField] private EnemiesSpawnerController _spawnerController;
        [SerializeField] private Vector2 _movementBoundaries = new Vector2(-0.1f, 0.1f);
        [SerializeField] private float _sideMovementStepVal;
        [SerializeField] private float _moveDelay = 0.1f;
        [SerializeField] private float _minMoveDelay = 0.03f;

        private Coroutine _movingCoroutine;

        private Vector2 _currentGroupBorderColsPos = Vector2.zero;
        private float _movementSign = 1f;
        private bool _allEnemiesKilled = false;

        private void Awake()
        {
            _spawnerController.EnemyGroupBorderColumnsPosChanged += OnEnemyGroupBorderColumnsPosChanged;
        }

        private void Start()
        {
            _spawnerController.EnemyKilled += OnEnemyKilled;
            _spawnerController.EnemyCountReachedZero += (sender, args) => _allEnemiesKilled = true;
            StartMovingEnemyGroup();
        }

#if UNITY_EDITOR
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.K))
            {
                StopMovingEnemyGroup();
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                StartMovingEnemyGroup();
            }
        }
#endif

        public void StartMovingEnemyGroup()
        {
            _movingCoroutine = StartCoroutine(StartMovingEnemyGroupCoroutine());
        }

        public void StopMovingEnemyGroup()
        {
            StopCoroutine(_movingCoroutine);
        }

        private IEnumerator StartMovingEnemyGroupCoroutine()
        {
            while (!_allEnemiesKilled)
            {
                MoveGroupToSide();
                yield return new WaitForSeconds(_moveDelay);
            }
        }
        
        private void UpdateCurrPosByOneStepTowardsPlayer(ref Vector3 currGroupPos)
        {
            currGroupPos.z -= 0.03f;
        }

        private void MoveGroupToSide()
        {
            var currGroupPos = this.transform.position;
            
            if (currGroupPos.x < _movementBoundaries.x || currGroupPos.x > _movementBoundaries.y)
            {
                UpdateCurrPosByOneStepTowardsPlayer(ref currGroupPos);

                _movementSign *= -1f;
            }

            currGroupPos.x += _sideMovementStepVal * _movementSign;

            this.transform.position = currGroupPos;
        }
        
        private void OnEnemyGroupBorderColumnsPosChanged(Vector2 borderColsPos)
        {
            // init
            if (_currentGroupBorderColsPos == Vector2.zero)
            {
                _currentGroupBorderColsPos = borderColsPos;
                return;
            }
            
            
            // some column in middle was cleared
            if(_currentGroupBorderColsPos == borderColsPos)
                return;

            var prevWidth = _currentGroupBorderColsPos;
            // update group border columns positions
            _currentGroupBorderColsPos = borderColsPos;

            // change movement boundaries
            _movementBoundaries.x += (prevWidth.x - _currentGroupBorderColsPos.x);
            _movementBoundaries.y += (prevWidth.y - _currentGroupBorderColsPos.y);
        }

        private void OnEnemyKilled(object enemyObj, EnemyKilledEventArgs e)
        {
            _moveDelay -= 0.002f;
        }
    }
}

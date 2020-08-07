﻿using System.Collections;
using NaughtyAttributes;
using SceneManagement;
using UnityEngine;

namespace Enemy
{
    public class EnemyGroupMover : MonoBehaviour
    {
        [SerializeField] private EnemiesSpawnerController _spawnerController;
        [SerializeField] private Vector2 _movementBoundaries = new Vector2(-0.1f, 0.1f);
        [SerializeField] private float _sideMovementStepVal;

        [SerializeField]
        [ValidateInput("IsGreaterThanMinVal", "StartMoveDelay should be greater than MinMoveDelay and greater than 0")]
        private float _startMoveDelay = 0.15f;

        [SerializeField]
        [ValidateInput("IsLesserThanStartVal", "MinMoveDelay should be lesser than StartMoveDelay and greater than 0")]
        private float _minMoveDelay = 0.02f;

        private Coroutine _movingCoroutine;

        private Vector2 _currentGroupBorderColsPos = Vector2.zero;
        private float _movementSign = 1f;
        private float _movementDecreaseStep;
        private bool _allEnemiesKilled = false;

        private void Awake()
        {
            _spawnerController.EnemyGroupBorderColumnsPosChanged += OnEnemyGroupBorderColumnsPosChanged;
            _spawnerController.EnemiesSpawned += OnEnemiesSpawned;
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
                yield return new WaitForSeconds(_startMoveDelay);
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

        private void OnEnemiesSpawned(int count)
        {
            _movementDecreaseStep = (_startMoveDelay - _minMoveDelay) / count;
        }

        private void OnEnemyKilled(object enemyObj, EnemyKilledEventArgs e)
        {
            _startMoveDelay = Mathf.Clamp(_startMoveDelay - _movementDecreaseStep, _minMoveDelay, _startMoveDelay);
        }

        #region NaughtyAttributes Methods
        private bool IsLesserThanStartVal(float value)
        {
            return value < _startMoveDelay && value > 0;
        }

        private bool IsGreaterThanMinVal(float value)
        {
            return value > _minMoveDelay && value > 0;
        }
        #endregion
    }
}
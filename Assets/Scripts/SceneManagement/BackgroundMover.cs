using System;
using NaughtyAttributes;
using UnityEngine;

namespace SceneManagement
{
    public class BackgroundMover : MonoBehaviour
    {
        private const float TOLERANCE = 0.001f;
        
        [SerializeField] private GameObject _firstBgGameObject;
        [SerializeField] private GameObject _secondBgGameObject;

        [SerializeField] private bool EnableMovingBg;
        [SerializeField] [ShowIf("EnableMovingBg")] private float BgMovementSpeed = 2.0f;

        private float backgroundWidth_;

        private void Awake()
        {
            backgroundWidth_ = _firstBgGameObject.transform.localScale.x;
        }

        // Update is called once per frame
        void Update()
        {
            if (!EnableMovingBg)
            {
                return;
            }

            var frameMovementDelta = Vector3.right * (BgMovementSpeed * Time.deltaTime);
            _firstBgGameObject.transform.position += frameMovementDelta;
            _secondBgGameObject.transform.position += frameMovementDelta;

            if (AnyBgReachedBoundry())
            {
                MoveBgToStartPos();
            }
        }

        private void MoveBgToStartPos()
        {
            var bgToRepos = BgReachedEnd(_firstBgGameObject.transform.position.x)
                ? _firstBgGameObject
                : _secondBgGameObject;

            bgToRepos.transform.position = Vector3.left * backgroundWidth_;
        }

        private bool AnyBgReachedBoundry()
        {
            return BgReachedEnd(_firstBgGameObject.transform.position.x) ||
                   BgReachedEnd(_secondBgGameObject.transform.position.x);
        }

        private bool BgReachedEnd(float bgPosX)
        {
            return Math.Abs(bgPosX - backgroundWidth_) < TOLERANCE;
        }
    }
}
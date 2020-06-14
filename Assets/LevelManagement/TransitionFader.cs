using LevelManagement.Utility;
using System.Collections;
using UnityEngine;

namespace LevelManagement
{
    public class TransitionFader : ScreenFader
    {
        [SerializeField]
        private float _lifetime = 1f;
        [SerializeField]
        private float _delay = 0.3f;
        public float Delay { get => _delay; }
        private void Awake()
        {
            _lifetime = Mathf.Clamp(_lifetime, FadeOnDuration + FadeOffDuration + _delay, 10f);
        }

        private IEnumerator PlayRoutine()
        {
            SetAplha(_clearAlpha);
            yield return new WaitForSeconds(_delay);

            FadeOn();
            float onTime = _lifetime - (FadeOffDuration + _delay);
            yield return new WaitForSeconds(onTime);

            FadeOff();
            Destroy(gameObject, FadeOffDuration);
        }

        public void Play()
        {
            StartCoroutine(PlayRoutine());
        }

        public static void PlayTransition(TransitionFader transitionPrefab)
        {
            if (transitionPrefab != null)
            {
                TransitionFader instance = Instantiate(transitionPrefab, Vector3.zero, Quaternion.identity);
                instance.Play();
            }
        }
    }
}
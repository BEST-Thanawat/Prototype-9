using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LevelManagement.Utility
{
    public class ScreenFader : MonoBehaviour
    {
        [SerializeField]
        protected float _solidAlpha = 1f, _clearAlpha = 0f;
        [SerializeField]
        private float _fadeOnDuration = 2f, _fadeOffDuration = 2f;
        public float FadeOnDuration { get => _fadeOnDuration; }
        public float FadeOffDuration { get => _fadeOffDuration; }
        [SerializeField]
        private MaskableGraphic[] graphicsFade;

        protected void SetAplha(float alpha)
        {
            foreach (MaskableGraphic graphic in graphicsFade)
            {
                if (graphic != null)
                {
                    graphic.canvasRenderer.SetAlpha(alpha);
                }
            }
        }
        private void Fade(float targetAlpha, float duration)
        {
            foreach (MaskableGraphic graphic in graphicsFade)
            {
                if (graphic != null)
                {
                    graphic.CrossFadeAlpha(targetAlpha, duration, true);
                }
            }
        }

        public void FadeOff()
        {
            SetAplha(_solidAlpha);
            Fade(_clearAlpha, _fadeOffDuration);
        }

        public void FadeOn()
        {
            SetAplha(_clearAlpha);
            Fade(_solidAlpha, _fadeOnDuration);
        }
    }
}
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Utils
{
    public class GlobalLightManager : MonoBehaviour
    {
        [SerializeField] private Light2D globalLight;

        [SerializeField] private float intensity;

        [SerializeField] private bool isDarkArea;


        private void OnTriggerEnter2D(Collider2D other)
        {
            DOTween.To(() => globalLight.intensity, x => globalLight.intensity = x, intensity, 2f);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!isDarkArea) return;
            DOTween.To(() => globalLight.intensity, x => globalLight.intensity = x, 0.1f, 2f);
        }

        public void ChangeLight(float targetIntensity)
        {
            DOTween.To(() => globalLight.intensity, x => globalLight.intensity = x, targetIntensity, 1f);
        }

        public void PauseLightChange()
        {
            DOTween.Pause(globalLight);
        }
        
        public float LightIntensity => globalLight.intensity;
    }
}

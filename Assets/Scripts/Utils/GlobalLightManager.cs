using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Utils
{
    public class GlobalLightManager : MonoBehaviour
    {
        [SerializeField] private Light2D globalLight;

        [SerializeField] private float intensity;
    
        private void OnTriggerEnter2D(Collider2D other)
        {
            DOTween.To(() => globalLight.intensity, x => globalLight.intensity = x, intensity, 2f);
        }

        public void ChangeLight(float targetIntensity)
        {
            DOTween.To(() => globalLight.intensity, x => globalLight.intensity = x, targetIntensity, 1f);
        }
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Utils
{
    public class GamepadVibration : MonoBehaviour
    {
        public static GamepadVibration Instance;

        private void Awake()
        {
            Instance = this;
        }

        public void Rumble(float low, float high, float duration)
        {
            print("checking...");
            if (Gamepad.current != null)
                StartCoroutine(RumbleRoutine(low, high, duration));
        }

        private IEnumerator RumbleRoutine(float low, float high, float duration)
        {
            print("RUMBLE");

            Gamepad.current.SetMotorSpeeds(low, high);

            yield return new WaitForSeconds(duration);

            Gamepad.current.SetMotorSpeeds(0, 0);
        }
    }
}

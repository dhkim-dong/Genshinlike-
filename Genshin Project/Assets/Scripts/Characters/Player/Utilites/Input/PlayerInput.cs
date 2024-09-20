using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GenshinImpactMovementSystem
{
    public class PlayerInput : MonoBehaviour
    {
        // 새로운 Input Action 클래스를 받아온다.
        public PlayerInputActions InputActions { get; private set; }
        public PlayerInputActions.PlayerActions PlayerActions { get; private set; }

        private void Awake()
        {
            InputActions = new PlayerInputActions();
            PlayerActions = InputActions.Player;
        }

        // Enable() Disable() 해야하는 이유?

        private void OnEnable()
        {
            InputActions.Enable();
        }

        private void OnDisable()
        {
            InputActions.Disable();
        }

        public void DisableActionFor(InputAction action, float seconds)
        {
            StartCoroutine(DisableAction(action, seconds));
        }

        private IEnumerator DisableAction(InputAction action, float seconds)
        {
            action.Disable();

            yield return new WaitForSeconds(seconds);

            action.Enable();
        }
    }
}

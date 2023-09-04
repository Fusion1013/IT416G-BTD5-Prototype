using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class InputManager : MonoBehaviour, Controls.IGameplayActions
    {
        #region Fields

        private Controls _controls;
        
        // Events
        public static event Action OnGameplayInteract;

        #endregion

        private void OnEnable()
        {
            if (_controls == null)
            {
                _controls = new Controls();
                _controls.Gameplay.SetCallbacks(this);
            }
            _controls.Gameplay.Enable();
        }

        private void OnDisable()
        {
            _controls.Gameplay.Disable();
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started) OnGameplayInteract?.Invoke();
        }
    }
}

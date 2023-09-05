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
        public static event Action OnGameplayCancel;

        #endregion

        #region Init

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

        #endregion

        #region Input Events

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started) OnGameplayInteract?.Invoke();
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started) OnGameplayCancel?.Invoke();
        }

        #endregion
    }
}

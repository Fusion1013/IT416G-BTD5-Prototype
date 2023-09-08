using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class InputManager : MonoBehaviour, Controls.IGameplayActions, Controls.IUIActions
    {
        #region Fields

        private Controls _controls;
        
        // Events
        public static event Action OnGameplayInteract;
        public static event Action OnGameplayCancel;
        public static event Action OnUIOpenCheats;

        #endregion

        #region Init

        private void OnEnable()
        {
            if (_controls == null)
            {
                _controls = new Controls();
                _controls.Gameplay.SetCallbacks(this);
                _controls.UI.SetCallbacks(this);
            }
            _controls.Gameplay.Enable();
            _controls.UI.Enable();
        }

        private void OnDisable()
        {
            _controls.Gameplay.Disable();
            _controls.UI.Disable();
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
        
        public void OnOpenCheatMenu(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started) OnUIOpenCheats?.Invoke();
        }

        #endregion
    }
}

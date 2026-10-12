using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace World.Player.Unity
{
    public class PlayerInputBehaviour : MonoBehaviour
    {
        [SerializeField] private InputActionAsset inputActionAsset;

        private PlayerInput _playerInput;
        private InputActionMap _inputActionMap;
        
        private InputAction _moveAction;
        private InputAction _dashAction;
        
        public void Initialize(PlayerInput playerInput)
        {
            this._playerInput = playerInput;
            
            _inputActionMap = inputActionAsset.FindActionMap(
                "Player", true);
            
            _moveAction = _inputActionMap.FindAction("Move", true);
            _dashAction = _inputActionMap.FindAction("Dash", true);
            
            _inputActionMap.Enable();
        }

        public void CaptureInput()
        {
            Vector2 movement = _moveAction.ReadValue<Vector2>();

            _playerInput.MovementValue = new(
                movement.x,
                movement.y);

            _playerInput.Dash |= _dashAction.WasPressedThisFrame();
        }

        private void OnDisable()
        {
            inputActionAsset?.Disable();
        }
    }
}
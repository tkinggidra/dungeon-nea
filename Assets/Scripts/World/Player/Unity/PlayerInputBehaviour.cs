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
        
        public void Initialize(PlayerInput playerInput)
        {
            this._playerInput = playerInput;
            
            _inputActionMap = inputActionAsset.FindActionMap(
                "Player", true);
            
            _moveAction = _inputActionMap.FindAction("Move", true);
            
            _inputActionMap.Enable();
        }

        public void CaptureInput()
        {
            var movement = _moveAction.ReadValue<Vector2>();
            
            _playerInput.MovementValue = new(movement.x, movement.y);
            // print(_playerInput.MovementValue);
        }

        private void OnDisable()
        {
            inputActionAsset?.Disable();
        }
    }
}
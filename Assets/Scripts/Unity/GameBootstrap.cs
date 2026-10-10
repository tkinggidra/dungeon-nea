using System;
using Core.Tick;
using Core.Tick.Unity;
using Entity;
using Entity.Unity;
using UnityEngine;
using World;
using World.Player;
using World.Player.Unity;

namespace Unity
{
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private TickSchedulerBehaviour tickSchedulerBehaviour;
        [SerializeField] private PlayerInputBehaviour playerInputBehaviour;
        [SerializeField] private PlayerBehaviour playerBehaviour;

        
        private TickScheduler _tickScheduler;
        private PlayerInput _playerInput;
        private Player _player;
        private Level _level;

        private void Awake()
        {
            _tickScheduler = new TickScheduler();
            _playerInput = new PlayerInput();
            _level = new Level();

            _player = _level.AddPlayer(_playerInput);
            
            _tickScheduler.Register(_level);
            tickSchedulerBehaviour.Initialize(_tickScheduler);
            playerInputBehaviour.Initialize(_playerInput);
            playerBehaviour.Initialize(_player);
        }
        
        private void Update()
        {
            playerInputBehaviour.CaptureInput();
            
            _tickScheduler?.Advance(Time.deltaTime);
            
            playerBehaviour.SyncTransform();
        }
    }
}
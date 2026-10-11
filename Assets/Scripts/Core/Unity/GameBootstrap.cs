using System;
using Core.Tick;
using Core.Tick.Unity;
using World.Entity;
using World.Entity.Unity;
using UnityEngine;
using World.Level;
using World.Navigation;
using World.Player;
using World.Player.Unity;

namespace Core.Unity
{
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private TickSchedulerBehaviour tickSchedulerBehaviour;
        [SerializeField] private PlayerInputBehaviour playerInputBehaviour;
        [SerializeField] private PlayerBehaviour playerBehaviour;
        [SerializeField] private EntityBehaviour entityBehaviour;

        private TESTChaserMob _testMob;
        
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
            
            var pathFinder = CreateTestPathFinder();

            _player.Transform.SetPosition(
                new System.Numerics.Vector3(8f, 0f, 8f)
            );

            _testMob = new TESTChaserMob(pathFinder);

            _testMob.Transform.SetPosition(
                new System.Numerics.Vector3(2f, 0f, 2f)
            );

            _level.AddEntity(_testMob);

            entityBehaviour.Initialize(
                _testMob,
                _tickScheduler
            );
            
            _tickScheduler.Register(_level);
            tickSchedulerBehaviour.Initialize(_tickScheduler);
            playerInputBehaviour.Initialize(_playerInput);
            playerBehaviour.Initialize(
                _player,
                _tickScheduler);
        }
        
        private void Update()
        {
            playerInputBehaviour.CaptureInput();
            
            _tickScheduler?.Advance(Time.deltaTime);
        }

        private void LateUpdate()
        {
            playerBehaviour.Render(
                (float)_tickScheduler.InterpolationAlpha);
            
            entityBehaviour.Render(
                (float)_tickScheduler.InterpolationAlpha
            );
        }
        
        private static PathFinder CreateTestPathFinder()
        {
            var grid = new NavigationGrid(20, 20);

            for (int x = 0; x < 20; x++)
            {
                for (int z = 0; z < 20; z++)
                {
                    grid.SetWalkable(x, z, true);
                }
            }

            return new PathFinder(grid);
        }
    }
}
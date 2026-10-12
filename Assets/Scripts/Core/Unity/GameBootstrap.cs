using System;
using Core.Physics;
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
        [SerializeField] private PlayerBehaviour playerPrefab;
        [SerializeField] private EntityBehaviour entityPrefab;
        [SerializeField] private Transform entityContainer;
        
        private EntityView _entityView;
        private TickScheduler _tickScheduler;
        private EntityRenderer _entityRenderer;
        private PlayerInput _playerInput;
        private Player _player;
        private Level _level;

        private void Awake()
        {
            _tickScheduler = new TickScheduler();
            _entityRenderer = new EntityRenderer();
            _playerInput = new PlayerInput();
            _level = new Level(CreateTestPathFinder());
            _level.Geometry.AddFloor(
                new AABB(-10f, -10f, 10f, 10f));
            _level.Geometry.AddSolid(
                new AABB(3f, -3f, 4f, 3f));
            
            _entityView = new EntityView(
                _level,
                _tickScheduler,
                _entityRenderer,
                playerPrefab,
                entityPrefab,
                entityContainer);

            _player = _level.AddPlayer(_playerInput);
            
            _tickScheduler.Register(_level);
            tickSchedulerBehaviour.Initialize(_tickScheduler);
            
            playerInputBehaviour.Initialize(_playerInput);
        }
        
        private static PathFinder CreateTestPathFinder()
        {
            var grid = new NavigationGrid(40, 40);

            for (int x = 0; x < grid.Width; x++)
            {
                for (int z = 0; z < grid.Height; z++)
                {
                    grid.SetWalkable(new BlockPos(x, z), true);
                }
            }

            return new PathFinder(grid);
        }
        
        private void Update()
        {
            playerInputBehaviour.CaptureInput();
            
            _tickScheduler?.Advance(Time.deltaTime);
        }

        private void LateUpdate()
        {
            _entityRenderer.Render(
                (float)_tickScheduler.InterpolationAlpha);
        }

        private void OnDestroy()
        {
            _entityView?.Dispose();
        }
    }
}
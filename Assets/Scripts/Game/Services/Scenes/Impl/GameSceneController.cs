using System.Collections.Generic;
using System.Threading.Tasks;
using Cinemachine;
using DG.Tweening;
using Runner.Core.Services.Resource;
using Runner.Core.Services.Ticks;
using Runner.Game.Controllers;
using Runner.Game.Controllers.Player;
using Runner.Game.Services.Config;
using Runner.Game.Services.Gameplay.Interfaces;
using UnityEngine;

namespace Runner.Game.Services.Scenes.Impl
{
    /// <summary>
    /// Контроллера игровой сцены.
    /// Отвечает за спавн игрока, монеток.
    /// </summary>
    public class GameSceneController : SceneControllerAbstract, ITickable
    {
        public override SceneName SceneName => SceneName.Game;

        private readonly IPlayerService _playerService;
        private readonly ITokenService _tokenService;
        private readonly ITickService _tickService;
        private readonly float _tokenSpawnRate;
        
        private Transform _spawnPoint;
        private Transform _tokenContainer;
        private CinemachineVirtualCamera _cinemachineCamera;
        private Transform _ground1;
        private Transform _ground2;
        private float _groundLength;
        private float _groundOffset;
        private Transform _activeGround;

        private PlayerController _playerController;
        private readonly List<TokenController> _tokenControllers = new();
        private float _tokenSpawnTimer;
        private bool _gameIsPaused;

        public GameSceneController(IResourcesService resourcesService,
            IPlayerService playerService,
            ITokenService tokenService,
            ITickService tickService,
            IConfigService configService) : base(resourcesService)
        {
            _playerService = playerService;
            _tokenService = tokenService;
            _tickService = tickService;
            
            _tokenSpawnRate = configService.GameConfig.WorldConfig.TokenSpawnRate;
        }

        /// <summary>
        /// Получение ссылок на объекты в сцене
        /// </summary>
        /// <returns></returns>
        protected override Task<bool> OnComponentInitialize()
        {
            _spawnPoint = Component.Get<Transform>("SpawnPoint");
            _tokenContainer = Component.Get<Transform>("TokenContainer");
            _cinemachineCamera = Component.Get<CinemachineVirtualCamera>("CinemachineCamera");
            _ground1 = Component.Get<Transform>("Ground1");
            _ground2 = Component.Get<Transform>("Ground2");
            
            _groundLength = Component.GetSetting<float>("GroundLength");
            _groundOffset = Component.GetSetting<float>("GroundOffset");
            
            _activeGround = _ground1;

            return Task.FromResult(true);
        }

        /// <summary>
        /// Инициализвация игровой сцены 
        /// </summary>
        /// <returns></returns>
        protected override async Task<bool> OnInitialized()
        {
            await SpawnPlayer(_spawnPoint);

            _tokenSpawnTimer = _tokenSpawnRate;

            _tickService.Subscribe(this);

            return true;
        }

        /// <summary>
        /// Деинициализация сцены
        /// </summary>
        protected override void OnReleased()
        {
            _tickService.Unsubscribe(this);
            
            _playerController.OnTokenCollected -= HandleTokenCollected;
            _playerController.OnPlayerDied -= HandlePlayerDied;
            _playerController = null;
            
            _cinemachineCamera.Follow = _spawnPoint;
            _cinemachineCamera.LookAt = _spawnPoint;
            
            ResourcesService.Release(this);
        }

        void ITickable.OnTick(float deltaTime)
        {
            if (_gameIsPaused)
            {
                return;
            }
            
            _tokenSpawnTimer -= deltaTime;

            if (_tokenSpawnTimer <= 0f)
            {
                SpawnToken();
                _tokenSpawnTimer = _tokenSpawnRate;
            }

            // проверяем перешел ли игрок на следующую платформу
            var playerGroundPosition = _playerController.transform.position.x - (_activeGround.position.x + _groundLength);
            if (playerGroundPosition > _groundOffset)
            {
                // двигаем платформу вперед
                var groundPosition = _activeGround.position;
                groundPosition.x += _groundLength * 2;
                _activeGround.position = groundPosition;

                _activeGround = _activeGround == _ground1 ? _ground2 : _ground1;
            }
        }

        /// <summary>
        /// Спавн игрока в заданой точке
        /// </summary>
        /// <param name="spawnPoint"></param>
        /// <returns></returns>
        private async Task<bool> SpawnPlayer(Transform spawnPoint)
        {
            var playerModel = _playerService.GetPlayerModel();

            if (string.IsNullOrEmpty(playerModel.PlayerPrefabName))
            {
                Debug.LogError($"[{nameof(GameSceneController)}] Failed instantiate player, prefab name is empty. Please check GameConfig!");
                return false;
            }

            _playerController = await ResourcesService.InstantiateAsync<PlayerController>(this, playerModel.PlayerPrefabName);
            var playerTransform = _playerController.transform;
            playerTransform.position = spawnPoint.position;

            _cinemachineCamera.Follow = playerTransform;
            _cinemachineCamera.LookAt = playerTransform;
            
            _playerController.Setup(playerModel);
            _playerController.OnTokenCollected += HandleTokenCollected;
            _playerController.OnPlayerDied += HandlePlayerDied;

            return true;
        }

        /// <summary>
        /// Спавн токена на линии игрока
        /// </summary>
        private async void SpawnToken()
        {
            var tokenModel = _tokenService.GetNextToken();
            var tokenPosition = _playerController.transform.position + Vector3.right * 10f;

            var tokenController = await ResourcesService.InstantiateAsync<TokenController>(this, tokenModel.PrefabName);
            tokenController.transform.SetParent(_tokenContainer);
            tokenController.transform.SetPositionAndRotation(tokenPosition, Quaternion.identity);
            tokenController.Setup(tokenModel);
            
            _tokenControllers.Add(tokenController);
        }

        /// <summary>
        /// Перезапуск игры
        /// </summary>
        private async void RestartGame()
        {
            _playerController.Dispose();
            
            ResourcesService.ReleaseInstance(this, _playerController.gameObject);
            
            foreach (var tokenController in _tokenControllers)
            {
                ResourcesService.ReleaseInstance(this, tokenController.gameObject);
            }
            _tokenControllers.Clear();
            
            var groundPosition = _activeGround.position;
            groundPosition.x += _groundLength * 2;
            _ground1.position = Vector3.zero;
            _ground2.position = Vector3.right * _groundLength;

            _activeGround = _ground1;
            
            await SpawnPlayer(_spawnPoint);

            _tokenSpawnTimer = _tokenSpawnRate;
            
            _gameIsPaused = false;
        }

        /// <summary>
        /// Обработчик собранной монеты, уничтожает объект
        /// </summary>
        /// <param name="tokenController"></param>
        private void HandleTokenCollected(TokenController tokenController)
        {
            _tokenControllers.Remove(tokenController);
            ResourcesService.ReleaseInstance(this, tokenController.gameObject);
        }

        /// <summary>
        /// Обработчик смерти игрока
        /// </summary>
        private void HandlePlayerDied()
        {
            _gameIsPaused = true;
            
            // перезапускаем игру через 2 сек, чтобы отыграла анимация смерти
            DOVirtual.DelayedCall(2f, RestartGame);
        }
    }
}
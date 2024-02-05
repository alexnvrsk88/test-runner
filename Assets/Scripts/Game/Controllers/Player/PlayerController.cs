using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Runner.Core.Utils;
using Runner.Game.Animations;
using Runner.Game.Controllers.Player.Behaviour;
using Runner.Game.Models.Player;
using Runner.Game.Models.Token;
using Runner.Shared.Shared;
using UnityEngine;
using UnityEngine.Pool;

namespace Runner.Game.Controllers.Player
{
    /// <summary>
    /// Контроллер персонажа игры. Инициализируется моделью игрока, которая содержит базовые параметры.
    /// </summary>
    public class PlayerController : MonoBehaviour, IPlayerController
    {
        public event Action<TokenController> OnTokenCollected;
        public event Action OnPlayerDied;

        IPlayerModel IPlayerController.PlayerModel => _playerModel;
        Rigidbody2D IPlayerController.Rigidbody => _body;
        Animator IPlayerController.Animator => _animator;

        private int ObstaclesLayer => LayerMask.NameToLayer("Obstacles");
        
        [SerializeField] private Rigidbody2D _body;
        [SerializeField] private Animator _animator;

        private IPlayerModel _playerModel;

        // список активных контроллеров поведения
        private readonly List<IPlayerBehaviour> _activeBehaviours = new();
        // список контроллеров поведения относящихся к параметрам игрока
        private readonly Dictionary<PlayerParameter, IPlayerBehaviour> _behavioursMap = new();
        // словарь активных эффектов на игроке
        private readonly Dictionary<PlayerParameter, List<ITokenEffectModel>> _parameterEffects = new();

        /// <summary>
        /// Инициализация контроллера при спавне игрока, создаются базоые контроллеры поведения
        /// </summary>
        /// <param name="playerModel"></param>
        public void Setup(IPlayerModel playerModel)
        {
            _playerModel = playerModel;
            
            _behavioursMap.Clear();
            _behavioursMap.Add(PlayerParameter.RunningSpeed, new RunningBehaviour(this));
            _behavioursMap.Add(PlayerParameter.FlightHeight, new FlyBehaviour(this));
            
            _activeBehaviours.Clear();

            _animator.SetBool(AnimationConstants.GroundedAnimatorId, true);
            _animator.SetBool(AnimationConstants.DeadAnimatorId, false);
            _animator.Play(AnimationConstants.SpawnAnimation, -1, 0f);

            DOVirtual.DelayedCall(2f, StartRun);
        }

        public void Dispose()
        {
            _behavioursMap.Clear();

            for (var i = 0; i < _activeBehaviours.Count; i++)
            {
                _activeBehaviours[i].Deactivate();
            }
            _activeBehaviours.Clear();
            
            _animator.SetBool(AnimationConstants.GroundedAnimatorId, true);
            _animator.SetBool(AnimationConstants.DeadAnimatorId, false);
            _animator.ResetTrigger(AnimationConstants.HurtAnimatorId);
        }
        
        IReadOnlyList<ITokenEffectModel> IPlayerController.GetParameterEffects(PlayerParameter parameter)
        {
            return _parameterEffects.TryGetValue(parameter, out var effects)
                ? effects
                : Array.Empty<ITokenEffectModel>();
        }
        
        /// <summary>
        /// Активация бега 
        /// </summary>
        private void StartRun()
        {
            var runningBehaviour = _behavioursMap[PlayerParameter.RunningSpeed];
            _activeBehaviours.Add(runningBehaviour);
            
            runningBehaviour.Activate();
        }

        private void Update()
        {
            if (Input.GetButtonDown("Jump") || Input.GetMouseButtonDown(0))
            {
                TryAddJumpBehaviour();
            }

            var deltaTime = Time.deltaTime;
            
            UpdateTokenEffects(deltaTime);

            UpdateActiveBehaviours(deltaTime);
        }

        private void FixedUpdate()
        {
            for (var i = 0; i < _activeBehaviours.Count; i++)
            {
                _activeBehaviours[i].FixedUpdate(Time.fixedDeltaTime);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (CheckObstacleTriggered(other.gameObject))
            {
                return;
            }

            CheckTokenTriggered(other.gameObject);
        }

        /// <summary>
        /// Проверка столкновения с монеткой
        /// </summary>
        /// <param name="other"></param>
        private void CheckTokenTriggered(GameObject other)
        {
            if (other.TryGetComponent<TokenController>(out var triggeredTokenController) == false)
            {
                return;
            }

            var tokenModel = triggeredTokenController.TokenModel;
            AddTokenEffects(tokenModel);

            OnTokenCollected.SafeInvoke(triggeredTokenController);
        }

        /// <summary>
        /// Проверка столкновения с препятствием
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        private bool CheckObstacleTriggered(GameObject other)
        {
            if (other.layer != ObstaclesLayer)
            {
                return false;
            }
            
            _activeBehaviours.Clear();
            _animator.SetTrigger(AnimationConstants.HurtAnimatorId);
            _animator.SetBool(AnimationConstants.DeadAnimatorId, true);
            
            OnPlayerDied.SafeInvoke();
            
            return true;
        }

        /// <summary>
        /// Обновление поведений игрока
        /// </summary>
        /// <param name="deltaTime"></param>
        private void UpdateActiveBehaviours(float deltaTime)
        {
            var inactiveBehaviours = ListPool<IPlayerBehaviour>.Get();

            for (var i = 0; i < _activeBehaviours.Count; i++)
            {
                var playerBehaviour = _activeBehaviours[i];
                playerBehaviour.Update(deltaTime);
                if (playerBehaviour.IsActive == false)
                {
                    inactiveBehaviours.Add(playerBehaviour);
                }
            }

            // деактивация завершенных эффектов
            for (var i = 0; i < inactiveBehaviours.Count; i++)
            {
                var inactiveBehaviour = inactiveBehaviours[i];
                inactiveBehaviour.Deactivate();
                _activeBehaviours.Remove(inactiveBehaviour);
            }

            ListPool<IPlayerBehaviour>.Release(inactiveBehaviours);
        }

        /// <summary>
        /// Активация прыжка
        /// </summary>
        private void TryAddJumpBehaviour()
        {
            if (_activeBehaviours.Any(b => b is FlyBehaviour { IsActive: true }) == false && 
                _activeBehaviours.Any(b => b is JumpBehaviour) == false)
            {
                var jumpBehaviour = new JumpBehaviour(this);
                jumpBehaviour.Activate();
                
                _activeBehaviours.Add(jumpBehaviour);
            }
        }

        /// <summary>
        /// Добавление эффекта собранного токена
        /// </summary>
        /// <param name="tokenModel"></param>
        private void AddTokenEffects(ITokenModel tokenModel)
        {
            for (var i = 0; i < tokenModel.Effects.Count; i++)
            {
                var tokenEffectModel = tokenModel.Effects[i];
                if (_parameterEffects.TryGetValue(tokenEffectModel.Parameter, out var effects))
                {
                    effects.Add(tokenEffectModel);
                }
                else
                {
                    effects = new List<ITokenEffectModel>()
                    {
                        tokenEffectModel
                    };
                    _parameterEffects[tokenEffectModel.Parameter] = effects;
                }

                // активация поведения для эффекта если он ещё не активен
                if (_behavioursMap.TryGetValue(tokenEffectModel.Parameter, out var parameterBehaviour) &&
                    _activeBehaviours.Contains(parameterBehaviour) == false)
                {
                    _activeBehaviours.Add(parameterBehaviour);
                }
            }
        }

        /// <summary>
        /// Обновление таймеров эффектов
        /// </summary>
        /// <param name="deltaTime"></param>
        private void UpdateTokenEffects(float deltaTime)
        {
            var completedEffects = ListPool<ITokenEffectModel>.Get();
            
            foreach (var (_, effects) in _parameterEffects)
            {
                for (var i = 0; i < effects.Count; i++)
                {
                    var effect = effects[i];
                    effect.UpdateEffectTime(deltaTime);
                    
                    if (effect.IsAlive == false)
                    {
                        completedEffects.Add(effect);
                    }
                }
            }

            // очистка завершенных эффектов
            for (var i = 0; i < completedEffects.Count; i++)
            {
                var completedEffect = completedEffects[i];
                _parameterEffects[completedEffect.Parameter].Remove(completedEffect);
            }

            ListPool<ITokenEffectModel>.Release(completedEffects);
        }
    }
}
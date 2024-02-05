using Runner.Game.Models.Player;
using Runner.Shared.Shared;
using UnityEngine;

namespace Runner.Game.Controllers.Player.Behaviour
{
    public abstract class BehaviourAbstract : IPlayerBehaviour
    {
        public virtual bool IsActive => true;

        protected Animator Animator => _playerController.Animator;
        protected IPlayerModel PlayerModel => _playerController.PlayerModel;
        protected Rigidbody2D Rigidbody => _playerController.Rigidbody;

        private readonly IPlayerController _playerController;

        protected BehaviourAbstract(IPlayerController playerController)
        {
            _playerController = playerController;
        }

        /// <summary>
        /// Получение действия от эффекта на параметр, возвращает пару абсолютное значение и множитель 
        /// </summary>
        /// <param name="playerParameter"></param>
        /// <returns></returns>
        protected (float effectValue, float effectMultiplier) GetEffectsValue(PlayerParameter playerParameter)
        {
            var effectValue = 0f;
            var effectMultiplier = 0f;

            var effects = _playerController.GetParameterEffects(playerParameter);
            for (var i = 0; i < effects.Count; i++)
            {
                var tokenEffectModel = effects[i];
                if (tokenEffectModel.IsMultiplier)
                {
                    effectMultiplier += tokenEffectModel.Value;
                }
                else
                {
                    effectValue += tokenEffectModel.Value;
                }
            }

            return (effectValue, effectMultiplier);
        }

        public virtual void Activate()
        {
        }
        
        public virtual void Deactivate()
        {
        }
        
        public virtual void Update(float deltaTime)
        {
        }

        public virtual void FixedUpdate(float deltaTime)
        {
        }
    }
}
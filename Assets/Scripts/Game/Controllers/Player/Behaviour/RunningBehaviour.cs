using Runner.Game.Animations;
using Runner.Shared.Shared;
using UnityEngine;

namespace Runner.Game.Controllers.Player.Behaviour
{
    /// <summary>
    /// Поведение бега игрока. Двигает игрока по горизонтали
    /// </summary>
    public sealed class RunningBehaviour : BehaviourAbstract
    {
        private const float MIN_SPEED = .5f;
        
        private float BaseSpeed => PlayerModel.GetBaseParameterValue(PlayerParameter.RunningSpeed);
        
        private float _runningSpeed;
        
        public RunningBehaviour(IPlayerController playerController) : base(playerController)
        {
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            
            _runningSpeed = CalculateSpeed();
        }

        public override void FixedUpdate(float deltaTime)
        {
            base.FixedUpdate(deltaTime);
            
            Rigidbody.position += Vector2.right * _runningSpeed * deltaTime;
            
            Animator.SetFloat(AnimationConstants.VelocityXAnimatorId, Mathf.Abs(_runningSpeed) / BaseSpeed);
        }

        private float CalculateSpeed()
        {
            var (effectValue, effectMultiplier) = GetEffectsValue(PlayerParameter.RunningSpeed);

            var totalValue = BaseSpeed + effectValue;
            var multiplier = 1f + effectMultiplier;
            
            return Mathf.Max(MIN_SPEED, totalValue * multiplier);
        }
    }
}
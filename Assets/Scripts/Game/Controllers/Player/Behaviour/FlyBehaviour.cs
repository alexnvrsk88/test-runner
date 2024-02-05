using Runner.Game.Animations;
using Runner.Shared.Shared;
using UnityEngine;

namespace Runner.Game.Controllers.Player.Behaviour
{
    /// <summary>
    /// Поведение полета игрока. Поднимает игрока на заданную высоту, пока действует эффект
    /// </summary>
    public sealed class FlyBehaviour : BehaviourAbstract
    {
        public override bool IsActive => _startFlight || _isGrounded == false;

        private const float MAX_HEIGHT = 5f;
        private const float SHELL_SIZE = 0.01f;
        private const float TAKE_OFF_SPEED = 2f;

        private readonly ContactFilter2D _contactFilter;
        private readonly RaycastHit2D[] _hitBuffer = new RaycastHit2D[16];

        private float _flightHeight;
        private float _verticalSpeed;
        private bool _startFlight;
        private bool _isGrounded;

        public FlyBehaviour(IPlayerController playerController) : base(playerController)
        {
            _contactFilter.useTriggers = false;
            _contactFilter.SetLayerMask(LayerMask.GetMask("Default"));
            _contactFilter.useLayerMask = true;
        }

        public override void Activate()
        {
            _startFlight = true;
        }

        public override void Deactivate()
        {
            Animator.SetBool(AnimationConstants.FlightAnimatorId, false);
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            _flightHeight = CalculateFlyHeight();

            if (_flightHeight <= 0)
            {
                _verticalSpeed = -TAKE_OFF_SPEED;
            }
            else
            {
                _verticalSpeed = Rigidbody.position.y < _flightHeight ? TAKE_OFF_SPEED : 0f;
            }
            Animator.SetBool(AnimationConstants.FlightAnimatorId, true);
        }

        public override void FixedUpdate(float deltaTime)
        {
            base.FixedUpdate(deltaTime);

            var moveDirection = Vector2.up * (_verticalSpeed * Time.fixedDeltaTime);
            var distance = moveDirection.magnitude;

            var count = Rigidbody.Cast(moveDirection, _contactFilter, _hitBuffer, distance + SHELL_SIZE);
            _isGrounded = count > 0;

            if (_startFlight && _isGrounded == false)
            {
                _startFlight = false;
            }

            // проверка соприкосновения с платформой для вырвнимания по вертикали
            for (var i = 0; i < count; i++)
            {
                var hitDistance = _hitBuffer[i].distance - SHELL_SIZE;
                distance = hitDistance < distance ? hitDistance : distance;
            }

            Rigidbody.position += moveDirection.normalized * distance;

            Animator.SetBool(AnimationConstants.GroundedAnimatorId, _isGrounded);
        }

        private float CalculateFlyHeight()
        {
            var (effectValue, effectMultiplier) = GetEffectsValue(PlayerParameter.FlightHeight);
            var multiplier = 1f + effectMultiplier;

            return Mathf.Clamp(effectValue * multiplier, 0, MAX_HEIGHT);
        }
    }
}
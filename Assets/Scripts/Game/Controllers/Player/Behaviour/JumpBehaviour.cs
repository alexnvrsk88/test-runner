using Runner.Game.Animations;
using UnityEngine;

namespace Runner.Game.Controllers.Player.Behaviour
{
    /// <summary>
    /// Поведение прыжка. Дает моментальный импульс по вертикали игроку
    /// </summary>
    public class JumpBehaviour : BehaviourAbstract
    {
        public override bool IsActive => _isGrounded == false;

        private const float SHELL_SIZE = 0.01f;
        
        private readonly ContactFilter2D _contactFilter;
        private readonly RaycastHit2D[] _hitBuffer = new RaycastHit2D[16];
        
        private float _jumpSpeed;
        private bool _isGrounded;
        
        public JumpBehaviour(IPlayerController playerController) : base(playerController)
        {
            _contactFilter.useTriggers = false;
            _contactFilter.SetLayerMask(LayerMask.GetMask("Default"));
            _contactFilter.useLayerMask = true;
        }

        public override void Activate()
        {
            _jumpSpeed = 6f;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            _jumpSpeed += Physics2D.gravity.y * deltaTime;
        }
        
        public override void FixedUpdate(float deltaTime)
        {
            base.FixedUpdate(deltaTime);
            
            var moveDirection = Vector2.up * (_jumpSpeed * Time.fixedDeltaTime);
            var distance = moveDirection.magnitude;

            var count = Rigidbody.Cast(moveDirection, _contactFilter, _hitBuffer, distance + SHELL_SIZE);
            _isGrounded = count > 0;

            // проверка соприкосновения с платформой для вырвнимания по вертикали
            for (var i = 0; i < count; i++)
            {
                var hitDistance = _hitBuffer[i].distance - SHELL_SIZE;
                distance = hitDistance < distance ? hitDistance : distance;
            }

            Rigidbody.position += moveDirection.normalized * distance;

            Animator.SetBool(AnimationConstants.GroundedAnimatorId, _isGrounded);
        }
    }
}
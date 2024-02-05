using UnityEngine;

namespace Runner.Game.Animations
{
    public static class AnimationConstants
    {
        public static readonly int GroundedAnimatorId = Animator.StringToHash("grounded");
        public static readonly int VelocityXAnimatorId = Animator.StringToHash("velocityX");
        public static readonly int FlightAnimatorId = Animator.StringToHash("flight");
        public static readonly int HurtAnimatorId = Animator.StringToHash("hurt");
        public static readonly int DeadAnimatorId = Animator.StringToHash("dead");
        public static readonly int SpawnAnimation = Animator.StringToHash("Player-Spawn");
    }
}
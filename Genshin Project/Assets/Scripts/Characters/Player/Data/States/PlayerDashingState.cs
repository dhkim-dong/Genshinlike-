using UnityEngine;
using UnityEngine.InputSystem;

namespace GenshinImpactMovementSystem
{
    public class PlayerDashingState : PlayerGroundedState
    {
        private PlayerDashData dashData; // dashData를 많이 사용할 예정이므로 캐싱해준다.

        private float startTime;

        private int consecutiveDashesUsed;

        public PlayerDashingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            dashData = movementData.DashData;
        }

        #region IState Methods
        public override void Enter()
        {
            base.Enter();

            stateMachine.ReusableData.MovementSpeedModifier = dashData.SpeedModifier;
            AddForceOnTransitionFromStationaryState();

            UpdateConsecutiveDashes();

            startTime = Time.time;
        }

        public override void OnAnimationTranstionEvent()
        {
            base.OnAnimationTranstionEvent();

            if(stateMachine.ReusableData.MovementInput == Vector2.zero)
            {
                stateMachine.ChangeState(stateMachine.IdleState);

                return;
            }

            stateMachine.ChangeState(stateMachine.SprintState);
        }
        #endregion

        #region Main Methods
        private void AddForceOnTransitionFromStationaryState()
        {
            if(stateMachine.ReusableData.MovementInput != Vector2.zero)
            {
                return;
            }

            Vector3 characterRotationDirection = stateMachine.Player.transform.forward;
            characterRotationDirection.y = 0f;

            stateMachine.Player.Rigidbody.velocity = characterRotationDirection * GetMovementSpeed();

            // 연속 대쉬 제한 기능.
            // 시간을 측정해서, Limit에 해당하면 비활성화한다.
        }

        private void UpdateConsecutiveDashes()
        {
            if (!IsConSecutive()) // 연속 대시가 아닌 경우
            {
                consecutiveDashesUsed = 0;
            }

            ++consecutiveDashesUsed;

            if(consecutiveDashesUsed == dashData.ConsecutiveDashesLimitAmount)
            {
                consecutiveDashesUsed = 0;

                stateMachine.Player.Input.DisableActionFor(stateMachine.Player.Input.PlayerActions.Dash, dashData.DashLimitReachedCooldown);
            }
        }

        private bool IsConSecutive()
        {
            return Time.time < startTime + dashData.TimeToBeConsideredConsecutive; // 게임 시작 후 시간(5) < 4 + 2 (true) 
        }
        #endregion

        #region InputMethod

        protected override void OnMovementCancled(InputAction.CallbackContext context)
        {
           
        }

        protected override void OnDashStarted(InputAction.CallbackContext context)
        {
          
        }
        #endregion
    }
}

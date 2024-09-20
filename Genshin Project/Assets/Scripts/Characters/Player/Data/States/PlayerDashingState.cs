using UnityEngine;
using UnityEngine.InputSystem;

namespace GenshinImpactMovementSystem
{
    public class PlayerDashingState : PlayerGroundedState
    {
        private PlayerDashData dashData; // dashData�� ���� ����� �����̹Ƿ� ĳ�����ش�.

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

            // ���� �뽬 ���� ���.
            // �ð��� �����ؼ�, Limit�� �ش��ϸ� ��Ȱ��ȭ�Ѵ�.
        }

        private void UpdateConsecutiveDashes()
        {
            if (!IsConSecutive()) // ���� ��ð� �ƴ� ���
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
            return Time.time < startTime + dashData.TimeToBeConsideredConsecutive; // ���� ���� �� �ð�(5) < 4 + 2 (true) 
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

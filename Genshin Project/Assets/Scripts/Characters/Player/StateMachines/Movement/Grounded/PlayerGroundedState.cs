using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GenshinImpactMovementSystem
{
    public class PlayerGroundedState : PlayerMovementState
    {
        private SlopeData slopeData;

        public PlayerGroundedState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            slopeData = stateMachine.Player.ColliderUtility.SlopeData;
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            Float();
        }

        private void Float()
        {
            Vector3 capsuleColliderCenterInWorldSpace = stateMachine.Player.ColliderUtility.CapsuleColliderData.Collider.bounds.center;

            Ray downwardsRayFromCapsuleCenter = new Ray(capsuleColliderCenterInWorldSpace, Vector3.down); // Vector3.down 항상 월드기준 아래, transform.up은 회전에 영향


            // Quert.Ignore : 레이어가 있찌만 Trigger Colliders인 개체는 무시한다.

            if(Physics.Raycast(downwardsRayFromCapsuleCenter, out RaycastHit hit, slopeData.FloatRayDistance, stateMachine.Player.LayerData.GroundLayer, QueryTriggerInteraction.Ignore))
            {
                float groundAngle = Vector3.Angle(hit.normal, -downwardsRayFromCapsuleCenter.direction);

                float slopeSpeedModifier = SetSlopeSpeedModifierOnAngle(groundAngle);

                if(slopeSpeedModifier == 0f)
                {
                    return;
                }

                float distanceToFloatingPoint = stateMachine.Player.ColliderUtility.CapsuleColliderData.ColliderCenterInLocalSpace.y * stateMachine.Player.transform.localScale.y - hit.distance;

                if(distanceToFloatingPoint == 0f)
                {
                    return;
                }

                float amountToLit = distanceToFloatingPoint * slopeData.StepReachForce - GetPlayerVerticalVelocity().y;

                Vector3 liftForce = new Vector3(0f, amountToLit, 0f);

                stateMachine.Player.Rigidbody.AddForce(liftForce, ForceMode.VelocityChange);
            }
        }

        private float SetSlopeSpeedModifierOnAngle(float angle)
        {
            float slopeSpeedModifier = movementData.SlopeSpeedAngles.Evaluate(angle);

            stateMachine.ReusableData.MovementOnSlopeSpeedModifier = slopeSpeedModifier;

            return slopeSpeedModifier;
        }

        protected override void AddInputActionsCallbacks()
        {
            base.AddInputActionsCallbacks();

            stateMachine.Player.Input.PlayerActions.Movement.canceled += OnMovementCancled;

            stateMachine.Player.Input.PlayerActions.Dash.started += OnDashStarted;
        }


        protected override void RemoveInputActionCallbacks()
        {
            stateMachine.Player.Input.PlayerActions.Movement.canceled -= OnMovementCancled;
            
            stateMachine.Player.Input.PlayerActions.Dash.canceled -= OnDashStarted;

        }

        protected virtual void OnMove()
        {
            if (stateMachine.ReusableData.ShouldWalk)
            {
                stateMachine.ChangeState(stateMachine.WalkState);

                return;
            }

            stateMachine.ChangeState(stateMachine.RunningState);
        }

        protected virtual void OnMovementCancled(InputAction.CallbackContext context)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }

        protected virtual void OnDashStarted(InputAction.CallbackContext context)
        {
            stateMachine.ChangeState(stateMachine.DashingState);
        }
    }
}

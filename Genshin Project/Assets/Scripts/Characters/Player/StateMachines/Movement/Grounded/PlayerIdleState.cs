using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace GenshinImpactMovementSystem
{
    public class PlayerIdleState : PlayerGroundedState
    {
        public PlayerIdleState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();

            stateMachine.ReusableData.MovementSpeedModifier = 0f;

            ResetVelocity();
        }

        public override void Update()
        {
            base.Update();

            if(stateMachine.ReusableData.MovementInput == Vector2.zero)
            {
                return;
            }

            OnMove();
        }


        public override void Exit() 
        {
            stateMachine.ReusableData.MovementSpeedModifier = 1f;
        }
    }
}

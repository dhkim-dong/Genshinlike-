using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenshinImpactMovementSystem
{
    public class PlayerMovementStateMachine : StateMachine
    {
        // private set vs only set; 
        // set Only : settable from the constructor. ��𼭵� read only�� ���ȴ�.
        // private set : can be set from everywhere inside that class : Ŭ���� ���ο����� set����

        public Player Player { get; }
        public PlayerStateReusableData ReusableData { get; }

        public PlayerIdleState IdleState { get; }  
        public PlayerDashingState DashingState { get; }  
        public PlayerWalking WalkState { get; }  
        public PlayerRunningState RunningState { get; }  
        public PlayerSprintState SprintState { get; }  

        public PlayerMovementStateMachine(Player player) // Caching ���� ����
        {
            Player = player;
            ReusableData = new PlayerStateReusableData();

            IdleState = new PlayerIdleState(this);
            DashingState = new PlayerDashingState(this);

            WalkState = new PlayerWalking(this);
            RunningState = new PlayerRunningState(this);
            SprintState = new PlayerSprintState(this);
        }
    }
}

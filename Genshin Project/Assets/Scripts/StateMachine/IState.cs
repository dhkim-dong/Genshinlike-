using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenshinImpactMovementSystem
{
    public interface IState
    {
        public void Enter(); // State 상태로 변경
        public void Exit(); // State 상태를 나갈 때

        public void HandleInput();

        public void Update(); // Monobehaviour의 Update

        public void PhysicsUpdate(); // FixedUpdate
    }
}

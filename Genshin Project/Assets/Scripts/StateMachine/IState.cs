using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenshinImpactMovementSystem
{
    public interface IState
    {
        public void Enter(); // State ���·� ����
        public void Exit(); // State ���¸� ���� ��

        public void HandleInput();

        public void Update(); // Monobehaviour�� Update

        public void PhysicsUpdate(); // FixedUpdate
    }
}

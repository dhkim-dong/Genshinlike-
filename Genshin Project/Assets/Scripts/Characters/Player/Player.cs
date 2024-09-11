using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenshinImpactMovementSystem
{
    [RequireComponent(typeof(PlayerInput))]
    public class Player : MonoBehaviour
    {
        [field:Header("Reference")]
        [field:SerializeField] public PlayerSO Data { get; private set; }
        
        public Rigidbody Rigidbody { get; private set; }

        public Transform MainCameraTransform { get; private set; }

        public PlayerInput Input { get; private set; }
        private PlayerMovementStateMachine movementStateMachine;

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
            Input = GetComponent<PlayerInput>();
            MainCameraTransform = Camera.main.transform; // 캐싱하는 이유 : Camera.main을 찾는데 비용이 많이 사용되기 때문이다.
            movementStateMachine = new PlayerMovementStateMachine(this);
        }

        private void Start()
        {
            movementStateMachine.ChangeState(movementStateMachine.IdleState);
        }

        private void Update()
        {
            movementStateMachine.HandleInput();

            movementStateMachine.Update();
        }

        private void FixedUpdate()
        {
            movementStateMachine.PhysicsUpdate();
        }
    }
}

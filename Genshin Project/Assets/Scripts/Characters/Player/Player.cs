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
        [field : Header("Collisions")]
        [field : SerializeField] public CapsuleColliderUtility ColliderUtility { get; private set; }
        [field : SerializeField] public PlayerLayerData LayerData { get; private set; }
        public Rigidbody Rigidbody { get; private set; }

        public Transform MainCameraTransform { get; private set; }

        public PlayerInput Input { get; private set; }
        private PlayerMovementStateMachine movementStateMachine;

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
            Input = GetComponent<PlayerInput>();

            ColliderUtility.Initialize(gameObject);
            ColliderUtility.CalculateCapsuleColliderDimesions();

            MainCameraTransform = Camera.main.transform; // 캐싱하는 이유 : Camera.main을 찾는데 비용이 많이 사용되기 때문이다.
            movementStateMachine = new PlayerMovementStateMachine(this);
        }

        private void OnValidate() // 실행을 하지 않고 에디트 모드에서 변경해도 적용을 확인할 수 있게 해주는 이벤트 함수
        {
            ColliderUtility.Initialize(gameObject);
            ColliderUtility.CalculateCapsuleColliderDimesions();
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

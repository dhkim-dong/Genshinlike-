using UnityEngine;
using UnityEngine.InputSystem;

namespace GenshinImpactMovementSystem
{
    public class PlayerMovementState : IState
    {
        protected PlayerMovementStateMachine stateMachine;

        protected PlayerGroundedData movementData;
        public PlayerMovementState(PlayerMovementStateMachine playerMovementStateMachine)
        {
            stateMachine = playerMovementStateMachine;

            movementData = stateMachine.Player.Data.GroundedData;

            InitializeData();
        }

        private void InitializeData()
        {
            stateMachine.ReusableData.TimeToReachTargetRotation = movementData.BaseRotationData.TargetRotationReachTime;
        }

        #region IStateMethod

        public virtual void Enter()
        {
            Debug.Log("State: " + GetType().Name);

            AddInputActionsCallbacks();
        }

 
        public virtual void Exit()
        {
            RemoveInputActionCallbacks();
        }

       
        public virtual void HandleInput()
        {
            ReadMovementInput();
        }

        public virtual void Update()
        {

        }

        public virtual void PhysicsUpdate()
        {
            Move();
        }

        public virtual void OnAnimationEnterEvent()
        {
            throw new System.NotImplementedException();
        }

        public virtual void OnAnimationExitEvent()
        {
            throw new System.NotImplementedException();
        }

        public virtual void OnAnimationTranstionEvent()
        {
            throw new System.NotImplementedException();
        }

        #endregion


        #region Main Methods
        private void ReadMovementInput()
        {
            stateMachine.ReusableData.MovementInput = stateMachine.Player.Input.PlayerActions.Movement.ReadValue<Vector2>();
        }

        private void Move()
        {
            if( stateMachine.ReusableData.MovementInput == Vector2.zero || stateMachine.ReusableData.MovementSpeedModifier == 0f)
            {
                return;
            }

            Vector3 movementDirection = GetMovementInputDirection();

            float targetRotationYAngle = Rotate(movementDirection);

            Vector3 targetRotationDirection = GetTargetRotationDirection(targetRotationYAngle);
            float movementSpeed = GetMovementSpeed();

            Vector3 currentPlayerHorizontalVelocity = GetPlayerHorizontalVelocity();
            stateMachine.Player.Rigidbody.AddForce(targetRotationDirection * movementSpeed - currentPlayerHorizontalVelocity, ForceMode.VelocityChange); // 기존 힘에 추가되는 방식( 무한 속도 증가)
            // 해결 하기 위해서 추가하려는 힘에서 기존 속도를 제거한다.
        }

       

        private float Rotate(Vector3 direction)
        {
            float directionAngle = UpdateTargetRotation(direction);

            RotateTowardsTargetRotation();
            return directionAngle;
        }

       

        private void UpdateTargetRotationData(float targetAngle)
        {
            stateMachine.ReusableData.CurrentTargetRotation.y = targetAngle;

            stateMachine.ReusableData.DampedTargetRotationPassedTime.y = 0f;
        }

        private float AddCameraRotationAngle(float angle)
        {
            angle += stateMachine.Player.MainCameraTransform.eulerAngles.y;

            if (angle > 360f)
            {
                angle -= 360f;
            }

            return angle;
        }

        private float GetDirectionAngle(Vector3 direction)
        {
            // Mathf. Atan2 
            // 앞으로 나아가고 있다. w (0, 1)
            // 유니티에서 90도로 회전해서 움직이라고 하면, 캐릭터 기준이 아닌 월드 기준 오른쪽으로 움직인다.
            float directionAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            if (directionAngle < 0f)
            {
                directionAngle += 360f;
            }

            return directionAngle;
        }

        #endregion

        #region Reusable Methods
        protected Vector3 GetMovementInputDirection()
        {
            return new Vector3(stateMachine.ReusableData.MovementInput.x, 0f, stateMachine.ReusableData.MovementInput.y);
        }

        protected float GetMovementSpeed()
        {
            return movementData.BaseSpeed * stateMachine.ReusableData.MovementSpeedModifier * stateMachine.ReusableData.MovementOnSlopeSpeedModifier;
        }
        protected Vector3 GetPlayerHorizontalVelocity()
        {
            Vector3 PlayerHorizontalVelocity = stateMachine.Player.Rigidbody.velocity;

            PlayerHorizontalVelocity.y = 0f;

            return PlayerHorizontalVelocity;
        }

        protected Vector3 GetPlayerVerticalVelocity()
        {
            return new Vector3(0f, stateMachine.Player.Rigidbody.velocity.y, 0f);
        }

        protected void RotateTowardsTargetRotation()
        {
            float currentYAngle = stateMachine.Player.Rigidbody.rotation.eulerAngles.y;

            if( currentYAngle == stateMachine.ReusableData.CurrentTargetRotation.y)
            {
                return;
            }

            float smoothedYAngle = Mathf.SmoothDampAngle(currentYAngle, stateMachine.ReusableData.CurrentTargetRotation.y, 
                ref stateMachine.ReusableData.DampedTargetRotationCurrentVelocity.y, stateMachine.ReusableData.TimeToReachTargetRotation.y - stateMachine.ReusableData.DampedTargetRotationCurrentVelocity.y);

            stateMachine.ReusableData.DampedTargetRotationPassedTime.y += Time.deltaTime;

            Quaternion targetRotation = Quaternion.Euler(0f, smoothedYAngle, 0f);

            stateMachine.Player.Rigidbody.MoveRotation(targetRotation);
        }

        protected float UpdateTargetRotation(Vector3 direction, bool shouldConsiderCameraRotation = true)
        {
            float directionAngle = GetDirectionAngle(direction);

            if(shouldConsiderCameraRotation)
            {
                directionAngle = AddCameraRotationAngle(directionAngle);
            }

    
            if (directionAngle != stateMachine.ReusableData.CurrentTargetRotation.y)
            {
                UpdateTargetRotationData(directionAngle);
            }

            return directionAngle;
        }
        protected Vector3 GetTargetRotationDirection(float targetAngle)
        {
            return Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }

        protected void ResetVelocity()
        {
            stateMachine.Player.Rigidbody.velocity = Vector3.zero;
        }

        protected virtual void AddInputActionsCallbacks()
        {
            stateMachine.Player.Input.PlayerActions.WalkToggle.started += OnWalkToggleStarted;
        }


        protected virtual void RemoveInputActionCallbacks()
        {
            stateMachine.Player.Input.PlayerActions.WalkToggle.started -= OnWalkToggleStarted;
        }
        #endregion

        protected virtual void OnWalkToggleStarted(InputAction.CallbackContext context)
        {
            stateMachine.ReusableData.ShouldWalk = !stateMachine.ReusableData.ShouldWalk;

        }


    }

}

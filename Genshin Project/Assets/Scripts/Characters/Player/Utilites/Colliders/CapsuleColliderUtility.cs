using System;
using UnityEngine;

namespace GenshinImpactMovementSystem
{
    [System.Serializable]
    public class CapsuleColliderUtility
    {
       public CapsuleColliderData CapsuleColliderData { get; private set; }
       [field: SerializeField]  public DefaultColliderData DefaultColliderData { get; private set; }
       [field : SerializeField]  public SlopeData SlopeData { get; private set; }

        public void Initialize(GameObject gameObject)
        {
            if(CapsuleColliderData != null)
            {
                return;
            }

            CapsuleColliderData = new CapsuleColliderData();

            CapsuleColliderData.Initialize(gameObject);
        }

        public void CalculateCapsuleColliderDimesions()
        {
            SetCapsuleCOlliderRadius(DefaultColliderData.Radius);
            SetCapsuleCOlliderHeight(DefaultColliderData.Height * (1f - SlopeData.StepHeightPercentage)); // 높이 재계산이 필요

            ReCalculateCapsuleColliderCenter();

            float halfColliderHeight = CapsuleColliderData.Collider.height / 2f;

            if (halfColliderHeight < CapsuleColliderData.Collider.radius)
            {
                SetCapsuleCOlliderRadius(halfColliderHeight);
            }

            CapsuleColliderData.UpdateColliderData();
        }

        

        public void SetCapsuleCOlliderRadius(float radius)
        {
            CapsuleColliderData.Collider.radius = radius;
        }

        public void SetCapsuleCOlliderHeight(float height)
        {
            CapsuleColliderData.Collider.height = height;
        }

        public void ReCalculateCapsuleColliderCenter()
        {
            float colliderHeightDifference = DefaultColliderData.Height - CapsuleColliderData.Collider.height;

            Vector3 newColliderCenter = new Vector3(0f, DefaultColliderData.CenterY + (colliderHeightDifference / 2f), 0f);

            CapsuleColliderData.Collider.center = newColliderCenter;
        }
    }
}

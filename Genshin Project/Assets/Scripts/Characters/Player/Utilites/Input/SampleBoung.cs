using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenshinImpactMovementSystem
{
    public class SampleBoung : MonoBehaviour
    {
        private SkinnedMeshRenderer meshRenderer;

        private void Awake()
        {
            meshRenderer = GetComponent<SkinnedMeshRenderer>();
        }

        private void Start()
        {
            Debug.Log(meshRenderer.bounds.size);
        }
    }
}

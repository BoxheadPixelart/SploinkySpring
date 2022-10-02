using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Storm.SploinkySpring;

namespace Storm.SploinkySpring
{
    public class SploinkyTransform : MonoBehaviour
    {
        public Transform target;
        public Sploinky.TransformSpring transformSpring = new Sploinky.TransformSpring();
        public Vector3 positionOffset;
        public Vector3 rotationOffset; 
        public Vector3 scaleOffset; 
        // Start is called before the first frame update

        
        public void Update()
        {
            transformSpring.Spring(target,positionOffset);
        }
        private void LateUpdate()
        {
            transform.localScale = transformSpring.scale.Output + scaleOffset;
            transform.SetPositionAndRotation(transformSpring.position.Output, transformSpring.rotation.Output * Quaternion.Euler(rotationOffset));
        }

        public void SetTarget(Transform t)
        {
            target = t;
        }

    }
}
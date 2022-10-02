using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Storm.SploinkySpring;
using Unity.VisualScripting;


namespace Storm.SploinkySpring
{
    public class SploinkyChain : MonoBehaviour
    {

        public Vector3 offset;
        public Transform baseTarget;
        public SpringData posData = new SpringData(1, 1, 1);
        public SpringData rotscaleData = new SpringData(1, 1, 1);
        public List<Transform> links = new List<Transform>();
        public List<SploinkyTransform> springs = new List<SploinkyTransform>();
        // Start is called before the first frame update
        void Start()
        {
            Init();
        }

        // Update is called once per frame
        void Update()
        {

        }
        private void OnEnable()
        {
            Init();
        }

        public void CollectLinks()
        {
            links.Clear();
            springs.Clear();
            foreach (Transform child in transform)
            {
                links.Add(child);
                SploinkyTransform sT = child.gameObject.AddComponent<SploinkyTransform>(); 
                //springs.Add(sT);
            }
        }


        public void Init()
        {
            CollectLinks();
            LinkChain();

        }
        public void LinkChain()
        {
            springs[0].SetTarget(baseTarget);

            int count = links.Count;
            //Link first chain
            for (int i = 1; i < count; i++)
            {
                springs[i].SetTarget(links[i - 1]);
                springs[i].positionOffset = offset;
                springs[i].transformSpring.SetSpringData(posData, rotscaleData, rotscaleData);
            }
        }

        public void SetOutput()
        {

        }

    }
}

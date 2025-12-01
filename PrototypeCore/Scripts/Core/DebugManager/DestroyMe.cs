using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    public class DestroyMe : MonoBehaviour
    {
        float _timer;
        public bool Immediately;
        public float Deathtimer = 10;
        public Action OnDestroy;

        // Use this for initialization
        void Start()
        {
            if (Immediately)
            {
                StartDestroy();
            }
        }

        // Update is called once per frame
        void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= Deathtimer)
            {
                StartDestroy();
            }
        }

        void StartDestroy()
        {
            if (OnDestroy != null)
            {
                OnDestroy();
            }
            Destroy(gameObject);
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Demo.PoolablePrefab
{
    public class DemoRoot : MonoBehaviour
    {
        float _timerA;
        float _timerB;
        List<PoolTestScript> instance = new();
        public PoolTestScript GamePrefab;

        void Start()
        {
            for (int i = 0; i < 50; i++)
            {
                instance.Add(PoolManager.Instance.Spawn(GamePrefab
                    , Vector3.zero + new Vector3(Random.Range(-50f, 50f), 0, Random.Range(-50f, 50f))
                    , Quaternion.identity));
            }
            for (int i = 0; i < instance.Count; i++)
            {
                PoolManager.Instance.Despawn(instance[i], Random.Range(1, 5));
            }
        }


        private void Update()
        {
            _timerA += Time.deltaTime;
            _timerB += Time.deltaTime;
            if (_timerA >= 8)
            {
                _timerA = 0;
                List<PoolTestScript> temp = new List<PoolTestScript>();
                for (int i = 0; i < Random.Range(10, 21); i++)
                {
                    temp.Add(PoolManager.Instance.Spawn(GamePrefab
                        , Vector3.zero + new Vector3(Random.Range(-50f, 50f), 0, Random.Range(-50f, 50f))
                        , Quaternion.identity));
                }
                for (int i = 0; i < temp.Count; i++)
                {
                    PoolManager.Instance.Despawn(temp[i], Random.Range(1, 5));
                }
            }
            if (_timerB >= 11)
            {
                _timerB = 0;
                List<PoolTestScript> temp = new List<PoolTestScript>();
                for (int i = 0; i < Random.Range(10, 21); i++)
                {
                    temp.Add(PoolManager.Instance.Spawn(GamePrefab
                        , Vector3.zero + new Vector3(Random.Range(-50f, 50f), 0, Random.Range(-50f, 50f))
                        , Quaternion.identity));
                }
                for (int i = 0; i < temp.Count; i++)
                {
                    PoolManager.Instance.Despawn(temp[i], Random.Range(1, 5));
                }
            }
        }
    }
}
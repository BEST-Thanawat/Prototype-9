using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelManagement.Utility
{
    public class DonotDestroyOnload : MonoBehaviour
    {
        private void Awake()
        {
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
    }
}
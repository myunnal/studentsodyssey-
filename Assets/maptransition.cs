using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class maptransition : MonoBehaviour
{
    [SerializeField] PolygonCollider2D mapBoundry;
    CinemachineConfiner confiner;

    private void Awake()
    {
        confiner=FindObjectOfType<CinemachineConfiner>();
    }
}

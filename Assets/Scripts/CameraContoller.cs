using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraContoller : MonoBehaviour
{
    public static CameraContoller Instance;
    [SerializeField] List<float> CameraXPositions=new();

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {

    }

    void Update()
    {

    }

    public void ChangeCameraPosition(int posNumber)
    {
        transform.DOMoveX(CameraXPositions[posNumber],0.5f);
    }
}


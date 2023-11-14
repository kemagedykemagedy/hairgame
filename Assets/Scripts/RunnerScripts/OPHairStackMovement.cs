using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;

public class OPHairStackMovement : MonoBehaviour
{
    public bool isMoving=false;


    [Range(0.1f, 15f)]
    [SerializeField] float followPadding;

    List<GameObject> Team=new();
    private void OnEnable()
    {
        ActionController.OnPlayerStartToMove += (()=>isMoving=true);
        ActionController.OnChangeOnTeam += UpdateTeam;
        ActionController.OnLevelFailed+=(()=>{
            isMoving=false;
        });
        ActionController.OnLevelEndReached+=((List<GameObject> a)=>{
            isMoving=false;
        });
    }

    private void OnDisable()
    {
        ActionController.OnPlayerStartToMove -= (()=>isMoving=true);
        ActionController.OnChangeOnTeam -= UpdateTeam;
        ActionController.OnLevelFailed-=(()=>{
            isMoving=false;
        });
        ActionController.OnLevelEndReached-=((List<GameObject> a)=>{
            isMoving=false;
        });
        
    }

    void UpdateTeam(List<GameObject> newTeam)
    {
        Team=newTeam;
    }

    void Update()
    {
        if(isMoving) TeamMove();
    }

    [BurstCompile]
    void TeamMove()
    {
        int LerpTime = 15;
        Vector3 point = transform.position;
        point.z = transform.position.z ;
        point.y = 0;
        //LerpTime = Team.Count;
        if (LerpTime == 1) LerpTime = 2;
        if (Team.Count!=0) Team[0].transform.position = Vector3.Lerp(Team[0].transform.position, point, Time.deltaTime *100);


        for (int i = 1; i < Team.Count; i++)
        {
            point = Team[i - 1].transform.position;
            point.z = Team[i - 1].transform.position.z;
            point.y = 0;
            if (LerpTime == 1) LerpTime = 2;
            Team[i].transform.position = Vector3.Lerp(Team[i].transform.position, point, Time.deltaTime * (LerpTime + followPadding));
        }

    }

   
}


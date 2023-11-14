using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class Doll : MonoBehaviour
{
    public NavMeshAgent agent;
    public DollState currentDollState;
    [HideInInspector] public List<GameObject> PatrolPoints=new();
    int currentPatrolPointNum=0;
    [HideInInspector] public Vector3 CreationPosition;
    #region States
    [HideInInspector] public DollCreationState dollCreationState = new DollCreationState();
    [HideInInspector] public DollIdleState dollIdleState = new DollIdleState();
    [HideInInspector] public DollWalkState dollWalkState = new DollWalkState();
    #endregion

    public void CreateDoll()
    {
        SetPlayerState(dollCreationState);
    }

    private void SetPlayerState(DollState newDollState)
    {
        currentDollState = newDollState;
        currentDollState.StartState(this);
    }
    public void ChangeState(DollState newDollState)
    {
        SetPlayerState(newDollState);
    }

    public void WalkAnimation(bool state)
    {
        
        agent.speed = 1;
    }

    public void GoToPos(Vector3 bayPoint)
    {
       // ResetCircle();
        RunPassenger();
        agent.SetDestination(bayPoint);
    
        SetPlayerState(dollWalkState);

    }

    public void ReachedToWaitPoint()
    {
        Debug.Log("Point reached");
        SetPlayerState(dollIdleState);
      StartCoroutine(NavigateOtherPoint());
      
    }
    IEnumerator NavigateOtherPoint()
    {
        Debug.Log("new Point ");
        yield return new WaitForSeconds(PatrolPoints[currentPatrolPointNum].GetComponent<Point>().WaitTime);
        if(PatrolPoints[currentPatrolPointNum].GetComponent<Point>().PointType==Enums.PointTypes.Shop)
        {
            
                PatrolPoints[currentPatrolPointNum].GetComponent<Point>().ShopController.SpawnCoin();
          
            
        }
        currentPatrolPointNum++;
        if(currentPatrolPointNum>=PatrolPoints.Count) currentPatrolPointNum=0;
        if(PatrolPoints[currentPatrolPointNum].GetComponent<Point>().PointType==Enums.PointTypes.Shop &&
           !PatrolPoints[currentPatrolPointNum].GetComponent<Point>().ShopController.isConstructed){
                currentPatrolPointNum++;
            }
        
        GoToPos(PatrolPoints[currentPatrolPointNum].transform.position);

    }
    public float CheckDistance()
    {
        float distance = 0;
        Debug.Log("distance "+distance);
            distance= agent.remainingDistance;
       
            //Destroy(gameObject);
        
        return distance;
    }

    public void StopPassenger()
    {
        gameObject.GetComponent<NavMeshAgent>().speed = 0;
    }
    public void RunPassenger()
    {
        if (gameObject.GetComponent<NavMeshAgent>().speed ==0)
        {
            gameObject.GetComponent<NavMeshAgent>().speed = 1;
        }
       
    }

    public void DollCreationPosition()
    {
       
        transform.DOJump(CreationPosition,5f,1,1f).SetDelay(1f).OnComplete(()=>{
            ActionController.OnUpdateNavmesh.Invoke();
          GoToPos(PatrolPoints[currentPatrolPointNum].transform.position);  
        });
    }

  
    // Start is called before the first frame update
    void Start()
    {
        SetPlayerState(dollIdleState);
        //GoToPos(PatrolPoints[currentPatrolPointNum].transform.position);
        //SetPlayerState(dollCreationState);
    }

    // Update is called once per frame
    void Update()
    {
currentDollState.UpdateStateContinuously(this);
    }
}


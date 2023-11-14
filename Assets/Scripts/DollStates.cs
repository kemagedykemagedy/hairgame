using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class DollCreationState : DollState
{
    public DollState StartState(Doll DC)
    {
        Debug.Log("DollCreationState");
        DC.DollCreationPosition();
        return DC.dollCreationState;
    }

    public DollState UpdateStateContinuously(Doll DC)
    {

        return DC.dollCreationState;
    }
}
public class DollIdleState : DollState
{
    public DollState StartState(Doll DC)
    {
        Debug.Log("DOLL IDLE STATE");
        DC.WalkAnimation(false);
        return DC.dollIdleState;
    }

    public DollState UpdateStateContinuously(Doll DC)
    {

        return DC.dollIdleState;
    }
}

public class DollWalkState : DollState
{

    //istasyona giderken
    public DollState StartState(Doll DC)
    {
       // Debug.Log("WALKstate");
        DC.WalkAnimation(true);
        return DC.dollWalkState;
    }

    public DollState UpdateStateContinuously(Doll DC)
    {
      
      Debug.Log("WalkState");
        if (DC.CheckDistance() < 0.1f)
        {
            DC.ReachedToWaitPoint();
        }
        return DC.dollWalkState;
    }

 
}
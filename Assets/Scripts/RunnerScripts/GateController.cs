using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateController : MonoBehaviour
{
    public List<GameObject> GatePairsList;//gates have to be assined by order on spline

    private void OnEnable()
    {
      //  ActionController.OnGateCrossed += UpdateGateAmount;
    }

    private void OnDisable()
    {
       // ActionController.OnGateCrossed -= UpdateGateAmount;
    }


    void UpdateGateAmount(int totalHairNum,bool isGateCrossed,bool isLengthGate)
    {
        // GatePairsList[0].SetActive(false);
        // GatePairsList.Remove(GatePairsList[0]);
        // foreach (var GatePair in GatePairsList)
        // {
        //     foreach (Transform child in GatePair.transform)
        //     {
        //         if(isLengthGate)
        //         {
        //             if(child.GetComponent<Gate>().gateType==Gate.GateType.Length)
        //             {
        //                 child.GetComponent<Gate>().Amount/=3;
        //             }else{
        //                 child.GetComponent<Gate>().Amount*=2;
        //             }
        //         }else{
        //             if(child.GetComponent<Gate>().gateType==Gate.GateType.Length)
        //             {
        //                 child.GetComponent<Gate>().Amount*=2;
        //             }else{
        //                 child.GetComponent<Gate>().Amount/=3;
        //             }
        //         }
        //     }
        // }
    }

    GameObject FindGateByType(Transform gateParent,bool isLength)
    {
        GameObject updatedGate=null;
        foreach (Transform child in gateParent)
        {
            if(isLength && child.GetComponent<Gate>().gateType==Gate.GateType.Length)
            {
                updatedGate=child.gameObject;
            }else if(!isLength && child.GetComponent<Gate>().gateType==Gate.GateType.Width)
            {
                updatedGate=child.gameObject;
            }
        }
        return updatedGate;
    }
    
}


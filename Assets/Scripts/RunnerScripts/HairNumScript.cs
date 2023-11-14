using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairNumScript : MonoBehaviour
{
    bool isFollow = true;
    // Connections
    public Transform targetParent;
    Transform target;

    Vector3 offset = Vector3.zero;
    Vector3 newpos = Vector3.zero;

    [SerializeField] Vector3 startPos;
    [SerializeField] Vector3 starteuler;
    bool follow=false;
    private void OnEnable()
    {

        ActionController.OnLevelEndReached += LevelEndMovement;
        ActionController.OnChangeOnTeam += SetTarget;
        ActionController.OnResetForNewLevel+=NewLevel;
    }
    private void OnDisable()
    {
        ActionController.OnLevelEndReached -= LevelEndMovement;
        ActionController.OnChangeOnTeam -= SetTarget;
        ActionController.OnResetForNewLevel-=NewLevel;

    }


    void Start()
    {
       

    }
    private void Update()
    {
        if(follow)
        {
            // newpos = target.transform.position + offset;
        // newpos.y = offset.y;
        // newpos.x = offset.x;
        // transform.position = newpos;
        }
        
        // transform.GetChild(0).transform.LookAt(target.transform);
    }

    void SetTarget(List<GameObject> a)
    {
       StartCoroutine(SetTargetCoroutine());
    }
    IEnumerator SetTargetCoroutine()
    {
        yield return new WaitForSeconds(1f);
         target = targetParent.transform.GetChild(0);
        transform.SetParent(target);
        transform.localPosition=new Vector3(-0.15f,-0.35f,0);
         //offset = target.transform.position - transform.position;
        follow = true;
    }




    void LevelEndMovement(List<GameObject> list)
    {
        StartCoroutine(LevelEndMovementCoroutine());
    }

    IEnumerator LevelEndMovementCoroutine()
    {
        follow = false;
        transform.GetChild(0).gameObject.SetActive(false);
        transform.SetParent(targetParent);
        yield return new WaitForSeconds(0.7f);
        

    }




    public void NewLevel()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

}


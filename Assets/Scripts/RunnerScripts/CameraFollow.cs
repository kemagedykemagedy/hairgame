using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Dreamteck.Splines;
public class CameraFollow : MonoBehaviour
{

    bool isFollow = true;
    // Connections
    public Transform target;
    [SerializeField] GameObject LevelEnd;
    [SerializeField] public Vector3 offset = Vector3.zero;
    Vector3 newpos = Vector3.zero;
    [SerializeField] GameObject cameraObj;
    [SerializeField] GameObject WaterShader;
    [SerializeField] Vector3 startPos;
    [SerializeField] Vector3 starteuler;
    bool levelend = false;
    bool isMove = false;
    private void OnEnable()
    {
        ActionController.OnLevelEndReached += LevelEndMovement;
        ActionController.OnHairDropCompleted += LevelSuccessEndMovement;
        ActionController.OnPlayerStartToMove+=(()=>{
            isMove = true;
           // cameraObj.transform.localPosition = new Vector3(0,0,0);
        cameraObj.transform.localEulerAngles = new Vector3(28.4f,0,0);
        });
       // ActionController.OnLevelStarted+=MoveBeforeSTartPosToStartPos;
       ActionController.OnResetForNewLevel+=NewLevel;
    }
    private void OnDisable()
    {
        ActionController.OnLevelEndReached -= LevelEndMovement;
        ActionController.OnHairDropCompleted -= LevelSuccessEndMovement;
        ActionController.OnPlayerStartToMove-=(()=>{
            isMove = true;
        });
        //ActionController.OnLevelStarted-=MoveBeforeSTartPosToStartPos;
        ActionController.OnResetForNewLevel-=NewLevel;
    }


    void Start()
    {

    }
    private void LateUpdate()
    {
        if(isFollow)
        {
            newpos = target.transform.position + offset;
        newpos.y = offset.y;
        newpos.x = offset.x;
        transform.position = newpos;
        if(isMove)
        {
            cameraObj.transform.SetParent(this.transform);
        }
        }

        
        
    }




    void LevelEndMovement(List<GameObject> list)
    {
        StartCoroutine(LevelEndMovementCoroutine());
    }

    IEnumerator LevelEndMovementCoroutine()
    {
        levelend = true;
        isFollow = false;
       // isGameStart = false;
        yield return new WaitForSeconds(0.7f);
        cameraObj.transform.SetParent(LevelEnd.transform);
        cameraObj.transform.DOLocalMove(new Vector3(0, 8.69f, -0.26f), 1f);
        cameraObj.transform.DOLocalRotate(new Vector3(40f, 0, 0), 1f);

    }

    void LevelSuccessEndMovement()
    {
        Debug.Log("CAMERA END MOVEMENT");
        StartCoroutine(LevelSuccessEndMovementCoroutine());
    }

    IEnumerator LevelSuccessEndMovementCoroutine()
    {
        isFollow = false;
        WaterShader.SetActive(false);
        yield return new WaitForSeconds(1f);

        cameraObj.transform.DOLocalMove(new Vector3(0, -1.63f, -21.4f), 0.4f);
        cameraObj.transform.DOLocalRotate(new Vector3(3.04f, 0, 0), 0.4f);

    }

    public void NewLevel()
    {
        isMove=false;
        isFollow = true;
        //isGameStart=true;
        levelend = false;
        WaterShader.SetActive(true);
        cameraObj.transform.SetParent(transform);
        cameraObj.transform.localPosition = startPos;
        cameraObj.transform.localEulerAngles = starteuler;
        
        StartCoroutine(CameraStartPos());
    }
    IEnumerator CameraStartPos()
    {
        cameraObj.transform.localEulerAngles = new Vector3(28.7f,0,0);
        cameraObj.transform.localPosition = new Vector3(0f,9.87f,-15.62f);
        yield return new WaitForSeconds(0.1f);
cameraObj.transform.SetParent(transform.parent);
cameraObj.transform.localEulerAngles = new Vector3(28.7f,0,0);
        cameraObj.transform.localPosition = new Vector3(0f,9.87f,-15.62f);


    }

    //public void MoveBeforeSTartPosToStartPos()
    //{
    // isGameStart=false;
    //    cameraObj.transform.DOLocalMove(startPos, 0.5f);
    //    cameraObj.transform.DOLocalRotate(starteuler, 0.5f).OnComplete(()=>{
    //        GameManager.Instance.StartGame();
    //        isGameStart = true;
    //    });
    //}




}









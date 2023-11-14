using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using Unity.Burst;
    public class OPPlayerMovement : MonoBehaviour
    {
        public float speed = 10;
    public float roadBorder = 2;
    [Header("Movement & Rotation")]
    [SerializeField] private float xMoveSpeed = 1f;
    [SerializeField] private float xMoveAmount = 8f;
    [SerializeField] private float xMovementSensitivity = 20f;
    private Vector2 splineFollowReferenceVelocity;
    private Vector3 cameraFollowReferenceVelocity;
 
    public SplineFollower splineFollower;
    SimpleUserTapInput simpleUserTapInput;
    public SwerveInput swerveInput;
    [SerializeField] Canvas ui = default;

    public int ActiveLineNumber=0;
   
   
    public bool canMove = true;
    public bool canSwerve = true;
    [SerializeField] Transform cameraObj;



    void Awake()
    {
        InitConnections();
        StartCoroutine(SetSpline());
    }

    private void OnEnable() {
        
    ActionController.OnLevelFailed += (()=>Disable());
ActionController.OnLevelEndReached+=LevelEndStopPlayer;
     }

     private void OnDisable() {
    ActionController.OnLevelFailed -= (()=>Disable());
    ActionController.OnLevelEndReached-=LevelEndStopPlayer;
     }

    void InitConnections()
    {
        swerveInput = GetComponent<SwerveInput>();

        splineFollower = GetComponent<SplineFollower>();
        splineFollower.followSpeed = speed;
    }

    void InitState()
    {
 
        canMove = true;
        canSwerve = true;
    }

    void Start()
    {



        InitState();
    }

   
    void Update()
    {
        if(canSwerve)
        {
            SwerveMove();
        
       
  
        xMoveAmount=4.8f-(ActiveLineNumber*0.06f);
        
        }
        

       
////        Debug.Log(xMoveAmount);
    }

[BurstCompile]
    void SwerveMove()
    {

        float newPositionOnX = splineFollower.motion.offset.x + swerveInput.MoveFactorX * Time.deltaTime * xMovementSensitivity;
        newPositionOnX = Mathf.Clamp(newPositionOnX, -xMoveAmount, xMoveAmount);
        Vector2 newOffsetPosition = new Vector2(newPositionOnX, 0.0f);
        splineFollower.motion.offset = Vector2.SmoothDamp(splineFollower.motion.offset, newOffsetPosition, ref splineFollowReferenceVelocity, Time.deltaTime * xMoveSpeed);


cameraObj.localPosition=Vector3.SmoothDamp(cameraObj.localPosition,new Vector3(newPositionOnX*0.3f,9.87f,-15.62f),ref cameraFollowReferenceVelocity, Time.deltaTime * xMoveSpeed*15f);
    }



    public void Disable()
    {
        splineFollower.follow = false;
        canMove = false;
        canSwerve = false;
        StartCoroutine(LevelEndStopPlayerCoroutine());
    }

    public IEnumerator Enable()
    {
     yield return new WaitForSeconds(0.2f);
        splineFollower.follow = true;
        canMove = true;
        canSwerve = true;
        ActionController.OnPlayerStartToMove.Invoke();
      
    }


    public void StopPlayer()
    {
        splineFollower.follow = false;
        canMove = false;
        canSwerve = false;
    }
    public void LevelEndStopPlayer(List<GameObject> a)
    {
        StartCoroutine(LevelEndStopPlayerCoroutine());
    }

    IEnumerator LevelEndStopPlayerCoroutine()
    {
        yield return new WaitForSeconds(0.8f);
        splineFollower.follow = false;
        splineFollower.enabled=false;
        canMove=false;
        canSwerve=false;
        transform.position=Vector3.zero;
        UIManager.Instance.RetryButton.gameObject.SetActive(true);
    }

    IEnumerator SetSpline()
    {
        yield return new WaitForSeconds(0.2f);
        splineFollower.spline = GameObject.FindGameObjectWithTag("PlayerSpline").GetComponent<SplineComputer>();
        splineFollower.enabled = true;
        yield return new WaitForSeconds(0.1f);
        
    }


    public void PlayerNewLevel()
    {
        StopPlayer();
        
        ActionController.OnNewLevelLoadCompleted.Invoke();
       StartCoroutine(SetSpline());
        splineFollower.SetDistance(0,false,false);
        splineFollower.follow=true;
        splineFollower.motion.offset=Vector2.zero;
        splineFollower.follow=false;
    }

    public void NewLevel()
    {
       
        StartCoroutine(SetSpline());
        splineFollower.SetDistance(0,false,false);
    }
    }


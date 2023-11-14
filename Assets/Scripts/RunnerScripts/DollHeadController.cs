using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DollHeadController : MonoBehaviour
{
    [SerializeField] GameObject HeadCenterPoint;
    [SerializeField] GameObject HeadPivotPoint;
    public float PCRotationSpeed = 10f;
    public float MobileRotationSpeed = 0.4f;
    public bool isSpinning = false;
    public bool isDollHeadActive = true;
    //Drag the camera object here
    public Camera cam;
    bool isMoving = false;
    public Vector3 targetAngle = new Vector3(0f, 0f, 0f);

    private Vector3 currentAngle;
    public bool OnFace = false;
    bool isFiring=false;
    bool CoroutineInvoked=false;
    bool StartToResetRot=false;

    private void OnEnable()
    {
        ActionController.OnLevelEndReached += ActivateMove;
    }
    private void OnDisable()
    {
        ActionController.OnLevelEndReached -= ActivateMove;
    }




    // private void OnTriggerEnter(Collider other)
    // {
        // if (other.tag == "FaceHairMaskObj")
        // {
            // OnFace = true;

            // //Debug.Log("FaceHairMaskObj ENTER");
            // ActionController.OnHeadSpinning.Invoke(false, OnFace);


        // }
    // }


    // private void OnTriggerExit(Collider other)
    // {
        // if (other.tag == "FaceHairMaskObj")
        // {

            // OnFace = false;
            // //Debug.Log("FaceHairMaskObj EXIT " + isSpinning + " " + isMoving);
            // //ActionController.OnHeadSpinning.Invoke(true,OnFace);


        // }
    // }

    void OnMouseDrag()
    {
        if (isDollHeadActive)
        {
            isMoving = true;
            float rotX = Input.GetAxis("Mouse X") * PCRotationSpeed;
            float rotY = Input.GetAxis("Mouse Y") * PCRotationSpeed;


            if (rotX == 0 && rotY == 0)
            {
                isSpinning = false;
               // ActionController.OnHeadSpinning.Invoke(false, OnFace);
            }
            else
            {
                if (!isSpinning)
                {
                    isSpinning = true;
                   // ActionController.OnHeadSpinning.Invoke(true, OnFace);
                }
                Vector3 right = Vector3.Cross(cam.transform.up, transform.position - cam.transform.position);
                Vector3 up = Vector3.Cross(transform.position - cam.transform.position, right);
                transform.rotation = Quaternion.AngleAxis(-rotX, up) * transform.rotation;
                transform.rotation = Quaternion.AngleAxis(rotY, right) * transform.rotation;

            }


        }

    }
    private void OnMouseUp()
    {
        isSpinning = false;
        //ActionController.OnHeadSpinning.Invoke(false);

        isMoving = false;
    }


    void Update()
    {
       // Debug.Log("INFACE "+OnFace);
       // Debug.Log("isSpinning "+isSpinning);
       if(!OnFace && isSpinning && !isFiring)
       {
        isFiring=true;
        ActionController.OnHeadSpinning.Invoke(true,OnFace);
       }else{
        isFiring=false;
        ActionController.OnHeadSpinning.Invoke(false,OnFace);
       }
        if (!isMoving)
        {
            if(!CoroutineInvoked){
                CoroutineInvoked=true;
                StartCoroutine(SmoothResetRot());
            } 
           if(StartToResetRot) transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, 0f), 1f * Time.deltaTime);
        }else{
           CoroutineInvoked=false;
    StartToResetRot=false;
        }


        
    }

    IEnumerator SmoothResetRot()
    {
        yield return new WaitForSeconds(2f);
        StartToResetRot=true;
    }
    public void ResetHeadPos()
    {
        transform.eulerAngles = targetAngle;
    }

    



    void ActivateMove(List<GameObject> li)
    {
      //  ActionController.OnHeadSpinning.Invoke(false, false);
        isMoving = true;
        isSpinning = false;
        OnFace=false;
    }
}


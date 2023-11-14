using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
public class PlayerController : MonoBehaviour
{
    public Animator animator;
    OPPlayerMovement OpPlayerMovement;
    SplineFollower splineFollower;
    [SerializeField] OPHairStackController oPHairStackController;
    [SerializeField] GameObject camera;
    public int ActiveHairNum=0;
    [SerializeField] public Color ActiveColor;
    [SerializeField] GameObject FinalHairParent;
    [SerializeField] public Color startColor;
    
    // Start is called before the first frame update
    void Start()
    {
        OpPlayerMovement = GetComponent<OPPlayerMovement>();
        splineFollower = GetComponent<SplineFollower>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
             Debug.Log("FINISH");
            
            FinishGame();
        }
    }
    
    public IEnumerator TriggerActivationCoroutine()
    {
        GetComponent<CapsuleCollider>().isTrigger=false;
        yield return new WaitForSeconds(1f);
        GetComponent<CapsuleCollider>().isTrigger=true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable() {
     ActionController.OnResetForNewLevel+=NewLevel;
     ActionController.OnColorChanged+=ChangeColorOfActiveHair;
     ActionController.OnShapeChanged+=ChangeShapeOfActiveHair;
     }

     private void OnDisable() {
ActionController.OnResetForNewLevel-=NewLevel;
ActionController.OnColorChanged-=ChangeColorOfActiveHair;
ActionController.OnShapeChanged-=ChangeShapeOfActiveHair;
     }

     public void ChangeColorOfActiveHair(Color color)
    {
        ActiveColor=color;
      // hairStackController.ChangeColorOfActiveHair(color);
    }
    public void ChangeShapeOfActiveHair(int shapeNum)
    {
        ActiveHairNum=shapeNum;
     //  hairStackController.ChangeShapeOfActiveHair(shapeNum);
    }

    public void StartGame()
    {
        StartCoroutine(OpPlayerMovement.Enable());
 
//        animator.SetBool("Move", true);
    }

    public void FinishGame()
    {
        GameManager.Instance.EndGame(true, 0);
        splineFollower.followSpeed = 0;
       
        
      //  animator.SetBool("Move", false);
    }

    public void PlayerOnNewLevel()
    {
        OpPlayerMovement.PlayerNewLevel();
    }

    public void OnSplineEndReached()
    {
        ActionController.OnLevelEndReached.Invoke(oPHairStackController.GetComponent<OPHairStackController>().GetHairCellsGO());
        UIManager.Instance.StartClickPrevent.SetActive(true);
    
     
       UIManager.Instance.StartClickPrevent.SetActive(false);
        
    }

    public void NewLevel()
    {
       ActiveHairNum = 0;
        ActiveColor = startColor;
        
    }
}

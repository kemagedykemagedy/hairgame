using System.Collections;
using Unity.Burst;
using UnityEngine;


public class HairCell : MonoBehaviour
{
    public UnityEngine.Vector2 HairCellPos;
    public bool isFalling = false;
    public bool onHead = false;
    public int ActiveHairNum=0;
    public Color ActiveColor;
    public Color BaseColor;
    [HideInInspector] public Transform PoolParent;
    [SerializeField] public ParticleSystem particle;
    [SerializeField] public AudioSource audioSource;
    [SerializeField] public AudioClip DropOnHead;
    [SerializeField] float zforce;


 [SerializeField] GameObject FirstBone;
 bool Haptic=true;
  bool Sound=true;

//  ChangeColor(BaseColor);
//     ChangeModel(0);



private void OnEnable() {
    ActionController.OnHeadSpinning+=ChangeMovementOnFalling;
    ActionController.OnSoundSettingsChanged+=((bool a)=>{
        Sound=a;
    });
    ActionController.OnHapticSettingsChanged+=((bool a)=>{
        Haptic=a;
    });
  
}

private void OnDisable() {
    ActionController.OnHeadSpinning-=ChangeMovementOnFalling;
    ActionController.OnSoundSettingsChanged-=((bool a)=>{
        Sound=a;
    });
    ActionController.OnHapticSettingsChanged-=((bool a)=>{
        Haptic=a;
    });
}

    public void HairActivation(bool isHairActiveBool)
    {
        transform.GetChild(0).gameObject.SetActive(isHairActiveBool);
        
    }

    public void DestroyedByObstacle()
    {
        ActionController.OnHairDestroyedByObstacle.Invoke(transform.parent.gameObject,this.gameObject);
    }

    public void AddForce()
    {
//        Debug.Log("Add Force");
        transform.GetChild(0).GetComponent<DynamicBone>().m_Force=new Vector3(0f,0f,zforce);
    }

    public bool IsHairActive()
    {
        return transform.GetChild(0).gameObject.activeSelf;
    }
    private void OnTriggerEnter(Collider other)
    {
        
        if (onHead && other.tag == "HairCell")
        {
            ActionController.OnHairReachToHead(false);

        }else if (other.tag == "Face")
        {
            //Destroy(gameObject);

        }
    }

    public void ChangeColor(Color color)
    {
        ActiveColor=color;
        for(int i=0;i<=3;i++)
        {
           //if(transform.GetChild(0).GetChild(i).gameObject.activeSelf)
           //{
                transform.GetChild(0).GetChild(i).gameObject.GetComponent<SkinnedMeshRenderer>().material.color=color;
            //}
        }
        
//              Debug.Log("CHANGE COLOR");

    
    }

    public void ChangeColorHSV(float V)
    {
        float cH,cS,cV;
        Color.RGBToHSV(transform.GetChild(0).GetChild(0).gameObject.GetComponent<SkinnedMeshRenderer>().material.color, out cH, out cS, out cV);
        for(int i=0;i<=3;i++)
        {
            
                transform.GetChild(0).GetChild(i).gameObject.GetComponent<SkinnedMeshRenderer>().material.color=Color.HSVToRGB(cH, cS, V);
            
        }
        
              Debug.Log("CHANGE COLOR");

    
    }
[BurstCompile]
    public void ChangeModel(int ModelType)
    {transform.GetChild(0).GetChild(ActiveHairNum).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(ModelType).gameObject.SetActive(true);
       
        ActiveHairNum=ModelType;
        
       //for(int i=0;i<=3;i++)
       //{
       //    if(i!=ModelType) transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
       //}
       
                

    
    }

    public void Cut(int level)
    {
        particle.startColor=ActiveColor;
        particle.Play();
         GetComponent<BoxCollider>().enabled=false;
   
        GameObject bone=FirstBone.transform.GetChild(0).gameObject;
        if(level>2) level=2;
        if(level<1) level=1;
        //        Debug.Log(level);
        
        for (int i = 0; i < level; i++)
        {
            bone=bone.transform.GetChild(0).gameObject;
            
        }
        bone.transform.localScale=UnityEngine.Vector3.zero;
        GetComponent<BoxCollider>().enabled=true;
       //StartCoroutine(ReActivateCollider());
    }

    IEnumerator ReActivateCollider()
    {
        yield return new WaitForSeconds(2f);
          GetComponent<BoxCollider>().enabled=true;
    }

    public void PlaySound(AudioClip audioClip)
    {
        audioSource.clip=audioClip;
        if(Sound) audioSource.Play();
    }

    void ChangeMovementOnFalling(bool firing, bool onFace)
    {
        
        if(isFalling && !onHead) 
        {
//            Debug.Log(a);
            if(onFace){
                GetComponent<Rigidbody>().useGravity=false;
                GetComponent<Rigidbody>().isKinematic=true;
            } 
            else{
                GetComponent<Rigidbody>().useGravity=true;
                GetComponent<Rigidbody>().isKinematic=false;
            } 
        }
    }

    public void ResetLevel()
    {
        transform.SetParent(PoolParent);
        transform.localPosition=Vector3.zero;
       
    }

    public void ResetColor()
    {
        ActiveColor=BaseColor;
        ChangeColor(ActiveColor);
    }

 


}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
public class Obstacle : MonoBehaviour
{

    [SerializeField] public AudioSource audioSource;
    [SerializeField] public AudioClip ObstacleSound;
    bool SoundPlayed = false;
    bool isAnyHairCutted=false;
    public enum ObstacleType
    {
        Full,
        RespectToHeight,
    }
    public ObstacleType obstacleType;

    [Range(0f, 3f)]
    public float MaxHeight;
  


bool Haptic=true;
  bool Sound=true;
    private void OnEnable()
    {
        ActionController.OnSoundSettingsChanged+=((bool a)=>{
        Sound=a;
    });
    ActionController.OnHapticSettingsChanged+=((bool a)=>{
        Haptic=a;
    });

    }

    private void OnDisable()
    {
        ActionController.OnSoundSettingsChanged-=((bool a)=>{
        Sound=a;
    });
    ActionController.OnHapticSettingsChanged-=((bool a)=>{
        Haptic=a;
    });

    }

    private void Start()
    {
        UpDownMovement();
    }

    float currentHeightOfObstacleModel = 0;
    [SerializeField] GameObject ObstacleGO;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "HairCell")
        {
            HairTriggered(other);
            isAnyHairCutted=true;
        }
        else if (other.tag == "TailGunner" && obstacleType!=ObstacleType.RespectToHeight && isAnyHairCutted)
        {
          
        
            
            ActionController.OnObstacleGone.Invoke();
           
        }
    }

    void HairTriggered(Collider other)
    {
        if (obstacleType == ObstacleType.Full)
        {
            other.GetComponent<HairCell>().DestroyedByObstacle();
        }
        else if (obstacleType == ObstacleType.RespectToHeight)
        {
            CutHair(other);
        }
    }

    void UpDownMovement()
    {
        if (obstacleType == ObstacleType.RespectToHeight)
        {
            ObstacleGO.transform.DOLocalMove(Vector3.zero, 0.5f).OnComplete(() =>
            {
                ObstacleGO.transform.DOLocalMove(new Vector3(0, MaxHeight, 0), 0.5f).OnComplete(() => UpDownMovement());
            });
        }

    }

    void CutHair(Collider other)
    {
        // Debug.Log(Mathf.RoundToInt(ObstacleGO.transform.localPosition.y*-5));
        if (other.GetComponent<HairCell>().IsHairActive())
        {
            if (!SoundPlayed)
            {
                SoundPlayed = true;
               if(Sound) PlaySound(ObstacleSound);
            }
            other.GetComponent<HairCell>().Cut(Mathf.RoundToInt(ObstacleGO.transform.localPosition.y * -5f) - 4);
        }

    }

    public void PlaySound(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }


}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.NiceVibrations;

    public class Gem : MonoBehaviour
    {
        bool taken=false;
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

    
        private void OnTriggerEnter(Collider other)
    {
        

        if (!taken && other.tag == "Player" || other.tag == "HairCell")
        {
            taken=true;
          ActionController.OnEarnGem.Invoke(transform.position);
          transform.GetChild(0).gameObject.SetActive(false);
          GetComponent<SphereCollider>().enabled=false;
          if(Haptic) PlayHaptics(HapticTypes.LightImpact);
        }
        
    }

    private void PlayHaptics(HapticTypes hapticType = HapticTypes.LightImpact)
    {
        MMVibrationManager.Haptic(hapticType);
    }
    }

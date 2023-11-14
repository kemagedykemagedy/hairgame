using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.NiceVibrations;

    public class StickableArea : MonoBehaviour
    {
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
        if (other.tag == "HairCell" && !other.GetComponent<HairCell>().onHead)
        {
            other.GetComponent<HairCell>().PlaySound(other.GetComponent<HairCell>().DropOnHead);
           if(Haptic) PlayHaptics(HapticTypes.LightImpact);
            other.GetComponent<Rigidbody>().useGravity = false;
            other.GetComponent<Rigidbody>().isKinematic = true;
            other.GetComponent<HairCell>().onHead=true;
           // other.GetComponent<BoxCollider>().enabled = false;
            other.transform.SetParent(transform);
            ActionController.OnHairReachToHead(true);
            

        }
    }

    private void PlayHaptics(HapticTypes hapticType = HapticTypes.LightImpact)
    {
        MMVibrationManager.Haptic(hapticType);
    }
    }


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.NiceVibrations;

    public class Coin : MonoBehaviour
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

    private void PlayHaptics(HapticTypes hapticType = HapticTypes.LightImpact)
    {
        MMVibrationManager.Haptic(hapticType);
    }
    }


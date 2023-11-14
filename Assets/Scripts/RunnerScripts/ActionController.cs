using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ActionController
{

    #region runnerActions

    
    public static Action<List<GameObject>> OnChangeOnTeam;
    public static Action OnPlayerStartToMove;
    public static Action OnObstacleGone;
    public static Action<bool,bool,int,bool> OnRowColumnAddRemove;
    public static Action<List<GameObject>> OnLevelEndReached;
    public static Action<bool,bool> OnHeadSpinning;
    public static Action<bool> OnHairReachToHead;//if place on another hair send false
    public static Action OnNewLevelLoadCompleted;
    public static Action OnResetForNewLevel;
    public static Action OnLevelStarted;
    public static Action OnLevelCompleted;
    public static Action OnLevelFailed;
    public static Action OnHairDropCompleted;
    public static Action <GameObject,GameObject> OnHairDestroyedByObstacle;//line,hair
    public static Action<bool> OnSoundSettingsChanged;
    public static Action<bool> OnHapticSettingsChanged;
    public static Action<int,int,int> OnGateCrossed;//total hair number
    public static Action<Color> OnColorChanged;
    public static Action<int> OnShapeChanged;
    public static Action<Vector3> OnEarnGem;
#endregion

#region idleActions
public static Action<GameObject> OnHouseGroundSelected;
public static Action<GameObject> OnShopGroundSelected;
public static Action<GameObject> OnBuildButtonClicked;
public static Action<GameObject> OnShopBuildButtonClicked;
public static Action<GameObject> OnCoinSafeSelected;
public static Action OnUpdateNavmesh;
public static Action<List<GameObject>,List<GameObject>> OnSpawnDolls;
public static Action<Vector3> OnEarnCoin;

#endregion





}

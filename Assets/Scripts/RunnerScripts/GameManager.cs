using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField]
    private Canvas canvas;
    [SerializeField] private GameObject[] levels;
    private int level = 0;
    private GameObject currentLevel = null;
    [SerializeField]  private PlayerController player;
    private bool isFinished = false;
    private void Awake()
    {
        Instance = this;
        level = PlayerPrefs.GetInt("Level", 0);
        LoadLevel(ClampLevel(level));
        Input.multiTouchEnabled = false;
        
    }


private void OnEnable() {
        ActionController.OnLevelStarted+=StartGame;
     }

     private void OnDisable() {
        ActionController.OnLevelStarted-=StartGame;
     }

    public void StartGame()
    {
         player.StartGame();

    }
    public void EndGame(bool win, int amount = 0)
    {
        if (isFinished) return;
        isFinished = true;
        if (win)
        {
            UIManager.Instance.OnPlayerCompletedLevel();
            UIManager.Instance.SpawnMoney(Vector3.zero, amount);
        }
        else
        {
           
            UIManager.Instance.OnPlayerFailedLevel();
        }
    }





    //levelloaderfunc
    public void NextLevel()
    {
        level++;
        PlayerPrefs.SetInt("Level", level);
        LoadLevel(ClampLevel(level));
        player.PlayerOnNewLevel();
        isFinished = false;
    }
    public void RestartLevel()
    {
        LoadLevel(ClampLevel(level));
        player.PlayerOnNewLevel();
        isFinished = false;
    }

    private void LoadLevel(int level)
    {
        if (currentLevel != null)
        {
            Destroy(currentLevel);
        }
        currentLevel = Instantiate(levels[level], Vector3.zero, Quaternion.identity);
    }
    private int ClampLevel(int level)
    {
        return level % levels.Length;
    }
}
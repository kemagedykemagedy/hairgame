using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.Burst;


public class OPHairStackController : MonoBehaviour
{

    List<GameObject> Team = new();
    float childWidth;
    [SerializeField] GameObject HairLine;
    [SerializeField] GameObject HairCell;
    ObjectPool objectPool;
    [SerializeField] Transform PoolParent;
    [SerializeField] OPPlayerMovement oPPlayerMovement;
    [SerializeField] HairNumScript HairNumberUI;
    [SerializeField] GameObject TailGunnerGO;
    int lastWidth = 0;
    [SerializeField] Color BaseColor;
    [SerializeField] float horizontalPadding;

    Color ActiveColor;
    int ActiveType = 0;
    [Range(1f, 5f)]
    [SerializeField] float EndColorPercent;
    [SerializeField] List<GameObject> NewCreatedList = new();
    [SerializeField] public AudioSource audioSource;
    [SerializeField] public AudioClip ColorgateSound;
    [SerializeField] public AudioClip ShapegateSound;
    [SerializeField] public AudioClip SizeSound;
    bool Haptic = true;
    bool Sound = true;
    private void OnEnable()
    {
        ActionController.OnRowColumnAddRemove += LengthWidthAddRemove;
        ActionController.OnChangeOnTeam += UpdateTeam;
        ActionController.OnNewLevelLoadCompleted += NewLevel;
        ActionController.OnHairDestroyedByObstacle += HairDestoryedByObstacle;
        ActionController.OnPlayerStartToMove += AddForceToHair;
        ActionController.OnObstacleGone += FillTeam;
        ActionController.OnColorChanged += ChangeColor;
        ActionController.OnShapeChanged += ChangeShape;
        ActionController.OnSoundSettingsChanged += ((bool a) =>
        {
            Sound = a;
        });
        ActionController.OnHapticSettingsChanged += ((bool a) =>
        {
            Haptic = a;
        });

    }

    private void OnDisable()
    {
        ActionController.OnRowColumnAddRemove -= LengthWidthAddRemove;
        ActionController.OnChangeOnTeam -= UpdateTeam;
        ActionController.OnNewLevelLoadCompleted -= NewLevel;
        ActionController.OnHairDestroyedByObstacle -= HairDestoryedByObstacle;
        ActionController.OnPlayerStartToMove -= AddForceToHair;
        ActionController.OnObstacleGone -= FillTeam;
        ActionController.OnColorChanged -= ChangeColor;
        ActionController.OnShapeChanged -= ChangeShape;
        ActionController.OnSoundSettingsChanged -= ((bool a) =>
        {
            Sound = a;
        });
        ActionController.OnHapticSettingsChanged -= ((bool a) =>
        {
            Haptic = a;
        });

    }

    private void Start()
    {
        objectPool = new ObjectPool(HairCell);
        objectPool.FillPool(500, PoolParent);
        ActiveColor = BaseColor;
        ActiveType = 0;
        DOTween.SetTweensCapacity(10000, 2000);
    }

    private void Update()
    {

    }

    void UpdateTeam(List<GameObject> newTeam)
    {
        Team = newTeam;
        SetTailGunnerPos();
        HairNumberUI.transform.GetChild(0).gameObject.SetActive(true);

    }
    [BurstCompile]
    IEnumerator UpdateTeamColorGradient(Color gradientStartColor, Color gradientEndColor, float delay)
    {

        yield return new WaitForSeconds(delay);



        Color[] interpolatedColors = InterpolateColors(gradientStartColor, gradientEndColor, Team.Count);
        List<Color> colorTones = new(interpolatedColors);

        for (int i = 0; i < Team.Count; i++)
        {
            foreach (Transform hairCell in Team[i].transform)
            {
                hairCell.GetComponent<HairCell>().ChangeColor(colorTones[i]);
            }
        }
        UIManager.Instance.UpdateHairNumber();
    }
    Color[] InterpolateColors(Color start, Color end, int numberOfTones)
    {
        Color[] colors = new Color[numberOfTones];
        for (int i = 0; i < numberOfTones; i++)
        {
            float t = i / (float)(numberOfTones - 1) / EndColorPercent;
            colors[i] = Color.Lerp(start, end, t);
        }
        return colors;
    }

    public void LengthWidthAddRemove(bool isLength, bool isAddition, int amount, bool isStart)
    {
        PlaySound(SizeSound);

        if (isAddition)
        {
            if (isLength)
            {
                StartCoroutine(AddHairLength(amount, isStart));
            }
            else if (!isLength)
            {
                StartCoroutine(AddHairWidth(-1, amount, isStart));
            }

        }
        else
        {
            if (isLength)
            {
                StartCoroutine(RemoveHairLength(amount));
            }
            else if (!isLength)
            {
                StartCoroutine(RemoveHairWidth(-1, amount));
            }
        }
Debug.Log("GetHairCellsGO() "+GetHairCellsGO().Count);
int tempInt=GetHairCellsGO().Count;
        ActionController.OnGateCrossed.Invoke(tempInt, Team.Count, GetWidth());
    }


    public IEnumerator AddHairLength(int amount, bool isSTart)
    {
        int currenWidth = GetWidth();
        int newLineNumber;
        int lastReminder;

        if (amount < (currenWidth - Team[^1].transform.childCount))
        {
            int fillAmount = currenWidth - Team[^1].transform.childCount;
            AddRemoveHairCell(amount, Team[^1].transform, true, true, isSTart);
            amount -= amount;

        }
        else
        {
            int fillAmount = currenWidth - Team[^1].transform.childCount;
            AddRemoveHairCell(fillAmount, Team[^1].transform, true, true, isSTart);
            amount -= fillAmount;

        }

        newLineNumber = amount / currenWidth;
        lastReminder = amount % currenWidth;
        AddRemoveLine(newLineNumber, true, currenWidth, isSTart);
        if (lastReminder != 0) AddRemoveLine(1, true, lastReminder, isSTart);
        StartCoroutine(UpdateTeamColorGradient(ActiveColor, Color.white, 0.1f));
        foreach (var line in Team)
        {
            ReorderCenterAligned(line.gameObject, horizontalPadding);
        }
        yield return new WaitForSeconds(0.3f);
        ActionController.OnChangeOnTeam.Invoke(Team);

    }

    public IEnumerator RemoveHairLength(int amount)
    {
        List<GameObject> EmptyLines = new();

        if (amount < GetTotalHairNumber())
        {
            while (amount > 0)
            {
                for (int i = Team.Count - 1; i >= 0; i--)
                {
                    while (Team[i].transform.childCount > 0 && amount > 0)
                    {

                        amount--;

                        //Destroy(Team[i].transform.GetChild(Team[i].transform.childCount - 1).gameObject);
                        objectPool.PushObjectToPool(Team[i].transform.GetChild(0).gameObject);

                        if (Team[i].transform.childCount <= 0) EmptyLines.Add(Team[i]);

                    }
                    if (amount <= 0) break;
                }
            }
        }
        else
        {
            foreach (var line in Team)
            {

                while (line.transform.childCount > 0)
                {
                    //Destroy(line.transform.GetChild(line.transform.childCount - 1).gameObject);
                    objectPool.PushObjectToPool(line.transform.GetChild(line.transform.childCount - 1).gameObject);



                }

                EmptyLines.Add(line);

            }
            yield return new WaitForEndOfFrame();
        }



        int tempInt = EmptyLines.Count;

        for (int i = 0; i < tempInt; i++)
        {
            GameObject tempObj = EmptyLines[i];
            Team.Remove(tempObj);
            Destroy(tempObj);
            if (Team.Count <= 0) OnFailedLevel();
            Debug.Log("FAIL UI 1");
        }
        ActionController.OnChangeOnTeam.Invoke(Team);
        foreach (var line in Team)
        {
            ReorderCenterAligned(line.gameObject, horizontalPadding);
        }
        StartCoroutine(UpdateTeamColorGradient(ActiveColor, Color.white, 0.1f));

    }



    public IEnumerator AddHairWidth(int LineNumber, int amount, bool isStart)
    {
        while (amount > 0)
        {
            foreach (var line in Team)
            {

                if (line.transform.childCount > 0 && amount > 0)
                {
                    AddRemoveHairCell(1, line.transform, true, false, isStart);
                    NewCreatedList.Add(line.transform.GetChild(line.transform.childCount - 1).gameObject);
                    //Destroy(line.transform.GetChild(line.transform.childCount-1).gameObject);

                    amount--;
                }



                if (amount <= 0) break;

            }

        }
        foreach (var line in Team)
        {
            ReorderCenterAlignedWithoutZ(line.gameObject, horizontalPadding, NewCreatedList);
        }
        lastWidth = GetWidth();
        StartCoroutine(UpdateTeamColorGradient(ActiveColor, Color.white, 0.1f));
        yield return new WaitForSeconds(0.1f);
        foreach (var line in Team)
        {
            ReorderCenterAligned(line.gameObject, horizontalPadding);
        }
        NewCreatedList.Clear();
        oPPlayerMovement.ActiveLineNumber = GetWidth();
    }

    public IEnumerator RemoveHairWidth(int LineNumber, int amount)
    {
        List<GameObject> EmptyLines = new();

        if (amount < GetTotalHairNumber())
        {
            while (amount > 0)
            {
                for (int i = Team.Count - 1; i >= 0; i--)
                {
                    Debug.Log("Line" + amount);
                    if (Team[i].transform.childCount > 0 && amount > 0)
                    {
                        //Destroy(line.transform.GetChild(line.transform.childCount - 1).gameObject);
                        objectPool.PushObjectToPool(Team[i].transform.GetChild(Team[i].transform.childCount - 1).gameObject);
                        //yield return new WaitForEndOfFrame();
                        amount--;
                    }
                    else if (Team[i].transform.childCount <= 0) EmptyLines.Add(Team[i]);

                    if (amount <= 0) break;


                }
            }
        }
        else
        {
            foreach (var line in Team)
            {

                while (line.transform.childCount > 0)
                {
                    //Destroy(line.transform.GetChild(line.transform.childCount - 1).gameObject);
                    objectPool.PushObjectToPool(line.transform.GetChild(line.transform.childCount - 1).gameObject);



                }

                EmptyLines.Add(line);

            }
        }


        int tempInt = EmptyLines.Count;

        for (int i = 0; i < tempInt; i++)
        {
            GameObject tempObj = EmptyLines[i];
            Team.Remove(tempObj);
            Destroy(tempObj);
            if (Team.Count <= 0)
            {
                OnFailedLevel();
                Debug.Log("FAIL UI 2");
            }
        }
        ActionController.OnChangeOnTeam.Invoke(Team);
        foreach (var line in Team)
        {
            ReorderCenterAligned(line.gameObject, horizontalPadding);
        }
        lastWidth = GetWidth();
        StartCoroutine(UpdateTeamColorGradient(ActiveColor, Color.white, 0.1f));
        yield return new WaitForEndOfFrame();
        oPPlayerMovement.ActiveLineNumber = GetWidth();
    }

    public void AddRemoveHairCell(int amount, Transform line, bool isAddition, bool EqualizeZPos, bool isStart)
    {
        if (isAddition)
        {
            for (int k = 0; k < amount; k++)
            {
                GameObject hairCellGO = objectPool.PullFromPool();
                if (!isStart) hairCellGO.GetComponent<HairCell>().AddForce();
                hairCellGO.GetComponent<HairCell>().ChangeColor(ActiveColor);
                hairCellGO.GetComponent<HairCell>().ChangeModel(ActiveType);
                //Instantiate(HairCell, Vector3.zero, Quaternion.identity);
                hairCellGO.transform.SetParent(line);

                hairCellGO.transform.SetAsLastSibling();


            }
        }
        else
        {
            for (int k = 0; k < amount; k++)
            {
                //Destroy(line.GetChild(0).gameObject);
                if (line.GetChild(0) != null) objectPool.PushObjectToPool(line.GetChild(0).gameObject);

            }
        }
        //if (EqualizeZPos) ReorderCenterAligned(line.gameObject, horizontalPadding);
        //else
        //{
        //    ReorderCenterAlignedWithoutZ(line.gameObject, horizontalPadding);
        //    EqualizeZpos();
        //}
    }

    void HairDestoryedByObstacle(GameObject line, GameObject hair)
    {
        objectPool.PushObjectToPool(hair);
        if (GetHairCellsGO().Count <= 0)
        {
            Debug.Log("FAIL UI 3");
            OnFailedLevel();
        }
        UIManager.Instance.UpdateHairNumber();
    }

    void EqualizeZpos()
    {

        foreach (var line in Team)
        {
            foreach (Transform child in line.transform)
            {
                if (child.transform.localPosition.z != 0) child.transform.DOLocalMoveZ(0, 0.1f);
            }
        }
    }

    public void AddRemoveLine(int amount, bool isAddition, int lineWidth, bool isStart)
    {

        if (isAddition)
        {
            for (int i = 0; i < amount; i++)
            {
                GameObject hairLineGO = Instantiate(HairLine,
                                                     Vector3.zero,
                                                     Quaternion.identity);
                AddRemoveHairCell(lineWidth, hairLineGO.transform, true, true, isStart);
                Team.Add(hairLineGO);
                if (isStart)
                {
                    hairLineGO.transform.localPosition = Team[^2].transform.position + new Vector3(0, 0, -1f);
                }

            }

        }
    }

    public int GetTotalHairNumber()
    {
        int totalHairNumber = 0;
        foreach (var line in Team)
        {
            totalHairNumber += line.transform.childCount;
        }
        return totalHairNumber;
    }
    public List<GameObject> GetHairCellsGO()
    {
        List<GameObject> HairCellsGO = new();
        foreach (var line in Team)
        {
            foreach (Transform child in line.transform)
            {
                HairCellsGO.Add(child.gameObject);
            }
        }
        return HairCellsGO;
    }

    public int GetWidth()
    {
        int width = 0;
        foreach (var line in Team)
        {
            if (line.transform.childCount > width)
            {
                width = line.transform.childCount;
            }
        }

        return width;
    }

    public void ReorderCenterAligned(GameObject parentGO, float distanceBetweenCenters)
    {
        float totalWidth = (parentGO.transform.childCount - 1) * distanceBetweenCenters;
        float StartX = -totalWidth / 2;
        Vector3 StartLocalPos = new Vector3(StartX, 0, 0);


        foreach (Transform child in parentGO.transform)
        {
            child.transform.DOLocalMove(new Vector3(StartX, 0, 0), 0.5f);
            StartX += distanceBetweenCenters;
        }

    }

    public void ReorderCenterAlignedWithoutZ(GameObject parentGO, float distanceBetweenCenters, List<GameObject> newList)
    {
        float totalWidth = (parentGO.transform.childCount - 1) * distanceBetweenCenters;
        float StartX = -totalWidth / 2;

        foreach (Transform child in parentGO.transform)
        {
            if (newList.Contains(child.gameObject))
            {
                child.transform.localPosition = new Vector3(StartX, 0, -15);

            }
            StartX += distanceBetweenCenters;
        }
    }

    public void FillTeam()
    {
        StartCoroutine(FillTeamCoroutine());
        StartCoroutine(UpdateTeamColorGradient(ActiveColor, Color.white, 0.1f));

    }

    IEnumerator FillTeamCoroutine()
    {
        Debug.Log("FillTeamCoroutine");
        yield return new WaitForSeconds(0.5f);
        foreach (var line in Team)
        {
            ReorderCenterAligned(line, horizontalPadding);

        }

        if (GetWidth() > lastWidth / 2) lastWidth = GetWidth();


        //  -----------------------------------------------
        if (NeedsFullFill())
        {
            if (GetWidth() < lastWidth / 4) lastWidth = lastWidth / 3;
            int LastFilledLineNumber = GetTotalHairNumber() / lastWidth;
            int LastReminder = GetTotalHairNumber() % lastWidth;
            int fillNumber = 0;
            for (int i = 0; i < LastFilledLineNumber; i++)
            {
                try
                {
                    fillNumber += lastWidth - Team[i].transform.childCount;
                }
                catch
                {
                    Debug.Log("i " + i);
                }

            }
            for (int i = 0; i < fillNumber + 1; i++)
            {
                GameObject missingHairLine = FindFirstMissingHairLine();
                GameObject movedHairCell = FindLastHairCell();

                movedHairCell.transform.SetParent(missingHairLine.transform);
                ReorderCenterAligned(missingHairLine, horizontalPadding);
            }
        }



        //  -----------------------------------------------



        List<GameObject> EmptyLines = new();

        foreach (var line in Team)
        {
            if (line.transform.childCount <= 0)
            {
                EmptyLines.Add(line);
            }
        }


        int tempInt = EmptyLines.Count;

        for (int i = 0; i < tempInt; i++)
        {
            GameObject tempObj = EmptyLines[i];
            Team.Remove(tempObj);
            Destroy(tempObj);
            if (Team.Count <= 0)
            {
                OnFailedLevel();
                Debug.Log("FAIL UI 4");
            }
        }

        ActionController.OnChangeOnTeam.Invoke(Team);
        if (GetHairCellsGO().Count <= 0)
        {
            OnFailedLevel();
            Debug.Log("FAIL UI 5");
        }
    }

    bool NeedsFullFill()
    {
        bool fillFull = false;
        int maxChildCount = 0;
        int minChildCount = 1000;
        for (int i = 0; i < Team.Count - 1; i++)
        {
            if (maxChildCount < Team[i].transform.childCount)
            {
                maxChildCount = Team[i].transform.childCount;
            }

            if (minChildCount > Team[i].transform.childCount)
            {
                minChildCount = Team[i].transform.childCount;
            }
        }

        if (maxChildCount != minChildCount) fillFull = true;
        else fillFull = false;
        return fillFull;
    }

    GameObject FindFirstMissingHairLine()
    {
        GameObject emptyLine = Team[0];
        foreach (var line in Team)
        {
            if (line.transform.childCount < lastWidth)
            {
                emptyLine = line;
                return emptyLine;
            }
        }
        return emptyLine;
    }
    GameObject FindLastHairCell()
    {
        GameObject lastHaircell = default;
        for (int i = Team.Count - 1; i >= 0; i--)
        {
            if (Team[i].transform.childCount > 0)
            {
                lastHaircell = Team[i].transform.GetChild(0).gameObject;
                return lastHaircell;
            }
        }
        return lastHaircell;
    }



    public void AddForceToHair()
    {
        foreach (var line in Team)
        {
            foreach (Transform hair in line.transform)
            {
                hair.GetComponent<HairCell>().AddForce();
            }

        }
    }
    void SetTailGunnerPos()
    {
        TailGunnerGO.transform.localPosition = new Vector3(0, 0, -Team.Count * 0.45f);

    }

    void OnFailedLevel()
    {
        Debug.Log("ophsc Open FAILUI 1");
        UIManager.Instance.OnPlayerFailedLevel();
    }

    public void NewLevel()
    {
        HairNumberUI.NewLevel();
        foreach (var line in Team)
        {
            foreach (Transform child in line.transform)
            {
                objectPool.PushObjectToPool(child.gameObject);
            }
        }
        int tempInt = Team.Count;
        for (int i = 0; i < tempInt; i++)
        {
            Destroy(Team[i]);
        }
        Team.Clear();
        ActiveColor = BaseColor;
        ActiveType = 0;
        ActionController.OnChangeOnTeam.Invoke(Team);
        // GetComponent<OPHairStackCreator>().NewLevel();
        ActionController.OnResetForNewLevel.Invoke();
        UIManager.Instance.StartClickPrevent.SetActive(false);

    }

    void ChangeColor(Color color)
    {
        PlaySound(ColorgateSound);
        List<GameObject> HairList = GetHairCellsGO();
        ActiveColor = color;

        foreach (var hairCell in HairList)
        {
            hairCell.GetComponent<HairCell>().ChangeColor(color);
        }
        StartCoroutine(UpdateTeamColorGradient(ActiveColor, Color.white, 0.1f));
    }

    void ChangeShape(int shapeNum)
    {
        PlaySound(ShapegateSound);
        List<GameObject> HairList = GetHairCellsGO();
        ActiveType = shapeNum;
        foreach (var hairCell in HairList)
        {
            hairCell.GetComponent<HairCell>().ChangeModel(shapeNum);
        }
        StartCoroutine(UpdateTeamColorGradient(ActiveColor, Color.white, 0.1f));
    }

    public void PlaySound(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        if (Sound) audioSource.Play();
    }
}



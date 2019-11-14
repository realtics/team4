using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

enum ESceneState
{
    Start,
    Play,
    Stop,
    SetWinner,
    WaitRestart,
};
public class RopePullGameManager : MonoBehaviour
{
    private float startTimerSecond;
    private float playTime;

    private ESceneState sceneState;
    private GameObject winner;
    private GameObject loser;

    //[SerializeField] Insepector에서 보이나 private 같은 효과
    //protected GameObject onlyInspector;

    public GameObject playerableObjects;
    public Text playTimeText;
    public Text startCountingTimeText;
    public Text leftText;
    public Text rightText;
    public Button restartButton;

    void Start()
    {
        sceneState = ESceneState.Start;
        InitTimerValue();
        StartCoroutine(SecondCount());
    }

    void Update()
    {
        UpdateScene();
    }

    void InitTimerValue()
    {
        startTimerSecond = 6.0f;
        playTime = -5.0f;
    }

    IEnumerator SecondCount()
    {
        while (true)
        {
            --startTimerSecond;
            if(sceneState != ESceneState.SetWinner && sceneState != ESceneState.WaitRestart)
                ++playTime;
            yield return new WaitForSeconds(1);
        }
    }

    public void NotifyWinner(Transform _winner)
    {
        SetWinnerGame();
        SetResultText();
        winner = _winner.gameObject;
        SetOtherPlyerResult(winner, "winner");
    }

    public void NotifyLoser(Transform _loser)
    {
        SetWinnerGame();
        SetResultText();
        loser = _loser.gameObject;
        SetOtherPlyerResult(loser, "loser");
    }

    void UpdateScene()
    {
        if (sceneState == ESceneState.Start)
        {
            UpdateStartTime();
        }
        if (sceneState == ESceneState.Play)
        {
            UpdateFever();
            UpdatePlayTime();
        }
        if (sceneState == ESceneState.Stop)
        {

        }
        if (sceneState == ESceneState.SetWinner)
        {
            restartButton.gameObject.SetActive(true);
            sceneState = ESceneState.WaitRestart;
        }
        if (sceneState == ESceneState.WaitRestart)
        {
            
        }
        
    }

    void UpdatePlayTime()
    {
        if (playTime <= 0.0f)
        {
            playTimeText.text = string.Empty;
        }
        else
        {
            playTimeText.text = playTime.ToString();
        }
    }

    void UpdateStartTime()
    {
        if (startTimerSecond <= 0.0f)
        {
            startCountingTimeText.text = string.Empty;
            StartPlayGame();
        }
        else if(startTimerSecond <= 5.0f)
        {
            startCountingTimeText.text = startTimerSecond.ToString();
            
        }
    }

    void UpdateFever()
    {
        if (playTime >= 10.0f)
        {
            playerableObjects.GetComponent<RopePullMoveRopeWithKey>().SetFeverTime();
        }
    }

    void StartPlayGame()
    {
        sceneState = ESceneState.Play;
        SetObjectsMove(true);
    }

    public void RestartPlayGame()
    {
        sceneState = ESceneState.Start;
        winner = null;
        loser = null;
        leftText.text = "Left";
        rightText.text = "Right";
        InitTimerValue();
        SetRopeRestartPosition();
        UpdatePlayTime();
        playerableObjects.GetComponent<RopePullMoveRopeWithKey>().ResetFeverTime();

    }

    void SetRopeRestartPosition()
    {
        playerableObjects.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
    }

    void StopPlayGame()
    {
        sceneState = ESceneState.Stop;
        SetObjectsMove(false);
    }

    void SetObjectsMove(bool isMove)
    {
        playerableObjects.GetComponent<RopePullMoveRopeWithKey>().IsStart = isMove;
    }

    void SetWinnerGame()
    {
        sceneState = ESceneState.SetWinner;
        SetObjectsMove(false);
    }

    void SetResultText()
    {
        if (winner == gameObject.CompareTag("LeftPlayer"))
        {
            leftText.text = "Left Winner";
            rightText.text = "Right Loser";
        }
        else 
        {
            leftText.text = "Left Loser";
            rightText.text = "Right Winner";
        }
    }

    void SetOtherPlyerResult(GameObject ownObject, string result)
    {

        if (ownObject.CompareTag("LeftPlayer"))
        {
            if (result.CompareTo("winner") == 0 || result.CompareTo("Winner") == 0)
            {
                loser = GameObject.FindGameObjectWithTag("RightPlayer");
            }
            else if (result.CompareTo("loser") == 0 || result.CompareTo("Loser") == 0)
            {
                winner = GameObject.FindGameObjectWithTag("RightPlayer");
            }
        }
        else if (ownObject.CompareTag("RightPlayer"))
        {
            if (result.CompareTo("winner") == 0 || result.CompareTo("Winner") == 0)
            {
                loser = GameObject.FindGameObjectWithTag("LeftPlayer");
            }
            else if (result.CompareTo("loser") == 0 || result.CompareTo("Loser") == 0)
            {
                winner = GameObject.FindGameObjectWithTag("LeftPlayer");
            }
        }
        
    }
}

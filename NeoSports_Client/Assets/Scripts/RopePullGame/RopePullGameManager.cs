using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace RopePullGame
{
    public class RopePullGameManager : MonoBehaviour
    {
        enum ESceneState
        {
            Start,
            Play,
            Stop,
            SetWinner,
            WaitRestart,
        };
        ESceneState sceneState;

        GameObject winner;
        GameObject loser;

        float startTimerSecond;
        float playTime;

        public GameObject playerableObjects;
        public Text playTimeText;
        public Text startCountingTimeText;
        public Text leftText;
        public Text rightText;
        public Button restartButton;

        RopePullMoveRopeWithKey _RopePullMove;
        Character[] _Characters;

        void Start()
        {
            sceneState = ESceneState.Start;
            
            InitTimerValue();
            CachingValue();

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

        void CachingValue()
        {
            _RopePullMove = playerableObjects.GetComponent<RopePullMoveRopeWithKey>();
            _Characters = playerableObjects.GetComponentsInChildren<Character>();
        }

        IEnumerator SecondCount()
        {
            while (true)
            {
                --startTimerSecond;
                if (sceneState != ESceneState.SetWinner && sceneState != ESceneState.WaitRestart)
                    ++playTime;
                yield return new WaitForSeconds(1);
            }
        }

        public void NotifyWinner(Transform _winner)
        {
            SetWinnerGame();
            SetResultText();
            winner = _winner.gameObject;
        }

        public void NotifyLoser(Transform _loser)
        {
            SetWinnerGame();
            SetResultText();
            loser = _loser.gameObject;
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
            else if (startTimerSecond <= 5.0f)
            {
                startCountingTimeText.text = startTimerSecond.ToString();

            }
        }

        void UpdateFever()
        {
            if (playTime >= 10.0f)
            {
                _RopePullMove.SetFeverTime();
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
            _RopePullMove.ResetFeverTime();

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
            _RopePullMove.IsStart = isMove;
            SetCharactersAnimation(isMove);
        }

        void SetCharactersAnimation(bool isMove)
        {
            if (isMove)
            {
                foreach (Character ch in _Characters)
                {
                    ch.StartRun();
                }
            }
            else
            {
                foreach (Character ch in _Characters)
                {
                    ch.EndRun();
                }
            }
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

    }
}

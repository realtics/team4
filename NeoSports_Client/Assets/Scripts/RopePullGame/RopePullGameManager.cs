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

        public GameObject playerableObjects;
        public Text playTimeText;
        public Text startCountingTimeText;
        public Text leftText;
        public Text rightText;
        public Button restartButton;

        GameObject _winner;
        GameObject _loser;

        float startTimerSecond;
        float playTime;

        RopePullMoveRopeWithKey _ropePullMove;
        Character[] _characters;
        RunnigEffect[] _runnigEffects;

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
            _ropePullMove = playerableObjects.GetComponent<RopePullMoveRopeWithKey>();
            _characters = playerableObjects.GetComponentsInChildren<Character>();
            _runnigEffects = playerableObjects.GetComponentsInChildren<RunnigEffect>();

            foreach (RunnigEffect effect in _runnigEffects)
            {
                effect.EndEffect();
            }
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

        public void NotifyWinner(Transform winner)
        {
            SetWinnerGame();
            SetResultText();
            _winner = winner.gameObject;
        }

        public void NotifyLoser(Transform loser)
        {
            SetWinnerGame();
            SetResultText();
            _loser = loser.gameObject;
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
                _ropePullMove.SetFeverTime();
            }
        }

        void StartPlayGame()
        {
            sceneState = ESceneState.Play;
            _winner = null;
            _loser = null;
            SetObjectsMove(true);
        }

        public void RestartPlayGame()
        {
            sceneState = ESceneState.Start;
            _winner = null;
            _loser = null;
            leftText.text = "Left";
            rightText.text = "Right";
            InitTimerValue();
            SetRopeRestartPosition();
            UpdatePlayTime();
            _ropePullMove.ResetFeverTime();

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
            _ropePullMove.IsStart = isMove;
            SetCharactersAnimation(isMove);
            SetRuningEffects(isMove);
        }

        void SetCharactersAnimation(bool isMove)
        {
            if (isMove)
            {
                foreach (Character ch in _characters)
                {
                    ch.StartRun();
                }
            }
            else
            {
                foreach (Character ch in _characters)
                {
                    ch.EndRun();
                }
            }
        }

        void SetRuningEffects(bool isMove)
        {
            if (isMove)
            {
                foreach (RunnigEffect effect in _runnigEffects)
                {
                    effect.StartEffect();
                }
            }
            else
            {
                foreach (RunnigEffect effect in _runnigEffects)
                {
                    effect.EndEffect();
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
            if (_winner == gameObject.CompareTag("LeftPlayer"))
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

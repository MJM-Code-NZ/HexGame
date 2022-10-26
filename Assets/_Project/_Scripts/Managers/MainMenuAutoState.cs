using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MJM.HG
{
    public class MainMenuAutoState : MainMenuState
    {
        const string MainMenuUIGameObjectName = "MainMenuUI";
        MainMenuUI _mainMenuUI;

        public override void Awake() //WorldState(int worldSize, int numberOfPlayers) : base()
        {
            base.Awake();

            _stateName = GameStateName.MainMenuAutoState;
        }

        public override void Enter(GameStateName prevGameState)
        {
            if (!(prevGameState == GameStateName.MainMenuState))
            {
                _gmInstance.ProcessSceneLoad(GameManager.MenuScene);
            }
            else
            {
                _mainMenuUI = GameObject.Find(MainMenuUIGameObjectName).GetComponent<MainMenuUI>();
            }

            Execute();
        }

        public override void LoadSceneComplete()
        {
            _mainMenuUI = GameObject.Find(MainMenuUIGameObjectName).GetComponent<MainMenuUI>();
        }

        public override void Exit(GameStateName nextGameState)
        {
            if (!(nextGameState == GameStateName.MainMenuState))
            {
                _gmInstance.ProcessSceneUnload(GameManager.MenuScene);
            }
        }



        // This is the extra "automation" logic for the menu screen
        public override void Execute()
        {
            StartCoroutine(AutoProcess());
        }

        public IEnumerator AutoProcess()
        {
            int _targetWorldSize = RandomHelper.RandomRange(2, 14) * 2 + 1;
            int _targetPlayers = RandomHelper.RandomRange(1, PlayerSystem.CalcMaxPlayers((int)(_targetWorldSize - 1) / 2));

            float timer = 0;
            float duration = 13;

            float start = _mainMenuUI.WorldSizeSlider.value;

            while (timer <= duration)
            {
                timer += 1;

                float newPosition = Mathf.Lerp(start, _targetWorldSize, timer / duration);

                _mainMenuUI.WorldSizeSlider.value = newPosition;

                yield return new WaitForSeconds(1);
            }

            timer = 0;
            start = _mainMenuUI.PlayerSlider.value;

            while(timer <= duration)
            {
                timer += 1;

                float newPosition = Mathf.Lerp(start, _targetPlayers, timer / duration);

                _mainMenuUI.PlayerSlider.value = newPosition;

                yield return new WaitForSeconds(1);
            }

            yield return new WaitForSeconds(4);

            _gmInstance.HandleNewGameRequest((int)_mainMenuUI.WorldSizeSlider.value, (int)_mainMenuUI.PlayerSlider.value);

            yield return null;
        }
    }
}

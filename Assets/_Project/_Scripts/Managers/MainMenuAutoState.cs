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

        //public override void Awake() //WorldState(int worldSize, int numberOfPlayers) : base()
        //{
        //    base.Awake();
        //}

        public override void Enter(GameStateName prevGameStateName)
        {
            _stateName = GameStateName.MainMenuAutoState;
            _sceneName = SceneName.MenuScene;

            bool _loadRequired;

            _loadRequired = !(prevGameStateName == GameStateName.MainMenuState);

            if (_loadRequired)
            {
                _gmInstance.ProcessSceneLoad(_sceneName);
            }
            else
            {
                PostAndNoLoadShared();

                Execute();
            }         
        }

        public override void LoadSceneComplete() //GameStateName prevGameState
        {
            PostAndNoLoadShared();

            _mainMenuUI.AutoToggle.isOn = true;

            Execute();
        }

        // This is intended to catch common logic that must be performed regardless of whether a scene was loaded or not but
        // that cannot be performed until after the scene has been loaded
        public override void PostAndNoLoadShared()
        {
            _mainMenuUI = GameObject.Find(MainMenuUIGameObjectName).GetComponent<MainMenuUI>();
        }

        public override void Exit(GameStateName nextGameStateName)
        {
            bool _unloadRequired;

            StopCoroutine("AutoProcess");

            _unloadRequired = !(nextGameStateName == GameStateName.MainMenuState);

            if (_unloadRequired)
            {
                _gmInstance.ProcessSceneUnload(_sceneName);
            }
        }

        // This is the extra "automation" logic for the menu screen
        public override void Execute()
        {
            Debug.Log("Starting Menu Coroutine");
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

            _gmInstance.HandleNewGameRequest((int)(_mainMenuUI.WorldSizeSlider.value - 1) / 2, (int)_mainMenuUI.PlayerSlider.value, true);

            yield return null;
        }
    }
}

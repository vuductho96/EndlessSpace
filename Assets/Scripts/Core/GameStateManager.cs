using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SpaceShooter.Core
{
    public enum GameState
    {
        Boot,
        MainMenu,
        Playing,
        Paused,
        GameOver
    }

    public class GameStateManager : MonoBehaviour
    {
        public static GameStateManager Instance { get; private set; }

        [Header("State")]
        [SerializeField] private GameState _currentState = GameState.Playing;
        public GameState CurrentState => _currentState;

        public event Action<GameState> OnStateChanged;

        private readonly Stack<Action> _backButtonStack = new Stack<Action>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            // Android Back Button / Escape Key handling
            var kb = Keyboard.current;
            var pad = Gamepad.current;
            bool backPressed = (kb != null && kb.escapeKey.wasPressedThisFrame) || 
                               (pad != null && pad.bButton.wasPressedThisFrame);

            if (backPressed)
            {
                HandleBackNavigation();
            }
        }

        public void SetState(GameState newState)
        {
            if (_currentState == newState) return;
            _currentState = newState;

            Time.timeScale = (_currentState == GameState.Paused) ? 0f : 1f;
            OnStateChanged?.Invoke(_currentState);
            Debug.Log($"[GameStateManager] State transitioned to: {_currentState}");
        }

        public void TogglePause()
        {
            if (_currentState == GameState.Playing)
            {
                SetState(GameState.Paused);
            }
            else if (_currentState == GameState.Paused)
            {
                SetState(GameState.Playing);
            }
        }

        public void PushBackAction(Action action)
        {
            _backButtonStack.Push(action);
        }

        public void PopBackAction()
        {
            if (_backButtonStack.Count > 0) _backButtonStack.Pop();
        }

        public void HandleBackNavigation()
        {
            if (_backButtonStack.Count > 0)
            {
                var action = _backButtonStack.Pop();
                action?.Invoke();
                return;
            }

            if (_currentState == GameState.Playing)
            {
                TogglePause();
            }
            else if (_currentState == GameState.Paused)
            {
                TogglePause();
            }
        }

        // Android Lifecycle handling
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && _currentState == GameState.Playing)
            {
                // Auto pause when backgrounded or incoming phone call
                SetState(GameState.Paused);
                Debug.Log("[GameStateManager] App backgrounded: Game paused safely.");
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus && _currentState == GameState.Playing)
            {
                SetState(GameState.Paused);
            }
        }
    }
}

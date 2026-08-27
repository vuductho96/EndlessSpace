using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace SpaceShooter.Core
{
    [Serializable]
    public class SaveData
    {
        public int SaveVersion = 1;
        public long Timestamp;

        // Player Progression
        public int Credits = 0;
        public int HighScore = 0;
        public int SelectedShipIndex = 0;
        public string SelectedFighterId = "vanguard";
        public System.Collections.Generic.List<string> UnlockedFighterIds = new System.Collections.Generic.List<string> { "vanguard" };
        public float TotalActivePlayTimeSeconds = 0f;
        public int HullUpgradeLevel = 1;
        public int ShieldUpgradeLevel = 1;
        public int EnergyUpgradeLevel = 1;
        public int WeaponUpgradeLevel = 1;

        // Settings
        public int QualityLevel = 1; // 0=Low, 1=Med, 2=High
        public int TargetFps = 60;   // 30 or 60
        public bool HapticsEnabled = true;
        public float MasterVolume = 1.0f;
        public float MusicVolume = 0.8f;
        public float SfxVolume = 1.0f;
        
        // Touch Controls
        public float JoystickSize = 1.0f;
        public float JoystickOpacity = 0.8f;
        public int AimAssistLevel = 2; // 0=Off, 1=Low, 2=Med, 3=High
    }

    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }
        public SaveData CurrentSave { get; private set; }

        public event Action OnSaveDataChanged;

        private string SaveFilePath => Path.Combine(Application.persistentDataPath, "player_save_v1.json");

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadSaveData();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void LoadSaveData()
        {
            try
            {
                if (File.Exists(SaveFilePath))
                {
                    string json = File.ReadAllText(SaveFilePath, Encoding.UTF8);
                    CurrentSave = JsonUtility.FromJson<SaveData>(json);
                    
                    // Schema Migration Check
                    if (CurrentSave.SaveVersion < 1)
                    {
                        MigrateSaveData(CurrentSave);
                    }
                    Debug.Log($"[SaveManager] Save loaded successfully (Version: {CurrentSave.SaveVersion}, HighScore: {CurrentSave.HighScore})");
                }
                else
                {
                    CurrentSave = new SaveData { Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() };
                    SaveGame();
                    Debug.Log("[SaveManager] Initialized default save file.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveManager] Failed to load save file: {ex.Message}. Creating default fallback.");
                CurrentSave = new SaveData();
            }
        }

        public void SaveGame()
        {
            if (CurrentSave == null) return;
            CurrentSave.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            try
            {
                string json = JsonUtility.ToJson(CurrentSave, true);
                string tempPath = SaveFilePath + ".tmp";
                
                // Atomic file write
                File.WriteAllText(tempPath, json, Encoding.UTF8);
                if (File.Exists(SaveFilePath)) File.Delete(SaveFilePath);
                File.Move(tempPath, SaveFilePath);

                Debug.Log("[SaveManager] Game state saved atomically to disk.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveManager] Save failed: {ex.Message}");
            }
        }

        public bool IsFighterUnlocked(string fighterId)
        {
            if (CurrentSave == null || CurrentSave.UnlockedFighterIds == null) return string.Equals(fighterId, "vanguard", StringComparison.OrdinalIgnoreCase);
            return CurrentSave.UnlockedFighterIds.Exists(id => string.Equals(id, fighterId, StringComparison.OrdinalIgnoreCase));
        }

        public bool UnlockFighter(string fighterId)
        {
            if (CurrentSave == null) return false;
            if (CurrentSave.UnlockedFighterIds == null) CurrentSave.UnlockedFighterIds = new System.Collections.Generic.List<string>();

            if (!IsFighterUnlocked(fighterId))
            {
                CurrentSave.UnlockedFighterIds.Add(fighterId.ToLowerInvariant());
                SaveGame();
                OnSaveDataChanged?.Invoke();
                Debug.Log($"<color=#00FFCC><b>[SaveManager]</b></color> Fighter permanently unlocked: <b>{fighterId}</b>");
                return true;
            }
            return false;
        }

        public void SetSelectedFighter(string fighterId)
        {
            if (CurrentSave == null) return;
            CurrentSave.SelectedFighterId = fighterId.ToLowerInvariant();
            SaveGame();
            OnSaveDataChanged?.Invoke();
            Debug.Log($"[SaveManager] Selected fighter set to: {fighterId}");
        }

        public void AddActivePlayTime(float seconds)
        {
            if (CurrentSave == null || seconds <= 0f) return;
            CurrentSave.TotalActivePlayTimeSeconds += seconds;
            OnSaveDataChanged?.Invoke();
        }

        private void MigrateSaveData(SaveData oldData)
        {
            Debug.Log($"[SaveManager] Migrating save from v{oldData.SaveVersion} to v1...");
            oldData.SaveVersion = 1;
            if (oldData.UnlockedFighterIds == null || oldData.UnlockedFighterIds.Count == 0)
            {
                oldData.UnlockedFighterIds = new System.Collections.Generic.List<string> { "vanguard" };
            }
            if (string.IsNullOrEmpty(oldData.SelectedFighterId))
            {
                oldData.SelectedFighterId = "vanguard";
            }
            SaveGame();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                SaveGame();
            }
        }

        private void OnApplicationQuit()
        {
            SaveGame();
        }
    }
}

using LevelManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LevelManagement.Menus
{
    public class WinScreeen : Menu<WinScreeen>
    {
        public void OnNextLevelPressed()
        {
            base.OnBackPressed();
            LevelLoader.LoadNextLevel();
        }
        public void OnRestartPressed()
        {
            base.OnBackPressed();
            LevelLoader.ReloadLevel();
        }
        public void OnMainMenuPressed()
        {
            LevelLoader.LoadMainMenuLevel();
            MainMenu.Open();
        }
    }
}
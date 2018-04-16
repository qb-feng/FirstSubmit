using UnityEngine;

public class SharkGame : MonoBehaviour
{

    public SharkPlayerManager sharkPlayerManager;
    public SharkLevelGenerator sharkLevelGenerator;

    //Restarts the level
    public void Restart()
    {
        sharkLevelGenerator.Reset();
        sharkPlayerManager.Reset();

        StartLevel();
    }
    //Returns to the main menu
    public void QuitToMain()
    {
        sharkPlayerManager.Reset();
        sharkLevelGenerator.Reset();
    }
    //Starts the level
    public void StartLevel()
    {
        sharkLevelGenerator.SetPauseState(false);
        sharkPlayerManager.EnableShark();
        sharkLevelGenerator.StartToGenerate();
    }
    //Pauses the level
    public void PauseLevel()
    {
        sharkPlayerManager.SetPauseState(true);
        sharkLevelGenerator.SetPauseState(true);
    }
    //Resume the level
    public void ResumeLevel()
    {
        sharkPlayerManager.SetPauseState(false);
        sharkLevelGenerator.SetPauseState(false);
    }
    //Stops the level 
    public void StopLevel()
    {
        sharkLevelGenerator.StopGeneration(2);
    }
    //Revives the player, launches a sonic wave, and continue the level generation
    public void ReviveUsed()
    {
        sharkPlayerManager.Revive();
    }
}

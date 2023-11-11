using UnityEngine;

/// <summary>
/// Stage where user looks at start button when they are ready
/// </summary>
public class StartButtonStage : Stage {
    /// <summary>
    /// Activates the start button and deactivates everything else
    /// </summary>
    public override void Start() {
        StageStatic.GameObjects["startButton"].SetActive(true);
        StageStatic.GameObjects["instructions"].SetActive(false);
        StageStatic.GameObjects["target"].SetActive(false);
    }
    
    /// <summary>
    /// Moves the start button to 60 Unity distances away from the user
    /// </summary>
    public override void Update() {
        StageStatic.moveStartButtonTo(60);
    }
    
    /// <summary>
    /// Checks if the user has looked at the start button or the user has pressed A on the keyboard
    /// </summary>
    /// <returns>True if the user has indicated they want to start and false otherwise</returns>
    public override bool Finished() {
        return (StageStatic.HasActiveUser && (StageStatic.EyeDataCol.lObjectName == "StartButton" || StageStatic.EyeDataCol.rObjectName == "StartButton")) 
            || Input.GetKeyDown(KeyCode.A);
    }
    
    /// <summary>
    /// Does nothing
    /// </summary>
    public override void End() { }
}
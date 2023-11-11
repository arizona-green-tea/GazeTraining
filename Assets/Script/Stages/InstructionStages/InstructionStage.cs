using UnityEngine;

public class InstructionStage : Stage {
    /// <summary>
    /// Stores the time that has elapsed since the stage started
    /// </summary>
    private float _timeElapsed;
    
    /// <summary>
    /// Hides all game objects other than instructions
    /// </summary>
    public override void Start() {
        StageStatic.GameObjects["startButton"].SetActive(false);
        StageStatic.GameObjects["instructions"].SetActive(true);
        StageStatic.GameObjects["target"].SetActive(false);
        _timeElapsed = 0;
    }
    
    /// <summary>
    /// Updates the time and moves the instructions to the correct location
    /// </summary>
    public override void Update() {
        _timeElapsed += Time.deltaTime;
        StageStatic.moveInstructionsTo(50);
    }
    
    /// <summary>
    /// Checks if 10 seconds have elapsed
    /// </summary>
    /// <returns>True if 10 seconds elapsed, false otherwise</returns>
    public override bool Finished() {
        return _timeElapsed >= 10.0f;
    }
    
    /// <summary>
    /// Does nothing
    /// </summary>
    public override void End() { }
}
using System;

/// <summary>
/// A stage which only defines itself when it is ready to start (to prevent lag)
/// </summary>
public class FutureStage : Stage {
    /// <summary>
    /// The method which creates and returns the stage once it is ready
    /// </summary>
    private readonly Func<Stage> _stage;
    /// <summary>
    /// Stores the stage once the Start method is called
    /// </summary>
    private Stage _actualStage;
    
    /// <summary>
    /// Constructor which stores the stage function
    /// </summary>
    /// <param name="stage">The stage to store</param>
    public FutureStage(Func<Stage> stage) {
        _stage = stage;
    }
    
    /// <summary>
    /// Defines the stage and starts it
    /// </summary>
    public override void Start() {
        _actualStage = _stage();
        _actualStage.Start();
    }
    
    /// <summary>
    /// Updates the stage
    /// </summary>
    public override void Update() {
        if (!_actualStage.Finished()) _actualStage.Update();
    }
    
    /// <summary>
    /// Checks if the stage is finished
    /// </summary>
    /// <returns>True if the stage is finished and false otherwise</returns>
    public override bool Finished() { return _actualStage.Finished(); }
    
    /// <summary>
    /// Runs the End method of the stage
    /// </summary>
    public override void End() { _actualStage.End(); }
}
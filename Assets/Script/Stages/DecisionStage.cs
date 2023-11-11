using System;

/// <summary>
/// Stage which executes one of two stages based on a condition which is computed when executed
/// </summary>
public class DecisionStage : Stage {
    /// <summary>
    /// The function which determines which stage to use
    /// </summary>
    private readonly Func<bool> _useFirstStage;
    
    /// <summary>
    /// The two stages to choose from
    /// </summary>
    private readonly Stage _stage1, _stage2;
    
    /// <summary>
    /// The stage which is being used
    /// </summary>
    private Stage _usedStage;
    
    /// <summary>
    /// Constructor which stores the function and two stages to choose from
    /// </summary>
    /// <param name="useFirstStage">Function which returns true if the first stage should be used and false otherwise</param>
    /// <param name="stage1">The first stage to choose from</param>
    /// <param name="stage2">The second stage to choose from</param>
    public DecisionStage(Func<bool> useFirstStage, Stage stage1, Stage stage2) {
        _useFirstStage = useFirstStage;
        _stage1 = stage1;
        _stage2 = stage2;
    }
    
    /// <summary>
    /// Determines which stage to use and starts it
    /// </summary>
    public override void Start() {
        _usedStage = _useFirstStage() ? _stage1 : _stage2;
        _usedStage.Start();
    }
    
    /// <summary>
    /// Updates the stage that is being used
    /// </summary>
    public override void Update() {
        if (!_usedStage.Finished()) _usedStage.Update();
    }
    
    /// <summary>
    /// Checks if the stage that is being used is finished
    /// </summary>
    /// <returns>True if the stage is finished and false otherwise</returns>
    public override bool Finished() { return _usedStage.Finished(); }
    
    /// <summary>
    /// Ends the stage that is being used
    /// </summary>
    public override void End() { _usedStage.End(); }
}
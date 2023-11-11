/// <summary>
/// Stage which executes various other stages in succession
/// </summary>
public class StageList : Stage {
    /// <summary>
    /// Stores the stages to play
    /// </summary>
    private readonly Stage[] _stages;
    
    /// <summary>
    /// Stores the index of the current stage that is playing
    /// </summary>
    private int _index;
    
    /// <summary>
    /// Constructor which initializes all of the stages
    /// </summary>
    /// <param name="allStages">The stages to store</param>
    public StageList(params Stage[] allStages) { _stages = allStages; }
    
    /// <summary>
    /// Starts the first stage (if there is a first stage)
    /// </summary>
    public override void Start() { _index = 0; if (_stages.Length > 0) _stages[_index].Start(); }
    
    /// <summary>
    /// Updates the StageList during execution: ends the current stage if it is finished and starts the next one
    /// </summary>
    public override void Update() {
        if (!_stages[_index].Finished()) {
            _stages[_index].Update();
        } else {
            _stages[_index].End();
            _index++;
            if (!Finished()) _stages[_index].Start();
        }
    }
    
    /// <summary>
    /// Returns whether or not the stages have all finished
    /// </summary>
    /// <returns>True if all stages are finished and false otherwise</returns>
    public override bool Finished() { return _index == _stages.Length; }
    
    /// <summary>
    /// Executes nothing when the StageList ends (since each individual stage's end method has already been called)
    /// </summary>
    public override void End() { }
}
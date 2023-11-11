/// <summary>
/// Stage which executes any number of stages simultaneously until they both end
/// </summary>
class SimultaneousStage : Stage {
    /// <summary>
    /// The stages to parallelize
    /// </summary>
    private readonly Stage _stage1, _stage2;
    
    /// <summary>
    /// Constructor which converts a list of stages to parallelize into two stages
    /// </summary>
    /// <param name="stages">The list of stages to parallelize</param>
    public SimultaneousStage(params Stage[] stages) {
        if (stages.Length > 2) {
            _stage1 = stages[0];
            Stage[] restOfStages = new Stage[stages.Length - 1];
            for (int i = 1; i < stages.Length; i++) restOfStages[i - 1] = stages[i];
            _stage2 = new SimultaneousStage(restOfStages);
        } else if (stages.Length == 2) {
            _stage1 = stages[0];
            _stage2 = stages[1];
        } else if (stages.Length == 1) {
            _stage1 = stages[0];
            _stage2 = new CodeSegStage(() => {});
        } else {
            _stage1 = new CodeSegStage(() => {});
            _stage2 = new CodeSegStage(() => {});
        }
    }
    /// <summary>
    /// Starts all stages
    /// </summary>
    public override void Start() {
        _stage1.Start();
        _stage2.Start();
    }
    /// <summary>
    /// Updates all stages
    /// </summary>
    public override void Update() {
        if (!_stage1.Finished()) _stage1.Update();
        if (!_stage2.Finished()) _stage2.Update();
    }
    /// <summary>
    /// Checks if all stages are finished
    /// </summary>
    /// <returns>True if both stages are finished and false otherwise</returns>
    public override bool Finished() {
        return _stage1.Finished() && _stage2.Finished();
    }
    /// <summary>
    /// Runs the end method of all stages
    /// </summary>
    public override void End() {
        _stage1.End();
        _stage2.End();
    }
}
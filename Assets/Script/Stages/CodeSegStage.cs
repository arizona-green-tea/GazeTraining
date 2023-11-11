using System;

/// <summary>
/// Stage which runs based on CodeSeg which are passed in (wrapper making it easier to use)
/// </summary>
public class CodeSegStage : Stage {
    /// <summary>
    /// CodeSeg which stores start/update code 
    /// </summary>
    private readonly Action _startCodeSeg, _updateCodeSeg;
    
    /// <summary>
    /// Returns true if the stage is finished
    /// </summary>
    private readonly Func<bool> _finishedFunc;
    
    /// <summary>
    /// Constructor for a Stage which only executes code once
    /// </summary>
    /// <param name="startCodeSeg">Start method</param>
    public CodeSegStage(Action startCodeSeg) {
        _startCodeSeg = startCodeSeg;
        _updateCodeSeg = () => {};
        _finishedFunc = () => true;
    }
    
    /// <summary>
    /// Constructor for a Stage which only executes code once and then waits
    /// </summary>
    /// <param name="startCodeSeg">Start method</param>
    /// <param name="finished">Returns true when the stage should stop waiting</param>
    public CodeSegStage(Action startCodeSeg, Func<bool> finished) {
        _startCodeSeg = startCodeSeg;
        _updateCodeSeg = () => {};
        _finishedFunc = finished;
    }
    
    /// <summary>
    /// Constructor for a Stage which has all functionality
    /// </summary>
    /// <param name="startCodeSeg">Start method</param>
    /// <param name="updateCodeSeg">Update method</param>
    /// <param name="finished">Returns true when the stage is finished</param>
    public CodeSegStage(Action startCodeSeg, Action updateCodeSeg, Func<bool> finished) {
        _startCodeSeg = startCodeSeg;
        _updateCodeSeg = updateCodeSeg;
        _finishedFunc = finished;
    }
    
    /// <summary>
    /// Starts the stage
    /// </summary>
    public override void Start() { _startCodeSeg(); }
    
    /// <summary>
    /// Updates the stage
    /// </summary>
    public override void Update() { _updateCodeSeg(); }
    
    /// <summary>
    /// Checks if the stage is finished
    /// </summary>
    /// <returns>True if the stage has finished and false otherwise</returns>
    public override bool Finished() { return _finishedFunc(); }
    
    /// <summary>
    /// Ends the stage (unused)
    /// </summary>
    public override void End() { }
}
/// <summary>
/// A segment of code to execute in an organized manner
/// </summary>
public abstract class Stage {
    /// <summary>
    /// Executes once when the stage begins
    /// </summary>
    public abstract void Start();
    /// <summary>
    /// Executes in a loop after Start() until the stage is finished
    /// </summary>
    public abstract void Update();
    /// <summary>
    /// Returns whether or not this stage is finished
    /// </summary>
    /// <returns>True if the stage is finished and false otherwise</returns>
    public abstract bool Finished();
    /// <summary>
    /// Executes once when the stage ends
    /// </summary>
    public abstract void End();
}

using System;
using UnityEngine;

public class IPDSetupStage : Stage
{
    /// <summary>
    /// Stores the distance away from the user the dartboard should be
    /// </summary>
    private readonly float _distance;
    /// <summary>
    /// Stores amount to change IPD by every frame that the user is giving input
    /// </summary>
    private readonly float _dx;
    /// <summary>
    /// Stores true if the user is using their left eye and false if they are using their right
    /// </summary>
    private readonly bool _usingLeftEye;

    /// <summary>
    /// Constructor which stores all parameters
    /// </summary>
    /// <param name="usingLeftEye">True if the user is using their left eye and false if they are using their right</param>
    /// <param name="dx">The amount to change the IPD by every frame that the user is giving input</param>
    /// <param name="distance">Optional: the distance away from the user the dartboard should be</param>
    public IPDSetupStage(bool usingLeftEye, float dx, float distance=6) {
        _usingLeftEye = usingLeftEye;
        _distance = distance;
        _dx = dx;
    }
    
    /// <summary>
    /// Activates the dartboard and deactivates everything else
    /// </summary>
    public override void Start() {
        StageStatic.GameObjects["startButton"].SetActive(false);
        StageStatic.GameObjects["instructions"].SetActive(false);
        StageStatic.GameObjects["target"].SetActive(true);
    }

    /// <summary>
    /// Adjusts position/size/distance of dartboard and adjusts IPD based on user input
    /// </summary>
    public override void Update() {
        StageStatic.moveDartboardTo(_distance, 5, 0, 0);
        float change = 0;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            change = -_dx;
        } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            change = _dx;
        }
        if (_usingLeftEye) change *= -1;
        StageStatic.changeIPDBy(change);
    }
    
    /// <summary>
    /// Checks if the user indicated that they are happy with the displayed IPD
    /// </summary>
    /// <returns>True if the user is pressing A and false otherwise</returns>
    public override bool Finished() {
        return Input.GetKeyDown(KeyCode.A);
    }

    /// <summary>
    /// Unused
    /// </summary>
    public override void End() { }
}

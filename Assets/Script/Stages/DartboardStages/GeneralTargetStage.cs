using UnityEngine;
using System;

/// <summary>
/// Stage which deals with the dartboard and the user looking at the dartboard
/// </summary>
public class GeneralTargetStage : Stage {
    public double TimeElapsedTotal;
    private double _timeElapsedUser, _distance, _size, _xAng, _yAng;
    private readonly double _userViewTime, _maxTime; 
    public bool UserSucceeded;
    private Vector3 _stCamPos, _stCamRot;

    /// <summary>
    /// Constructor to initialize all fields
    /// </summary>
    /// <param name="distance">Distance from the user the dartboard should be at</param>
    /// <param name="size">Size the dartboard should be</param>
    /// <param name="xAng">Angle in x axis</param>
    /// <param name="yAng">Angle in y axis</param>
    /// <param name="userViewTime">Minimum time user needs to look at dartboard to succeed</param>
    /// <param name="maxTime">Maximum time user can take to succeed</param>
    public GeneralTargetStage(double distance, double size, double xAng, double yAng, double userViewTime, double maxTime) {
        _distance = distance;
        _size = size;
        _xAng = xAng;
        _yAng = yAng;
        _userViewTime = userViewTime;
        _maxTime = maxTime;
    }
    
    /// <summary>
    /// Creates new stage which changes the size & distance to make the dartboard seem to be of size sz
    /// </summary>
    /// <param name="sz">The size to make the dartboard</param>
    /// <returns>The modified stage</returns>
    public GeneralTargetStage GetWithSizeAdjustDistance(double sz) {
        return new GeneralTargetStage(
            _distance * (180 / Math.PI * 2 * Math.Atan(Math.Tan(sz * Math.PI / 180.0f * 0.5f)) / sz),
            sz, _xAng, _yAng, _userViewTime, _maxTime);
    }
    
    /// <summary>
    /// Creates new stage which changes the size of the dartboard to sz
    /// </summary>
    /// <param name="sz">The size to make the dartboard</param>
    /// <returns>The modified stage</returns>
    public GeneralTargetStage GetWithSize(double sz) {
        return new GeneralTargetStage(_distance, sz, _xAng, _yAng, _userViewTime, _maxTime);
    }
    
    /// <summary>
    /// Set the dartboard to be a certain distance away and adjust size accordingly
    /// </summary>
    /// <param name="dis">Distance to make the dartboard</param>
    public void SetDistance(double dis) {
        _size = 180/Math.PI * 2 * Math.Atan(Math.Tan(_size * Math.PI/180 * 1/2) * _distance/dis);
        _distance = dis;
    }
    
    /// <summary>
    /// Set the size of the dartboard
    /// </summary>
    /// <param name="sz">The size to make the dartboard</param>
    public void SetSize(double sz) {
        _size = sz;
    }
    
    /// <summary>
    /// Set the angle along the x axis to put the dartboard at
    /// </summary>
    /// <param name="xAng">Angle to move dartboard to</param>
    public void SetXAng(double xAng) { _xAng = xAng; }
    
    /// <summary>
    /// Set the angle along the y axis to put the dartboard at
    /// </summary>
    /// <param name="yAng">Angle to move dartboard to</param>
    public void SetYAng(double yAng) { _yAng = yAng; }
    
    /// <summary>
    /// Makes only the dartboard visible, resets timers, and updates camera positions
    /// </summary>
    public override void Start() {
        StageStatic.GameObjects["startButton"].SetActive(false);
        StageStatic.GameObjects["instructions"].SetActive(false);
        StageStatic.GameObjects["target"].SetActive(true);
        TimeElapsedTotal = 0;
        _timeElapsedUser = 0;
        StageStatic.StartingCameraPosition = StageStatic.GameObjects["camera"].transform.position;
        StageStatic.StartingCameraRotation = StageStatic.GameObjects["camera"].transform.localRotation.eulerAngles;
        StageStatic.moveDartboardTo(_distance, _size, _xAng, _yAng);
    }
    
    /// <summary>
    /// Updates timer, dartboard position, and checks if the user is looking at the dartboard or pressing A
    /// </summary>
    public override void Update() {
        TimeElapsedTotal += Time.deltaTime;

        StageStatic.moveDartboardTo(_distance, _size, _xAng, _yAng);

        if (StageStatic.EyeDataCol.worldPosL != new Vector3(0, 0, 0)) {
            StageStatic.GameObjects["left"].SetActive(true);
            StageStatic.GameObjects["left"].transform.position = StageStatic.EyeDataCol.worldPosL;
        }
        if (StageStatic.EyeDataCol.worldPosR != new Vector3(0, 0, 0)) {
            StageStatic.GameObjects["right"].SetActive(true);
            StageStatic.GameObjects["right"].transform.position = StageStatic.EyeDataCol.worldPosR;
        }

        var leftNum = -1;
        var rightNum = -1;
        if (StageStatic.EyeDataCol.lObjectName.Length > 6) {
            int.TryParse(StageStatic.EyeDataCol.lObjectName.Substring(6), out leftNum);
        }
        if (StageStatic.EyeDataCol.rObjectName.Length > 6) {
            int.TryParse(StageStatic.EyeDataCol.rObjectName.Substring(6), out rightNum);
        }

        if ((StageStatic.HasActiveUser && (leftNum < 1 || rightNum < 1)) || (!StageStatic.HasActiveUser && !Input.GetKey(KeyCode.A))) {
            _timeElapsedUser = -0.1f;
        } else {
            _timeElapsedUser += Time.deltaTime;
            if (TimePassedNumber()) {
                StageStatic.stopAllAudio();
                StageStatic.Audios["" + (_userViewTime - Math.Truncate(_timeElapsedUser))].Play();
            }
        }
    }
    
    /// <summary>
    /// Gets the size of the dartboard
    /// </summary>
    /// <returns>Size of the dartboard</returns>
    public double GetSize() { return _size; }
    
    /// <summary>
    /// Checks if the time just passed a whole number
    /// </summary>
    /// <returns>True if the time passed a whole number, false otherwise</returns>
    private bool TimePassedNumber() {
        return _timeElapsedUser - Time.deltaTime < Math.Truncate(_timeElapsedUser);
    }
    
    /// <summary>
    /// Checks if the user has succeeded or failed
    /// </summary>
    /// <returns>True if the user has succeeded or failed, false if in-progress</returns>
    public override bool Finished() {
        UserSucceeded = _timeElapsedUser >= _userViewTime;
        return TimeElapsedTotal >= _maxTime || _timeElapsedUser >= _userViewTime;
    }
    
    /// <summary>
    /// Does nothing
    /// </summary>
    public override void End() { }
}
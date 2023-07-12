using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ViveSR.anipal.Eye;
using System;

public class GeneralTargetStage : Stage {
    public float timeElapsedTotal, timeElapsedUser;
    private float distance, size, xAng, yAng, userViewTime, maxTime; 
    public bool userSucceeded;
    private Vector3 stCamPos, stCamRot;

    public GeneralTargetStage(float distance, float size, float xAng, float yAng, float userViewTime, float maxTime) {
        this.distance = distance;
        this.size = size;
        this.xAng = xAng;
        this.yAng = yAng;
        this.userViewTime = userViewTime;
        this.maxTime = maxTime;
    }
    public GeneralTargetStage getWithDistance(float dis) {
        return new GeneralTargetStage(dis,
            (float)(180/Math.PI * 2 * (float)Math.Atan((float)Math.Tan(size * Math.PI/180 * 1/2) * distance/dis)),
            xAng, yAng, userViewTime, maxTime);
    }
    public GeneralTargetStage getWithSize(float sz) {
        return new GeneralTargetStage(distance, sz, xAng, yAng, userViewTime, maxTime);
    }
    public void setDistance(float dis) {
        size = (float)(180/Math.PI * 2 * (float)Math.Atan((float)Math.Tan(size * Math.PI/180 * 1/2) * distance/dis));
        this.distance = dis;
        Debug.Log("Set distance");
    }
    public void setSize(float sz) {
        this.size = sz;
    }
    public void setXAng(float xAng) { this.xAng = xAng; }
    public void setYAng(float yAng) { this.yAng = yAng; }
    public override void start() {
        StageStatic.GameObjects["startButton"].SetActive(false);
        StageStatic.GameObjects["instructions"].SetActive(false);
        StageStatic.GameObjects["target"].SetActive(true);
        timeElapsedTotal = 0;
        timeElapsedUser = 0;
        StageStatic.startingCameraPosition = StageStatic.GameObjects["camera"].transform.position;
        StageStatic.startingCameraRotation = StageStatic.GameObjects["camera"].transform.localRotation.eulerAngles;
        StageStatic.moveDartboardTo(distance, size, xAng, yAng);
    }
    public override void update() {
        timeElapsedTotal += Time.deltaTime;

        StageStatic.moveDartboardTo(distance, size, xAng, yAng);

        if (StageStatic.EyeDataCol.worldPosL != new Vector3(0, 0, 0)) {
            StageStatic.GameObjects["left"].SetActive(true);
            StageStatic.GameObjects["left"].transform.position = StageStatic.EyeDataCol.worldPosL;
        }
        if (StageStatic.EyeDataCol.worldPosR != new Vector3(0, 0, 0)) {
            StageStatic.GameObjects["right"].SetActive(true);
            StageStatic.GameObjects["right"].transform.position = StageStatic.EyeDataCol.worldPosR;
        }

        int leftNum = -1;
        int rightNum = -1;
        if (StageStatic.EyeDataCol.lObjectName.Length > 6) {
            int.TryParse(StageStatic.EyeDataCol.lObjectName.Substring(6), out leftNum);
        }
        if (StageStatic.EyeDataCol.rObjectName.Length > 6) {
            int.TryParse(StageStatic.EyeDataCol.rObjectName.Substring(6), out rightNum);
        }

        if ((StageStatic.hasActiveUser && (leftNum < 1 || rightNum < 1)) || (!StageStatic.hasActiveUser && !Input.GetKey(KeyCode.A))) {
            timeElapsedUser = -0.1f;
        } else {
            timeElapsedUser += Time.deltaTime;
            if (timePassedNumber()) {
                StageStatic.stopAllAudio();
                StageStatic.Audios["" + (userViewTime - Math.Truncate(timeElapsedUser))].Play();
            }
        }
    }
    private bool timePassedNumber() {
        return (timeElapsedUser - Time.deltaTime < Math.Truncate(timeElapsedUser));
    }
    public override bool finished() {
        userSucceeded = timeElapsedUser >= userViewTime;
        return timeElapsedTotal >= maxTime || timeElapsedUser >= userViewTime;
    }
    public override void end() { }
}
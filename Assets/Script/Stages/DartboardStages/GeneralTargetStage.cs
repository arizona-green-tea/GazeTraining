using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ViveSR.anipal.Eye;
using System;

public class GeneralTargetStage : Stage {
    public double timeElapsedTotal, timeElapsedUser;
    private double distance, size, xAng, yAng, userViewTime, maxTime; 
    public bool userSucceeded;
    private Vector3 stCamPos, stCamRot;

    public GeneralTargetStage(double distance, double size, double xAng, double yAng, double userViewTime, double maxTime) {
        this.distance = distance;
        this.size = size;
        this.xAng = xAng;
        this.yAng = yAng;
        this.userViewTime = userViewTime;
        this.maxTime = maxTime;
    }
    public GeneralTargetStage getWithSizeAdjustDistance(double sz) {
        return new GeneralTargetStage(
            distance * (180 / Math.PI * 2 * Math.Atan(Math.Tan(sz * Math.PI / 180.0f * 0.5f)) / sz),
            sz, xAng, yAng, userViewTime, maxTime);
    }
    public GeneralTargetStage getWithSize(double sz) {
        return new GeneralTargetStage(distance, sz, xAng, yAng, userViewTime, maxTime);
    }
    public void setDistance(double dis) {
        size = 180/Math.PI * 2 * Math.Atan(Math.Tan(size * Math.PI/180 * 1/2) * distance/dis);
        distance = dis;
    }
    public void setSize(double sz) {
        size = sz;
    }
    public void setXAng(double xAng) { this.xAng = xAng; }
    public void setYAng(double yAng) { this.yAng = yAng; }
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
    public double getSize() { return size; }
    private bool timePassedNumber() {
        return (timeElapsedUser - Time.deltaTime < Math.Truncate(timeElapsedUser));
    }
    public override bool finished() {
        userSucceeded = timeElapsedUser >= userViewTime;
        return timeElapsedTotal >= maxTime || timeElapsedUser >= userViewTime;
    }
    public override void end() { }
}
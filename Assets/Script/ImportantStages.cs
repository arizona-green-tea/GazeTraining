using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ViveSR.anipal.Eye;
using System;
using System.Linq;

public static class ImportantStages
{
    private static double binarySearchResult;
    private static double thresholdResult;
    
    /**
     * Plays a piece of audio until it finishes
     */
    public static Stage PlayAudio(AudioSource audio) {
        return new CodesegStage(() => {StageStatic.stopAllAudio(); audio.Play();}, () => {return !audio.isPlaying;});
    }
    
    /**
     * Executes a GeneralTargetStage, and then plays audio based on the result
     */
    public static Stage PlayAudioFromResult(GeneralTargetStage targetStage, AudioSource success, AudioSource failure) {
        return new StageList(
            targetStage,
            new DecisionStage(() => targetStage.userSucceeded, PlayAudio(success), PlayAudio(failure))
        );
    }
    
    /**
     * Conducts binary search on the approximate size where the user can see the the dartboard
     * Either changes distance or size, based on parameter input
     * Parameters: minimum size to check, maximum size to check, and precision of size (visual degrees)
     */
    private static Stage binarySearch(GeneralTargetStage targetStage, double minSize, double maxSize, double precision, bool changeDistance) {
        var newStage = changeDistance ? targetStage.getWithSizeAdjustDistance((minSize + maxSize)/2) : targetStage.getWithSize((minSize + maxSize)/2);
        if (maxSize - minSize < precision)
        {
            return new StageList(
                new CodesegStage(() => binarySearchResult = newStage.getSize())
                // PlayAudio(StageStatic.Audios["smallestVisualAngleAudio"]),
                // PlayAudio(StageStatic.Audios["" + Math.Truncate(newStage.getSize())]),
                // PlayAudio(StageStatic.Audios["degreesAudio"])
            );
        }
        return new StageList(
            newStage,
            new DecisionStage(
                () => newStage.userSucceeded,
                new FutureStage(() => binarySearch(targetStage, minSize, (minSize + maxSize)/2, precision, changeDistance)),
                new FutureStage(() => binarySearch(targetStage, (minSize + maxSize)/2, maxSize, precision, changeDistance))
            )
        );
    }
    
    /**
     * Conducts three down one up for the approximate threshold which the user can see at __% of the time 
     */
    private static Stage threeDownOneUp(GeneralTargetStage targetStage, double currentSize, double percentage, int currentSuccesses, List<double> failedPlaces, bool changeDistance) {
        var newStage = changeDistance ? targetStage.getWithSizeAdjustDistance(currentSize) : targetStage.getWithSize(currentSize);
        const double negativeStep = 1;
        var positiveStep = negativeStep * Math.Pow(percentage, 3) / (1 - Math.Pow(percentage, 3));
        
        if (failedPlaces.Count == 5) {
            var threshold = failedPlaces.Average();
            return new StageList(
                PlayAudio(StageStatic.Audios["smallestVisualAngleAudio"]),
                PlayAudio(StageStatic.Audios["" + Math.Truncate(threshold)]),
                PlayAudio(StageStatic.Audios["degreesAudio"])
            );
        }
        return new StageList(
            newStage,
            new DecisionStage(
                () => newStage.userSucceeded,
                new DecisionStage(
                    () => currentSuccesses == 2,
                    new FutureStage(() => 
                        threeDownOneUp(targetStage, currentSize - positiveStep, percentage, 0, failedPlaces, changeDistance)
                    ),
                    new FutureStage(() => 
                        threeDownOneUp(targetStage, currentSize, percentage, currentSuccesses + 1, failedPlaces, changeDistance)
                    )
                ),
                new FutureStage(() =>
                {
                    failedPlaces.Add(currentSize);
                    return threeDownOneUp(targetStage, currentSize + negativeStep, percentage, 0,
                        failedPlaces, changeDistance);
                })
            )
        );
    }

    public static Stage findThreshold(double xAng, double yAng, double minTimeToView, double timePerAttempt, 
        double percentageCertainty, bool changingDistance) {
        return new StageList(
            binarySearch(
                new GeneralTargetStage(100, 10, xAng, yAng, minTimeToView, timePerAttempt),
                1, 50, 1, changingDistance
            ),
            new FutureStage(() => threeDownOneUp(
                new GeneralTargetStage(100, 10, xAng, yAng, minTimeToView, timePerAttempt),
                binarySearchResult, percentageCertainty/100, 0, new List<double>(),
                changingDistance
            ))
        );
    }
    
    /**
     * Moves the target around while changing distance from the user 
     */
    public static Stage movingTargetChangingDis(double userViewTime, double maxTime, double stSize, Func<double, double> distance, Func<double, double> xAng, Func<double, double> yAng) {
        var mainStage = new GeneralTargetStage(distance(0), stSize, xAng(0), yAng(0), userViewTime, maxTime);
        return new SimulataneousStage(
            mainStage,
            new CodesegStage(() => {}, () => {
                mainStage.setDistance(distance(mainStage.timeElapsedTotal));
                mainStage.setXAng(xAng(mainStage.timeElapsedTotal));
                mainStage.setYAng(yAng(mainStage.timeElapsedTotal));
            }, () => mainStage.finished())
        );
    }
    
    /**
     * Moves the target around while changing size
     */
    public static Stage movingTargetChangingSize(double userViewTime, double maxTime, double stDistance, Func<double, double> size, Func<double, double> xAng, Func<double, double> yAng) {
        var mainStage = new GeneralTargetStage(stDistance, size(0), xAng(0), yAng(0), userViewTime, maxTime);
        return new SimulataneousStage(
            mainStage,
            new CodesegStage(() => {}, () => {
                mainStage.setSize(size(mainStage.timeElapsedTotal));
                mainStage.setXAng(xAng(mainStage.timeElapsedTotal));
                mainStage.setYAng(yAng(mainStage.timeElapsedTotal));
            }, () => mainStage.finished())
        );
    }
}
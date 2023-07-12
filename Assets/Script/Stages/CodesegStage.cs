using System;
public class CodesegStage : Stage {
    Action startCodeseg, updateCodeseg;
    Func<bool> finishedFunc;
    public CodesegStage(Action startCodeseg) {
        this.startCodeseg = startCodeseg;
        this.updateCodeseg = () => {};
        finishedFunc = () => {return true;};
    }
    public CodesegStage(Action startCodeseg, Func<bool> finished) {
        this.startCodeseg = startCodeseg;
        this.updateCodeseg = () => {};
        finishedFunc = finished;
    }
    public CodesegStage(Action startCodeseg, Action updateCodeseg, Func<bool> finished) {
        this.startCodeseg = startCodeseg;
        this.updateCodeseg = updateCodeseg;
        this.finishedFunc = finished;
    }
    public override void start() { startCodeseg(); }
    public override void update() { updateCodeseg(); }
    public override bool finished() { return finishedFunc(); }
    public override void end() { }
}
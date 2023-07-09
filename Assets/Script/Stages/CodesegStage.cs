using System;
public class CodesegStage : Stage {
    Action codeSeg;
    Func<bool> finishedFunc;
    public CodesegStage(Action codeseg) { codeSeg = codeseg; finishedFunc = () => {return true;}; }
    public CodesegStage(Action codeseg, Func<bool> finished) { codeSeg = codeseg; finishedFunc = finished; }
    public override void start() { codeSeg(); }
    public override void update() { }
    public override bool finished() { return finishedFunc(); }
    public override void end() { }
}
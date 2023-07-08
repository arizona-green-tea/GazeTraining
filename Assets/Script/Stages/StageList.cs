public class StageList : Stage {
    private Stage[] stages;
    private int index;
    public StageList(params Stage[] allStages) { stages = allStages; }
    public override void start() { index = 0; }
    public override void update() {
        if (!stages[index].finished()) {
            stages[index].update();
        } else {
            stages[index].end();
            index++;
            if (!finished()) stages[index].start();
        }
    }
    public override bool finished() { return index == stages.Length; }
    public override void end() { }
}
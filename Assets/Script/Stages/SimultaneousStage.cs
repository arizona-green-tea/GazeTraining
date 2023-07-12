class SimulataneousStage : Stage {
    Stage stage1, stage2;
    public SimulataneousStage(params Stage[] stages) {
        if (stages.Length > 2) {
            stage1 = stages[0];
            Stage[] restOfStages = new Stage[stages.Length - 1];
            for (int i = 1; i < stages.Length; i++) restOfStages[i - 1] = stages[i];
            stage2 = new SimulataneousStage(restOfStages);
        } else if (stages.Length == 2) {
            stage1 = stages[0];
            stage2 = stages[1];
        } else if (stages.Length == 1) {
            stage1 = stages[0];
            stage2 = new CodesegStage(() => {});
        } else {
            stage1 = new CodesegStage(() => {});
            stage2 = new CodesegStage(() => {});
        }
    }
    public override void start() {
        stage1.start();
        stage2.start();
    }
    public override void update() {
        stage1.update();
        stage2.update();
    }
    public override bool finished() {
        return stage1.finished() && stage2.finished();
    }
    public override void end() {
        stage1.end();
        stage2.end();
    }
}
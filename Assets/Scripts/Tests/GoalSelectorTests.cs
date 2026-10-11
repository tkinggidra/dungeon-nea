using NUnit.Framework;
using World.Entity.AI;

public class GoalSelectorTests
{
    private sealed class TestGoal : Goal
    {
        public bool Enabled { get; set; } = true;
        public bool Continue { get; set; } = true;

        public int Starts { get; private set; }
        public int Ticks { get; private set; }
        public int Stops { get; private set; }

        public override GoalFlag Flags { get; }

        public TestGoal(GoalFlag flags)
        {
            Flags = flags;
        }

        public override bool CanUse() => Enabled;

        public override bool CanContinue() => Continue;

        public override void Start() => Starts++;

        public override void Tick() => Ticks++;

        public override void Stop() => Stops++;
    }

    [Test]
    public void EligibleGoalStartsAndTicks()
    {
        var selector = new GoalSelector();
        var goal = new TestGoal(GoalFlag.Move);

        selector.AddGoal(1, goal);
        selector.Tick();
        selector.Tick();

        Assert.AreEqual(1, goal.Starts);
        Assert.AreEqual(2, goal.Ticks);
        Assert.AreEqual(0, goal.Stops);
    }

    [Test]
    public void DisabledGoalDoesNotStart()
    {
        var selector = new GoalSelector();
        var goal = new TestGoal(GoalFlag.Move)
        {
            Enabled = false
        };

        selector.AddGoal(1, goal);
        selector.Tick();

        Assert.AreEqual(0, goal.Starts);
        Assert.AreEqual(0, goal.Ticks);
    }

    [Test]
    public void HigherPriorityGoalInterruptsLowerPriorityGoal()
    {
        var selector = new GoalSelector();

        var wander = new TestGoal(GoalFlag.Move);
        var chase = new TestGoal(GoalFlag.Move)
        {
            Enabled = false
        };

        selector.AddGoal(2, wander);
        selector.AddGoal(1, chase);

        selector.Tick();

        Assert.AreEqual(1, wander.Starts);

        chase.Enabled = true;
        selector.Tick();

        Assert.AreEqual(1, wander.Stops);
        Assert.AreEqual(1, chase.Starts);
        Assert.AreEqual(1, chase.Ticks);
    }

    [Test]
    public void CompatibleGoalsRunTogether()
    {
        var selector = new GoalSelector();

        var move = new TestGoal(GoalFlag.Move);
        var look = new TestGoal(GoalFlag.Look);

        selector.AddGoal(1, move);
        selector.AddGoal(1, look);

        selector.Tick();

        Assert.AreEqual(1, move.Ticks);
        Assert.AreEqual(1, look.Ticks);
    }

    [Test]
    public void EqualPriorityConflictingGoalsCannotBothRun()
    {
        var selector = new GoalSelector();

        var first = new TestGoal(GoalFlag.Move);
        var second = new TestGoal(GoalFlag.Move);

        selector.AddGoal(1, first);
        selector.AddGoal(1, second);

        selector.Tick();

        Assert.AreEqual(1, first.Starts + second.Starts);
        Assert.AreEqual(1, first.Ticks + second.Ticks);
    }

    [Test]
    public void GoalStopsWhenItCannotContinue()
    {
        var selector = new GoalSelector();
        var goal = new TestGoal(GoalFlag.Attack);

        selector.AddGoal(1, goal);
        selector.Tick();

        goal.Continue = false;
        goal.Enabled = false;

        selector.Tick();

        Assert.AreEqual(1, goal.Stops);
        Assert.AreEqual(1, goal.Ticks);
    }
}
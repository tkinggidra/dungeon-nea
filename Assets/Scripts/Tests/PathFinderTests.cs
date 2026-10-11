using Core.Collection;
using NUnit.Framework;
using World.Navigation;

public class PathFinderTests
{
    private static NavigationGrid CreateGrid(int width, int height)
    {
        var grid = new NavigationGrid(width, height);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                grid.SetWalkable(x, z, true);
            }
        }

        return grid;
    }

    [Test]
    public void StraightPath()
    {
        var grid = CreateGrid(10, 10);
        var finder = new PathFinder(grid);

        var path = finder.FindPath(1, 1, 5, 1);

        Assert.AreEqual(5, path.Count);
        Assert.AreEqual((1, 1), path[0]);
        Assert.AreEqual((5, 1), path[path.Count - 1]);
    }

    [Test]
    public void DiagonalPath()
    {
        var grid = CreateGrid(10, 10);
        var finder = new PathFinder(grid);

        var path = finder.FindPath(1, 1, 5, 5);

        foreach (var cell in path)
            TestContext.WriteLine($"({cell.X}, {cell.Z})");

        Assert.AreEqual(5, path.Count);
        Assert.AreEqual((1, 1), path[0]);
        Assert.AreEqual((5, 5), path[path.Count - 1]);
    }
    
    [Test]
    public void MinHeapReturnsLowestPriority()
    {
        var heap = new MinHeap<string>();

        heap.Enqueue("Straight", 62);
        heap.Enqueue("Diagonal", 56);
        heap.Enqueue("Other", 70);

        Assert.AreEqual("Diagonal", heap.Dequeue());
        Assert.AreEqual("Straight", heap.Dequeue());
        Assert.AreEqual("Other", heap.Dequeue());
    }

    [Test]
    public void PathAroundWall()
    {
        var grid = CreateGrid(10, 10);

        for (int z = 0; z < 9; z++)
        {
            grid.SetWalkable(4, z, false);
        }

        var finder = new PathFinder(grid);
        var path = finder.FindPath(1, 1, 8, 1);

        Assert.IsNotEmpty(path);
        Assert.Contains((4, 9), path);

        foreach (var cell in path)
        {
            Assert.IsTrue(grid.IsWalkable(cell.X, cell.Z));
        }
    }

    [Test]
    public void UnreachableGoal()
    {
        var grid = CreateGrid(10, 10);

        grid.SetWalkable(4, 5, false);
        grid.SetWalkable(6, 5, false);
        grid.SetWalkable(5, 4, false);
        grid.SetWalkable(5, 6, false);

        var finder = new PathFinder(grid);
        var path = finder.FindPath(1, 1, 5, 5);

        Assert.IsEmpty(path);
    }

    [Test]
    public void CannotCutCorners()
    {
        var grid = CreateGrid(3, 3);

        grid.SetWalkable(1, 0, false);
        grid.SetWalkable(0, 1, false);

        var finder = new PathFinder(grid);
        var path = finder.FindPath(0, 0, 1, 1);

        Assert.IsEmpty(path);
    }

    [Test]
    public void StartEqualsGoal()
    {
        var grid = CreateGrid(5, 5);
        var finder = new PathFinder(grid);

        var path = finder.FindPath(2, 2, 2, 2);

        Assert.AreEqual(1, path.Count);
        Assert.AreEqual((2, 2), path[0]);
    }
}
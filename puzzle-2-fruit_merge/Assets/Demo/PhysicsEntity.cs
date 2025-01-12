using Ex.Verlet;

public class PhysicsEntity
{
    public Point point;
    public bool isFruit;
    public FruitType fruitType;
    public float weight;

    public PhysicsEntity(Point point, bool isFruit, FruitType fruitType, float weight)
    {
        this.point = point;
        this.isFruit = isFruit;
        this.fruitType = fruitType;
        this.weight = weight;
    }
}
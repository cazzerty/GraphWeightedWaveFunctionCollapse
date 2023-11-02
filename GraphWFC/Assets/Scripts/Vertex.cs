using UnityEngine;

public class Vertex
{
    [SerializeField] private double weight = 1;
    [SerializeField] private Vector3 worldPosition;

    public Vertex(Vector3 worldPosition, double weight)
    {
        this.worldPosition = worldPosition;
        this.weight = weight;
    }

    public double GetWeight()
    {
        return weight;
    }
    public void SetWeight(double weight)
    {
        this.weight = weight;
    }
    public Vector3 GetWorldPosition()
    {
        return worldPosition;
    }
    public void SetWorldPosition(Vector3 worldPosition)
    {
        this.worldPosition = worldPosition;
    }
}
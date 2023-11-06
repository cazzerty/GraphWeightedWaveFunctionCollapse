using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class GraphManager : MonoBehaviour
{
    [SerializeField] private List<Vertex> _vertices = new List<Vertex>();
    //[SerializeField] private List<Vector3> _verticesToAdd;
    [SerializeField] private double[,] _adjacencyMatrix;

    [SerializeField] private bool renderGizmos = true;
    [SerializeField] private bool renderShortestDistanceToLine = true;

    [SerializeField] public int start = 0;

    [SerializeField] public int end = 1;
    // Start is called before the first frame update
    void Awake()
    {
        CreateDebugGraph();
    }

    public void CreateDebugGraph()
    {
        Graph2();

        end = _vertices.Count - 1;
    }

    private void Graph1()
    {
        AddVertex(1,new Vector3(20,23,0));
        AddVertex(1,new Vector3(20,26,0));
        AddVertex(1,new Vector3(22,26,0));
        AddVertex(1,new Vector3(13,22,0));
        AddVertex(1,new Vector3(25,28,0));
        AddEdge(1,0,1);
        AddEdge(1,1,2);
        AddEdge(1,1,3);
        AddEdge(1,2,4);
    }

    private void Graph2()
    {
        AddVertex(1,new Vector3(5,5,0)); //0
        AddVertex(1,new Vector3(15,5,0)); //1
        AddVertex(1,new Vector3(25,5,0)); //2
        AddVertex(1,new Vector3(35,5,0)); //3
        
        AddVertex(1,new Vector3(5,15,0)); //4
        AddVertex(1,new Vector3(15,15,0)); //5
        AddVertex(1,new Vector3(35,15,0)); //6
        AddVertex(1,new Vector3(45,15,0)); //7
        
        AddEdge(4,0,1);
        AddEdge(4,1,2);
        AddEdge(4,2,3);
        
        AddEdge(4,0,4);
        AddEdge(4,1,5);
        AddEdge(4,3,6);
        
        AddEdge(4,4,5);
        AddEdge(4, 6, 7);

    }
    
    private void Graph3()
    {
        AddVertex(1,new Vector3(5,5,0)); //0
        AddVertex(1,new Vector3(15,4,0)); //1
        AddVertex(1,new Vector3(25,6,0)); //2
        AddVertex(1,new Vector3(35,5,0)); //3
        
        AddVertex(1,new Vector3(5,15,0)); //4
        AddVertex(1,new Vector3(15,15,0)); //5
        AddVertex(1,new Vector3(35,16,0)); //6
        AddVertex(1,new Vector3(45,15,0)); //7
        
        AddVertex(1,new Vector3(38,24,0)); //7
        
        AddEdge(3,0,1);
        AddEdge(3,1,2);
        AddEdge(4,2,3);
        
        AddEdge(3,0,4);
        AddEdge(4,1,5);
        AddEdge(3,3,6);
        
        AddEdge(6,4,5);
        AddEdge(3, 6, 7);
        
        AddEdge(3, 7, 8);
        AddEdge(1, 5, 8);

    }

    public void AddVertex(double weight, Vector3 worldPosition)
    {
        _vertices.Add(new Vertex(worldPosition,weight));
        SetAdjacencyMatrixSize();
    }

    public void AddEdge(double weight, int i1, int i2)
    {
        _adjacencyMatrix[i1, i2] = weight;
        _adjacencyMatrix[i2, i1] = weight;
    }

    private void SetAdjacencyMatrixSize()
    {
        int size = _vertices.Count;
        _adjacencyMatrix = new double[size,size];
    }

    public Vector3 GetVertexWorldPosition(int index)
    {
        return _vertices[index].GetWorldPosition();
    }

    #region DistanceMath

    public double GetDistanceToClosestEdge(Vector3 point)
    {
        if(_adjacencyMatrix == null){return -1;}
        
        double minDistance = -1;
        
        for (int i = 0; i < _adjacencyMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < _adjacencyMatrix.GetLength(1); j++)
            {
                if (_adjacencyMatrix[i, j] > 0)
                {
                    double distance = GetDistanceToClosestPointOnFiniteLine(point, _vertices[i].GetWorldPosition(), _vertices[j].GetWorldPosition());
                    if (minDistance < 0 || distance < minDistance) { minDistance = distance; }
                }
            }  
        }
        
        return minDistance;
    }
    
    public double[] GetDistanceToClosestEdgeAndEdgeWeight(Vector3 point)
    {
        if(_adjacencyMatrix == null){return new double[]{-1,-1};}
        
        double minDistance = -1;
        double edgeWeight = -1;
        
        for (int i = 0; i < _adjacencyMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < _adjacencyMatrix.GetLength(1); j++)
            {
                if (_adjacencyMatrix[i, j] > 0)
                {
                    double distance = GetDistanceToClosestPointOnFiniteLine(point, _vertices[i].GetWorldPosition(), _vertices[j].GetWorldPosition());
                    if (minDistance < 0 || distance < minDistance)
                    {
                        minDistance = distance;
                        edgeWeight = _adjacencyMatrix[i, j];
                    }
                }
            }  
        }
        
        return new double[]{minDistance,edgeWeight};
    }
    private double GetDistanceToClosestPointOnFiniteLine(Vector3 point, Vector3 v1, Vector3 v2)
    {
        double px = v2.x - v1.x; //length of edge x
        double py = v2.y - v1.y; //length of edge x
        double temp = (px * px) + (py * py); //length of edge
        double u=((point.x - v1.x) * px + (point.y - v1.y) * py) / (temp); //Dot / edge length
        if(u>1){
            u=1;
        }
        else if(u<0){
            u=0;
        }
        double x = v1.x + u * px;
        double y = v1.y + u * py;
        
        float xOut = Convert.ToSingle(x);
        float yOut = Convert.ToSingle(y);

        if (renderShortestDistanceToLine)
        {
            Vector3 vec3 = new Vector3(xOut, yOut, 0);
        
            Debug.DrawLine(point,vec3,Color.gray);
        }
        double dx = x - point.x;
        double dy = y - point.y;
        double dist = Math.Sqrt(dx*dx + dy*dy);
        return dist;
    }
    
    Vector3 GetClosestPointOnFiniteLine(Vector3 point, Vector3 v1, Vector3 v2)
    {
        double px = v2.x - v1.x; //length of edge x
        double py = v2.y - v1.y; //length of edge x
        double temp = (px * px) + (py * py); //length of edge
        double u=((point.x - v1.x) * px + (point.y - v1.y) * py) / (temp); //Dot / edge length
        if(u>1){
            u=1;
        }
        else if(u<0){
            u=0;
        }
        double x = v1.x + u * px;
        double y = v1.y + u * py;
        
        float xOut = Convert.ToSingle(x);
        float yOut = Convert.ToSingle(y);
        
        return new Vector3(xOut, yOut, 0);
    }


    private double GetDistanceToClosestPointOnInfiniteLine(Vector3 point, Vector3 v1, Vector3 v2)
    {
        double e1 = Math.Abs((v2.x - v1.x)*(v1.y - point.y) - (v1.x - point.x)*(v2.y - v1.y));
        double e2 = Math.Sqrt(Math.Pow((v2.x - v1.x), 2) + Math.Pow((v2.y - v1.y), 2));
        return e1 / e2;
    }

    #endregion
    #region DebugGizmos

    private void OnDrawGizmos()
    {
        if(!renderGizmos){return;}
        if(_vertices == null){return;}
        for (int i = 0; i < _vertices.Count; i++)
        {
            Gizmos.color = Color.red;
            Vector3 pos = _vertices[i].GetWorldPosition();
            if(i == start){Gizmos.color = Color.green;}
            if(i == end){Gizmos.color = Color.magenta;}
            
            Gizmos.DrawSphere(pos, 0.2f);
            Handles.Label(pos, i.ToString());
        }
        if(_adjacencyMatrix == null){return;}
        for (int i = 0; i < _adjacencyMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < _adjacencyMatrix.GetLength(1); j++)
            {
                if (_adjacencyMatrix[i, j] > 0)
                {
                    Debug.DrawLine(_vertices[i].GetWorldPosition(),_vertices[j].GetWorldPosition(),Color.red);
                    Handles.Label((_vertices[i].GetWorldPosition() + _vertices[j].GetWorldPosition()) / 2, _adjacencyMatrix[i,j].ToString());
                }
            }  
        }
    }

    #endregion
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TerrainFace : MonoBehaviour
{
    /// <summary>
    /// This script handles the terrain face generation
    /// </summary>
    /// 
    ShapeGenerator shapeGenerator;
    Mesh mesh;
    MeshCollider meshCollider;
    int resolution;
    Vector3 localUp;
    Vector3 axisA;
    Vector3 axisB;
    Transform mountainTransform;
    int seed, spawnDensity;
    
    GameObject circle;
    GameObject[] enemyPrefab, otherPrefabs, otherPrefabsWR;
    Player player;
    GameObject[] enemies, others, othersWR, circles;
    int enemyIndex, otherIndex, otherWRIndex;
    int enemyMaxNum, othersMaxNum, othersWRMaxNum;

    [HideInInspector]
    public Vector3 peakPoint;

    [HideInInspector]
    public GameObject[] circleParent, enemyParent, otherParent, otherWRParent;

    private int groupNum = 4;

    //Contructor
    public TerrainFace(ShapeGenerator shapeGenerator, Mesh mesh, MeshCollider meshCollider, int resolution, 
        Vector3 localUp, int seed, int spawnDensity, GameObject[] enemyPrefab, 
        GameObject[] otherPrefabs, GameObject[] otherPrefabsWithRotation, GameObject circle, Transform mountainTransform, int enemyMaxNum, int othersMaxNum, int othersWRMaxNum, Player player) {
        this.shapeGenerator = shapeGenerator;
        this.mesh = mesh;
        this.meshCollider = meshCollider;
        this.resolution = resolution;
        this.localUp = localUp;
        this.seed = seed;
        this.spawnDensity = spawnDensity;
        this.enemyPrefab = enemyPrefab;
        this.otherPrefabs = otherPrefabs;
        this.otherPrefabsWR = otherPrefabsWithRotation;
        this.circle = circle;
        this.mountainTransform = mountainTransform;
        this.enemyMaxNum = enemyMaxNum;
        this.othersMaxNum = othersMaxNum;
        this.othersWRMaxNum = othersWRMaxNum;
        this.player = player;
        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);
    }

    public void ConstructMesh() {
        Vector3[] vertices = new Vector3[resolution * resolution];//List of vertex
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];//Number of triangles
        int triIndex = 0;
        enemyIndex = 0;
        otherIndex = 0;
        otherWRIndex = 0;

        circles = new GameObject[vertices.Length];
        enemies = new GameObject[enemyMaxNum];
        others = new GameObject[othersMaxNum];
        othersWR = new GameObject[othersWRMaxNum];
        enemyParent = new GameObject[groupNum];
        otherParent = new GameObject[groupNum];
        otherWRParent = new GameObject[groupNum];
        for (int i = 0; i < groupNum; i++)
        {
            enemyParent[i] = new GameObject("enemyParent");
            enemyParent[i].transform.parent = mountainTransform.transform;
            enemyParent[i].transform.position = new Vector3(0, 0, 0);
            enemyParent[i].AddComponent<ContentSection>();
            enemyParent[i].GetComponent<ContentSection>().isEnemy = true;
           


            otherParent[i] = new GameObject("otherParent");
            otherParent[i].transform.parent = mountainTransform.transform;
            otherParent[i].transform.position = new Vector3(0,0,0);
            otherParent[i].AddComponent<ContentSection>();
            otherParent[i].GetComponent<ContentSection>().isEnemy = false;

            otherWRParent[i] = new GameObject("otherWRParent");
            otherWRParent[i].transform.parent = mountainTransform.transform;
            otherWRParent[i].transform.position = new Vector3(0, 0, 0);
            otherWRParent[i].AddComponent<ContentSection>();
            otherWRParent[i].GetComponent<ContentSection>().isEnemy = false;

            if (player != null)
            {
                enemyParent[i].GetComponent<ContentSection>().player = player;
                otherParent[i].GetComponent<ContentSection>().player = player;
                otherWRParent[i].GetComponent<ContentSection>().player = player;
            }
        }

        for (int y = 0; y < resolution; y++) {
            for (int x = 0; x < resolution; x++) {
                int i = x + y * resolution;
                Vector2 percent = new Vector2(x, y) / (resolution - 1);//What percent of the generation is complete
                Vector3 pointOnUnitCube = localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;//Current position on the face
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                
                vertices[i] = shapeGenerator.CalculatePointOnMountain(pointOnUnitSphere, seed, i);//add the points to array with noise applied

                

                //Random.seed = System.DateTime.Now.Millisecond;
                if (vertices[i].y > 3) {

                    GenerateContent(vertices[i], i);
                }
             

                if (x != resolution-1&&y != resolution-1) {//Not at the edge
                    //Triangle1
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + resolution + 1;
                    triangles[triIndex + 2] = i + resolution;
                    //Triangle2
                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 1;
                    triangles[triIndex + 5] = i + resolution + 1;
                    triIndex += 6;
                }
            }
        }
        //Adding vertices and triangles to the mesh
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        //Set peak spawn location
        if (localUp == Vector3.up)
        {
            peakPoint = vertices[shapeGenerator.elevationMinMax.Index] + mountainTransform.position;//Assign peak vertex
        }
        //Set mesh
        if (meshCollider != null) {
            meshCollider.sharedMesh = mesh;
        }

        circleParent = new GameObject[8];

        //Set circleParent
        for (int i = 0; i < 8; i ++) {
            circleParent[i] = new GameObject("circleParent");
            circleParent[i].transform.parent = mountainTransform.transform;
            circleParent[i].transform.position = new Vector3(0, 0, 0);
            circleParent[i].AddComponent<ContentSection>();
            circleParent[i].GetComponent<ContentSection>().isCircle = true;
            if (player != null) {
                circleParent[i].GetComponent<ContentSection>().player = player;
            }
        }

        for (int i = 0; i < vertices.Length; i++) {
            if (vertices[i].y > -0.1)
            {
                if (i <= (vertices.Length / 8)) {

                    GenerateCircle(vertices[i] + mountainTransform.position, i, circleParent[0].transform);
                } else if (i <= ((vertices.Length / 8) * 2)) {

                    GenerateCircle(vertices[i] + mountainTransform.position, i, circleParent[1].transform);
                } else if (i <= ((vertices.Length / 8) * 3)) {

                    GenerateCircle(vertices[i] + mountainTransform.position, i, circleParent[2].transform);
                }
                else if (i <= ((vertices.Length / 8) * 4))
                {

                    GenerateCircle(vertices[i] + mountainTransform.position, i, circleParent[3].transform);
                }
                else if (i <= ((vertices.Length / 8) * 5))
                {

                    GenerateCircle(vertices[i] + mountainTransform.position, i, circleParent[4].transform);
                }
                else if (i <= ((vertices.Length / 8) * 6))
                {

                    GenerateCircle(vertices[i] + mountainTransform.position, i, circleParent[5].transform);
                }
                else if (i <= ((vertices.Length / 8) * 7))
                {

                    GenerateCircle(vertices[i] + mountainTransform.position, i, circleParent[6].transform);
                }
                else if (i <= vertices.Length) {

                    GenerateCircle(vertices[i] + mountainTransform.position, i, circleParent[7].transform);
                }
            
        }

        }
    }

    void GenerateContent(Vector3 currentVertex, int index) {
        

        int randomNumber = UnityEngine.Random.Range(0, resolution * resolution);
        //Debug.Log(randomNumber);
        if (randomNumber <= spawnDensity) {
            if (UnityEngine.Random.Range(0, 3) == 0)
            {//Choose one
                AssignParent(currentVertex, index, 0, enemyParent);
            }
            else if (UnityEngine.Random.Range(0, 3) == 1)
            {
                AssignParent(currentVertex, index, 1, otherParent);
            } else if (UnityEngine.Random.Range(0, 3) == 2) {

                AssignParent(currentVertex, index, 2, otherWRParent);
            }

        }
        
    }

    void AssignParent(Vector3 currentVertex, int index, int content, GameObject[] parent) {
        if (index <= ((resolution * resolution) / groupNum))
        {
            ChooseGeneration(currentVertex, content, parent, 0);
        }
        else if (index <= (((resolution * resolution) / groupNum) * 2))
        {

            ChooseGeneration(currentVertex, content, parent, 1);
        }
        else if (index <= (((resolution * resolution) / groupNum) * 3))
        {

            ChooseGeneration(currentVertex, content, parent, 2);
        }
        else if (index <= (resolution * resolution))
        {

            ChooseGeneration(currentVertex, content, parent, 3);
        }
    }

    void ChooseGeneration(Vector3 currentVertex, int content, GameObject[] parent, int index) {
        switch (content) {
            case 0:
                GenerateEnemy(currentVertex + mountainTransform.position, parent[index].transform);
                break;
            case 1:
                GenerateOthers(currentVertex + mountainTransform.position, parent[index].transform);
                break;
            case 2:
                GenerateOthersWR(currentVertex + mountainTransform.position, parent[index].transform);
                break;
        }
    }

    public void DeactivateEnenmies() {

        foreach (GameObject enemy in enemies) {

			if (enemy != null)
			{

				enemy.SetActive(false);
			}
        }
    }

    public void ActivateEnenmies()
    {
        foreach (GameObject enemy in enemies)
        {
			if (enemy != null) {

				enemy.SetActive(true);
			}
        }
    }


    void GenerateCircle(Vector3 spawnLocation, int index, Transform parent) {
        if (circle != null) {
            //Debug.Log(shapeGenerator.elevationMinMax.elevation.Count);
            
            circles[index] = Instantiate(circle);
            circles[index].transform.parent = parent;
            circles[index].transform.position = spawnLocation + (Vector3.up*0.5f);
            circles[index].GetComponent<TerrainCircle>().elevation = shapeGenerator.elevationMinMax.elevation[index];
            circles[index].GetComponent<TerrainCircle>().maxElevation = shapeGenerator.elevationMinMax.Max;
            circles[index].GetComponent<TerrainCircle>().minElevation = shapeGenerator.elevationMinMax.Min;
            if (index < (shapeGenerator.elevationMinMax.elevation.Count-5) && index > 0) {
                circles[index].GetComponent<TerrainCircle>().prevElevation = shapeGenerator.elevationMinMax.elevation[(index - 1)];
                circles[index].GetComponent<TerrainCircle>().nextElevation = shapeGenerator.elevationMinMax.elevation[(index + 1)];
            }
            if (player != null) {
                circles[index].GetComponent<TerrainCircle>().player = player;
            }
            circles[index].gameObject.SetActive(false);
            //Debug.Log(index);
            //circles[index].SetActive(false);
        }
    }

    void GenerateEnemy(Vector3 spawnLocation, Transform parent) {

        int index;
        if (enemyPrefab.Length != 0)
        {
            index = UnityEngine.Random.Range(0, enemyPrefab.Length - 1);
            if (enemyPrefab[Mathf.Abs(index)] != null && enemyIndex < enemyMaxNum)
            {
                enemies[enemyIndex] = Instantiate(enemyPrefab[index]);
                enemies[enemyIndex].transform.parent = parent;
                enemies[enemyIndex].transform.position = spawnLocation;
                enemies[enemyIndex].SetActive(false);
                enemies[enemyIndex].transform.eulerAngles = new Vector3(0, spawnLocation.y, 0);
                enemyIndex++;
            }

        }

    }

    void GenerateOthers(Vector3 spawnLocation, Transform parent) {

        int index;
        if (otherPrefabs != null) {
            if (otherPrefabs.Length != 0)
            {
                index = UnityEngine.Random.Range(0, otherPrefabs.Length - 1);
                if (otherPrefabs[Mathf.Abs(index)]!= null && otherIndex < othersMaxNum)
                {
                    others[otherIndex] = Instantiate(otherPrefabs[index]);
                    others[otherIndex].transform.parent = parent;
                    others[otherIndex].transform.position = spawnLocation;
                    others[otherIndex].SetActive(false);
                    others[otherIndex].transform.eulerAngles = new Vector3(0, 0, 0);
                    otherIndex++;
                }
            }
        }

    }

    void GenerateOthersWR(Vector3 spawnLocation, Transform parent)
    {
        int index;
        if (otherPrefabsWR != null) {
            if (otherPrefabsWR.Length != 0)
            {
                index = UnityEngine.Random.Range(0, otherPrefabsWR.Length - 1);
                if (otherPrefabsWR[Mathf.Abs(index)] != null && otherWRIndex < othersWRMaxNum)
                {
                    othersWR[otherWRIndex] = Instantiate(otherPrefabsWR[index]);
                    othersWR[otherWRIndex].transform.parent = parent;
                    othersWR[otherWRIndex].transform.position = spawnLocation;
                    othersWR[otherWRIndex].SetActive(false);
                    othersWR[otherWRIndex].transform.eulerAngles = new Vector3(-spawnLocation.x, spawnLocation.y, -spawnLocation.z);
                    otherWRIndex++;
                }
            }
        }
    }
}

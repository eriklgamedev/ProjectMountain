using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using OVR;
public class Mountain : MonoBehaviour
{
    /// <summary>
    /// This script handles the mountain generation
    /// </summary>
    /// 
    [SerializeField]
    private bool generateOnStart = true;
    [SerializeField]
    private bool randomGenerateOnStart = true;

    [Range(0, 400)]
    public int spawnDensity = 200;

    [SerializeField]
    private int enemyMaxNum = 40, othersMaxNum = 5, othersWRMaxNum = 5;

    [SerializeField]
    private GameObject peakPrefab, terrainCircle;
    [SerializeField]
    private GameObject[] enemyPrefab, otherPrefabs, otherPrefabsWithRotation;
    [SerializeField]
    private Player player;


    [Range(2, 256)]
    public int resolution = 10;
    public int seed;
    public bool autoUpdate = false;
    public enum FaceRenderMask { All, Top, Bottom, Left, Right, Front, Back };
    public FaceRenderMask faceRenderMask;


//#if (UNITY_EDITOR)
    public ShapeSettings shapeSettings;
    public ColorSettings colorSettings;

    [HideInInspector]
    public bool shapeSettingsFoldout;
    [HideInInspector]
    public bool colorSettingsFoldout;

//#endif

    ShapeGenerator shapeGenerator = new ShapeGenerator();
    ColorGenerator colorGenerator = new ColorGenerator();

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;//For rendering
    MeshCollider[] meshColliders;
    TerrainFace[] terrainFaces;
	[SerializeField]
	private Text seedText;

    private void OnValidate() {//Editor update
        //seed = Random.Range(0, 99999);
        //GenerateFoundation();
    }

    private void Awake()
    {
        if (generateOnStart == true) {

            GenerateFoundation();
        } else if (randomGenerateOnStart == true) {
            GenerateSeed();
        }
    }

    void Initialize() {
        shapeGenerator.UpdateSettings(shapeSettings);
        colorGenerator.UpdateSettings(colorSettings);

        if (meshFilters == null || meshFilters.Length == 0) {//Only create meshFilter when null
            meshFilters = new MeshFilter[6];
        }
        if (meshColliders == null || meshColliders.Length == 0) {
            meshColliders = new MeshCollider[6];
        }

        terrainFaces = new TerrainFace[6];

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        for (int i = 0; i < 6; i++) {
            if (meshFilters[i] == null) {//Only create when empty
                GameObject meshObj = new GameObject("mesh");
                meshObj.transform.parent = transform;
                meshObj.transform.position = new Vector3(0,0,0);
				meshObj.gameObject.tag = "Gardenable";
                meshObj.AddComponent<MeshRenderer>();
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
				
                if (meshColliders[i] == null) {
                    meshColliders[i] = meshObj.AddComponent<MeshCollider>();
                    meshObj.GetComponent<MeshCollider>().sharedMesh = null;
                    meshObj.GetComponent<MeshCollider>().sharedMesh = meshFilters[i].sharedMesh;
                }
            }

           
            meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = colorSettings.mountainMaterial;
            terrainFaces[i] = new TerrainFace(shapeGenerator, meshFilters[i].sharedMesh, meshColliders[i], 
                resolution, directions[i],seed, spawnDensity, enemyPrefab, otherPrefabs, otherPrefabsWithRotation, terrainCircle,
                meshFilters[i].gameObject.transform, enemyMaxNum, othersMaxNum, othersWRMaxNum, player);//Add to terrainface
                                                                                    //Debug.Log(transform.position);
            
            bool renderFace = faceRenderMask == FaceRenderMask.All || (int)faceRenderMask - 1 == i;
            meshFilters[i].gameObject.SetActive(renderFace);
            if (meshColliders[i] != null) {
                meshColliders[i].gameObject.SetActive(renderFace);
            }
        }
    }

    private void Update()
    {
        
		if (seedText != null) {
			seedText.text = seed.ToString();

		}
	}




	public void GenerateFoundation()
    {
        Initialize();
        GenerateMesh();
        GenerateColors();
    }

    public void GenerateSeed() {
        seed = Random.Range(0, 99999);
        Initialize();
        GenerateMesh();
        GenerateColors();
    }


    public void OnShapeSettingUpdated() {//Only when shape setting changed
        if (autoUpdate) {
            Initialize();
            GenerateMesh();
        }
    }

    public void OnColorSettingUpdated() {//Only when color setting changed
        if (autoUpdate)
        {
            Initialize();
            GenerateColors();
        }
    }

    void GenerateMesh() {
        for (int i = 0; i < 6; i++) {
            if (meshFilters[i].gameObject.activeSelf) {
                //Debug.Log(meshFilters[i].gameObject.name);
                if (meshFilters[i].gameObject.transform.childCount > 0) {
                    for (int _i = meshFilters[i].gameObject.transform.childCount - 1; _i >= 0; _i--)
                    {
                        if (Application.isEditor == true && Application.isPlaying == false)
                        {
                            if (meshFilters[i].gameObject.transform.GetChild(_i).gameObject != null)
                            {
                                DestroyImmediate(meshFilters[i].gameObject.transform.GetChild(_i).gameObject);
                            }
                        }
                        else if (Application.isPlaying)
                        {
                            if (meshFilters[i].gameObject.transform.GetChild(_i).gameObject != null)
                            {
                                Destroy(meshFilters[i].gameObject.transform.GetChild(_i).gameObject);
                            }
                        }
                    }
                }
                    
            }
                
                terrainFaces[i].ConstructMesh();
            }
        GeneratePeak(terrainFaces[0].peakPoint);//Only for up face
        colorGenerator.UndateElevation(shapeGenerator.elevationMinMax);
    }

    public void DeactivateEnemiesForEachFace() {
        for (int i = 0; i < 6; i++) {
            terrainFaces[i].DeactivateEnenmies();
        }
    }

    public void ActivateEnemiesForEachFace() {
        for (int i = 0; i < 6; i++)
        {
            terrainFaces[i].ActivateEnenmies();
        }
    }

    public void ActivateCirclesForEachFace() {
        for (int i = 0; i < 6; i++)
        {
            //terrainFaces[i].circleParent.SetActive(true);
        }
    }
    public void DeactivateCirclesForEachFace()
    {
        for (int i = 0; i < 6; i++)
        {
            //terrainFaces[i].circleParent.SetActive(false);
        }
    }

    void GeneratePeak(Vector3 peakPoint) {
		if (peakPrefab != null) {
			GameObject peak = Instantiate(peakPrefab);
			peak.transform.parent = meshFilters[0].gameObject.transform;
			peak.transform.position = peakPoint;
		}
        
    }



    void GenerateColors() {
        colorGenerator.UpdateColors();
    }
}

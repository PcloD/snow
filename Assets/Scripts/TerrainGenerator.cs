﻿using UnityEngine;
using System.Collections;

namespace COMP30019.Project2
{
    public class TerrainGenerator : MonoBehaviour
    {
        [Tooltip("The total size in world units of the terrain")]
        public Vector3 size = new Vector3(1024.0f, 1024.0f, 1024.0f);

        [Tooltip("Resolution of the heightmap for the terrain, value of 2^n + 1")]
        public int heightmapResolution = 1025;

        [Tooltip("x and y parameters of Mathf.PerlinNoise are multiplied by this value")]
        public float noiseVariation = 0.5f;

        [Tooltip("Value between 0.0f and 1.0f")]
        public float noiseMagnitude = 0.1f;

        [Tooltip("Value between 0.0f and 1.0f")]
        public float slopeTopHeight = 0.7f;

        [Tooltip("Value between 0.0f and 1.0f")]
        public float slopeBottomHeight = 0.3f;

        [Tooltip("The material for the terrain")]
        public Material terrainMaterial;

        private GameObject terrainObj;
        private TerrainData terrainData;
        private Terrain terrain;
        private float[,] heightmap;

        public void Generate()
        {
            terrainData = new TerrainData();
            terrainData.size = new Vector3(size.x / 32, size.z , size.y / 32);
            terrainData.heightmapResolution = heightmapResolution;

            GenerateHeightmap();
            terrainData.SetHeights(0, 0, heightmap);

            terrainObj = Terrain.CreateTerrainGameObject(terrainData);
            terrainObj.tag = "Terrain";

            terrain = terrainObj.GetComponent<Terrain>();
            terrain.materialType = Terrain.MaterialType.Custom;
            terrain.materialTemplate = terrainMaterial;

            terrainMaterial.SetColor("_PointLightColor", Color.white);
            terrainMaterial.SetVector("_PointLightPosition", new Vector4(size.x*2, size.y*2, size.z*2));
        }

        public float[,] GetHeightmap()
        {
            return heightmap;
        }

        private void GenerateHeightmap()
        {
            float height;
            float noise;

            float seed = Random.Range(0.0f, 100.0f);

            heightmap = new float[heightmapResolution, heightmapResolution];

            for(int i = 0; i < heightmapResolution; i++)
                for(int j = 0; j < heightmapResolution; j++)
                {
                    height = Mathf.Lerp(slopeTopHeight, slopeBottomHeight, (float)i / (heightmapResolution - 1));

                    noise = Mathf.PerlinNoise(i * noiseVariation * 0.01f + seed, j * noiseVariation * 0.01f + seed);
                    noise = Mathf.Clamp(noise, 0.0f, 1.0f);
                    noise = noise - 0.5f;
                    noise *= noiseMagnitude;
                    
                    SetHeightmapValue(i, j, height + noise);
                }
        }

        private void SetHeightmapValue(int x, int y, float value)
        {
            heightmap[x, y] = Mathf.Clamp(value, 0.0f, 1.0f);
        }
    }
}

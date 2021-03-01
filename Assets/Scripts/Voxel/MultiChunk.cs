using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace Voxel
{
	public class MultiChunk : MonoBehaviour
	{
		public GameObject ChunkRendererPrefab;

		private class ChunkData
		{
			public ChunkData(int x, int y, int z)
			{
				X = x;
				Y = y;
				Z = z;
			}

			public int X;
			public int Y;
			public int Z;
			public Chunk chunk;
			public ChunkRenderer renderer;
		}

		public static int ChunkCountX = 30;
		public static int ChunkCountY = 2;
		public static int ChunkCountZ = 30;

		private ChunkData[,,] _data = new ChunkData[ChunkCountX, ChunkCountY, ChunkCountZ];

		private Queue<ChunkData> _generationQueue = new Queue<ChunkData>();

		void Start()
		{
			for(int x = 0; x < ChunkCountX; x++)
			{
				for(int y = 0; y < ChunkCountY; y++)
				{
					for(int z = 0; z < ChunkCountZ; z++)
					{
						var data = new ChunkData(x, y, z);
						_data[x,y,z] = data;
						data.chunk = new Chunk();

						var newChunkObj = GameObject.Instantiate(ChunkRendererPrefab, transform.position, transform.rotation, transform);
						Vector3 newChunkPosition = new Vector3(x * Chunk.ChunkSizeX, y * Chunk.ChunkSizeY, z * Chunk.ChunkSizeZ);
						newChunkObj.transform.localPosition = newChunkPosition;

						data.renderer = newChunkObj.GetComponent<ChunkRenderer>();
						Debug.Assert(data.renderer != null);

						_generationQueue.Enqueue(data);
					}
				}
			}
		}

		void Update()
		{
			int generationsPerFrame = 10;
			while(generationsPerFrame-- > 0 && _generationQueue.Count > 0)
			{
				Profiler.BeginSample("Creating Single Chunk...");
				
				var data = _generationQueue.Peek();
				data.chunk.Generate(this, data.X,data.Y,data.Z);
				data.renderer.CreateMesh(this, data.chunk);
				_generationQueue.Dequeue();

				Profiler.EndSample();
			}
		}
	
		//BROKEN!
		public Material GetMaterialAtGlobal(int x, int y, int z)
		{
			//This modulo and divsion aren't working as intended
			int xVoxelIndex = x % Chunk.ChunkSizeX;
			int yVoxelIndex = y % Chunk.ChunkSizeY;
			int zVoxelIndex = z % Chunk.ChunkSizeZ;
			int xChunkIndex = x / Chunk.ChunkSizeX;
			int yChunkIndex = y / Chunk.ChunkSizeY;
			int zChunkIndex = z / Chunk.ChunkSizeZ;
			if(x < 0) xChunkIndex -= 1;
			if(y < 0) yChunkIndex -= 1;
			if(z < 0) zChunkIndex -= 1;

			var defaultMaterial = Material.Air;
			if(xChunkIndex < 0 || xChunkIndex >= ChunkCountX) return defaultMaterial;
			if(yChunkIndex < 0 || yChunkIndex >= ChunkCountY) return defaultMaterial;
			if(zChunkIndex < 0 || zChunkIndex >= ChunkCountZ) return defaultMaterial;

			Chunk chunk = _data[xChunkIndex, yChunkIndex, zChunkIndex].chunk;
			return chunk.GetMaterialAtLocal(xVoxelIndex, yVoxelIndex, zVoxelIndex);
		}
	}
}

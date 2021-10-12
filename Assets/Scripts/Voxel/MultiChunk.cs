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

		public static int ChunkCountX = 5;
		public static int ChunkCountY = 5;
		public static int ChunkCountZ = 5;

		private ChunkData[,,] _data = new ChunkData[ChunkCountX, ChunkCountY, ChunkCountZ];

		private Queue<ChunkData> _generationQueue = new Queue<ChunkData>();
		private Queue<ChunkData> _physicsQueue = new Queue<ChunkData>();

		void Start()
		{
			//if(transform.childCount == 0)
			//	Generate(false);
		}

		void Update()
		{
			GenerationTick();
		}

		private void Allocate()
		{
			_data = new ChunkData[ChunkCountX, ChunkCountY, ChunkCountZ];
			_generationQueue = new Queue<ChunkData>();
			_physicsQueue = new Queue<ChunkData>();

			for(int x = 0; x < ChunkCountX; x++)
			{
				for(int y = 0; y < ChunkCountY; y++)
				{
					for(int z = 0; z < ChunkCountZ; z++)
					{
						var data = new ChunkData(x, y, z);
						_data[x,y,z] = data;

						var newChunkObj = GameObject.Instantiate(ChunkRendererPrefab, transform.position, transform.rotation, transform);
						Vector3 newChunkPosition = new Vector3(x * Chunk.ChunkSizeX, y * Chunk.ChunkSizeY, z * Chunk.ChunkSizeZ);
						newChunkObj.transform.localPosition = newChunkPosition;

						data.chunk = newChunkObj.GetComponent<Chunk>();
						Debug.Assert(data.chunk != null);

						data.renderer = newChunkObj.GetComponent<ChunkRenderer>();
						Debug.Assert(data.renderer != null);
					}
				}
			}
		}

		public void HardReset()
		{
			for (int i = this.transform.childCount; i > 0; --i)
			{
				DestroyImmediate(this.transform.GetChild(0).gameObject);
			}

			_data = new ChunkData[ChunkCountX, ChunkCountY, ChunkCountZ];
			_generationQueue = new Queue<ChunkData>();
			_physicsQueue = new Queue<ChunkData>();
		}

		public void Generate(bool immediate)
		{
			HardReset();
			Allocate();

			for(int x = 0; x < ChunkCountX; x++)
			{
				for(int y = 0; y < ChunkCountY; y++)
				{
					for(int z = 0; z < ChunkCountZ; z++)
					{
						_generationQueue.Enqueue(_data[x,y,z]);
					}
				}
			}

			BoxCollider box = GetComponent<BoxCollider>();
			if(box != null)
			{
				Vector3 size = new Vector3(ChunkCountX * Chunk.ChunkSizeX, ChunkCountY * Chunk.ChunkSizeY, ChunkCountZ * Chunk.ChunkSizeZ);
				box.size = size;
				box.center = size / 2.0f;
			}

			if(immediate)
			{
				while(GenerationTick()) {}
			}
		}

		//returns true if there is work being done, false if the work is complete
		public bool GenerationTick()
		{
			int generationsPerFrame = 10;
			int physicsEnabledPerFrame = 100;
			if(_generationQueue.Count > 0)
			{
				while(generationsPerFrame-- > 0 && _generationQueue.Count > 0)
				{
					Profiler.BeginSample("Creating Single Chunk...");

					var data = _generationQueue.Peek();
					_physicsQueue.Enqueue(data);
					_generationQueue.Dequeue();

					data.chunk.Generate(this, data.X,data.Y,data.Z);
					data.renderer.CreateMesh();

					Profiler.EndSample();
				}
				return true;
			}
			else if(_physicsQueue.Count > 0)
			{
				while(physicsEnabledPerFrame-- > 0 && _physicsQueue.Count > 0)
				{
					var data = _physicsQueue.Peek();
					_physicsQueue.Dequeue();

					data.renderer.EnablePhysics();
				}
				return true;
			}
			return false;
		}
	
		//BROKEN!
		/*
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
		*/
	}
}

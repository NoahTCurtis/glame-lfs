using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace Voxel
{
	public class Chunk : MonoBehaviour
	{
		public static int ChunkSizeX = 16;
		public static int ChunkSizeY = 16;
		public static int ChunkSizeZ = 16;

		[SerializeField] private int _xOffset = -1;
		[SerializeField] private int _yOffset = -1;
		[SerializeField] private int _zOffset = -1;

		private MultiChunk _multiChunk;

		[SerializeField] public Material[] _materials = new Material[ChunkSizeX * ChunkSizeY * ChunkSizeZ];

		private Vector3[] LineCenters = { new Vector3(128,128,128), new Vector3(128,200,80) };
		private Vector3[] LineVectors = { new Vector3(1.6f,1,1).normalized, new Vector3(0,7,1).normalized };
		private float[] LineThicks = { 17f, 9f };

		private void SetMaterial(int x, int y, int z, Material material)
		{
			_materials[x * ChunkSizeY * ChunkSizeZ + y * ChunkSizeZ + z] = material;
		}

		private Material GetMaterial(int x, int y, int z)
		{
			return _materials[x * ChunkSizeY * ChunkSizeZ + y * ChunkSizeZ + z];
		}

		public void Generate(MultiChunk multiChunk, int xOffset, int yOffset, int zOffset)
		{
			Profiler.BeginSample("Chunk.Generate");

			_multiChunk = multiChunk;
			_xOffset = ChunkSizeX * xOffset;
			_yOffset = ChunkSizeY * yOffset;
			_zOffset = ChunkSizeZ * zOffset;

			for(int localX = 0; localX < ChunkSizeX; localX++)
			{
				for(int localY = 0; localY < ChunkSizeY; localY++)
				{
					for(int localZ = 0; localZ < ChunkSizeZ; localZ++)
					{
						//actual block position
						float x = _xOffset + localX;
						float y = _yOffset + localY;
						float z = _zOffset + localZ;

						//rules for generation
						/*
						float xScale = Mathf.PI * 1f;
						float xNoise = Mathf.PerlinNoise(y/xScale, z/xScale);
						bool xLine = xNoise > 0.9f;

						float yScale = Mathf.PI * 5f;
						float yNoise = Mathf.PerlinNoise(x/yScale, z/yScale);
						bool yLine = yNoise > 0.8f;

						float zScale = Mathf.PI * 0.5f;
						float zNoise = Mathf.PerlinNoise(x/zScale, y/zScale);
						bool zLine = zNoise > 0.9f;

						Vector3 point = new Vector3(x, y + Mathf.Sin((x+z)/30f)*10f, z);
						bool isTubeExterior = false;
						bool isTubeInterior = false;
						for(int i = 0; i < LineCenters.Length; i++)
						{
							float distSq = Vector3.Cross(LineCenters[i]-point, LineVectors[i]).sqrMagnitude;
							float outer = LineThicks[i] * LineThicks[i];
							float inner = (LineThicks[i] * 0.8f) * (LineThicks[i] * 0.8f);
							isTubeExterior |= distSq < outer;
							isTubeInterior |= distSq < inner;
						}

						bool solid = (xLine || yLine || zLine || isTubeExterior) && !isTubeInterior;

						//Set material
						if(solid)
						{
							SetMaterial(localX, localY, localZ, Material.Concrete);
						}
						else
						{
							SetMaterial(localX, localY, localZ, Material.Air);
						}
						*/
						SetMaterial(localX, localY, localZ, Material.Concrete);
					}
				}
			}

			Profiler.EndSample();
		}

		public void BeamCut(BeamData beam)
		{
			Debug.Assert(_xOffset != -1);
			
			float radius = beam.radius + 0.2165f;
			float radiusSq = radius * radius;

			for(int localX = 0; localX < ChunkSizeX; localX++)
			{
				for(int localY = 0; localY < ChunkSizeY; localY++)
				{
					for(int localZ = 0; localZ < ChunkSizeZ; localZ++)
					{
						//actual block position
						float x = _xOffset + localX;
						float y = _yOffset + localY;
						float z = _zOffset + localZ;
						Vector3 point = new Vector3(x, y, z);
						Vector3 vecToBeamOrigin = beam.ray.origin - point;

						float dot = Vector3.Dot(beam.ray.direction, vecToBeamOrigin);
						if(dot < 0)
						{
							float distSq = Vector3.Cross(vecToBeamOrigin, beam.ray.direction).sqrMagnitude;
							if(distSq < radiusSq)
							{
								SetMaterial(localX, localY, localZ, Material.Air);
							}
						}
					}
				}
			}
		}

		public Material GetMaterialAtLocal(int x, int y, int z)
		{
			if(IsValidIndex(x,y,z))
			{
				return GetMaterial(x,y,z);
			}
			else
			{
				return Material.Air;
			}
		}

		private bool IsValidIndex(int x, int y, int z)
		{
			return !(x < 0 || y < 0 || z < 0 || x >= ChunkSizeX || y >= ChunkSizeY || z >= ChunkSizeZ);
		}

		/*public Material GetMaterialAtLocalOrNeighbor(int x, int y, int z)
		{
			//x = x % Chunk.ChunkSizeX;
			//y = y % Chunk.ChunkSizeY;
			//z = z % Chunk.ChunkSizeZ;
			Debug.Assert(IsValidIndex(x,y,z), $"Invalid Index! ({x},{y},{z})");
			return _materials[x,y,z];
		}

		public Material GetMaterialAtGlobal(int x, int y, int z)
		{
			x += _xOffset;
			y += _yOffset;
			z += _zOffset;
			return _multiChunk.GetMaterialAtGlobal(x, y, z);
		}
		*/
	}
}

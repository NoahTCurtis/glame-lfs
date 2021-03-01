using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace Voxel
{
	public class Chunk
	{
		public static int ChunkSizeX = 16;
		public static int ChunkSizeY = 16;
		public static int ChunkSizeZ = 16;

		private int _xOffset;
		private int _yOffset;
		private int _zOffset;

		private MultiChunk _multiChunk;

		private Material[,,] _materials = new Material[ChunkSizeX, ChunkSizeY, ChunkSizeZ];

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
						float x = _xOffset + localX;
						float y = _yOffset + localY;
						float z = _zOffset + localZ;

						float minHeight = 16f;
						float maxHeight = 48f;
						float xzScale = Mathf.PI * 25f;
						float xzNoise = Mathf.PerlinNoise(x/xzScale, z/xzScale);
						float terrainHeight = Mathf.Lerp(minHeight, maxHeight, xzNoise);

						bool solid = y < terrainHeight;

						if(solid)
						{
							_materials[localX, localY, localZ] = Material.Concrete;
						}
						else
						{
							_materials[localX, localY, localZ] = Material.Air;
						}
					}
				}
			}

			Profiler.EndSample();
		}

		public Material GetMaterialAtLocal(int x, int y, int z)
		{
			if(IsValidIndex(x,y,z))
			{
				return _materials[x,y,z];
			}
			else
			{
				return Material.Air;
			}
		}

		public Material GetMaterialAtLocalOrNeighbor(int x, int y, int z)
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

		private bool IsValidIndex(int x, int y, int z)
		{
			return !(x < 0 || y < 0 || z < 0 || x >= ChunkSizeX || y >= ChunkSizeY || z >= ChunkSizeZ);
		}
	}
}

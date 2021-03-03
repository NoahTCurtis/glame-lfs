using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace Voxel
{
	public class ChunkRenderer : MonoBehaviour
	{
		public const int ChunkSizeX = 16;
		public const int ChunkSizeY = 16;
		public const int ChunkSizeZ = 16;
		private Material[,,] _materials = new Material[ChunkSizeX, ChunkSizeY, ChunkSizeZ];

		private MultiChunk _mc;
		private Chunk _chunk;
		private Mesh _mesh;

		Vector3[] vertices;
		Vector3[] normals;
		Vector2[] uvs;
		int[] tris;

		int vertIndex = 0;
		int triIndex = 0;

		//Normals
		Vector3 Xpos = new Vector3(+1,0,0);
		Vector3 Xneg = new Vector3(-1,0,0);
		Vector3 Ypos = new Vector3(0,+1,0);
		Vector3 Yneg = new Vector3(0,-1,0);
		Vector3 Zpos = new Vector3(0,0,+1);
		Vector3 Zneg = new Vector3(0,0,-1);

		//UVs
		Vector2 uvA = new Vector2(0,0);
		Vector2 uvB = new Vector2(0,1);
		Vector2 uvC = new Vector2(1,1);
		Vector2 uvD = new Vector2(1,0);

		void Awake()
		{
			_chunk = GetComponent<Chunk>();
		}

		public void CreateMesh()
		{
			Profiler.BeginSample("ChunkRenderer.CreateMesh");

			vertIndex = 0;
			triIndex = 0;

			AllocateBigBuffers();

			//x y & z are position the minimum corner of the voxel being made
			Profiler.BeginSample("CreateMesh: Triple-for-loop");
			for(int x = 0; x < Chunk.ChunkSizeX; x++)
			{
				for(int y = 0; y < Chunk.ChunkSizeY; y++)
				{
					for(int z = 0; z < Chunk.ChunkSizeZ; z++)
					{
						var material = _chunk.GetMaterialAtLocal(x,y,z);
						if(material == Material.Concrete)
						{
							AddCube(x, y, z);
						}
					}
				}
			}
			Profiler.EndSample();

			SubmitMesh();

			Profiler.EndSample();
		}

		public void AllocateBigBuffers()
		{
			Profiler.BeginSample("CreateMesh: Allocating big buffers");
			int voxelCount = Chunk.ChunkSizeX * Chunk.ChunkSizeY * Chunk.ChunkSizeZ;
			vertices = new Vector3[voxelCount * 8 * 4]; //8 sides, 4 verts each
			normals = new Vector3[voxelCount * 8 * 4]; //8 sides, 4 verts each
			uvs = new Vector2[voxelCount * 8 * 4]; //8 sides, 4 verts each
			tris = new int[voxelCount * 8 * 2 * 3]; //8 sides, 2 tris per side, 3 verts per tri
			Profiler.EndSample();
		}

		void AddCube(int x, int y, int z)
		{
			int a,b,c,d;

			//x- face
			if(Solid(x-1, y, z) == false)
			{
				a = AddVert(new Vector3(x,y,z+1), Xneg, uvA);
				b = AddVert(new Vector3(x,y+1,z+1), Xneg, uvB);
				c = AddVert(new Vector3(x,y+1,z), Xneg, uvC);
				d = AddVert(new Vector3(x,y,z), Xneg, uvD);
				AddTri(a, b, c);
				AddTri(a, c, d);
			}

			//x+ face
			if(Solid(x+1, y, z) == false)
			{
				a = AddVert(new Vector3(x+1,y,z), Xpos, uvA);
				b = AddVert(new Vector3(x+1,y+1,z), Xpos, uvB);
				c = AddVert(new Vector3(x+1,y+1,z+1), Xpos, uvC);
				d = AddVert(new Vector3(x+1,y,z+1), Xpos, uvD);
				AddTri(a, b, c);
				AddTri(a, c, d);
			}

			//y- face
			if(Solid(x, y-1, z) == false)
			{
				a = AddVert(new Vector3(x,y,z), Yneg, uvA);
				b = AddVert(new Vector3(x+1,y,z), Yneg, uvB);
				c = AddVert(new Vector3(x+1,y,z+1), Yneg, uvC);
				d = AddVert(new Vector3(x,y,z+1), Yneg, uvD);
				AddTri(a, b, c);
				AddTri(a, c, d);
			}

			//y+ face
			if(Solid(x, y+1, z) == false)
			{
				a = AddVert(new Vector3(x,y+1,z), Ypos, uvA);
				b = AddVert(new Vector3(x,y+1,z+1), Ypos, uvB);
				c = AddVert(new Vector3(x+1,y+1,z+1), Ypos, uvC);
				d = AddVert(new Vector3(x+1,y+1,z), Ypos, uvD);
				AddTri(a, b, c);
				AddTri(a, c, d);
			}

			//z- face
			if(Solid(x, y, z-1) == false)
			{
				a = AddVert(new Vector3(x,y,z), Zneg, uvA);
				b = AddVert(new Vector3(x,y+1,z), Zneg, uvB);
				c = AddVert(new Vector3(x+1,y+1,z), Zneg, uvC);
				d = AddVert(new Vector3(x+1,y,z), Zneg, uvD);
				AddTri(a, b, c);
				AddTri(a, c, d);
			}

			//z+ face
			if(Solid(x, y, z+1) == false)
			{
				a = AddVert(new Vector3(x+1,y,z+1), Zpos, uvA);
				b = AddVert(new Vector3(x+1,y+1,z+1), Zpos, uvB);
				c = AddVert(new Vector3(x,y+1,z+1), Zpos, uvC);
				d = AddVert(new Vector3(x,y,z+1), Zpos, uvD);
				AddTri(a, b, c);
				AddTri(a, c, d);
			}
		}

		bool Solid(int x, int y, int z)
		{
			return MatUtil.IsSolid( _chunk.GetMaterialAtLocal(x, y, z) );
		}

		int AddVert(Vector3 vertex, Vector3 normal, Vector2 uv)
		{
			vertices[vertIndex] = vertex;
			normals[vertIndex] = normal;
			uvs[vertIndex] = uv;

			return vertIndex++;
		}

		void AddTri(int a, int b, int c)
		{
			tris[triIndex++] = a;
			tris[triIndex++] = b;
			tris[triIndex++] = c;
		}

		void SubmitMesh()
		{
			Profiler.BeginSample("CreateMesh.SubmitMesh()");
			_mesh = new Mesh();
			_mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

			Array.Resize(ref vertices, vertIndex);
			Array.Resize(ref normals, vertIndex);
			Array.Resize(ref uvs, vertIndex);
			Array.Resize(ref tris, triIndex);

			_mesh.vertices = vertices;
			_mesh.normals = normals;
			_mesh.uv = uvs;
			_mesh.triangles = tris;

			MeshFilter meshFilter = GetComponent<MeshFilter>();
			meshFilter.mesh = _mesh;
			Profiler.EndSample();
		}

		public void EnablePhysics()
		{
			if(_mesh != null)
			{
				MeshCollider meshCollider = GetComponent<MeshCollider>();
				if(meshCollider != null)
				{
					meshCollider.sharedMesh = _mesh;
				}
			}
		}
	}
}

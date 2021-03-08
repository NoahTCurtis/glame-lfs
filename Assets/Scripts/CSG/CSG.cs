using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSG
{
	public void Subtract()
	{

	}

	private (Mesh, Mesh) Refine(Mesh target, Mesh brush)
	{
		//In order to perform any Boolean Operations, we need to bisect the polygons of each object that intersect polygons of the other object.
		//To clarify, this means that if a polygon from object A intersects a polygon from object B, then both polygons need to be bisected by one another:
		//object A's polygon is bisected by the plane of object B's polygon, and object B's polygon is bisected by the plane of object A's polygon.
		//The trick is, you only need to do this when there is an actual intersection of the two polygons.
		//There are a few ways of going about doing this, but I'll describe a simple technique.

		//Start by making two copies of each object;
		//we'll call these objects A' and B'.
		//We'll need to do this because we'll be replacing polygons with the multiple pieces from any bisection that takes place.
		Mesh newTarget = CopyMesh(target); //A'
		Mesh newBrush = CopyMesh(brush); //B'

		//Visit each polygon in A' and compare its bounding box with the bounding box from each polygon in B'.
		//If no overlap exists, then there's no intersection (and no bisection is required.)
		//If there is an overlap, go ahead and attempt to bisect each polygon by the other polygon's plane.
		//If both polygons end up being bisected, then replace the original polygons from both objects with their bisected counterparts.
		//However, if one polygon or the other ends up not being bisected, then leave both polygons unmodified.
		Debug.Assert(newTarget.subMeshCount == 1);
		Debug.Assert(newBrush.subMeshCount == 1);
		var targetTris = newTarget.GetTriangles(0);
		var brushTris = newBrush.GetTriangles(0);

		List<(int, int)> collisions = new List<(int, int)>();
		for(int targetTri = 0; targetTri < targetTris.Length; targetTri += 3)
		{
			Vector3 A = newTarget.vertices[targetTri + 0];
			Vector3 B = newTarget.vertices[targetTri + 1];
			Vector3 C = newTarget.vertices[targetTri + 2];
			for(int brushTri = 0; brushTri < brushTris.Length; brushTri += 3)
			{
				Vector3 D = newBrush.vertices[brushTri + 0];
				Vector3 E = newBrush.vertices[brushTri + 1];
				Vector3 F = newBrush.vertices[brushTri + 2];

				if(TriTriCollision(A,B,C,D,E,F))
				{
					collisions.Add( (targetTri/3, brushTri/3) );
				}
			}
		}

		//A' and B' are now 'refined' for the Boolean Operations.
		//The only thing left to do is to figure out which polygons from each object is part of the result.


		return (null, null);
	}

	private bool TriTriCollision(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector3 e, Vector3 f)
	{
		//Checks for collision between triangles ABC & DEF
		return false;
	}

	private Mesh CopyMesh(Mesh mesh)
	{
		Mesh newMesh = new Mesh();
		newMesh.vertices = mesh.vertices;
		newMesh.triangles = mesh.triangles;
		newMesh.uv = mesh.uv;
		newMesh.normals = mesh.normals;
		newMesh.colors = mesh.colors;
		newMesh.tangents = mesh.tangents;
		return newMesh;
	}
}

using System.Collections.Generic;
using PathCreation.Utility;
using UnityEngine;

namespace PathCreation.Examples {
    public class RoadMeshCreator : PathSceneTool {
        [Header ("Road settings")]
        public float roadWidth = .4f;
        [Range (0, .5f)]
        public float thickness = .15f;
        public bool flattenSurface;

        [Header ("Side Walls settings")]
        public bool sideWalls;
        public float sideWallsHeight = 2.0f;
        [Range (0, .5f)]
        public float sideWallsWidth = .1f;

        [Header ("Test")]
        public int testVal = 8;

        [Header ("Material settings")]
        public Material roadMaterial;
        public Material undersideMaterial;
        public float textureTiling = 1;

        [Header ("Side Walls Material settings")]
        public Material sideWallMaterial;
        public Material sideWallUndersideMaterial;
        public float sideWallTextureTiling = 1;

        [SerializeField, HideInInspector]
        GameObject meshHolder;
        GameObject meshHolderLeftWall;
        GameObject meshHolderRightWall;

        MeshFilter meshFilter;
        MeshFilter meshFilterLeftWall;
        MeshFilter meshFilterRightWall;

        MeshRenderer meshRenderer;
        MeshRenderer meshRendererLeftWall;
        MeshRenderer meshRendererRightWall;

        Mesh mesh;
        Mesh meshLeftWall;
        Mesh meshRightWall;

        protected override void PathUpdated () {
            if (pathCreator != null) {
                AssignMeshComponents ("Road", "Road Mesh Holder");
                AssignMaterials ("Road", roadMaterial, undersideMaterial, textureTiling);
                CreateRoadMesh ("Road", roadWidth, thickness, flattenSurface);
                if (sideWalls) {
                    // Bu durumlar için duvarların yol genişliği kadar sağa veya sola kaydırılması yapılacak.
                    AssignMeshComponents ("LeftWall", "Left Wall Mesh Holder");
                    AssignMaterials ("LeftWall", sideWallMaterial, sideWallUndersideMaterial, sideWallTextureTiling);
                    CreateRoadMesh ("LeftWall", sideWallsWidth, sideWallsHeight, false);

                    AssignMeshComponents ("RightWall", "Right Wall Mesh Holder");
                    AssignMaterials ("RightWall", sideWallMaterial, sideWallUndersideMaterial, sideWallTextureTiling);
                    CreateRoadMesh ("RightWall", sideWallsWidth, sideWallsHeight, false);
                }
            }
        }

        void CreateRoadMesh (string focusType, float roadWidth, float thickness, bool flattenSurface) {
            Vector3[] verts = new Vector3[path.NumPoints * 8];
            Vector2[] uvs = new Vector2[verts.Length];
            Vector3[] normals = new Vector3[verts.Length];

            int numTris = 2 * (path.NumPoints - 1) + ((path.isClosedLoop) ? 2 : 0);
            int[] roadTriangles = new int[numTris * 3];
            int[] underRoadTriangles = new int[numTris * 3];
            int[] sideOfRoadTriangles = new int[numTris * 2 * 3];

            int vertIndex = 0;
            int triIndex = 0;

            // Vertices for the top of the road are layed out:
            // 0  1
            // 8  9
            // and so on... So the triangle map 0,8,1 for example, defines a triangle from top left to bottom left to bottom right.
            int[] triangleMap = { 0, 8, 1, 1, 8, 9 };
            int[] sidesTriangleMap = { 4, 6, 14, 12, 4, 14, 5, 15, 7, 13, 15, 5 };

            bool usePathNormals = !(path.space == PathSpace.xyz && flattenSurface);

            for (int i = 0; i < path.NumPoints; i++) {
                Vector3 localUp = (usePathNormals) ? Vector3.Cross (path.GetTangent (i), path.GetNormal (i)) : path.up;
                Vector3 localRight = (usePathNormals) ? path.GetNormal (i) : Vector3.Cross (localUp, path.GetTangent (i));

                // Find position to left and right of current path vertex
                Vector3 vertSideA = path.GetPoint (i) - localRight * Mathf.Abs (roadWidth);
                Vector3 vertSideB = path.GetPoint (i) + localRight * Mathf.Abs (roadWidth);

                // Add top of road vertices
                verts[vertIndex + 0] = vertSideA;
                verts[vertIndex + 1] = vertSideB;
                // Add bottom of road vertices
                verts[vertIndex + 2] = vertSideA - localUp * thickness;
                verts[vertIndex + 3] = vertSideB - localUp * thickness;

                // Duplicate vertices to get flat shading for sides of road
                verts[vertIndex + 4] = verts[vertIndex + 0];
                verts[vertIndex + 5] = verts[vertIndex + 1];
                verts[vertIndex + 6] = verts[vertIndex + 2];
                verts[vertIndex + 7] = verts[vertIndex + 3];

                // Set uv on y axis to path time (0 at start of path, up to 1 at end of path)
                uvs[vertIndex + 0] = new Vector2 (0, path.times[i]);
                uvs[vertIndex + 1] = new Vector2 (1, path.times[i]);

                // Top of road normals
                normals[vertIndex + 0] = localUp;
                normals[vertIndex + 1] = localUp;
                // Bottom of road normals
                normals[vertIndex + 2] = -localUp;
                normals[vertIndex + 3] = -localUp;
                // Sides of road normals
                normals[vertIndex + 4] = -localRight;
                normals[vertIndex + 5] = localRight;
                normals[vertIndex + 6] = -localRight;
                normals[vertIndex + 7] = localRight;

                // Set triangle indices
                if (i < path.NumPoints - 1 || path.isClosedLoop) {
                    for (int j = 0; j < triangleMap.Length; j++) {
                        roadTriangles[triIndex + j] = (vertIndex + triangleMap[j]) % verts.Length;
                        // reverse triangle map for under road so that triangles wind the other way and are visible from underneath
                        underRoadTriangles[triIndex + j] = (vertIndex + triangleMap[triangleMap.Length - 1 - j] + 2) % verts.Length;
                    }
                    for (int j = 0; j < sidesTriangleMap.Length; j++) {
                        sideOfRoadTriangles[triIndex * 2 + j] = (vertIndex + sidesTriangleMap[j]) % verts.Length;
                    }

                }

                vertIndex += 8;
                triIndex += 6;
            }

            
            if (focusType == "Road") {
                mesh.Clear ();
                mesh.vertices = verts;
                mesh.uv = uvs;
                mesh.normals = normals;
                mesh.subMeshCount = 3;
                mesh.SetTriangles (roadTriangles, 0);
                mesh.SetTriangles (underRoadTriangles, 1);
                mesh.SetTriangles (sideOfRoadTriangles, 2);
                mesh.RecalculateBounds ();
            }else if (focusType == "LeftWall") {
                meshLeftWall.Clear ();
                meshLeftWall.vertices = verts;
                meshLeftWall.uv = uvs;
                meshLeftWall.normals = normals;
                meshLeftWall.subMeshCount = 3;
                meshLeftWall.SetTriangles (roadTriangles, 0);
                meshLeftWall.SetTriangles (underRoadTriangles, 1);
                meshLeftWall.SetTriangles (sideOfRoadTriangles, 2);
                meshLeftWall.RecalculateBounds ();
            }else if (focusType == "RightWall") {
                meshRightWall.Clear ();
                meshRightWall.vertices = verts;
                meshRightWall.uv = uvs;
                meshRightWall.normals = normals;
                meshRightWall.subMeshCount = 3;
                meshRightWall.SetTriangles (roadTriangles, 0);
                meshRightWall.SetTriangles (underRoadTriangles, 1);
                meshRightWall.SetTriangles (sideOfRoadTriangles, 2);
                meshRightWall.RecalculateBounds ();
            }
        }

        // Add MeshRenderer and MeshFilter components to this gameobject if not already attached
        void AssignMeshComponents (string focusType, string meshHolderName) {
            if (focusType == "Road") {
                if (meshHolder == null) {
                    meshHolder = new GameObject (meshHolderName);
                }

                meshHolder.transform.rotation = Quaternion.identity;
                meshHolder.transform.position = Vector3.zero;
                meshHolder.transform.localScale = Vector3.one;

                // Ensure mesh renderer and filter components are assigned
                if (!meshHolder.gameObject.GetComponent<MeshFilter> ()) {
                    meshHolder.gameObject.AddComponent<MeshFilter> ();
                }
                if (!meshHolder.GetComponent<MeshRenderer> ()) {
                    meshHolder.gameObject.AddComponent<MeshRenderer> ();
                }

                meshRenderer = meshHolder.GetComponent<MeshRenderer> ();
                meshFilter = meshHolder.GetComponent<MeshFilter> ();
                if (mesh == null) {
                    mesh = new Mesh ();
                }
                meshFilter.sharedMesh = mesh;
            }else if (focusType == "LeftWall") {
                if (meshHolderLeftWall == null) {
                    meshHolderLeftWall = new GameObject (meshHolderName);
                }

                meshHolderLeftWall.transform.rotation = Quaternion.identity;
                meshHolderLeftWall.transform.position = Vector3.zero;
                meshHolderLeftWall.transform.localScale = Vector3.one;

                // Ensure mesh renderer and filter components are assigned
                if (!meshHolderLeftWall.gameObject.GetComponent<MeshFilter> ()) {
                    meshHolderLeftWall.gameObject.AddComponent<MeshFilter> ();
                }
                if (!meshHolderLeftWall.GetComponent<MeshRenderer> ()) {
                    meshHolderLeftWall.gameObject.AddComponent<MeshRenderer> ();
                }

                meshRendererLeftWall = meshHolderLeftWall.GetComponent<MeshRenderer> ();
                meshFilterLeftWall = meshHolderLeftWall.GetComponent<MeshFilter> ();
                if (meshLeftWall == null) {
                    meshLeftWall = new Mesh ();
                }
                meshFilterLeftWall.sharedMesh = meshLeftWall;
            }else if (focusType == "RightWall") {
                if (meshHolderRightWall == null) {
                    meshHolderRightWall = new GameObject (meshHolderName);
                }

                meshHolderRightWall.transform.rotation = Quaternion.identity;
                meshHolderRightWall.transform.position = Vector3.zero;
                meshHolderRightWall.transform.localScale = Vector3.one;

                // Ensure mesh renderer and filter components are assigned
                if (!meshHolderRightWall.gameObject.GetComponent<MeshFilter> ()) {
                    meshHolderRightWall.gameObject.AddComponent<MeshFilter> ();
                }
                if (!meshHolderRightWall.GetComponent<MeshRenderer> ()) {
                    meshHolderRightWall.gameObject.AddComponent<MeshRenderer> ();
                }

                meshRendererRightWall = meshHolderRightWall.GetComponent<MeshRenderer> ();
                meshFilterRightWall = meshHolderRightWall.GetComponent<MeshFilter> ();
                if (meshRightWall == null) {
                    meshRightWall = new Mesh ();
                }
                meshFilterRightWall.sharedMesh = meshRightWall;
            }
            
        }

        void AssignMaterials (string focusType, Material roadMaterial, Material undersideMaterial, float textureTiling) {
            if (focusType == "Road") {
                if (roadMaterial != null && undersideMaterial != null) {
                    meshRenderer.sharedMaterials = new Material[] { roadMaterial, undersideMaterial, undersideMaterial };
                    meshRenderer.sharedMaterials[0].mainTextureScale = new Vector3 (1, textureTiling);
                }
            }else if (focusType == "LeftWall") {
                if (roadMaterial != null && undersideMaterial != null) {
                    meshRendererLeftWall.sharedMaterials = new Material[] { roadMaterial, undersideMaterial, undersideMaterial };
                    meshRendererLeftWall.sharedMaterials[0].mainTextureScale = new Vector3 (1, textureTiling);
                }
            }else if (focusType == "RightWall") {
                if (roadMaterial != null && undersideMaterial != null) {
                    meshRendererRightWall.sharedMaterials = new Material[] { roadMaterial, undersideMaterial, undersideMaterial };
                    meshRendererRightWall.sharedMaterials[0].mainTextureScale = new Vector3 (1, textureTiling);
                }
            }
            
        }

    }
}
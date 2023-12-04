
using System.Collections.Generic;
using UnityEngine;

class DynamicMesh : MonoBehaviour{

    public Mesh mesh;

    public static DynamicMesh instance;

    void Awake() {
        instance = this;
    }

    public void updateColors() {
        if (!mesh) {
            return;
        }

        List<Particle> particles = SpawnParticles.instance.edgeParticles;
        Color[] colors = new Color[particles.Count];

        for (int i = 0; i < particles.Count; i++)
        {
            colors[i] = particles[i].color; // Assuming Particle has a 'color' field
        }

        mesh.colors = colors; // Set the colors array
    }

    public void GenerateMesh()
    {
        // Create a new mesh
        mesh = new Mesh();

        List<Particle> particles = SpawnParticles.instance.edgeParticles;

        // Set the vertex positions
        Vector3[] vertices = new Vector3[particles.Count];
        Color[] colors = new Color[particles.Count];

        for (int i = 0; i < particles.Count; i++)
        {
            vertices[i] = new Vector3(particles[i].position.x, particles[i].position.y, particles[i].position.z);
            colors[i] = particles[i].color; // Assuming Particle has a 'color' field
        }
        Debug.Log("vertices: " + vertices.Length);

        mesh.vertices = vertices;
        mesh.colors = colors; // Set the colors array

        // Automatically generate triangles based on the order of vertices
        var tr = GenerateTriangles(particles);

        int j = 0;
        while (j < tr.Length) {
            if (tr[j++] == vertices.Length) {
                Debug.LogError("TOO HIGH INDEX at j=" + j);
            }
            // Debug.Log($"triangle: {tr[j++]}, {tr[j++]}, {tr[j++]}");
        }

        Debug.Log("triangles indices: " + tr.Length);
        mesh.triangles = tr; 

        // Create a new GameObject to hold the mesh
        GameObject newObj = new GameObject("CustomMesh");

        // Add MeshFilter and MeshRenderer components to the GameObject
        MeshFilter meshFilter = newObj.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = newObj.AddComponent<MeshRenderer>();

        // Assign the created mesh to the MeshFilter
        meshFilter.mesh = mesh;

        // Create a new material with your custom shader
        Material material = new Material(Shader.Find("Custom/VertexColorShader")); // Use the name of your custom shader
        // Set other material properties as needed
        material.color = Color.blue; // Set the color as needed

        // Assign the material to the MeshRenderer
        meshRenderer.material = material;
    }


    int[] GenerateTriangles(List<Particle> particles)
    {
        // Generate triangles based on the order of vertices
        List<int> triangles = new();
        
        foreach (Particle particle in particles) {

            if (particle.getEdgesNeighborsCount() == 3) {
                triangles.Add(particle.index);
                triangles.Add(particle.getEdgesNeighbors()[0].index);
                triangles.Add(particle.getEdgesNeighbors()[1].index);

                triangles.Add(particle.index);
                triangles.Add(particle.getEdgesNeighbors()[0].index);
                triangles.Add(particle.getEdgesNeighbors()[2].index);

                triangles.Add(particle.index);
                triangles.Add(particle.getEdgesNeighbors()[1].index);
                triangles.Add(particle.getEdgesNeighbors()[2].index);
            }

            if (particle.getEdgesNeighborsCount() == 4) {
                Vector3 oneAxisPosition = particle.getEdgesNeighbors()[0].position;
                int aAxisIndex1 = 0;
                int aAxisIndex2 = 4;

                for (int i = 1; i < 4; i++) {
                    if (particle.getEdgesNeighbors()[i].position.x == oneAxisPosition.x ||
                     particle.getEdgesNeighbors()[i].position.y == oneAxisPosition.y) {
                        aAxisIndex2 = i;
                        break;
                    }
                }

                if (aAxisIndex2 == 4) {
                    Debug.LogError("aAxisIndex2 has index 4");
                }
                
                int bAxisIndex1 = 4;
                int bAxisIndex2 = 4;
                for (int i = 1; i < 4; i++) {
                    if (i != aAxisIndex2) {
                        bAxisIndex1 = i;
                    }
                }

                if (bAxisIndex1 == 4) {
                    Debug.LogError("bAxisIndex1 has index 4");
                }

                for (int i = bAxisIndex1; i < 4; i++) {
                    if (i != aAxisIndex2) {
                        bAxisIndex2 = i;
                    }
                }

                if (bAxisIndex2 == 4) {
                    Debug.LogError("bAxisIndex2 has index 4");
                }

                triangles.Add(particle.index);
                triangles.Add(particle.getEdgesNeighbors()[aAxisIndex1].index);
                triangles.Add(particle.getEdgesNeighbors()[bAxisIndex1].index);

                triangles.Add(particle.index);
                triangles.Add(particle.getEdgesNeighbors()[aAxisIndex1].index);
                triangles.Add(particle.getEdgesNeighbors()[bAxisIndex2].index);

                triangles.Add(particle.index);
                triangles.Add(particle.getEdgesNeighbors()[aAxisIndex2].index);
                triangles.Add(particle.getEdgesNeighbors()[bAxisIndex1].index);

                triangles.Add(particle.index);
                triangles.Add(particle.getEdgesNeighbors()[aAxisIndex2].index);
                triangles.Add(particle.getEdgesNeighbors()[bAxisIndex2].index);
            }
        }


        return triangles.ToArray();
    }
}
using UnityEngine;
using TMPro;

[ExecuteAlways]
public class WordArtTextEffect : MonoBehaviour
{
    public float warpStrength = 0.5f; 
    
    public float curveWidth = 300f;

    private TextMeshProUGUI tmp;
    
    private Mesh mesh;
    
    private Vector3[] vertices;

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (tmp == null) return;

        tmp.ForceMeshUpdate();

        mesh = tmp.mesh;

        vertices = mesh.vertices;

        int charCount = tmp.textInfo.characterCount;

        for (int i = 0; i < charCount; i++)
        {
            var charInfo = tmp.textInfo.characterInfo[i];

            if (!charInfo.isVisible) continue;

            int vertexIndex = charInfo.vertexIndex;

            Vector3 offsetToMid = (vertices[vertexIndex] + vertices[vertexIndex + 2]) / 2;

            for (int j = 0; j < 4; j++)
            {
                Vector3 orig = vertices[vertexIndex + j];

                float relativeX = (orig.x - offsetToMid.x) / curveWidth;
                
                float scale = 1f - warpStrength * Mathf.Pow(relativeX, 2);

                Vector3 modified = new Vector3(
                    offsetToMid.x + (orig.x - offsetToMid.x) * scale,
                    orig.y / scale,
                    orig.z
                );

                vertices[vertexIndex + j] = modified;
            }
        }

        mesh.vertices = vertices;

        tmp.canvasRenderer.SetMesh(mesh);
    }
}
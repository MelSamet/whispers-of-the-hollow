using UnityEngine;
using TMPro;
using System.Collections;

public class PenWritingAnimation : MonoBehaviour
{
    public TextMeshProUGUI textComponent; // Animasyon uygulanacak TextMeshProUGUI
    public float baseTypeSpeed = 0.05f; // Temel harf başına gecikme (saniye)
    public float waveHeight = 1f; // Harflerin dalgalanma yüksekliği
    public float waveSpeed = 2f; // Dalgalanma hızı

    private string targetText; // Animasyon uygulanacak metin

    // Metni animasyonla yazdırmak için bu metodu çağır
    public void StartWriting(string text)
    {
        targetText = text;
        StopAllCoroutines(); // Önceki animasyonu durdur
        StartCoroutine(PenWritingEffect());
    }

    // Kalemle yazma efekti için coroutine
    private IEnumerator PenWritingEffect()
    {
        if (textComponent == null || string.IsNullOrEmpty(targetText)) yield break;

        textComponent.text = "";
        TMP_TextInfo textInfo = textComponent.textInfo;

        foreach (char c in targetText)
        {
            textComponent.text += c;
            textComponent.ForceMeshUpdate();

            // Harflerin hafif dalgalanması (kalemle yazma hissi)
            for (int i = 0; i < textInfo.characterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (charInfo.isVisible)
                {
                    int vertexIndex = charInfo.vertexIndex;
                    Vector3[] vertices = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

                    for (int j = 0; j < 4; j++)
                    {
                        Vector3 orig = vertices[vertexIndex + j];
                        float offset = Mathf.Sin(Time.time * waveSpeed + orig.x * 0.1f) * waveHeight;
                        vertices[vertexIndex + j] = orig + new Vector3(0, offset, 0);
                    }
                }
            }

            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                textComponent.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
            }

            // Rastgele hız değişimi (kalemle yazma doğallığı)
            float randomDelay = UnityEngine.Random.Range(baseTypeSpeed * 0.8f, baseTypeSpeed * 1.2f);
            yield return new WaitForSeconds(randomDelay);
        }

        Debug.Log("Pen Writing Effect Completed");
    }

    // Unity Inspector'da bileşenlerin atanmadığını kontrol et
    void OnValidate()
    {
        if (textComponent == null)
            textComponent = GetComponent<TextMeshProUGUI>();
    }
}
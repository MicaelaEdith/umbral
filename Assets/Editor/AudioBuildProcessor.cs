using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class AudioBuildProcessor : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        if (report.summary.platform != BuildTarget.WebGL)
            return;

        string[] guids = AssetDatabase.FindAssets("t:AudioClip");
        int modified = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AudioImporter importer = AssetImporter.GetAtPath(path) as AudioImporter;
            if (importer == null) continue;

            AudioImporterSampleSettings settings = importer.defaultSampleSettings;
            AudioImporterSampleSettings webglSettings = importer.GetOverrideSampleSettings("WebGL");

            bool changed = false;

            if (webglSettings.compressionFormat != AudioCompressionFormat.Vorbis)
            {
                webglSettings.compressionFormat = AudioCompressionFormat.Vorbis;
                changed = true;
            }
            if (webglSettings.loadType != AudioClipLoadType.CompressedInMemory)
            {
                webglSettings.loadType = AudioClipLoadType.CompressedInMemory;
                changed = true;
            }
            if (Mathf.Abs(webglSettings.quality - 0.8f) > 0.01f)
            {
                webglSettings.quality = 0.8f;
                changed = true;
            }

            if (changed)
            {
                importer.SetOverrideSampleSettings("WebGL", webglSettings);
                modified++;
            }
        }

        if (modified > 0)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"AudioBuildProcessor: Forzados {modified} AudioClip(s) a Vorbis/CompressedInMemory para WebGL.");
        }
        else
        {
            Debug.Log("AudioBuildProcessor: Todos los AudioClips ya están configurados para WebGL.");
        }
    }
}

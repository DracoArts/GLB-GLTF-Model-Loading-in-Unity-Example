using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI; // For accessing the Slider component
using Siccity.GLTFUtility;
using System;
using System.IO;
using UnityEditor;

public class ModelLoader : MonoBehaviour
{

    [Header("Replace Your GLB/GLTF file link")]
    // Replace Your glb file link
    [Space(10)]
    public string url = "https://dracoarts-logo.s3.eu-north-1.amazonaws.com/test.glb";

    string filePath;
    public GameObject loadedModelContainer;

    public Slider downloadProgressSlider;

    void Start()
    {
        filePath = $"{Application.persistentDataPath}/Files/test.glb";
        Debug.Log(filePath);


        if (downloadProgressSlider != null)
            downloadProgressSlider.gameObject.SetActive(false);
    }

    public void StartDownload()
    {
        DownloadAndLoadModel(url);
    }

    public void DownloadAndLoadModel(string url)
    {
        string path = filePath;

        if (File.Exists(path))
        {
            Debug.Log("Found file locally, loading from cache");
            LoadModel(path);
            return;
        }

        StartCoroutine(DownloadFile(url, path));
    }

    IEnumerator DownloadFile(string url, string savePath)
    {
        // Ensure directory exists
        string directory = Path.GetDirectoryName(savePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            req.downloadHandler = new DownloadHandlerFile(savePath);

            if (downloadProgressSlider != null)
            {
                downloadProgressSlider.gameObject.SetActive(true);
                downloadProgressSlider.value = 0;
            }

            req.SendWebRequest();

            while (!req.isDone)
            {
                if (downloadProgressSlider != null)
                    downloadProgressSlider.value = req.downloadProgress; // Update slider value

                yield return null; // Wait for the next frame
            }

            // Hide slider after download completes
            if (downloadProgressSlider != null)
                downloadProgressSlider.gameObject.SetActive(false);

            if (req.isNetworkError || req.isHttpError)
            {
                Debug.LogError($"Download failed: {req.error}");
            }
            else
            {
                Debug.Log("Download completed successfully");
                LoadModel(savePath);
            }
        }
    }

    void LoadModel(string path)
    {
        ResetModelContainer();

        try
        {
            GameObject model = Importer.LoadFromFile(path);
            if (model != null)
            {
                model.transform.SetParent(loadedModelContainer.transform);
                model.transform.localPosition = Vector3.zero;
                model.transform.localRotation = Quaternion.identity;
        
                Debug.Log("Model loaded successfully");
            }
            else
            {
                Debug.LogError("Failed to load model - importer returned null");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading model: {e.Message}");
        }
    }

    void ResetModelContainer()
    {
        if (loadedModelContainer != null)
        {
            foreach (Transform child in loadedModelContainer.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}

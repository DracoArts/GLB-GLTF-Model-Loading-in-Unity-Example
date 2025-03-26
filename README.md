
# Welcome to DracoArts

![Logo](https://dracoarts-logo.s3.eu-north-1.amazonaws.com/DracoArts.png)




# Runtime GLB/GLTF Model Loading in Unity Using GLTFUtility

  This system enables dynamic loading of 3D models in GLB or GLTF format during gameplay or application runtime, rather than requiring models to be imported at development time. The GLTFUtility plugin provides an efficient way to parse these standardized 3D file formats and convert them into Unity GameObjects.
# Key Features
 ## Format Support
 - Handles both GLB (binary) and GLTF (JSON-based) formats

- Supports embedded textures or separate texture files

-   Maintains model hierarchy and structure
## 🔹Loading Methods

## Web Loading

 - Download models from cloud storage or web servers

- Progress tracking for downloads

- Automatic retry on network failures

## Local Loading

- Load from device storage (mobile/desktop)

- Supports Application.persistentDataPath for cross-platform use
## Model Processing
 - Automatic Scaling: Normalizes models to consistent size

- Centering: Positions model pivot appropriately

- Material Handling: Basic material support with options for override

- Component Preservation: Maintains mesh colliders if present in original

## Performance Considerations
 - Memory Management: Uses temporary cache files for web downloads

- Async Loading: Non-blocking import process

- Cleanup: Proper disposal of web requests and temporary assets

- Instance Reuse: Option to replace or additive loading

# Integration Requirements
 - GLTFUtility Plugin:


       "com.siccity.gltfutility": "https://github.com/siccity/gltfutility.git"

 - Get Newtonsoft.JSON 
      
       "com.unity.nuget.newtonsoft-json": "2.0.0-preview"
 - [ UnityWebRequest:](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Networking.UnityWebRequest.html)
     
     For network functionality


 
- [IO Permissions:](https://docs.unity3d.com/Manual/android-RequestingPermissions.html)
  

  Required for mobile/local file access

 ##  Typical Use Cases

- 3D model marketplaces/downloaders

- User-generated content systems

- Dynamic content loading in AR/VR applications

- Configuration-based model displays


## Usage/Examples

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
## Images

![](https://raw.githubusercontent.com/AzharKhemta/DemoClient/refs/heads/main/glb%20model%20import.gif)

## Authors

- [@MirHamzaHasan](https://github.com/MirHamzaHasan)
- [@WebSite](https://mirhamzahasan.com)


## 🔗 Links

[![linkedin](https://img.shields.io/badge/linkedin-0A66C2?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/company/mir-hamza-hasan/posts/?feedView=all/)
## Tech Stack
**Client:** Unity,C#

**Package:** GLTFUtility




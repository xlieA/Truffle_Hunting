using System;
using UnityEngine;
using Unity.Barracuda;
//Turn on HDR settings to make this work!!!
public class StyleTransfer : MonoBehaviour
{
   [Tooltip("Performs the preprocessing and postprocessing steps")]

    //[SerializeField] Shader _blurShader;
    public ComputeShader styleTransferShader;

    [Tooltip("Stylize the camera feed")]
    public bool stylizeImage = false;

    [Tooltip("The height of the image being fed to the model")]
    public int targetHeight = 540;

    [Tooltip("The model asset file that will be used when performing inference")]
    public NNModel modelAsset;

    [Tooltip("The backend used when performing inference")]
    public WorkerFactory.Type workerType = WorkerFactory.Type.Auto;

    // The compiled model used for performing inference
    private Model m_RuntimeModel;
    // The interface used to execute the neural network
    private IWorker engine;
  


    //Material _preprocessMaterial;
    // Start is called before the first frame update
    void Start()
    {
        m_RuntimeModel = ModelLoader.Load(modelAsset);

        engine = WorkerFactory.CreateWorker(workerType, m_RuntimeModel);

        //_preprocessMaterial = new Material(_blurShader);
        //_preprocessMaterial.SetFloat("_Sigma", 10.0f);
    
        
    }

    // OnDisable is called when the MonoBehavior becomes disabled or inactive
    private void OnDisable()
    {
        //Destroy(_preprocessMaterial);
        // Release the resources allocated for the inference engine
        engine.Dispose();
    }

    private void ProcessImage(RenderTexture image, string functionName)
    {
        try
        {
        // Specify the number of threads on the GPU
        int numthreads = 8;
        // Get the index for the specified function in the ComputeShader
        int kernelHandle = styleTransferShader.FindKernel(functionName);
        // Define a temporary HDR RenderTexture
        RenderTexture result = RenderTexture.GetTemporary(image.width, image.height, 24, RenderTextureFormat.ARGBHalf);
        // Enable random write access
        result.enableRandomWrite = true;
        // Create the HDR RenderTexture
        result.Create();

        // Set the value for the Result variable in the ComputeShader
        styleTransferShader.SetTexture(kernelHandle, "Result", result);
        // Set the value for the InputImage variable in the ComputeShader
        styleTransferShader.SetTexture(kernelHandle, "InputImage", image);

        // Execute the ComputeShader
        styleTransferShader.Dispatch(kernelHandle, result.width / numthreads, result.height / numthreads, 1);

        // Copy the result into the source RenderTexture
        Graphics.Blit(result, image);

        // Release the temporary RenderTexture
        RenderTexture.ReleaseTemporary(result);
        }
        catch (Exception e)
        {
            Debug.LogError($"An exception occurred in ProcessImage: {e.Message}");
            // Handle the exception or log additional information as needed
        }
    }


    private void StylizeImage(RenderTexture src)
    {

        // Create a new RenderTexture variable
        RenderTexture rTex;

        if (src.height > targetHeight && targetHeight >= 8)
        {
            
            // Calculate the scale value for reducing the size of the input image
            float scale = src.height / targetHeight;
            // Calcualte the new image width
            int targetWidth = (int)(src.width / scale);

            // Adjust the target image dimensions to be multiples of 8
            targetHeight -= (targetHeight % 8);
            targetWidth -= (targetWidth % 8);

            // Assign a temporary RenderTexture with the new dimensions
            rTex = RenderTexture.GetTemporary(targetWidth, targetHeight, 24, src.format);
        }
        else
        {
            // Assign a temporary RenderTexture with the src dimensions
            rTex = RenderTexture.GetTemporary(src.width, src.height, 24, src.format);
        }

        //rTex = RenderTexture.GetTemporary(256, 256, 24, src.format);
        //preprocess with blur

        //RenderTexture.active = null;
        Graphics.Blit(src, rTex);//Graphics.Blit(src, rTex, _preprocessMaterial);

        // Apply preprocessing steps
        ProcessImage(rTex, "ProcessInput");

        // Create a Tensor of shape [1, rTex.height, rTex.width, 3]
        Tensor input = new Tensor(rTex, channels: 3);

        // Execute neural network with the provided input
        engine.Execute(input);
        // Get the raw model output
        Tensor prediction = engine.PeekOutput();
        // Release GPU resources allocated for the Tensor
        input.Dispose();

        // Make sure rTex is not the active RenderTexture
        RenderTexture.active = null;
        // Copy the model output to rTex
        prediction.ToRenderTexture(rTex);
        // Release GPU resources allocated for the Tensor
        prediction.Dispose();

        // Apply postprocessing steps
        ProcessImage(rTex, "ProcessOutput");
        // Copy rTex into src
        Graphics.Blit(rTex, src);

        // Release the temporary RenderTexture
        RenderTexture.ReleaseTemporary(rTex);
    }


    /// <summary>
    /// OnRenderImage is called after the Camera had finished rendering 
    /// </summary>
    /// <param name="src">Input from the Camera</param>
    /// <param name="dest">The texture for the targer display</param>
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        
        if (stylizeImage)
        {
            //RenderTexture.active = null;
            //Graphics.Blit(src, dest, _preprocessMaterial);
            StylizeImage(src);
            
        }

        Graphics.Blit(src, dest);
    }
    
}
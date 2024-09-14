
// using UnityEngine.XR.ARFoundation;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine.XR.ARSubsystems;
// using UnityEngine;

// using UnityEngine.UI;
// using Unity.Barracuda;

// public class StyleTransfer : MonoBehaviour
// {
//     [SerializeField ]
//     public ComputeShader styleTransferShader;
//     public ARCameraManager arCameraManager;
//     public NNModel styleTransferModel; // Drag and drop your Barracuda style transfer model here
//     private IWorker styleTransferWorker;
//     private RenderTexture stylizedTexture;
//     public bool stylizeImage = true;
//      public int targetHeight = 224;
//     const string INPUT_NAME = "input1";
//     const string OUTPUT_NAME ="output1"; //"conv2d_transpose_8"; // 

//     void Start()
//     {
//         // Initialize the stylized texture with the desired size
//         stylizedTexture = new RenderTexture(224, 224, 0);
//         stylizedTexture.enableRandomWrite = true;
//         stylizedTexture.Create();

//         // Load the Barracuda model
//         var model = ModelLoader.Load(styleTransferModel);
//         styleTransferWorker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model);
//     }

//     void Update()
//     {
//         if (stylizeImage)
//         {
//             if (arCameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
//             {
            
//                 ProcessAndApplyStyleTransfer(image);
                

//                 GetComponent<RawImage>().texture = stylizedTexture;
                
//                 image.Dispose();
//             }
//         }
//     }
   

//     void ProcessAndApplyStyleTransfer(XRCpuImage cpuImage)
//     {
     
//         ConveretTexture2dIntoRenderTexture(out stylizedTexture, cpuImage, 224);
//         // Apply style transfer using Barracuda
//         StylizeImage(stylizedTexture);

//         // Update the RawImage component with the stylized texture
       
//     }
//     private void ProcessImage(RenderTexture image, string functionName)
//     {
//         int numthreads = 8;
//         int kernelHandle = styleTransferShader.FindKernel(functionName);
//         RenderTexture result = RenderTexture.GetTemporary(image.width, image.height, 24, RenderTextureFormat.ARGBHalf);
//         result.enableRandomWrite = true;
//         result.Create();

//         styleTransferShader.SetTexture(kernelHandle, "Result", result);
//         styleTransferShader.SetTexture(kernelHandle, "InputImage", image);

//         styleTransferShader.Dispatch(kernelHandle, result.width / numthreads, result.height / numthreads, 1);

//         Graphics.Blit(result, image);

//         RenderTexture.ReleaseTemporary(result);
//     }
//      private void StylizeImage(RenderTexture src)
//     {
//         // Create a new RenderTexture variable
//         RenderTexture rTex;

      
//         rTex = RenderTexture.GetTemporary(targetHeight, targetHeight, 24, src.format);
//         // Copy the src RenderTexture to the new rTex RenderTexture
//         Graphics.Blit(src, rTex);

//         // Apply preprocessing steps
//         ProcessImage(rTex, "ProcessInput");

//         // Create a Tensor of shape [1, rTex.height, rTex.width, 3]
//         Tensor input = new Tensor(rTex, channels: 3);
//         var inputs = new Dictionary<string, Tensor> {
//                 { INPUT_NAME, input }
//             };
//         // Execute neural network with the provided input
//         styleTransferWorker.Execute(inputs);
//         // Get the raw model output
//         Tensor prediction = styleTransferWorker.PeekOutput();
//         // Release GPU resources allocated for the Tensor
//         input.Dispose();

//         // Make sure rTex is not the active RenderTexture
//         RenderTexture.active = null;
//         // Copy the model output to rTex
//         prediction.ToRenderTexture(rTex);
//         // Release GPU resources allocated for the Tensor
//         prediction.Dispose();

//         // Apply postprocessing steps
//         ProcessImage(rTex, "ProcessOutput");
//         // Copy rTex into src
//         Graphics.Blit(rTex, src);

//         // Release the temporary RenderTexture
//         RenderTexture.ReleaseTemporary(rTex);
//     }
    
//     private void CustomTensorToRenderTexture(Tensor X, RenderTexture target, int batch, int fromChannel, Vector4 scale, Vector4 bias, Texture3D lut = null)
//     {
//         try
//         {
//             X.ToRenderTexture(target, batch, fromChannel, scale, bias, lut);
//         }
//         catch (System.Exception e)
//         {
//             Debug.LogError($"Error in CustomTensorToRenderTexture: {e.Message}");
//         }
//     }
//     private void ConveretTexture2dIntoRenderTexture(out RenderTexture out_renderTexture, in XRCpuImage input_texture2d, int resolution)
//         {
//             Texture2D resizedImage = new Texture2D(224, 224, TextureFormat.RGB24, false);
//             resizedImage.LoadRawTextureData(input_texture2d.GetPlane(0).data);
//             resizedImage.Apply();
//             out_renderTexture = new RenderTexture(resolution, resolution, 0);
//             out_renderTexture.enableRandomWrite = true;
//             RenderTexture.active = out_renderTexture;
//             Graphics.Blit(resizedImage, out_renderTexture);


//         }

//     void OnDestroy()
//     {
//         styleTransferWorker?.Dispose();
//     }
// }

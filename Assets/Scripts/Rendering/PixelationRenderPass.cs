using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

/*
 * AI-GENERATED IMPLEMENTATION
 *
 * This code was developed with AI assistance to support
 * supplementary features outside the core NEA objectives.
 *
 * It is primarily intended to improve the visual presentation
 * and overall user experience of the project, rather than
 * demonstrate the programming techniques being assessed.
 */

namespace Rendering
{
    public sealed class PixelationRenderPass
        : ScriptableRenderPass
    {
        private static readonly int ColorStepsId =
            Shader.PropertyToID("_ColorSteps");

        private static readonly int QuantizeColorsId =
            Shader.PropertyToID("_QuantizeColors");

        private readonly Material _material;
        private readonly PixelationSettings _settings;
        private readonly MaterialPropertyBlock _upscaleProperties = new();

        public PixelationRenderPass(
            Material material,
            PixelationSettings settings)
        {
            _material = material;
            _settings = settings;

            // We sample the complete camera colour, not the opaque texture.
            requiresIntermediateTexture = true;
        }

        public override void RecordRenderGraph(
            RenderGraph renderGraph,
            ContextContainer frameData)
        {
            UniversalResourceData resources =
                frameData.Get<UniversalResourceData>();

            if (resources.isActiveTargetBackBuffer)
            {
                return;
            }

            TextureHandle source =
                resources.activeColorTexture;

            if (!source.IsValid())
                return;

            TextureDesc sourceDesc =
                source.GetDescriptor(renderGraph);

            var sourceInfo = renderGraph.GetRenderTargetInfo(source);
            int sourceWidth = sourceInfo.width;
            int sourceHeight = sourceInfo.height;

            if (sourceWidth <= 0 || sourceHeight <= 0)
                return;

            int lowHeight = Mathf.Clamp(
                _settings.ReferenceHeight,
                1,
                sourceHeight
            );

            int lowWidth = Mathf.Max(
                1,
                Mathf.RoundToInt(
                    (float)sourceWidth * lowHeight /
                    sourceHeight
                )
            );

            TextureDesc outputDesc =
                new TextureDesc(sourceDesc);

            outputDesc.sizeMode = TextureSizeMode.Explicit;
            outputDesc.width = sourceWidth;
            outputDesc.height = sourceHeight;
            outputDesc.msaaSamples = MSAASamples.None;
            outputDesc.bindTextureMS = false;
            outputDesc.filterMode = FilterMode.Point;
            outputDesc.wrapMode = TextureWrapMode.Clamp;
            outputDesc.useMipMap = false;
            outputDesc.autoGenerateMips = false;
            outputDesc.enableRandomWrite = false;
            outputDesc.useDynamicScale = false;
            outputDesc.useDynamicScaleExplicit = false;
            outputDesc.memoryless = RenderTextureMemoryless.None;
            outputDesc.clearBuffer = false;
            outputDesc.name = "DungeonNEA_PixelatedOutput";

            TextureDesc lowDesc = new TextureDesc(outputDesc);
            lowDesc.width = lowWidth;
            lowDesc.height = lowHeight;
            lowDesc.name = "DungeonNEA_LowResolution";

            TextureHandle lowTexture =
                renderGraph.CreateTexture(lowDesc);

            TextureHandle outputTexture =
                renderGraph.CreateTexture(outputDesc);

            // Render Graph executes later; keep this pass's values separate
            // from the shared material and reuse the block across frames.
            _upscaleProperties.Clear();
            _upscaleProperties.SetFloat(
                QuantizeColorsId,
                _settings.QuantizeColors ? 1f : 0f
            );

            _upscaleProperties.SetFloat(
                ColorStepsId,
                Mathf.Clamp(_settings.ColorSteps, 2, 64)
            );

            // Pass 0: downsample the rendered scene.
            var downsample =
                new RenderGraphUtils.BlitMaterialParameters(
                    source,
                    lowTexture,
                    _material,
                    0
                );

            renderGraph.AddBlitPass(
                downsample,
                "DungeonNEA Pixelation Downsample"
            );

            // Pass 1: upscale and optionally quantize colours.
            var upscale =
                new RenderGraphUtils.BlitMaterialParameters(
                    lowTexture,
                    outputTexture,
                    _material,
                    1
                );

            upscale.propertyBlock = _upscaleProperties;

            renderGraph.AddBlitPass(
                upscale,
                "DungeonNEA Pixelation Upscale"
            );

            // Subsequent rendering uses our processed image.
            resources.cameraColor = outputTexture;
        }
    }
}

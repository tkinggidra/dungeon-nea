using UnityEngine;
using UnityEngine.Rendering;
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
    public sealed class PixelationRendererFeature
        : ScriptableRendererFeature
    {
        [SerializeField]
        private PixelationSettings settings = new();

        [SerializeField]
        private Shader shader;

        private Material _material;
        private PixelationRenderPass _pass;

        public override void Create()
        {
            CoreUtils.Destroy(_material);
            _material = null;
            _pass = null;

            Shader selectedShader = shader != null
                ? shader
                : Shader.Find("Hidden/DungeonNEA/Pixelation");

            if (selectedShader == null)
            {
                Debug.LogWarning(
                    "Dungeon NEA pixelation shader not found."
                );

                _pass = null;
                return;
            }

            _material = CoreUtils.CreateEngineMaterial(
                selectedShader
            );

            _pass = new PixelationRenderPass(
                _material,
                settings
            );

            _pass.renderPassEvent =
                RenderPassEvent.AfterRenderingPostProcessing;
        }

        public override void AddRenderPasses(
            ScriptableRenderer renderer,
            ref RenderingData renderingData)
        {
            if (_pass == null)
                return;

            if (renderingData.cameraData.cameraType
                != CameraType.Game)
            {
                return;
            }

            if (renderingData.cameraData.renderType
                != CameraRenderType.Base)
            {
                return;
            }

            renderer.EnqueuePass(_pass);
        }

        protected override void Dispose(bool disposing)
        {
            CoreUtils.Destroy(_material);
            _material = null;
            _pass = null;
        }
    }
}

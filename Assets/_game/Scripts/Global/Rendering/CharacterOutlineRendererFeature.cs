using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace Game
{
    public sealed class CharacterOutlineRendererFeature : ScriptableRendererFeature
    {
        [SerializeField]
        private Material _outlineMaterial;

        [SerializeField]
        private LayerMask _layerMask;

        private CharacterOutlineRenderPass _renderPass;

        public override void Create()
        {
            _renderPass = new CharacterOutlineRenderPass(_outlineMaterial, _layerMask);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(_renderPass);
        }

        private sealed class CharacterOutlineRenderPass : ScriptableRenderPass
        {
            private static readonly List<ShaderTagId> ShaderTagIds = new()
            {
                new ShaderTagId("UniversalForwardOnly"),
                new ShaderTagId("UniversalForward"),
                new ShaderTagId("SRPDefaultUnlit"),
                new ShaderTagId("LightweightForward")
            };

            private readonly Material _outlineMaterial;
            private readonly FilteringSettings _filteringSettings;

            public CharacterOutlineRenderPass(Material outlineMaterial, LayerMask layerMask)
            {
                _outlineMaterial = outlineMaterial;
                _filteringSettings = new FilteringSettings(RenderQueueRange.all, layerMask);
                renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
            }

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                var renderingData = frameData.Get<UniversalRenderingData>();
                var cameraData = frameData.Get<UniversalCameraData>();
                var lightData = frameData.Get<UniversalLightData>();
                var resourceData = frameData.Get<UniversalResourceData>();
                var drawingSettings = RenderingUtils.CreateDrawingSettings(
                    ShaderTagIds,
                    renderingData,
                    cameraData,
                    lightData,
                    cameraData.defaultOpaqueSortFlags);

                drawingSettings.overrideMaterial = _outlineMaterial;
                drawingSettings.overrideMaterialPassIndex = 0;

                var maskRendererListParameters = new RendererListParams(
                    renderingData.cullResults,
                    drawingSettings,
                    _filteringSettings);

                drawingSettings.overrideMaterialPassIndex = 1;

                var outlineRendererListParameters = new RendererListParams(
                    renderingData.cullResults,
                    drawingSettings,
                    _filteringSettings);

                using (var builder = renderGraph.AddRasterRenderPass<PassData>(
                           "Character Outline",
                           out var passData))
                {
                    passData.MaskRendererList = renderGraph.CreateRendererList(maskRendererListParameters);
                    passData.OutlineRendererList = renderGraph.CreateRendererList(outlineRendererListParameters);

                    builder.UseRendererList(passData.MaskRendererList);
                    builder.UseRendererList(passData.OutlineRendererList);
                    builder.SetRenderAttachment(resourceData.activeColorTexture, 0);
                    builder.SetRenderAttachmentDepth(resourceData.activeDepthTexture, AccessFlags.ReadWrite);
                    builder.SetRenderFunc(static (PassData data, RasterGraphContext context) =>
                    {
                        context.cmd.DrawRendererList(data.MaskRendererList);
                        context.cmd.DrawRendererList(data.OutlineRendererList);
                    });
                }
            }

            private sealed class PassData
            {
                public RendererListHandle MaskRendererList;
                public RendererListHandle OutlineRendererList;
            }
        }
    }
}

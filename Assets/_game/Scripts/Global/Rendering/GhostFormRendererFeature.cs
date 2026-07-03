using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace Game
{
    public sealed class GhostFormRendererFeature : ScriptableRendererFeature
    {
        private static GhostFormRendererFeature _instance;

        [SerializeField]
        private Material _ghostMaterial;

        [SerializeField]
        private LayerMask _layerMask;

        private GhostFormRenderPass _renderPass;

        public override void Create()
        {
            _renderPass = new GhostFormRenderPass(_ghostMaterial, _layerMask);
            _instance = this;
        }

        public static void SetColor(Color color)
        {
            if (_instance == null)
            {
                return;
            }

            _instance._renderPass.SetColor(color);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(_renderPass);
        }

        private sealed class GhostFormRenderPass : ScriptableRenderPass
        {
            private static readonly List<ShaderTagId> ShaderTagIds = new()
            {
                new ShaderTagId("UniversalForwardOnly"),
                new ShaderTagId("UniversalForward"),
                new ShaderTagId("SRPDefaultUnlit"),
                new ShaderTagId("LightweightForward")
            };

            private readonly Material _ghostMaterial;
            private readonly FilteringSettings _filteringSettings;

            public GhostFormRenderPass(Material ghostMaterial, LayerMask layerMask)
            {
                _ghostMaterial = ghostMaterial;
                _filteringSettings = new FilteringSettings(RenderQueueRange.all, layerMask);
                renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
            }

            public void SetColor(Color color)
            {
                _ghostMaterial.SetColor("_GhostColor", color);
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

                drawingSettings.overrideMaterial = _ghostMaterial;
                drawingSettings.overrideMaterialPassIndex = 0;

                var rendererListParameters = new RendererListParams(
                    renderingData.cullResults,
                    drawingSettings,
                    _filteringSettings);

                using (var builder = renderGraph.AddRasterRenderPass<PassData>(
                           "Ghost Form",
                           out var passData))
                {
                    passData.RendererList = renderGraph.CreateRendererList(rendererListParameters);

                    builder.UseRendererList(passData.RendererList);
                    builder.SetRenderAttachment(resourceData.activeColorTexture, 0);
                    builder.SetRenderAttachmentDepth(resourceData.activeDepthTexture, AccessFlags.ReadWrite);
                    builder.SetRenderFunc(static (PassData data, RasterGraphContext context) =>
                    {
                        context.cmd.DrawRendererList(data.RendererList);
                    });
                }
            }

            private sealed class PassData
            {
                public RendererListHandle RendererList;
            }
        }
    }
}

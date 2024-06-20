using DaveTheMonitor.Core.API;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.Renderers;
using StudioForge.TotalMiner.API;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DaveTheMonitor.Core.Animation
{
    internal sealed class ActorRenderer : ICoreActorRenderer
    {
        private delegate void SetLightInWorldMatrix(Map map, Vector3 footPos, Vector3 eyePos, ref Matrix world, byte alpha, bool perFrameUpdate);
        private static SetLightInWorldMatrix _setLightInWorldMatrix =
            AccessTools.Method("StudioForge.TotalMiner.NpcManager:SetLightInWorldMatrix").CreateDelegate<SetLightInWorldMatrix>();
        private static Action<int[]> _initIndices =
            AccessTools.Method("StudioForge.TotalMiner.Graphics.GraphicStatics:InitIndices", new Type[] { typeof(int[]) }).CreateDelegate<Action<int[]>>();

        public bool DrawPositions
        {
            get => _drawPositions;
            set
            {
                if (_drawPositions && !value)
                {
                    _lineRenderer.UnloadContent();
                    _lineRenderer = null;
                }
                _drawPositions = value;
            }
        }
        private VertexBuffer _instanceBuffer;
        private Effect _shader;
        private EffectParameter _world;
        private EffectParameter _viewProjection;
        private EffectParameter _cameraPosition;
        private EffectParameter _alpha;
        private LineRenderer _lineRenderer;
        private List<ActorPartSnapshot> _snapshot;
        private List<ICoreActor> _actorsToRender;
        private int _partsDrawn;
        private int _actorsDrawn;
        private int _drawCalls;
        private Dictionary<ActorPart, CustomArray<VertexInstance>> _instanceData;
        private Pool<CustomArray<VertexInstance>> _pool;
        private VertexBufferBinding[] _bindings;
        private IndexBuffer _indices;
        private int _vertexCount;
        private bool _drawPositions;
        private bool _disposedValue;

        public void LoadContent()
        {
            _shader = (Effect)AccessTools.TypeByName("StudioForge.TotalMiner.Graphics.GraphicStatics").GetNestedType("AvatarShader", BindingFlags.Public).Field("Effect").GetValue(null);
            _world = _shader.Parameters["World"];
            _viewProjection = _shader.Parameters["ViewProjection"];
            _cameraPosition = _shader.Parameters["CameraPosition"];
            _alpha = _shader.Parameters["Alpha"];
        }

        public void Update()
        {
            _actorsToRender.Clear();
        }

        public void AddActorToRender(ICoreActor actor)
        {
            _actorsToRender.Add(actor);
        }

        public void Draw(ICorePlayer player, ITMPlayer virtualPlayer)
        {
            _actorsDrawn = 0;
            _drawCalls = 0;
            _partsDrawn = 0;
            if (_actorsToRender.Count == 0)
            {
                return;
            }

            _viewProjection.SetValue(virtualPlayer.ViewMatrix * player.ProjectionMatrix);
            _cameraPosition.SetValue(virtualPlayer.EyePosition);
            _world.SetValue(virtualPlayer.WorldShake);
            _alpha.SetValue(1f);
            GraphicsDevice device = CoreGlobals.GraphicsDevice;
            device.BlendState = BlendState.AlphaBlend;
            device.RasterizerState = RasterizerState.CullNone;
            device.DepthStencilState = DepthStencilState.Default;
            device.SamplerStates[0] = SamplerState.PointClamp;

            int highestVertexCount = 0;

            foreach (ICoreActor actor in _actorsToRender)
            {
                if (actor.Model == null || !actor.IsActive)
                {
                    continue;
                }

                BoundingBox box = actor.Model.ViewBounds;
                box.Min += actor.Position;
                box.Max += actor.Position;
                if (!virtualPlayer.Frustum.Intersects(box))
                {
                    continue;
                }

                ActorModel model = actor.Model;
                AnimationController controller = actor.Animation;
                ActorAnimation animation = controller.CurrentState.Animation;

                float time = controller.CurrentTime;
                ActorPartSnapshot modelSnapshot;
                if (controller.TotalTime < 0.25f && controller.TryGetPreviousStateSnapshot(out ActorPartSnapshot[] controllerSnapshot, out ActorPartSnapshot controllerModelSnapshot))
                {
                    animation.Snapshot(actor, time, controllerSnapshot, controllerModelSnapshot, 0.25f, _snapshot, out modelSnapshot);
                }
                else
                {
                    animation.Snapshot(actor, time, _snapshot, out modelSnapshot);
                }
                Vector3 pos = actor.Position;
                Vector3 dir = Vector3.Normalize(new Vector3(actor.ViewDirection.X, 0, actor.ViewDirection.Z));

                Quaternion actorRot = Quaternion.CreateFromAxisAngle(Vector3.Up, MathF.Atan2(-dir.X, -dir.Z));
                Matrix actorMatrix = Matrix.CreateFromQuaternion(actorRot) * Matrix.CreateTranslation(pos);
                modelSnapshot.Transform *= actorMatrix;

                foreach (ActorPartSnapshot part in _snapshot)
                {
                    if (!part.Part.ReadyToRender && !part.Part.CheckForMeshLoaded())
                    {
                        continue;
                    }

                    if (part.Part.VertexCount > highestVertexCount)
                    {
                        highestVertexCount = part.Part.VertexCount;
                    }

                    Matrix transform = part.Transform * actorMatrix;

                    if (DrawPositions)
                    {
                        float scale = model.ModelHeight / model.BlockHeight;
                        Vector3 globalPos = pos + part.Part.Position;
                        int parent = part.Part.Parent;
                        while (parent != -1)
                        {
                            globalPos += _snapshot[parent].Part.Position;
                            parent = _snapshot[parent].Part.Parent;
                        }

                        DrawAxis(virtualPlayer.ViewMatrix, player.ProjectionMatrix, Vector3.Transform(part.Part.Pivot + Vector3.One, transform), scale, Color.Orange, Color.LightYellow, Color.Purple);
                        DrawAxis(virtualPlayer.ViewMatrix, player.ProjectionMatrix, Vector3.Transform(Vector3.One, transform), scale, Color.Red, Color.Green, Color.Blue);
                    }

                    _setLightInWorldMatrix((Map)actor.World.Map.TMMap, actor.Position + new Vector3(0, 0.1f, 0), actor.EyePosition + new Vector3(0, 0.1f, 0), ref transform, 255, true);
                    if (!_instanceData.TryGetValue(part.Part, out CustomArray<VertexInstance> instanceData))
                    {
                        int handle = _pool.GetNext();
                        instanceData = _pool.List[handle];
                        _instanceData[part.Part] = instanceData;
                    }
                    instanceData.Add(new VertexInstance(transform));
                }

                _actorsDrawn++;
            }

            if (highestVertexCount > _vertexCount)
            {
                _vertexCount = highestVertexCount;

                int[] indices = new int[(_vertexCount / 2) * 3];
                _initIndices(indices);

                _indices?.Dispose();
                _indices = new IndexBuffer(CoreGlobals.GraphicsDevice, IndexElementSize.ThirtyTwoBits, indices.Length, BufferUsage.WriteOnly);
                _indices.SetData(indices);
            }

            _shader.CurrentTechnique = _shader.Techniques["AvatarShaderInstancing"];
            foreach (KeyValuePair<ActorPart, CustomArray<VertexInstance>> pair in _instanceData)
            {
                ActorPart part = pair.Key;
                CustomArray<VertexInstance> instances = pair.Value;
                if (_instanceBuffer == null)
                {
                    _instanceBuffer = new VertexBuffer(device, typeof(VertexInstance), instances.Count, BufferUsage.WriteOnly);
                }
                else if (_instanceBuffer.VertexCount < instances.Count)
                {
                    _instanceBuffer.Dispose();
                    _instanceBuffer = new VertexBuffer(device, typeof(VertexInstance), instances.Count, BufferUsage.WriteOnly);
                }
                _instanceBuffer.SetData(instances.Array, 0, instances.Count);

                _shader.CurrentTechnique.Passes[0].Apply();

                device.SetVertexBuffers(new VertexBufferBinding(part.VertexBuffer, 0, 0), new VertexBufferBinding(_instanceBuffer, 0, 1));
                device.Indices = _indices;
                device.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, part.VertexBuffer.VertexCount / 2, instances.Count);
                _drawCalls++;
                _partsDrawn += instances.Count;

                instances.Clear();
            }

            _pool.ReleaseAll();
            _instanceData.Clear();

            if (DrawPositions)
            {
                DepthStencilState oldDepth = device.DepthStencilState;
                device.DepthStencilState = DepthStencilState.None;
                _lineRenderer.Present(virtualPlayer.ViewMatrix, player.ProjectionMatrix);
                device.DepthStencilState = oldDepth;
            }
        }

        private void DrawAxis(Matrix view, Matrix projection, Vector3 pos, float length, Color x, Color y, Color z)
        {
            if (_lineRenderer == null)
            {
                _lineRenderer = new LineRenderer();
                _lineRenderer.LoadContent(null);
            }

            _lineRenderer.DrawLine(pos, pos + (Vector3.Right * length), x, x);
            _lineRenderer.DrawLine(pos, pos + (Vector3.Up * length), y, y);
            _lineRenderer.DrawLine(pos, pos + (Vector3.Backward * length), z, z);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _indices?.Dispose();
                    _instanceBuffer?.Dispose();
                    _lineRenderer?.UnloadContent();
                }

                _shader = null;
                _world = null;
                _viewProjection = null;
                _cameraPosition = null;
                _alpha = null;
                _lineRenderer = null;
                _snapshot = null;
                _actorsToRender = null;
                _instanceData = null;
                _instanceBuffer = null;
                _lineRenderer = null;
                _pool = null;
                _bindings = null;
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public ActorRenderer()
        {
            _snapshot = new List<ActorPartSnapshot>();
            _actorsToRender = new List<ICoreActor>();
            _instanceData = new Dictionary<ActorPart, CustomArray<VertexInstance>>();
            _pool = new Pool<CustomArray<VertexInstance>>(16);
            _bindings = new VertexBufferBinding[2];
            _vertexCount = 0;
            _drawPositions = false;
        }
    }
}

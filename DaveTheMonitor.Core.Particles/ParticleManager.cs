using DaveTheMonitor.Core.API;
using DaveTheMonitor.Core.Plugin;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.TotalMiner.API;
using System;
using System.Collections.Generic;

namespace DaveTheMonitor.Core.Particles
{
    /// <summary>
    /// Manages particle updates and rendering.
    /// </summary>
    public sealed class ParticleManager : IDisposable
    {
        internal struct ParticleChunk
        {
            public const int Count = 128;
            public int Free;
        }

        private class ParticleDrawGroup
        {
            public const int DefaultCount = 128;
            public ParticleInstanceVertex[] Instances;
            public int InstanceCount;
            public EffectTechnique Technique;
            public ParticleMaterial Material;

            public ParticleDrawGroup(Effect effect, string technique, ParticleMaterial material)
            {
                Technique = effect.Techniques[technique];
                if (Technique == null)
                {
                    throw new InvalidOperationException($"Technique {technique} does not exist on effect {effect.Name}");
                }

                Material = material;
            }
        }

        /// <summary>
        /// The max number of particles that can be active at once.
        /// </summary>
        public int MaxParticles => _particles.Length;
        /// <summary>
        /// The number of currently active particles.
        /// </summary>
        public int ActiveParticles { get; private set; }
        /// <summary>
        /// The number of particles rendered when <see cref="Draw(Player, ITMPlayer, Viewport)"/> was last called.
        /// </summary>
        public int RenderedParticles => _renderedParticles;
        /// <summary>
        /// The number of vertices used for each particle.
        /// </summary>
        public int VerticesPerParticle => 4;
        /// <summary>
        /// The number of indices used for each particle.
        /// </summary>
        public int IndicesPerParticle => 6;
        /// <summary>
        /// All particles, including dead particles. Test <see cref="ParticleInstance.Dead"/> if only active particles are needed.
        /// </summary>
        public IEnumerable<ParticleInstance> Particles => _particles;
        internal int ChunkCount => _chunks.Length;
        private ParticleRegistry _registry;
        private ParticleInstance[] _particles;
        private ParticleChunk[] _chunks;
        private ICoreWorld _world;
        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;
        private DynamicVertexBuffer _instanceBuffer;
        private VertexBufferBinding[] _bindings;
        private int _renderedParticles;
        private ParticleDrawGroup _alphaTest;
        private ParticleDrawGroup _alphaTestNoFog;
        private ParticleDrawGroup _opaque;
        private ParticleDrawGroup _opaqueNoFog;
        private ParticleDrawGroup _alphaBlend;
        private ParticleDrawGroup _alphaBlendNoFog;
        private Effect _effect;
        private EffectParameter _viewMatrix;
        private EffectParameter _projectionMatrix;
        private EffectParameter _worldMatrix;
        private EffectParameter _fogColor;
        private EffectParameter _fogStart;
        private EffectParameter _fogEnd;
        private EffectParameter _skyStart;
        private EffectParameter _skyEnd;
        private EffectParameter _cameraPos;
        private EffectParameter _texture;
        private bool _disposedValue;

        /// <summary>
        /// Spawns a particle with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the particle definition to spawn.</param>
        /// <param name="position">The position of the particle.</param>
        /// <param name="velocity">The velocity of the particle. This may be overridden by the particle definition.</param>
        /// <returns>The particle spawned.</returns>
        public ParticleInstance SpawnParticle(string id, Vector3 position, Vector3 velocity)
        {
            if (_registry.TryGetDefinition(id, out ParticleDefinition definition))
            {
                return SpawnParticle(definition, position, velocity);
            }
            return null;
        }

        /// <summary>
        /// Spawns a particle with the specified definition.
        /// </summary>
        /// <param name="definition">The particle definition to spawn.</param>
        /// <param name="position">The position of the particle.</param>
        /// <param name="velocity">The velocity of the particle. This may be overridden by the particle definition.</param>
        /// <returns>The particle spawned.</returns>
        public ParticleInstance SpawnParticle(ParticleDefinition definition, Vector3 position, Vector3 velocity)
        {
            if (definition == null)
            {
                return null;
            }

            int i = GetNextFreeParticle(out int chunk);
            if (i == -1)
            {
                return null;
            }

            ActiveParticles++;
            ParticleInstance particle = _particles[i];
            particle.Initialize(definition, position, velocity);
            _chunks[chunk].Free--;
            return particle;
        }

        /// <summary>
        /// Destroys the particle with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the particle to destroy.</param>
        public void DestroyParticle(int id)
        {
            ParticleInstance particle = _particles[id];
            if (!particle.Dead)
            {
                particle.Dead = true;
                int chunk = GetChunk(particle.Id);
                _chunks[chunk].Free++;
                ActiveParticles--;
            }
        }

        /// <summary>
        /// Destroys the specified particle.
        /// </summary>
        /// <param name="particle">The particle to destroy.</param>
        public void DestroyParticle(ParticleInstance particle)
        {
            DestroyParticle(particle.Id);
        }

        /// <summary>
        /// Destroys all currently active particles.
        /// </summary>
        public void DestroyAllParticles()
        {
            ParticleInstance[] particles = _particles;
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].Dead = true;
            }
            for (int i = 0; i < _chunks.Length; i++)
            {
                _chunks[i].Free = ParticleChunk.Count;
            }
            ActiveParticles = 0;
        }

        private int GetNextFreeParticle(out int chunk)
        {
            // We use particle chunks to keep track of if a
            // particle is free in a given 'chunk' of the
            // array.
            //
            // Each chunk represents 128 particles.
            //
            // For an 8192 particle limit, there are 64
            // particle chunks.
            // This means that we will at maximum have
            // to search only 192 items instead of all
            // 8192.
            //
            // For a 32768 limit, there will be 256 chunks,
            // meaning a maximum search count of 384 instead
            // of 32768.
            //
            // The speed increase from this depends on how
            // deep in the array the first free particle is

            ParticleInstance[] array = _particles;
            for (int i = 0; i < _chunks.Length; i++)
            {
                if (_chunks[i].Free > 0)
                {
                    int start = i * ParticleChunk.Count;
                    int end = start + ParticleChunk.Count;
                    if (end > array.Length)
                    {
                        // This shouldn't be possible, but it allows the
                        // compiler to remove the array bounds check when
                        // indexing the particle array.
                        chunk = -1;
                        return -1;
                    }

                    for (int j = start; j < end; j++)
                    {
                        if (array[j].Dead)
                        {
                            chunk = i;
                            return j;
                        }
                    }
                }
            }

            chunk = -1;
            return -1;
        }

        private int GetChunk(int particleId)
        {
            return particleId / ParticleChunk.Count;
        }

        /// <summary>
        /// Sets the max number of particles that can be active at once.
        /// </summary>
        /// <param name="max">The max number of particles.</param>
        public void SetMaxParticles(int max)
        {
            if (max == _particles.Length)
            {
                return;
            }

            if (max < ParticleChunk.Count || max % ParticleChunk.Count != 0)
            {
                throw new ArgumentException($"Max must be a multiple of {ParticleChunk.Count}", nameof(max));
            }

            _particles = new ParticleInstance[max];
            _chunks = new ParticleChunk[max / ParticleChunk.Count];
            for (int i = 0; i < _particles.Length; i++)
            {
                _particles[i] = new ParticleInstance(i);
            }
            for (int i = 0; i < _chunks.Length; i++)
            {
                _chunks[i].Free = ParticleChunk.Count;
            }

            InitializeDrawGroups(_effect);

            int active = 0;
            ActiveParticles = active;
            _instanceBuffer.Dispose();
            _instanceBuffer = new DynamicVertexBuffer(CoreGlobals.GraphicsDevice, typeof(ParticleInstanceVertex), ParticleDrawGroup.DefaultCount, BufferUsage.WriteOnly);
        }

        public bool IsValidMax(int max)
        {
            return max >= ParticleChunk.Count && max % ParticleChunk.Count == 0;
        }

        /// <summary>
        /// Update all particles. Should be called every frame.
        /// </summary>
        public void Update(ITMPlayer virtualPlayer, bool update, bool buildInstances)
        {
            // TODO: this could be optimized, ~63000 particles on my machine runs
            // at ~48 fps, the majority of that time appears to be spent doing
            // matrix math
            // -DaveTheMonitor

            if (buildInstances)
            {
                PrepareForDraw(_alphaTest);
                PrepareForDraw(_alphaTestNoFog);
                PrepareForDraw(_opaque);
                PrepareForDraw(_opaqueNoFog);
                PrepareForDraw(_alphaBlend);
                PrepareForDraw(_alphaBlendNoFog);
            }

            ParticleInstance[] particles = _particles;
            int active = 0;
            int renderedParticles = 0;
            foreach (ParticleInstance particle in particles)
            {
                if (!particle.Dead)
                {
                    ParticleDefinition definition = _registry.GetDefinition(particle.Type);

                    if (update)
                    {
                        particle.Update(this, definition, _world);
                        active++;
                    }
                    
                    if (particle.Dead)
                    {
                        continue;
                    }

                    if (!definition.Draw || !buildInstances)
                    {
                        continue;
                    }

                    Vector2 size = definition.GetSize(particle);

                    if (!particle.ShouldRender(size, virtualPlayer.Frustum))
                    {
                        continue;
                    }

                    ParticleDrawGroup group = GetDrawGroup(definition);

                    Vector3 cameraPos = virtualPlayer.EyePosition;

                    Rectangle src = definition.GetSrc(particle);
                    src.X += definition._textureInfo.X;
                    src.Y += definition._textureInfo.Y;
                    Texture3D tex = _registry.Texture;
                    Color color = definition.GetColor(particle);
                    Vector2 texStart = new Vector2(src.X / (float)tex.Width, src.Y / (float)tex.Height);
                    Vector2 texEnd = new Vector2((src.X + src.Width) / (float)tex.Width, (src.Y + src.Height) / (float)tex.Height);

                    EnsureLength(group, group.InstanceCount + 1);
                    ParticleInstanceVertex[] instances = group.Instances;

                    float depth = definition._textureInfo.Depth > 0 ? tex.Depth / (float)definition._textureInfo.Depth : 0;
                    Vector3 texCoord0 = new Vector3(texStart, depth);
                    Vector3 texCoord1 = new Vector3(texEnd, depth);
                    ref ParticleInstanceVertex v = ref instances[group.InstanceCount];
                    v.Position = particle.Position;
                    v.Size = size;
                    v.Color = color;
                    v.TexCoord0 = texCoord0;
                    v.TexCoord1 = texCoord1;

                    group.InstanceCount++;
                    renderedParticles++;
                }
            }

            if (update)
            {
                ActiveParticles = active;
            }
            if (buildInstances)
            {
                _renderedParticles = renderedParticles;
            }
        }

        private ParticleDrawGroup GetDrawGroup(ParticleDefinition definition)
        {
            return definition.Material._numId switch
            {
                0 => _alphaTest,
                1 => _alphaTestNoFog,
                2 => _opaque,
                3 => _opaqueNoFog,
                4 => _alphaBlend,
                5 => _alphaBlendNoFog,
                _ => _alphaTest
            };
        }

        private void EnsureLength(ParticleDrawGroup group, int instances)
        {
            if (group.Instances == null)
            {
                group.Instances = new ParticleInstanceVertex[ParticleDrawGroup.DefaultCount];
            }

            EnsureLength(ref group.Instances, instances);
        }

        private void EnsureLength<T>(ref T[] data, int index)
        {
            if (data.Length > index)
            {
                return;
            }

            while (data.Length <= index)
            {
#if DEBUG
                CorePlugin.Log($"ParticleManager Array expanded to {data.Length * 2}");
#endif
                Array.Resize(ref data, data.Length * 2);
            }
        }

        private void UpdateBuffer(ParticleDrawGroup group)
        {
            if (_instanceBuffer.VertexCount < group.InstanceCount)
            {
                _instanceBuffer.Dispose();
                _instanceBuffer = new DynamicVertexBuffer(CoreGlobals.GraphicsDevice, typeof(ParticleInstanceVertex), Math.Max(_vertexBuffer.VertexCount * 2, group.Instances.Length), BufferUsage.WriteOnly);
            }
            _instanceBuffer.SetData(group.Instances, 0, group.InstanceCount);
        }

        private void PrepareForDraw(ParticleDrawGroup group)
        {
            group.InstanceCount = 0;
        }

        /// <summary>
        /// Draws all active particles.
        /// </summary>
        /// <param name="player">The main player.</param>
        /// <param name="virtualPlayer">The virtual player.</param>
        /// <param name="vp">The player's viewport.</param>
        public void Draw(ICorePlayer player, ITMPlayer virtualPlayer, Viewport vp)
        {
            if (_renderedParticles == 0)
            {
                return;
            }

            Draw(_alphaTest, player, virtualPlayer);
            Draw(_alphaTestNoFog, player, virtualPlayer);
            Draw(_opaque, player, virtualPlayer);
            Draw(_opaqueNoFog, player, virtualPlayer);
            Draw(_alphaBlend, player, virtualPlayer);
            Draw(_alphaBlendNoFog, player, virtualPlayer);
        }

        private void Draw(ParticleDrawGroup group, ICorePlayer player, ITMPlayer virtualPlayer)
        {
            if (group.InstanceCount == 0)
            {
                return;
            }

            GraphicsDevice device = CoreGlobals.GraphicsDevice;
            ParticleMaterial material = group.Material;

            _viewMatrix?.SetValue(virtualPlayer.ViewMatrix);
            _projectionMatrix?.SetValue(virtualPlayer.ProjectionMatrix);
            _worldMatrix?.SetValue(Matrix.Identity);
            _cameraPos?.SetValue(virtualPlayer.EyePosition);
            _texture?.SetValue(_registry.Texture);

            if (material.FogEnabled)
            {
                _fogColor?.SetValue(player.Game.GameShader.FogColor);
                _fogStart?.SetValue(player.Game.GameShader.FogStart);
                _fogEnd?.SetValue(player.Game.GameShader.FogEnd);
                _skyStart?.SetValue(player.FarClip * 0.85f);
                _skyEnd?.SetValue(player.FarClip);
            }

            UpdateBuffer(group);

            device.RasterizerState = group.Material.RasterizerState;
            device.BlendState = group.Material.BlendState;
            _effect.CurrentTechnique = group.Technique;
            foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _bindings[0] = new VertexBufferBinding(_vertexBuffer, 0, 0);
                _bindings[1] = new VertexBufferBinding(_instanceBuffer, 0, 1);
                device.SetVertexBuffers(_bindings);
                device.Indices = _indexBuffer;
                device.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, 6, group.InstanceCount);
            }
        }

        /// <summary>
        /// Gets the number of bytes used by the vertices and indices of each particle.
        /// </summary>
        /// <returns>The number of bytes used by each particle.</returns>
        public int GetVertexBytes()
        {
            int indexSize = _indexBuffer.IndexElementSize == IndexElementSize.SixteenBits ? 2 : 4;
            return (_vertexBuffer.VertexDeclaration.VertexStride * VerticesPerParticle) + (indexSize * IndicesPerParticle);
        }

        /// <summary>
        /// Gets the number of bytes in the vertex and index buffers.
        /// </summary>
        /// <returns>The number of bytes in the vertex and index buffers.</returns>
        public long GetBufferSize()
        {
            return _vertexBuffer.BufferSize() + _indexBuffer.BufferSize();
        }

        private void InitializeDrawGroups(Effect effect)
        {
            _alphaTest = new ParticleDrawGroup(effect, "AlphaTest", ParticleMaterial.AlphaTest);
            _alphaTestNoFog = new ParticleDrawGroup(effect, "AlphaTest_NoFog", ParticleMaterial.AlphaTestNoFog);
            _opaque = new ParticleDrawGroup(effect, "Opaque", ParticleMaterial.Opaque);
            _opaqueNoFog = new ParticleDrawGroup(effect, "Opaque_NoFog", ParticleMaterial.OpaqueNoFog);
            _alphaBlend = new ParticleDrawGroup(effect, "AlphaBlend", ParticleMaterial.AlphaBlend);
            _alphaBlendNoFog = new ParticleDrawGroup(effect, "AlphaBlend_NoFog", ParticleMaterial.AlphaBlendNoFog);
        }

        #region Dispose

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _vertexBuffer.Dispose();
                }

                _vertexBuffer = null;
                _particles = null;
                _disposedValue = true;
                // do not dispose _effect or Texture here, they may be used elsewhere
                _viewMatrix = null;
                _projectionMatrix = null;
                _worldMatrix = null;
                _fogColor = null;
                _fogStart = null;
                _fogEnd = null;
                _skyStart = null;
                _skyEnd = null;
                _cameraPos = null;
                _texture = null;
            }
        }

        /// <summary>
        /// Disposes this Particle Manager. The effect and texture are not disposed.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        /// Creates a new Particle Manager with the specified max particle count, and uses the specified Effect when drawing.
        /// </summary>
        /// <param name="world">The world this Particle Manager belongs to.</param>
        /// <param name="maxParticles">The max number of particles that can be active.</param>
        /// <param name="effect">The effect used when drawing active particles. This effect is not disposed when the Particle Manager is disposed.
        /// <para>This effect is passed the following:</para>
        /// <list type="bullet">
        /// <item>float4x4: View</item>
        /// <item>float4x4: Projection</item>
        /// <item>float4x4: World</item>
        /// <item>float3: CameraPos</item>
        /// <item>float3: FogColor</item>
        /// <item>float: FogStart</item>
        /// <item>float: FogEnd</item>
        /// <item>float: SkyStart</item>
        /// <item>float: SkyEnd</item>
        /// <item>texture3D: Tex</item>
        /// </list>
        /// </param>
        public ParticleManager(ICoreWorld world, int maxParticles, Effect effect, ParticleRegistry registry)
        {
            _particles = new ParticleInstance[maxParticles];
            _chunks = new ParticleChunk[maxParticles / ParticleChunk.Count];
            for (int i = 0; i < _particles.Length; i++)
            {
                _particles[i] = new ParticleInstance(i);
            }
            for (int i = 0; i < _chunks.Length; i++)
            {
                _chunks[i].Free = ParticleChunk.Count;
            }
            InitializeDrawGroups(effect);
            _world = world;
            ParticleVertex[] vertices = new ParticleVertex[4]
            {
                new(new Vector3(0.5f, -0.5f, 0)),
                new(new Vector3(0.5f, 0.5f, 0)),
                new(new Vector3(-0.5f, -0.5f, 0)),
                new(new Vector3(-0.5f, 0.5f, 0))
            };
            ushort[] indices = new ushort[6]
            {
                0, 1, 2,
                3, 2, 1
            };
            _vertexBuffer = new VertexBuffer(CoreGlobals.GraphicsDevice, typeof(ParticleVertex), 4, BufferUsage.WriteOnly);
            _vertexBuffer.SetData(vertices);
            _indexBuffer = new IndexBuffer(CoreGlobals.GraphicsDevice, IndexElementSize.SixteenBits, 6, BufferUsage.WriteOnly);
            _indexBuffer.SetData(indices);
            _bindings = new VertexBufferBinding[2];

            _instanceBuffer = new DynamicVertexBuffer(CoreGlobals.GraphicsDevice, typeof(ParticleInstanceVertex), ParticleDrawGroup.DefaultCount, BufferUsage.WriteOnly);
            _effect = effect;
            _viewMatrix = _effect.Parameters["View"];
            _projectionMatrix = _effect.Parameters["Projection"];
            _worldMatrix = _effect.Parameters["World"];
            _cameraPos = _effect.Parameters["CameraPos"];
            _fogColor = _effect.Parameters["FogColor"];
            _fogStart = _effect.Parameters["FogStart"];
            _fogEnd = _effect.Parameters["FogEnd"];
            _skyStart = _effect.Parameters["SkyStart"];
            _skyEnd = _effect.Parameters["SkyEnd"];
            _texture = _effect.Parameters["Tex"];
            _registry = registry;
        }
    }
}

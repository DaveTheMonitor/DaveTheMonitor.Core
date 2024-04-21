using DaveTheMonitor.Core.API;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DaveTheMonitor.Core.Particles
{
    public sealed class ParticleRegistry : DefinitionRegistry<ParticleDefinition>
    {
        public Texture3D Texture { get; private set; }

        protected override void OnRegister(ParticleDefinition definition)
        {
            definition.SetGame(Game);
            definition.SetParticleRegister(this);
        }

        /// <summary>
        /// Builds a texture atlas from the registered particles.
        /// </summary>
        /// <returns>The texture atlas.</returns>
        public Texture3D BuildTextureAtlas(out Dictionary<ParticleDefinition, ParticleTextureInfo> map)
        {
            map = new Dictionary<ParticleDefinition, ParticleTextureInfo>();
            List<Texture2D> textures = new List<Texture2D>();
            Dictionary<Texture2D, List<ParticleDefinition>> textureDefinitions = new Dictionary<Texture2D, List<ParticleDefinition>>();
            foreach (ParticleDefinition definition in GetAllDefinitions())
            {
                Texture2D texture = definition.Texture;
                if (texture != null)
                {
                    if (!textures.Contains(texture))
                    {
                        textures.Add(definition.Texture);
                    }

                    if (textureDefinitions.TryGetValue(texture, out List<ParticleDefinition> defs))
                    {
                        defs.Add(definition);
                    }
                    else
                    {
                        defs = new List<ParticleDefinition>();
                        defs.Add(definition);
                        textureDefinitions.Add(texture, defs);
                    }
                }
            }

            int[] sizes = new int[] { 8, 16, 32, 64, 128, 256, 512 };
            int[] maxTextures = new int[sizes.Length];
            int maxSize = sizes.Max();
            for (int i = 0; i < sizes.Length; i++)
            {
                maxTextures[i] = (maxSize / sizes[i]) * (maxSize / sizes[i]);
            }

            List<Texture2D>[] texturesBySize = GetTexturesBySize(textures, sizes);
            int depth = Math.Max(GetTextureDepth(texturesBySize, sizes, maxTextures), 1);
            int currentDepth = 0;

            Texture3D atlas = new Texture3D(CoreGlobals.GraphicsDevice, 512, 512, depth, false, SurfaceFormat.Color);
            List<(Texture2D, ParticleTextureInfo)> info = new List<(Texture2D, ParticleTextureInfo)>();
            for (int i = 0; i < texturesBySize.Length; i++)
            {
                if (texturesBySize[i].Count == 0)
                {
                    continue;
                }

                currentDepth += StitchAtlas(atlas, texturesBySize[i], currentDepth, i, sizes[i], maxTextures, out List<(Texture2D, ParticleTextureInfo)> o);
                info.AddRange(o);
            }

            foreach ((Texture2D, ParticleTextureInfo) item in info)
            {
                List<ParticleDefinition> list = textureDefinitions[item.Item1];
                foreach (ParticleDefinition def in list)
                {
                    map.Add(def, item.Item2);
                }
            }

            return atlas;
        }

        private List<Texture2D>[] GetTexturesBySize(List<Texture2D> textures, int[] sizes)
        {
            List<Texture2D>[] texturesList = new List<Texture2D>[sizes.Length];
            for (int i = 0; i < texturesList.Length; i++)
            {
                texturesList[i] = new List<Texture2D>();
            }

            foreach (Texture2D texture in textures)
            {
                int maxSize = sizes.Max();
                if (texture.Width > maxSize || texture.Height > maxSize)
                {
                    throw new InvalidOperationException("Particle texture must be 512x512 or smaller.");
                }

                int max = Math.Max(texture.Width, texture.Height);
                int index = Array.FindIndex(sizes, size => size >= max);
                int adjusted = sizes[index];
                texturesList[index].Add(texture);
            }

            return texturesList;
        }

        private int GetTextureDepth(List<Texture2D>[] textures, int[] sizes, int[] maxTextures)
        {
            int depth = 0;
            for (int i = 0; i < textures.Length; i++)
            {
                int count = textures[i].Count;
                if (count == 0)
                {
                    continue;
                }
                int max = maxTextures[i];

                depth += 1 + ((count - 1) / max);
            }
            return depth;
        }

        private int StitchAtlas(Texture3D atlas, List<Texture2D> list, int startDepth, int sizeIndex, int size, int[] maxTextures, out List<(Texture2D, ParticleTextureInfo)> info)
        {
            info = new List<(Texture2D, ParticleTextureInfo)>();
            int sectionSize = maxTextures[sizeIndex];
            int depthUsed = 1 + ((list.Count - 1) / sectionSize);
            if (depthUsed == 1)
            {
                StitchAtlas(atlas, list, startDepth, size, info);
            }
            else
            {
                for (int i = 0; i < depthUsed; i++)
                {
                    StitchAtlas(atlas, list.Take(new Range(i * sectionSize, Math.Min((i * sectionSize) + sectionSize, list.Count))), startDepth + i, size, info);
                }
            }
            return depthUsed;
        }

        private void StitchAtlas(Texture3D atlas, IEnumerable<Texture2D> textures, int depth, int size, List<(Texture2D, ParticleTextureInfo)> info)
        {
            int atlasWidth = atlas.Width;
            int i = 0;
            foreach (Texture2D texture in textures)
            {
                int x = (i % atlasWidth) * size;
                int y = ((int)Math.Floor(i / (double)atlasWidth)) * size;
                ParticleTextureInfo textureInfo = new ParticleTextureInfo(size, x, y, depth);
                info.Add((texture, textureInfo));

                Rectangle rect = new Rectangle(x, y, texture.Width, texture.Height);
                Color[] data = new Color[texture.Width * texture.Height];
                texture.GetData(data);

                atlas.SetData(0, rect.Left, rect.Top, rect.Right, rect.Bottom, depth, depth + 1, data, 0, data.Length);

                i++;
            }
        }

        /// <summary>
        /// Sets the texture used by this atlas. If previously used texture is not disposed.
        /// </summary>
        /// <param name="texture">The texture to use.</param>
        public void SetTexture(Texture3D texture)
        {
            Texture = texture;
        }

        public ParticleRegistry(ICoreGame game) : base(game, typeof(ParticleRegisterIgnoreAttribute))
        {
            
        }
    }
}

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raycaster
{
    internal static class ResourceRegistry
    {
        public static Dictionary<String, Texture2D> TEXTURES = new Dictionary<string, Texture2D>();

        public static void RegisterTexture(ContentManager Content, string texturePath)
        {
            TEXTURES.Add(texturePath, Content.Load<Texture2D>("Textures/" + texturePath));
        }

    }
}

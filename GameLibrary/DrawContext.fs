module DrawContext

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

type DrawContext =
        {
            SpriteBatch : SpriteBatch
            Asteroid    : Texture2D
            Ship        : Texture2D
            Star        : Texture2D
        }
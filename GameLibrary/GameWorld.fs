namespace AsteroidGame
module GameWorld =
    open DrawContext
    open Microsoft.Xna.Framework
    open Microsoft.Xna.Framework.Graphics
    open Common
    open Math
    open GameActors

    let dt = 0.016<s>


    type MyGameState = GameActors.World
    let initial_state = GameActors.World.Start
    let update (dt:float32<s>) (state:MyGameState) : MyGameState = GameActors.World.WorldUpdate state dt
    let draw (ctxt:DrawContext) (state:MyGameState) = GameActors.World.WorldDraw state ctxt
    let load_context (game : Game) =
        {
            SpriteBatch = new SpriteBatch(game.GraphicsDevice)
            Asteroid = game.Content.Load<Texture2D>("asteroid")
            Ship = game.Content.Load<Texture2D>("ship")
            Star = game.Content.Load<Texture2D>("star1")
            Bullet = game.Content.Load<Texture2D>("bullet")
            Target = game.Content.Load<Texture2D>("target")
        }
    let start_game() =
        new Common.Game2D<MyGameState>(initial_state, load_context, update, draw)
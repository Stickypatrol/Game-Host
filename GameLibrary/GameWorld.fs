namespace AsteroidGame
module GameWorld = 
    open DrawContext
    open Microsoft.Xna.Framework
    open Microsoft.Xna.Framework.Graphics
    open Common
    open Math

    let dt = 0.016<s>

    type MyGameState = GameActors.GameWorld
    let initial_state = GameActors.GameWorld.Start
    let update (dt:float32<s>) (state:MyGameState) : MyGameState = GameActors.WorldUpdate state dt
    let draw (ctxt:DrawContext) (state:MyGameState) = GameActors.WorldDraw state ctxt
    let load_context (game : Game) =
        {
            SpriteBatch = new SpriteBatch(game.GraphicsDevice)
            Asteroid = game.Content.Load<Texture2D>("asteroid")
            Ship = game.Content.Load<Texture2D>("ship")
            Star = game.Content.Load<Texture2D>("star1")
        }
    let start_game() =
        new Common.Game2D<MyGameState>(initial_state, load_context, update, draw)
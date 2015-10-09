namespace AsteroidGame
module GameWorld = 
    open DrawContext
    open Microsoft.Xna.Framework
    open Microsoft.Xna.Framework.Graphics
    open Common

    type MyGameState = GameActors.AsteroidShooterWorld
    let initial_state = GameActors.AsteroidShooterWorld.Inception
    let update (dt:float32) (state:MyGameState) : MyGameState =
        //apply everything to the state and then return it
        //state.Ships |> List.map actionstuff
        state
    let draw (ctxt:DrawContext) (state:MyGameState) =
        ()
    let load_context (game : Game) =    
        {
            SpriteBatch = new SpriteBatch(game.GraphicsDevice)
            Asteroid = game.Content.Load "asteroid"
        }
    let start_game() =
        new Common.Game2D<MyGameState>(initial_state, load_context, update, draw)
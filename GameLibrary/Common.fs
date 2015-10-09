namespace AsteroidGame
module Common =
    open System
    open System.Collections.Generic
    open Microsoft.Xna.Framework
    open Microsoft.Xna.Framework.Graphics
    open DrawContext
    open Math

    type Math.Vector2<[<Measure>] 'u > with
        member this.ToXNAVector = Vector2(this.X |> float32, this.Y |> float32)

    type Game2D<'s>(initial_state : 's, LoadContent : Game -> DrawContext, update : float32 -> 's -> 's, draw : DrawContext -> 's -> unit) as this =
        inherit Game()

        [<DefaultValue>]
        val mutable draw_context : DrawContext
        let mutable state = GameWorld.initial_state
        let graphics = new GraphicsDeviceManager(this)
        do graphics.GraphicsProfile <- GraphicsProfile.HiDef
        override this.LoadContent() =
            this.draw_context <- LoadContent this
            base.LoadContent()
        override this.Update gt =  
            state <- GameWorld.update (float32 gt.ElapsedGameTime.TotalSeconds) state
            base.Update gt
        override this.Draw gt =
            this.GraphicsDevice.Clear(Color.Black)
            do draw this.draw_context state
            base.Draw gt
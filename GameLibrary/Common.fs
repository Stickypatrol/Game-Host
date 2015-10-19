namespace AsteroidGame
module Common =
    open System
    open System.Collections.Generic
    open Microsoft.Xna.Framework
    open Microsoft.Xna.Framework.Graphics
    open DrawContext
    open Math




    type Math.Vector2<[<Measure>] 'u > with
        member this.ToXNAVector = Vector2(this.X |> float32 , this.Y |> float32)

    type Game2D<'s>(initial_state : 's, LoadContent : Game -> DrawContext, update : float32<s> -> 's -> 's, draw : DrawContext -> 's -> unit) as this =
        inherit Game()

        [<DefaultValue>]
        val mutable draw_context : DrawContext
        let mutable state = initial_state
        let graphics = new GraphicsDeviceManager(this)
        do graphics.GraphicsProfile <- GraphicsProfile.HiDef
        do base.Content.RootDirectory <- "Content"
        override this.LoadContent() =
            this.draw_context <- LoadContent this
            base.LoadContent()
        override this.Update gt =  
            state <- update ((float32 gt.ElapsedGameTime.TotalSeconds) * 1.0f<s>) state
            base.Update gt
        override this.Draw gt =
            this.GraphicsDevice.Clear(Color.Black)
            do this.draw_context.SpriteBatch.Begin()
            do draw this.draw_context state
            do this.draw_context.SpriteBatch.End()
            base.Draw gt
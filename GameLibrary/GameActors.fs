namespace AsteroidGame
module GameActors =
    open Microsoft.Xna.Framework.Graphics
    open Microsoft.Xna.Framework.Input
    open DrawContext
    open Common
    open Math
    open GameInput
    
    type Motion =
        static member Slow (x : float32<_>) =
            match x with
            | x when x > 5.0f<_> -> x - 5.0f<_>
            | x when x < -5.0f<_> -> x + 5.0f<_>
            | _ -> 0.0f<_>
        static member Cap (x : float32<_>) (c : float32<_>) =
            match x with
            | x when x > c -> c
            | x when x < -c -> -c
            | _ -> x
        static member Apply (v : Vector2<_>, b : Vector2<_>) (c : float32<_>) =
            match (v, b) with
            | (v, b) when b.X = 0.0f<_> && b.Y = 0.0f<_> -> {X = Motion.Slow v.X; Y = Motion.Slow v.Y}
            | (v, b) when b.X = 0.0f<_> -> {X = Motion.Slow v.X; Y = Motion.Cap (v.Y + b.Y) c}
            | (v, b) when b.Y = 0.0f<_> -> {X = Motion.Cap (v.X + b.X) c; Y = Motion.Slow v.Y}
            | _ -> {X = Motion.Cap (v.X + b.X) c; Y = Motion.Cap (v.Y + b.Y) c}
        
    type Ship =
        {
            Position    : Vector2<m>
            Velocity    : Vector2<m/s>
            InputBehavior : InputBehavior<Ship>
        }with
            member this.Update dt =
               let this = UpdateInput this.InputBehavior this (curKeyboard())
               { this with Position = this.Position + this.Velocity * dt }
            member this.Draw (d_c : DrawContext) =
               d_c.SpriteBatch.Draw(d_c.Ship, this.Position.ToXNAVector, Microsoft.Xna.Framework.Color.White)
            static member Zero =
                let move x_v y_v (ship:Ship) = 
                    { ship with Velocity = Motion.Apply (ship.Velocity, {X = x_v;Y = y_v}) 200.0f<m/s> }
                {
                    Position = {X = 380.0f<m>; Y = 200.0f<m>};
                    Velocity = Vector2.Zero
                    InputBehavior =
                        [
                            (Keys.W,None), move 0.0f<m/s> -10.0f<m/s>;
                            (Keys.S,None), move 0.0f<m/s> -10.0f<m/s>;
                            (Keys.Space,Some Keys.LeftShift), move 0.0f<m/s> -10.0f<m/s>;
                        ] |> Map.ofList
                }
    type Asteroid =
        {
            Position    : Vector2<m>
            Velocity    : Vector2<m/s>
        }with
            member this.Update (dt : float32<s>) =
               { this with Position = this.Position + this.Velocity * dt }
            member this.Draw (d_c : DrawContext) =
                d_c.SpriteBatch.Draw(d_c.Ship, this.Position.ToXNAVector, Microsoft.Xna.Framework.Color.White)
            static member Zero =
                            {
                                Position = Vector2.Zero
                                Velocity = Vector2.Zero
                            }
    type Bullet =
        {
            Position    : Vector2<m>
            Velocity    : Vector2<m/s>
        }with
            member this.Update (dt : float32<s>) =
               { this with Position = this.Position + this.Velocity * dt }
            member this.Draw (d_c : DrawContext) = ()
            static member Zero =
                            {
                                Position = Vector2.Zero
                                Velocity = Vector2.Zero
                            }
    type Star =
        {
            Position    : Vector2<m>
            Velocity    : Vector2<m/s>
        }with
            member this.Update (dt : float32<s>) =
                {this with Position = this.Position + this.Velocity * dt }
            member this.Draw (d_c : DrawContext) = ()
            static member Zero =
                            {
                                Position = Vector2.Zero//randomize location here
                                Velocity = Vector2.Zero//use mirrored "velocity of ship"
                            }
    type GameWorld =
        {
            Ship : Ship;
            Asteroids : Asteroid list
            Bullets : Bullet list
            Stars : Star List
        }with
            static member Start =
                                {
                                Ship =          Ship.Zero;
                                Asteroids =     []
                                Bullets =       []
                                Stars =         []
                                }

    let WorldUpdate (world : GameWorld) (dt : float32<s>) =
        {
        Ship =          world.Ship.Update dt
        Asteroids =     List.map (fun (a : Asteroid) -> a.Update dt) world.Asteroids
        Bullets =       List.map (fun (a : Bullet) -> a.Update dt) world.Bullets
        Stars =         List.map (fun (a : Star) -> a.Update dt) world.Stars
        }

    let WorldDraw (world : GameWorld) (d_c : DrawContext) =
        world.Ship.Draw d_c
        List.iter (fun (a : Asteroid) -> a.Draw d_c) world.Asteroids
        List.iter (fun (a : Bullet) -> a.Draw d_c) world.Bullets
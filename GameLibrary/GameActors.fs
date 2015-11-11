namespace AsteroidGame
module GameActors =
    open Microsoft.Xna.Framework.Graphics
    open Microsoft.Xna.Framework.Input
    open DrawContext
    open Common
    open Math
    open GameInput

   (* type Timer = Wait of float32
    type Script = 
      | CreateEnemiesFirstWave of int * Timer
      | CreateFirstBoss
      | WaitSecondWave of float32
    type Action =
      | CreateEnemy
      | CreateBoss
      | Nothing

    let run_script dt s =
      match s with
      | CreateEnemiesFirstWave(0,t) ->
        Nothing, CreateFirstBoss
      | CreateEnemiesFirstWave(n,Wait(t)) when t > 0.0f ->
        Nothing, CreateEnemiesFirstWave(n,Wait(t - dt))
      | CreateEnemiesFirstWave(n,Wait(t)) ->
        CreateEnemy, CreateEnemiesFirstWave(n-1,Wait(0.5f))
      | ...*)
        

    let rand = System.Random()
    let curKeyboard () = Keyboard.GetState()
    let curMouse () = Mouse.GetState()
    let timer  = System.Diagnostics.Stopwatch.StartNew()
    
    type Controller =
        {
            BaseTime      : int64
            Time          : int64
            Level         : int
            LevelTimeLimit: int64
            IsTimeRunning : bool
        }with
            static member Update (this : Controller) =
                {
                this with
                    Time = timer.ElapsedMilliseconds - this.BaseTime
                    Level = if this.Time >= this.LevelTimeLimit then  timer.Restart()
                                                                      this.Level + 1
                                                                else  this.Level
                    LevelTimeLimit = if this.Level % 2 = 0 then 20000L else 10000L
                    IsTimeRunning = if this.Level = 6 then false else true
                }
            static member Zero =
                {
                    BaseTime = timer.ElapsedMilliseconds
                    Time = 0L
                    Level = -1
                    LevelTimeLimit = 5L
                    IsTimeRunning = true
                }

    type Ship =
        {
            Position      : Vector2<m>
            Velocity      : Vector2<m/s>
            Dimensions    : Vector2<m>
            InputBehavior : InputBehavior<Ship>
            Health        : int
        }with
            member this.Draw (d_c : DrawContext) =
                d_c.SpriteBatch.Draw(d_c.Ship, this.Position.ToXNAVector, Microsoft.Xna.Framework.Color.White)
            static member Update this (dt : float32<s>) =
                let y = UpdateInput this.InputBehavior this  (KeyboardState(curKeyboard()))
                let z = UpdateInput y.InputBehavior y  (MouseState(curMouse()))
                { z with Position = z.Position + (z.Velocity * dt) }
            static member Remove this w =
                false
            static member Zero =
                let Move x_v y_v (ship:Ship) =
                  {ship with Velocity = ship.Velocity + {X = x_v; Y = y_v}}
                {
                    Position = {X = 375.0f<m>; Y = 215.0f<m>};
                    Velocity = Vector2.Zero
                    Dimensions = Vector2<m>.Zero
                    InputBehavior =
                        [
                        ] |> Map.ofList
                    Health = 0
                }
            static member LoadSize (ship : Ship) (d_c : DrawContext) =
                {ship with Dimensions = {X = (d_c.Ship.Width |> float32) * 1.0f<m>; Y = (d_c.Ship.Height |> float32) * 1.0f<m>}}

    type Asteroid =
        {
            Position    : Vector2<m>
            Velocity    : Vector2<m/s>
            Dimensions    : Vector2<m>
            InputBehavior : InputBehavior<Asteroid>
        }with
            member this.Draw (d_c : DrawContext) =
                d_c.SpriteBatch.Draw(d_c.Asteroid, this.Position.ToXNAVector, Microsoft.Xna.Framework.Color.White)
            static member Update this (dt : float32<s>) =
              let y = UpdateInput this.InputBehavior this (KeyboardState(curKeyboard()))
              { y with Position = y.Position + y.Velocity * dt }
            static member Create () =
              []
            static member Remove this w =
              match (this.Position.X, this.Position.Y) with
              | (x, y) when x > 900.0f<m> ->  true
              | (x, y) when x < -100.0f<m> -> true
              | (x, y) when y > 580.0f<m> ->  true
              | (x, y) when y < -100.0f<m> -> true
              | (x, y) -> false
            static member New (pos : Vector2<m>) (vel : Vector2<m/s>) (dim : Vector2<m>) (i_b : InputBehavior<Asteroid>) =
              let Move x_v y_v (asteroid:Asteroid) =
                {asteroid with Velocity = asteroid.Velocity + {X = x_v; Y = y_v}}
              {
                Position = pos
                Velocity = vel
                Dimensions = dim
                InputBehavior =
                    [
                        KeyboardInput(Keys.W,None), Move 0.0f<m/s> 5.0f<m/s>
                        KeyboardInput(Keys.S,None), Move 0.0f<m/s> -5.0f<m/s>
                        KeyboardInput(Keys.A,None), Move 5.0f<m/s> 0.0f<m/s>
                        KeyboardInput(Keys.D,None), Move -5.0f<m/s> 0.0f<m/s>
                    ] |> Map.ofList
              }
            static member Zero =
                let Move x_v y_v (asteroid:Asteroid) =
                  {asteroid with Velocity = asteroid.Velocity + {X = x_v; Y = y_v}}
                {
                    Position = {X = 120.0f<m>; Y = 100.0f<m>}
                    Velocity = Vector2.Zero
                    Dimensions = Vector2<m>.Zero
                    InputBehavior =
                        [
                            KeyboardInput(Keys.W,None), Move 0.0f<m/s> 5.0f<m/s>
                            KeyboardInput(Keys.S,None), Move 0.0f<m/s> -5.0f<m/s>
                            KeyboardInput(Keys.A,None), Move 5.0f<m/s> 0.0f<m/s>
                            KeyboardInput(Keys.D,None), Move -5.0f<m/s> 0.0f<m/s>
                        ] |> Map.ofList
                }
            static member LoadSize (asteroid : Asteroid) (d_c : DrawContext) =
                {asteroid with Dimensions = {X = (d_c.Asteroid.Width |> float32) * 1.0f<m>; Y = (d_c.Asteroid.Height |> float32) * 1.0f<m>}}

    type Bullet =
        {
            Position    : Vector2<m>
            Velocity    : Vector2<m/s>
            Dimensions    : Vector2<m>
            InputBehavior : InputBehavior<Bullet>
        }with
            member this.Draw (d_c : DrawContext) =
               d_c.SpriteBatch.Draw(d_c.Bullet, this.Position.ToXNAVector, Microsoft.Xna.Framework.Color.White)
            static member Update this (dt : float32<s>) =
              let y = UpdateInput this.InputBehavior this (KeyboardState(curKeyboard()))
              let z = UpdateInput y.InputBehavior y (MouseState(curMouse()))
              { z with Position = z.Position + (z.Velocity * dt)}
            static member Remove (this : Bullet) w =
              match (this.Position.X, this.Position.Y) with
              | (x, y) when x > 900.0f<m> ->      true
              | (x, y) when x < -100.0f<m> ->     true
              | (x, y) when y > 580.0f<m> ->      true
              | (x, y) when y < -100.0f<m> ->     true
              | (x, y) -> false
            static member Create () =
              let m_i = curMouse()
              if m_i.LeftButton = ButtonState.Pressed then 
                                          let Move x_v y_v (bullet:Bullet) =
                                            {bullet with Velocity = Vector2.Zero}
                                          [{Bullet.Zero with
                                                Position = {X = 393.5f<m>;Y = 233.5f<m>};
                                                Velocity = Bullet.BulletDirection()}] else []
            
            static member BulletDirection () = 
              let mousePos = {X = (curMouse().Position.X |> float32) * 1.0f<m/s>; Y = (curMouse().Position.Y |> float32) * 1.0f<m/s>}
              let angle = (cos(atan((mousePos.Y - 240.0f<m/s>)/(mousePos.X - 400.0f<m/s>))))
              let position = {X = angle * 400.0f<m/s>;Y = (sqrt((400.0f * 400.0f) - (angle * 400.0f) * abs(angle * 400.0f)) * 1.0f<m/s>)}
              match ((mousePos.X - 400.0f<m/s>), (mousePos.Y - 240.0f<m/s>)) with
              | (x,y) when x >= 0.0f<m/s> && y >= 0.0f<m/s> ->  {X = position.X;Y = position.Y}
              | (x,y) when x >= 0.0f<m/s> && y < 0.0f<m/s> ->   {X = position.X;Y = -position.Y}
              | (x,y) when x < 0.0f<m/s> && y >= 0.0f<m/s> ->   {X = -position.X;Y = position.Y}
              | (x,y) ->                                    {X = -position.X;Y = -position.Y}
            static member New (pos : Vector2<m>) (vel : Vector2<m/s>) (dim : Vector2<m>) (i_b : InputBehavior<Bullet>) =
              let Move x_v y_v (bullet:Bullet) =
                {bullet with Velocity = bullet.Velocity + {X = x_v; Y = y_v}}
              {
                Position = pos
                Velocity = vel
                Dimensions = dim
                InputBehavior =
                    [
                        KeyboardInput(Keys.W,None), Move 0.0f<m/s> 5.0f<m/s>
                        KeyboardInput(Keys.S,None), Move 0.0f<m/s> -5.0f<m/s>
                        KeyboardInput(Keys.A,None), Move 5.0f<m/s> 0.0f<m/s>
                        KeyboardInput(Keys.D,None), Move -5.0f<m/s> 0.0f<m/s>
                    ] |> Map.ofList
              }
            static member Zero =
              let Move x_v y_v (bullet:Bullet) =
                {bullet with Velocity = bullet.Velocity + {X = x_v; Y = y_v}}
              {
                Position = Vector2.Zero
                Velocity = Vector2.Zero
                Dimensions = Vector2<m>.Zero
                InputBehavior =
                    [
                        KeyboardInput(Keys.W,None), Move 0.0f<m/s> 10.0f<m/s>
                        KeyboardInput(Keys.S,None), Move 0.0f<m/s> -10.0f<m/s>
                        KeyboardInput(Keys.A,None), Move 10.0f<m/s> 0.0f<m/s>
                        KeyboardInput(Keys.D,None), Move -1.0f<m/s> 0.0f<m/s>
                    ] |> Map.ofList
              }
            static member LoadSize (this : Bullet) (d_c : DrawContext) =
              {this with Dimensions = {X = (d_c.Bullet.Width |> float32) * 1.0f<m>; Y = (d_c.Bullet.Height |> float32) * 1.0f<m>}}


    type Star =
        {
            Position    : Vector2<m>
            Velocity    : Vector2<m/s>
            Dimensions    : Vector2<m>
            InputBehavior : InputBehavior<Star>
        }with
            member this.Draw (d_c : DrawContext) =
               d_c.SpriteBatch.Draw(d_c.Star, this.Position.ToXNAVector, Microsoft.Xna.Framework.Color.White)
            static member Update this (dt : float32<s>) =
              let Clamp (star : Star)=
                  let x_d = star.Dimensions.X
                  let y_d = star.Dimensions.Y
                  match (star.Position.X, star.Position.Y) with
                  | (x, y) when x > 800.0f<m> -> {star with Position = {X = -x_d; Y = 480.0f<m> * (rand.NextDouble() |> float32)}}
                  | (x, y) when x + x_d < 0.0f<m> -> {star with Position = {X = 800.0f<m>; Y = 480.0f<m> * (rand.NextDouble() |> float32)}}
                  | (x, y) when y > 480.0f<m> -> {star with Position = {X = 800.0f<m> * (rand.NextDouble() |> float32); Y = -y_d}}
                  | (x, y) when y + y_d < 0.0f<m> -> {star with Position = {X = 800.0f<m> * (rand.NextDouble() |> float32); Y = 480.0f<m>}}
                  | (_,_) -> star
              let y = UpdateInput this.InputBehavior this (KeyboardState(curKeyboard()))
              let z = UpdateInput y.InputBehavior y (MouseState(curMouse()))
              let w = Clamp z
              { w with Position = w.Position + w.Velocity * dt }
            static member Remove this w =
              false
            static member Zero =
                let Move x_v y_v (star:Star) =
                  {star with Velocity = star.Velocity + {X = x_v; Y = y_v}}
                {
                    Position = {X = 800.0f<m>*(rand.NextDouble() |> float32) ; Y = 480.0f<m>*(rand.NextDouble() |> float32)}
                    Velocity = Vector2.Zero
                    Dimensions = Vector2.Zero
                    InputBehavior =
                        [
                            KeyboardInput(Keys.W,None), Move 0.0f<m/s> 2.0f<m/s>
                            KeyboardInput(Keys.S,None), Move 0.0f<m/s> -2.0f<m/s>
                            KeyboardInput(Keys.A,None), Move 2.0f<m/s> 0.0f<m/s>
                            KeyboardInput(Keys.D,None), Move -2.0f<m/s> 0.0f<m/s>
                        ] |> Map.ofList
                }
            static member LoadSize (star : Star) (d_c : DrawContext) =
                {star with Dimensions = {X = (d_c.Star.Width |> float32) * 1.0f<m>; Y = (d_c.Star.Height |> float32) * 1.0f<m>}}

    type Target = 
      {
        Position      : Vector2<m>
        Velocity      : Vector2<m/s>
        Dimensions    : Vector2<m>
        InputBehavior : InputBehavior<Star>
      }with
          member this.Draw (d_c : DrawContext) =
            d_c.SpriteBatch.Draw(d_c.Target, this.Position.ToXNAVector, Microsoft.Xna.Framework.Color.White)
          static member TargetPosition () = 
            let mousePos = {X = (curMouse().Position.X |> float32) * 1.0f<m>; Y = (curMouse().Position.Y |> float32) * 1.0f<m>}
            let angle = (cos(atan((mousePos.Y - 240.0f<m>)/(mousePos.X - 400.0f<m>))))
            let position = {X = angle * 75.0f<m>;Y = (sqrt((75.0f * 75.0f) - (angle * 75.0f) * abs(angle * 75.0f)) * 1.0f<m>)}
            match ((mousePos.X - 400.0f<m>), (mousePos.Y - 240.0f<m>)) with
            | (x,y) when x >= 0.0f<m> && y >= 0.0f<m> ->  {X = position.X + 393.5f<m>;Y = position.Y + 233.5f<m>}
            | (x,y) when x >= 0.0f<m> && y < 0.0f<m> ->   {X = position.X + 393.5f<m>;Y = -position.Y + 233.5f<m>}
            | (x,y) when x < 0.0f<m> && y >= 0.0f<m> ->   {X = -position.X + 393.5f<m>;Y = position.Y + 233.5f<m>}
            | (x,y) ->                                    {X = -position.X + 393.5f<m>;Y = -position.Y + 233.5f<m>}
          static member Update (this : Target) (dt : float32<s>) =
            {this with Position = Target.TargetPosition ()}
          static member Remove this w =
            false
          static member Zero =
            {
                Position = Vector2.Zero
                Velocity = Vector2.Zero
                Dimensions = Vector2<m>.Zero
                InputBehavior =
                  [
                  ] |> Map.ofList
            }
          static member LoadSize (target : Target) (d_c : DrawContext) =
                {target with Dimensions = {X = (d_c.Target.Width |> float32) * 1.0f<m>; Y = (d_c.Target.Height |> float32) * 1.0f<m>}}

    type Entities<'e, 'w> =
        {
          EntityList          : 'e list
          LoadSize            : 'e -> DrawContext -> 'e
          Update              : 'e -> float32<s> -> 'e
          Remove              : 'e -> 'w -> bool
          Create              : unit -> 'e list
        }with
              static member UpdateAll (w : 'w) (dt : float32<s>) (es : Entities<'e,'w>) =
                              {
                                es with
                                  EntityList = es.Create() @
                                                    [for e in es.EntityList do
                                                      if not (es.Remove e w) then
                                                        let e' = es.Update e dt
                                                        yield e']
                              }
              static member LoadSizeAll (w : 'w) (d_c : DrawContext) (es : Entities<'e,'w>) =
                              {
                                es with
                                  EntityList = [for e in es.EntityList do
                                                  let e' = es.LoadSize e d_c 
                                                  yield e']
                              }
    
    
    type World =
        {
            Ships     : Entities<Ship, World>
            Asteroids : Entities<Asteroid, World>
            Bullets   : Entities<Bullet, World>
            Stars     : Entities<Star, World>
            Targets   : Entities<Target, World>
            Control   : Controller
        }with
            static member Start =
                                {
                                Ships =
                                  {
                                    EntityList = [Ship.Zero]
                                    LoadSize = Ship.LoadSize
                                    Update = Ship.Update
                                    Remove = Ship.Remove
                                    Create = (fun () -> [])
                                  }
                                Asteroids =
                                  {
                                    EntityList = [Asteroid.Zero]
                                    LoadSize = Asteroid.LoadSize
                                    Update = Asteroid.Update
                                    Remove = Asteroid.Remove
                                    Create = (fun () ->[] )
                                  }
                                Bullets =
                                  {
                                    EntityList = []
                                    LoadSize = Bullet.LoadSize
                                    Update = Bullet.Update
                                    Remove = Bullet.Remove
                                    Create = Bullet.Create
                                  }
                                Stars = 
                                  {
                                    EntityList = [for x in 1 .. 15 -> Star.Zero]
                                    LoadSize = Star.LoadSize
                                    Update = Star.Update
                                    Remove = Star.Remove
                                    Create = (fun () -> [])
                                  }
                                Targets =
                                  {
                                    EntityList = [Target.Zero]
                                    LoadSize = Target.LoadSize
                                    Update = Target.Update
                                    Remove = Target.Remove
                                    Create = (fun () -> [])
                                  }
                                Control = Controller.Zero
                                }
            static member WorldUpdate (world : World) (dt : float32<s>) =
              {
                Ships = Entities.UpdateAll world dt world.Ships
                Asteroids = Entities.UpdateAll world dt world.Asteroids
                Bullets = Entities.UpdateAll world dt world.Bullets
                Stars = Entities.UpdateAll world dt world.Stars
                Targets = Entities.UpdateAll world dt world.Targets
                Control = Controller.Update world.Control
              }
            static member DimensionLoad (world : World) (d_c : DrawContext) =
              {
                world with
                  Ships = Entities.LoadSizeAll world d_c world.Ships
                  Asteroids = Entities.LoadSizeAll world d_c world.Asteroids
                  Bullets = Entities.LoadSizeAll world d_c world.Bullets
                  Stars = Entities.LoadSizeAll world d_c world.Stars
                  Targets = Entities.LoadSizeAll world d_c world.Targets
              }
            static member WorldDraw (world : World) (d_c : DrawContext) =
              List.iter (fun (a : Star) -> a.Draw d_c) world.Stars.EntityList
              List.iter (fun (a : Ship) -> a.Draw d_c) world.Ships.EntityList
              List.iter (fun (a : Bullet) -> a.Draw d_c) world.Bullets.EntityList
              List.iter (fun (a : Asteroid) -> a.Draw d_c) world.Asteroids.EntityList
              List.iter (fun (a : Target) -> a.Draw d_c) world.Targets.EntityList

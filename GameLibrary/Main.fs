namespace GameLogic
module Math = 
    [<Measure>]
    type m
    [<Measure>]
    type kg
    [<Measure>]
    type s
    [<Measure>]
    type N = kg*m/s^2

    type Vector2<[<Measure>] 'a> = {X: float<'a>; Y: float<'a>} 
        with
        static member Zero : Vector2<'a> = { X = 0.0<_>; Y = 0.0<_> }
        static member (+) (v1:Vector2<'a>, v2:Vector2<'a>):Vector2<'a> = {X = v1.X + v2.X; Y = v1.Y + v2.Y}
        static member (+) (v:Vector2<'a>, k:float<'a>):Vector2<'a> =
            {X = v.X + k; Y = v.Y + k}
        static member (+) (k:float<'a>,v:Vector2<'a>):Vector2<'a> =
            v + k
        static member (~-) (v: Vector2<'a>):Vector2<'a> = 
            {X = -v.X;Y = -v.Y}
        static member (-) (v1:Vector2<'a>, v2:Vector2<'a>):Vector2<'a> =
            {X = v1.X - v2.X; Y = v1.Y - v2.Y}
        static member (-) (v:Vector2<'a>, k:float<'a>):Vector2<'a> =
            v+(-k)
        static member (-) (k:float<'a>,v:Vector2<'a>):Vector2<'a> =
            k+(-v)
        static member (*) (v1:Vector2<'a>, v2:Vector2<'b>):Vector2<'a*'b> =
            {X = v1.X * v2.X; Y = v1.Y * v2.Y}
        static member (*) (v:Vector2<'a>, f:float<'b>):Vector2<'a * 'b> =
            {X = v.X * f; Y = v.Y * f}
        static member (*) (f:float<'b>,v:Vector2<'a>):Vector2<'b*'a> =
            {X = v.X * f; Y = v.Y * f}
        static member (/) 
            (v:Vector2<'a>, f:float<'b>):Vector2<'a/'b> = 
            v*(1.0/f)
        static member (/) 
            (f:float<'b>, v:Vector2<'a>):Vector2<'b/'a> = 
            v*(1.0/f)
        member this.Length : float<'a> = 
            sqrt((this.X*this.X+this.Y*this.Y))
        static member Distance(v1:Vector2<'a>, v2:Vector2<'a>) =
            (v1-v2).Length
        static member Normalize(v:Vector2<'a>) :Vector2<1> =
            {X = v.X/v.Length; Y = v.Y/v.Length}

module SmallAsteroidFieldSimulation =
    open System
    open System.Threading
    open Math
    
    type Asteroid = 
        {
        Position : Vector2<m>
        Velocity : Vector2<m/s>
        Mass : float<kg>
        Name : string
        }
    let dt = 60.2<s>
    let G = 6.67e-11<m^3*kg^-1*s^-2>

    let earth_radius = 6.37e6<m>
    let field_size = earth_radius*60.0
    let max_velocity = 2.3e4<m/s>
    let earth_mass = 5.97e24<kg>
    let moon_mass = 7.35e22<kg>

    let create_field num_asteroids =
        let lerp (x: float<'u>) (y:float<'u>) (a:float) =
            x*a+y*(1.0-a)

        let rand = new Random()

        [for i =1 to num_asteroids do
            let m =
                (lerp earth_mass moon_mass (rand.NextDouble()))*
                1.0e-4
            let x = lerp 0.0<m> field_size (rand.NextDouble())
            let y = lerp 0.0<m> field_size (rand.NextDouble())
            let vx = max_velocity*(rand.NextDouble()*2.0-1.0)*0.1
            let vy = max_velocity*(rand.NextDouble()*2.0-1.0)*0.1
            yield
                {
                Position = { X = x; Y = y }
                Velocity = { X = vx; Y = vy }
                Mass = m
                Name = "a"
                }
        ]
    let f0 = create_field 20
    let clamp (p:Vector2<_>, v:Vector2<_>) = 
        let p,v = 
            if p.X < 0.0<_> then
                {p with X = 0.0<_>},{v with X = -v.X}
            else p,v
        let p,v = 
            if p.X > field_size then
                {p with X = field_size},{v with X = -v.X}
            else p,v
        let p,v = 
            if p.Y < 0.0<_> then
                {p with Y = 0.0<_>},{v with Y = -v.Y}
            else p,v
        let p,v = 
            if p.Y > field_size then
                {p with Y = field_size},{v with X = -v.Y}
            else p,v    
        p,v
    let force (a:Asteroid, a':Asteroid) =
        let dir = a'.Position-a.Position
        let dist = dir.Length+1.0<m>
        G*a.Mass*a'.Mass*dir/(dist*dist*dist)
    let simulation_step (asteroids:Asteroid list) =
        [
        for a in asteroids do
            let forces =
                [
                for a' in asteroids do 
                    if a' <> a then
                        yield force(a, a')
                ]
            let F = List.sum forces
            let p',v' = clamp(a.Position,a.Velocity)
            yield
                {
                    a with
                        Position = p'+dt*v'
                        Velocity = v' + dt * F / a.Mass
                }
        ]
    let print_scene (asteroids:Asteroid list) =
        do Console.Clear()
        let mutable buffer = ""
        for i = 0 to 79 do
            for j = 0 to 23 do
                if i = 0 || j = 0 || i = 23 || j = 79 then
                    do buffer <- buffer + ("*")
            buffer <- buffer + "\n"
        let set_cursor_on_body b = 
            Console.SetCursorPosition(
                (b.Position.X/4.0e8<m>*78.0+1.0) |> int,
                (b.Position.Y/4.0e8<m>*23.0+1.0) |> int
                )
        for a in asteroids do
            do set_cursor_on_body a
            do Console.Write(a.Name)
        do Thread.Sleep(5)

    let simulation() =
        let rec simulation m =
            do print_scene m
            let m' = simulation_step m
            do simulation m'
        do simulation f0

module Common =
    // OTHER FILE!!!!
    open System
    open System.Collections.Generic
    open Microsoft.Xna.Framework
    open Microsoft.Xna.Framework.Graphics

    type Math.Vector2<[<Measure>] 'u > with
        member this.ToXNAVector = Vector2(this.X |> float32, this.Y |> float32)

    type DrawContext =
        {
            SpriteBatch : SpriteBatch
            Asteroid    : Texture2D
        }

    type Game2D<'s>(initial_state : 's, update : float32 -> 's -> 's, draw : DrawContext -> 's -> unit) as this =
        inherit Game()

        [<DefaultValue>]
        val mutable draw_context : DrawContext
        let mutable state = initial_state
        let graphics = new GraphicsDeviceManager(this)
        do graphics.GraphicsProfile <- GraphicsProfile.HiDef
        override this.LoadContent() =
            this.draw_context <- 
                {
                    SpriteBatch = new SpriteBatch(this.GraphicsDevice)
                    Asteroid = this.Content.Load "asteroid"
                }
            base.LoadContent()
        override this.Update gt =  
            state <- update (float32 gt.ElapsedGameTime.TotalSeconds) state
            base.Update gt
        override this.Draw gt =
            this.GraphicsDevice.Clear(Color.Black)
            do draw this.draw_context state
            base.Draw gt


module ActualGame1 =
    // OTHER FILE(S)!!!!
    type MyGameState = { Ships : List<Math.Vector2<Math.m>> }

    let initial_state = { Ships = [] }

    let update (dt:float32) (state:MyGameState) : MyGameState =
        failwith ""

    let draw (ctxt:Common.DrawContext) (state:MyGameState) =
        ()

    let start_game() =
        new Common.Game2D<MyGameState>(initial_state, update, draw)


module ActualGame2 =
    // OTHER FILE(S)!!!!
    type MyGameState = { Ships : List<Math.Vector2<Math.m>> }

    let initial_state = { Ships = [] }

    let update (dt:float32) (state:MyGameState) : MyGameState =
        failwith ""

    let draw (ctxt:Common.DrawContext) (state:MyGameState) =
        ()

    let start_game() =
        new Common.Game2D<MyGameState>(initial_state, update, draw)

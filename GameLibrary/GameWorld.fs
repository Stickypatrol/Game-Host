namespace AsteroidGame
module GameWorld = 
    type MyGameState = { Ships : List<Math.Vector2<Math.m>> }
    let initial_state = { Ships = [] }
    let update (dt:float32) (state:MyGameState) : MyGameState =
        printfn "updating"
        failwith ""
        state
        //update shit here
    let draw (ctxt:Common.DrawContext) (state:MyGameState) =
        ()
        //draw shit here
    let start_game() =
        new Common.Game2D<MyGameState>(initial_state, update, draw)
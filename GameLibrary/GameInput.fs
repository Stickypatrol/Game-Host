namespace AsteroidGame
module GameInput =
    open System
    open Common
    open Math
    open Microsoft.Xna.Framework.Input

    type KeyCombo = Keys * Option<Keys>    
    type InputBehavior<'a> = Map<KeyCombo, 'a->'a>

    let compose (c:bool) (f:'a->'a) (g:'a->'a) : 'a->'a =
        if c then
            fun x -> g(f(x))
        else
            g    

    let UpdateInput (i_b : InputBehavior<'a>) (elem : 'a) (k_s : KeyboardState) =
        compose (k_s.IsKeyDown(Keys.W))
                (i_b.[Keys.W, None]) // f_1
                (compose (k_s.IsKeyDown(Keys.S)) // g_1
                         (i_b.[Keys.S, None]) // f_2
                         (compose (k_s.IsKeyDown(Keys.Space) && k_s.IsKeyDown(Keys.LeftShift)) // g_2
                         i_b.[Keys.S, Some Keys.LeftShift] // f_3
                         (fun x -> x))) // g_3
                         elem
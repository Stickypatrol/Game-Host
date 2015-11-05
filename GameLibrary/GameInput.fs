namespace AsteroidGame
module GameInput =
  open System
  open Common
  open Math
  open Microsoft.Xna.Framework.Input
  
  
  type InputState =
    | KeyboardState of KeyboardState
    | MouseState of MouseState

  type MouseButtons =
    | LeftButton = 0
    | RightButton = 1
    | MiddleButton = 2

  type KeyCombo = Keys * Option<Keys>
  type MouseCombo = MouseButtons * Option<MouseButtons>

  type ButtonStates =
  | KeyboardInput of KeyCombo
  | MouseInput of MouseCombo

  type InputBehavior<'a> = Map<ButtonStates, 'a->'a>

  let compose (c:bool) ((fs:Map<'k,'a->'a>),k:'k) (g:'a->'a) : 'a->'a =
    if c then
      match fs |> Map.tryFind k with
      | Some(f) ->
        fun x -> g(f(x))
      | None -> g
    else
      g

  let (?) c (fs,k) = fun g -> compose c (fs,k) g

  let UpdateInput (i_b : InputBehavior<'a>) (elem : 'a) (i_s : InputState) : 'a =
    match i_s with
    | KeyboardState(k_s) ->
        k_s.IsKeyDown(Keys.W) ? (i_b,KeyboardInput(Keys.W, None))
          (k_s.IsKeyDown(Keys.S) ? (i_b,KeyboardInput(Keys.S, None))
          ((k_s.IsKeyDown(Keys.A)) ? (i_b,KeyboardInput(Keys.A, None))
          ((k_s.IsKeyDown(Keys.D)) ? (i_b,KeyboardInput(Keys.D, None))
          (fun x -> x))))
          elem
    | MouseState(m_i) ->
        (m_i.LeftButton = ButtonState.Pressed) ? (i_b,MouseInput(MouseButtons.LeftButton, None))
          ((m_i.RightButton = ButtonState.Pressed) ? (i_b,MouseInput(MouseButtons.RightButton, None))
          (fun x -> x))
          elem
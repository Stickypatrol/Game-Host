namespace AsteroidGame
module Actor =

    open Microsoft.Xna.Framework
    open Microsoft.Xna.Framework.Graphics
    open Math

    type ActorType= 
    | Player
    | Asteroid
    | Bullet
    | Boss

    type WorldActor =
        {
            ActorType : ActorType;
            Position : Vector2<m>;
            Texture : Texture2D option;
        }

    let CreateActor(actorType, textureName, position) =
        let texture = if not (System.String.IsNullOrEmpty textureName) then
                            Some ( textureName)
                        else
                            None
        { ActorType = actorType; Texture = texture; Position = position;}
            
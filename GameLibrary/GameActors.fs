namespace AsteroidGame
module GameActors =

    open Microsoft.Xna.Framework
    open Microsoft.Xna.Framework.Graphics
    open Math

    type AsteroidShooterWorld = 
        {
            Ship : Ship
            Asteroid : List<Asteroid>
            Projectiles : List<Projectile>
        }
        with static member Inception = {
                                            Ship = Ship.Zero;
                                            Asteroid = [Asteroid.Zero;Asteroid.Zero;Asteroid.Zero;];
                                            Projectiles = [];
                                        }
    and Ship = 
        {
            Lives : int
            Shields : int
            Weapon : WeaponType
            Position : Vector2
        }
        with static member Zero = {
                                    Lives = 0;
                                    Shields = 0;
                                    Weapon = Blaster;
                                    Position = Vector2.Zero;
                                  }
    and Asteroid =
        {
            Position : Vector2
        }
        with static member Zero = {Position = Vector2.Zero}
    and Projectile =
        {
            Position : Vector2
        }
        with static member Zero = {Position = Vector2.Zero}

    and WeaponType =
        Blaster 
        | DoubleBlaster
        | Gatling
        | GrenadeLauncher

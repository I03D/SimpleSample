// Максименко Данил
// Помазкин Александр
// И-03

using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Linq;

namespace igra {
    class Start {
        public static void Main(string[] args) {
            Console.Clear();
            Console.CursorVisible = false;

            /* Для "маленьких" консолей можно запускать с аргументом
               "little", чтобы установить ограничение генерации полем
               18x10 (20x12 символов включая рамки): */
            if (args.Length != 0 && args[0] == "little") {
                Game instance1 = new Game(18, 10);
            } else {
                Game instance1 = new Game(30, 20, 4, 4);
            }
        }
    }

	class GameObject {
        public int X;
        public int Y;
        public void Move() {
        }

        public bool Collide(GameObject gameObject) {
            return gameObject.X == X &&
                gameObject.Y == Y;
        }
    }
    class Game {
        static public int Exp;
        static public int Damage;
        static public int Health = 123456789;
        static public int Level;
        static public int Move = 0;
        static public int MovesToSpawn = 30;
        static public int XOffset = 0;
        static public int YOffset = 0;
        static public int Height = 10;
        static public int Width = 10;
        static public Spawn spawn;
        static public string Choose;
        static public List<Enemy> Enemies;
        static public List<Item> FieldItems;
        static bool Exit = false; // For debug.
        static public Random Dice;

        // public static Enemies;

        public Game(int width, int height) {
            Height = height;
            Width = width;

            Player P1 = new Player(4, 4);
            Bullet B1 = new Bullet();

            Dice = new Random();

            Enemies = new List<Enemy> {
                /* new Enemy(5,  5),
                new Enemy(10, 5),
                new Enemy(7,  10) */
            };

            FieldItems = new List<Item> {
                /* new Item(9, 11, "Mushroom"),
                new Item(3, 17, "Iron sword"),
                new Item(8, 3 , "Shield"),
                new Item(22, 5, "Arrow"),
                new Item(9, 9 , "Arrow") */
            };

            Weapon heroWeapon = new Weapon("targetWeapon", 1, 10);
            Weapon droppedWeapon = new Weapon("dropped", 1);

            Process();
        }

        public Game(int width, int height, int xOffset, int yOffset) {
            Height = height;
            Width = width;
            XOffset = xOffset;
            YOffset = yOffset;

            Dice = new Random();

            Enemies = new List<Enemy> {
                /* new Enemy(5,  5),
                new Enemy(10, 5),
                new Enemy(7,  10) */
            };

            FieldItems = new List<Item> {
                /* new Item(9, 11, "Mushroom"),
                new Item(3, 17, "Iron sword"),
                new Item(8, 3 , "Shield"),
                new Item(22, 5, "Arrow"),
                new Item(9, 9 , "Arrow") */
            };

            Weapon heroWeapon = new Weapon("targetWeapon", 1, 10);
            Weapon droppedWeapon = new Weapon("dropped", 1);

            Process();
        }

        public static void Process() {
            Player P1 = new Player(4, 4);
            Bullet B1 = new Bullet();

            /* while (true) {
                Console.ReadLine();
                Forge.EnhanceItem(heroWeapon, 50);
            } */

            /* Enemies.Add( new Enemy() );
            Enemies.Add( new Enemy() );
            Enemies.Add( new Enemy() );
            Enemy[] Enemies = { new Enemy(),
                                    new Enemy(),
                                    new Enemy() }; */

            DrawBorders();
            DrawHealth();
            DrawPlayer(P1);

            // Слишком долгий цикл, возможно стоит поменять:
            while(!Exit) {
                Move++;
                // Отрисовываем предметы, лежащие на земле:
                DrawItems();

                // Ожидаем действие игрока.
                DoAction(P1, B1, ReadActionKey() ); 

                /* Проверяем каждый предмет. Если он на месте игрока,
                то выводим информацию о предмете. Если сделать
                сетку, содержащую статические предметы, можно будет
                избежать перебирания каждой вещи в списке. */
                bool itemMsg = false;
                foreach (Item i in Game.FieldItems.Reverse<Item>()) {
                    if ( (P1.X == i.X) && (P1.Y == i.Y) ) {
                        Msg(i.Name);
                        itemMsg = true;
                    }
                }
                if (!itemMsg) { ClearMsg(); }

                /* Определяем, пора ли спавнить врага. Обязательно
                после хода врагов, чтобы не было рывков. */
                if (Move == MovesToSpawn - 10) {
                    spawn = new Spawn( Dice.Next(Game.XOffset, Game.Width + Game.XOffset), Dice.Next(Game.YOffset, Game.Height + Game.YOffset) );
                    Console.SetCursorPosition(spawn.X + 1, spawn.Y + 1);
                    Console.Write('!');
                } else if (Move == MovesToSpawn) {
                    Console.SetCursorPosition(spawn.X + 1, spawn.Y + 1);
                    Enemies.Add( new Enemy(spawn.X, spawn.Y) );
                    Move = 0;
                    Console.Write(' ');
                }

                // Каждый враг выполняет какое-нибудь действие:
                foreach (Enemy e in Enemies) {
                    EnemyAction(e, P1);
                }

                // Отрисовываем врагов:
                DrawEnemies();

                // Отладочная информация:

                Console.SetCursorPosition(0, 20);
                foreach (Enemy i in Game.Enemies.Reverse<Enemy>() ) {
                    Console.WriteLine(i.X + ":" + i.Y + ";   ");
                }

                // Чересчур сложная проверка столкновений врагов и пуль
                if (B1.LifeTime != 0) {
                    if ( (P1.X != B1.X) || (P1.Y != B1.Y) ) {
                        ClearTile(B1.X, B1.Y);
                    }
                    foreach (Item i in Game.FieldItems.Reverse<Item>()) {
                        if ( (B1.X == i.X) && (B1.Y == i.Y) ) {
                            Console.SetCursorPosition(i.X + 1, i.Y + 1);
                            Console.Write("?");
                        }
                    }
                    B1.Step();
                    if (B1.LifeTime != 0) {
                        bool draw = true;
                        foreach (Enemy i in Game.Enemies.Reverse<Enemy>() ) {
                            if ( (B1.X == i.X) && (B1.Y == i.Y) ) {
                                Msg("Killed!");
                                ClearTile(i.X, i.Y);
                                Game.Enemies.Remove(i);
                                B1.LifeTime = 0;
                                draw = false;

                                switch(Dice.Next(1, 7)) {
                                    case 1:
                                        FieldItems.Add( new Item(B1.X, B1.Y, "Shield") );
                                        break;
                                    case 2:
                                        FieldItems.Add( new Item(B1.X, B1.Y, "Iron Sword") );
                                        break;
                                    case 3:
                                        FieldItems.Add( new Item(B1.X, B1.Y, "Arrow") );
                                        break;
                                    case 4:
                                        FieldItems.Add( new Item(B1.X, B1.Y, "Mushroom") );
                                        break;
                                }
                            }
                        }

                        if (draw) { DrawBullet(B1); }

                        // Не помню, зачем писал это:
                        /* } else {
                            ClearTile(B1.X, B1.Y);
                        } */ 

                    }
                }
            }
        }

        public static void DrawBorders() {
            Console.SetCursorPosition(Game.XOffset, Game.YOffset);
            Console.Write("/");
            for (int i = 1; i < Game.Width + 1; i++) {
                Console.Write("-");
            }
            Console.Write("\\");

            for (int i = 0; i < Game.Height; i++) {
                Console.SetCursorPosition(Game.XOffset, Game.YOffset + i + 1);
                Console.Write("|");
                Console.SetCursorPosition(Game.XOffset + Game.Width + 1, Game.YOffset + 1 + i);
                Console.Write("|");
            }

            Console.SetCursorPosition(Game.XOffset, Game.YOffset + 1 + Game.Height);
            Console.Write("\\");
            for (int i = 0; i < Game.Width; i++) {
                Console.Write("-");
            }
            Console.Write("/");
        }

        public static void DrawHealth() {
            Console.SetCursorPosition( Game.XOffset + (Game.Width / 2) - ( (int)Math.Log10(Health) + 1 ) / 2, Game.YOffset);
            // if ( (int)Math.Log10(Health) + 1 == 3 ) {
                Console.Write(Health);
            // }
        }

        public static void DrawEnemies() {
            foreach (Enemy i in Game.Enemies) {
                Console.SetCursorPosition(i.X + 1, i.Y + 1);
                Console.Write("#");
            }
        }

        public static void DrawItems() {
            foreach (Item i in Game.FieldItems) {
                Console.SetCursorPosition(i.X + 1, i.Y + 1);
                Console.Write("?");
            }
        }

        public static void DrawPlayer(Player p) {
            Console.SetCursorPosition(p.X + 1, p.Y + 1);
            Console.Write("C");
        }

        public static void DrawBullet(Bullet b) {
            // Сделать ограничение, чтобы не рисовать за холстом.
            Console.SetCursorPosition(b.X + 1, b.Y + 1);
            Console.Write('+'); // b.LifeTime);
        }

        public static void ClearTile(int x, int y) {
            Console.SetCursorPosition(x + 1, y + 1);
            Console.Write(" ");
        }

        public static void Msg(string msg) {
            Console.SetCursorPosition(Game.XOffset, Game.Height + Game.YOffset + 2);
            Console.WriteLine(msg);
        }

        public static void ClearMsg() {
            Console.SetCursorPosition(0, Game.Height + Game.YOffset + 2);
            Console.WriteLine("                                        ");
        }

        public static string ReadActionKey() {
            ConsoleKeyInfo Key;

            Key = Console.ReadKey(true);

            switch(Key.KeyChar) {
                case 'k':
                    return "Down";
                case 'l':
                    return "Up";
                case 'j':
                    return "Left";
                case ';':
                    return "Right";
                case 's':
                    return "Down";
                case 'w':
                    return "Up";
                case 'a':
                    return "Left";
                case 'd':
                    return "Right";
                case 'f':
                    return "Shoot";
                case ' ':
                    return "Wait";
            }
            return "attack";
        }

        public static void DoAction(Player p, Bullet b, string action) {
            switch(action) {
                case "Down":
                    ClearTile(p.X, p.Y);
                    p.Down();
                    DrawPlayer(p);
                    break;
                case "Up":
                    ClearTile(p.X, p.Y);
                    p.Up();
                    DrawPlayer(p);
                    break;
                case "Left":
                    ClearTile(p.X, p.Y);
                    p.Left();
                    DrawPlayer(p);
                    break;
                case "Right":
                    ClearTile(p.X, p.Y);
                    p.Right();
                    DrawPlayer(p);
                    break;
                case "Shoot":
                    b.Shoot(p.X, p.Y, p.Facing);
                    break;
                case "Wait":
                    // Пропускаем ход (ждём).
                    break;
                default:
                    Game.Exit = true;
                    break;
            }
        }

        public static void EnemyAction(Enemy e, GameObject target) {
            /* Кубик подкрученный, враги чаще идут в сторону игрока:
            грани 3, 4, 5, 6 - обычные стороны; 1, 2 - подкрутка.
            7, 8 - сон */
            switch (Dice.Next(1, 9)) {
                case 1:
                    if (target.X < e.X) {
                        ClearTile(e.X, e.Y);
                        e.Left();
                    } else if (target.X > e.X) {
                        ClearTile(e.X, e.Y);
                        e.Right();
                    } else {
                        if (target.Y < e.Y) {
                            ClearTile(e.X, e.Y);
                            e.Up();
                        } else {
                            ClearTile(e.X, e.Y);
                            e.Down();
                        }
                    }
                    break;
                case 2:
                    if (target.Y < e.Y) {
                        ClearTile(e.X, e.Y);
                        e.Up();
                    } else if (target.Y > e.Y) {
                        ClearTile(e.X, e.Y);
                        e.Down();
                    } else {
                        if (target.X < e.X) {
                            ClearTile(e.X, e.Y);
                            e.Left();
                        } else {
                            ClearTile(e.X, e.Y);
                            e.Right();
                        }
                    }
                    break;
                case 3:
                    ClearTile(e.X, e.Y);
                    e.Up();
                    break;
                case 4:
                    ClearTile(e.X, e.Y);
                    e.Down();
                    break;
                case 5:
                    ClearTile(e.X, e.Y);
                    e.Left();
                    break;
                case 6:
                    ClearTile(e.X, e.Y);
                    e.Right();
                    break;
            }
        }
    }

    class Spawn : GameObject {
        public Spawn(int x, int y) {
            X = x;
            Y = y;
        }
    }

    class Enemy : GameObject {
        // public bool IsAlive => ; // lambda...

        public Enemy(int x, int y) {
            X = x;
            Y = y;
        }

        public void GetDamage(int damage) {
        }

        public void Down() {
            if (Y < Game.Height + Game.YOffset - 1) {
                Y++;
            }
        }

        public void Up() {
            if (Y > Game.YOffset) {
                Y--;
            }
        }

        public void Left() {
            if (X > Game.XOffset) {
                X--;
            }
        }

        public void Right() {
            if (X < Game.Width + Game.XOffset - 1) {
                X++;
            }
        }
    }

    class Player : GameObject {
        public string Facing = "Right";

        public Player(int x, int y) {
            X=x;
            Y=y;
        }

        public void attack() {
        }
        
        private void AddExp() {
        }

        public void Down() {
            if (Y < Game.Height + Game.YOffset - 1) {
                Y++;
                Facing = "Down";
            }
        }

        public void Up() {
            if (Y > Game.YOffset) {
                Y--;
                Facing = "Up";
            }
        }

        public void Left() {
            if (X > Game.XOffset) {
                X--;
                Facing = "Left";
            }
        }

        public void Right() {
            if (X < Game.Width + Game.XOffset - 1) {
                X++;
                Facing = "Right";
            }
        }
    }

    class Bullet : GameObject {
        public int LifeTime = 0;
        int StartLifeTime = 5;
        string Facing;

        public Bullet() {
            X=0;
            Y=0;
        }

        public void Shoot(int x, int y, string facing) {
            if (LifeTime == 0) {
                Facing = facing;
                X=x;
                Y=y;

                /* switch (facing) {
                    case "Down":
                        Y++;
                        break;
                    case "Up":
                        Y--;
                        break;
                    case "Left":
                        X--;
                        break;
                    case "Right":
                        X++;
                        break;
                } */

                LifeTime = StartLifeTime;
            }
        }

        public void Step() {
            if (LifeTime > 0) {
                LifeTime--;

                switch (Facing) {
                    case "Down":
                        Y++;
                        break;
                    case "Up":
                        Y--;
                        break;
                    case "Left":
                        X--;
                        break;
                    case "Right":
                        X++;
                        break;
                }
            }
        }
    }
}


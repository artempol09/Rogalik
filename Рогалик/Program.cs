using System;
using System.Collections.Generic;

namespace RoguelikeGame
{
    public class Weapon
    {
        public string Name { get; set; }
        public int Damage { get; set; }
        public int Durability { get; set; }

        public Weapon(string name, int damage, int durability)
        {
            Name = name;
            Damage = damage;
            Durability = durability;
        }

        public int Attack()
        {
            if (Durability > 0)
            {
                Durability--;
                return Damage;
            }
            return 0;
        }
    }

    public class Aid
    {
        public string Name { get; set; }
        public int HealAmount { get; set; }

        public Aid(string name, int healAmount)
        {
            Name = name;
            HealAmount = healAmount;
        }

        public int Use()
        {
            return HealAmount;
        }
    }

    public class Enemy
    {
        public string Name { get; set; }
        public int CurrentHealth { get; set; }
        public int MaxHealth { get; set; }
        public Weapon Weapon { get; set; }

        public Enemy(string name, int maxHealth, Weapon weapon)
        {
            Name = name;
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
            Weapon = weapon;
        }

        public int Attack()
        {
            return Weapon.Attack();
        }

        public void TakeDamage(int damage)
        {
            CurrentHealth -= damage;
            if (CurrentHealth < 0) CurrentHealth = 0;
        }

        public bool IsAlive()
        {
            return CurrentHealth > 0;
        }
    }

    // Игрок
    public class Player
    {
        public string Name { get; set; }
        public int CurrentHealth { get; set; }
        public int MaxHealth { get; set; }
        public Aid AidKit { get; set; }
        public Weapon Weapon { get; set; }
        public int Score { get; set; }

        public Player(string name, int maxHealth, Weapon weapon, Aid aidKit)
        {
            Name = name;
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
            Weapon = weapon;
            AidKit = aidKit;
            Score = 0;
        }

        public int Attack()
        {
            return Weapon.Attack();
        }

        public void TakeDamage(int damage)
        {
            CurrentHealth -= damage;
            if (CurrentHealth < 0) CurrentHealth = 0;
        }

        public void Heal()
        {
            if (AidKit != null)
            {
                int healAmount = AidKit.Use();
                CurrentHealth += healAmount;
                if (CurrentHealth > MaxHealth) CurrentHealth = MaxHealth;
                AidKit = null;
            }
        }

        public bool HasAidKit()
        {
            return AidKit != null;
        }

        public bool IsAlive()
        {
            return CurrentHealth > 0;
        }
    }

    public class Game
    {
        private Player player;
        private Random random;

        private List<string> enemyNames = new List<string>
        {
            "Бомж", "Сосед", "Гоблин", "Скелет", "Зомби", "Гопник", "Жирный одноклассник", "Пудж"
        };

        private List<Weapon> possibleWeapons = new List<Weapon>
        {
            new Weapon("Меч", 15, 10),
            new Weapon("Топор", 20, 8),
            new Weapon("Кинжал", 10, 15),
            new Weapon("Булава", 18, 7),
            new Weapon("Лук", 12, 12),
            new Weapon("Моргенштерн", 25, 5),
            new Weapon("Нож", 20, 6),
            new Weapon("Копье", 16, 9)
        };

        private List<Aid> possibleAidKits = new List<Aid>
        {
            new Aid("аптечка", 40),
        };

        public Game()
        {
            random = new Random();
        }

        public void Start()
        {
            Console.WriteLine("Добро пожаловать!");
            Console.WriteLine("Как тебя зовут:");
            Console.Write(" ");
            string playerName = Console.ReadLine();

            Weapon playerWeapon = possibleWeapons[random.Next(possibleWeapons.Count)];
            Aid playerAid = possibleAidKits[random.Next(possibleAidKits.Count)];

            player = new Player(playerName, 100, playerWeapon, playerAid);

            Console.WriteLine($"\nВаше имя {player.Name}");
            Console.WriteLine($"Вы нашли {player.Weapon.Name} ({player.Weapon.Damage}), " +
                            $"а также {player.AidKit.Name} ({player.AidKit.HealAmount}hp).");
            Console.WriteLine($"У вас {player.CurrentHealth}hp.\n");

            GameLoop();
        }

        private void GameLoop()
        {
            int enemyCount = 0;

            while (player.IsAlive())
            {
                enemyCount++;
                Enemy enemy = GenerateRandomEnemy();

                Console.WriteLine($"{player.Name} встречает врага {enemy.Name} ({enemy.CurrentHealth}hp), " +
                                $"у врага есть оружие {enemy.Weapon.Name} ({enemy.Weapon.Damage})");

                FightEnemy(enemy);

                if (player.IsAlive())
                {
                    player.Score += 10;
                    Console.WriteLine($"\nВы победили {enemy.Name}! +10 очков. Всего очков: {player.Score}");

                    if (random.Next(100) < 30 && !player.HasAidKit())
                    {
                        player.AidKit = possibleAidKits[random.Next(possibleAidKits.Count)];
                        Console.WriteLine($"Вы нашли {player.AidKit.Name}!");
                    }

                    Console.WriteLine("Нажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                    Console.WriteLine();
                }
            }

            GameOver(enemyCount);
        }

        private Enemy GenerateRandomEnemy()
        {
            string enemyName = enemyNames[random.Next(enemyNames.Count)];
            int enemyHealth = random.Next(30, 71);
            Weapon enemyWeapon = possibleWeapons[random.Next(possibleWeapons.Count)];

            return new Enemy(enemyName, enemyHealth, enemyWeapon);
        }

        private void FightEnemy(Enemy enemy)
        {
            while (player.IsAlive() && enemy.IsAlive())
            {
                Console.WriteLine("Что вы будете делать?");
                Console.WriteLine("1. Ударить");
                Console.WriteLine("2. Пропустить ход");
                if (player.HasAidKit())
                {
                    Console.WriteLine("3. Использовать аптечку");
                }

                Console.Write("> ");
                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        PlayerAttack(enemy);
                        break;
                    case "2":
                        Console.WriteLine($"{player.Name} пропускает ход");
                        break;
                    case "3" when player.HasAidKit():
                        player.Heal();
                        Console.WriteLine($"{player.Name} использовал аптечку");
                        Console.WriteLine($"Теперь у вас {player.CurrentHealth}hp");
                        break;
                    default:
                        Console.WriteLine("Неверный ввод, пропускаем ход");
                        break;
                }

                if (enemy.IsAlive())
                {
                    EnemyAttack(enemy);
                }
            }
        }

        private void PlayerAttack(Enemy enemy)
        {
            int damage = player.Attack();
            enemy.TakeDamage(damage);
            Console.WriteLine($"{player.Name} ударил противника {enemy.Name}");
            Console.WriteLine($"У противника {enemy.CurrentHealth}hp, у вас {player.CurrentHealth}hp");
        }

        private void EnemyAttack(Enemy enemy)
        {
            int damage = enemy.Attack();
            player.TakeDamage(damage);
            Console.WriteLine($"Противник {enemy.Name} ударил вас!");
            Console.WriteLine($"У противника {enemy.CurrentHealth}hp, у вас {player.CurrentHealth}hp");
        }

        private void GameOver(int enemiesDefeated)
        {
            Console.WriteLine("\nИГРА ОКОНЧЕНА");
            Console.WriteLine($"Игрок: {player.Name}");
            Console.WriteLine($"Побеждено врагов: {enemiesDefeated - 1}");
            Console.WriteLine($"Финальный счет: {player.Score}");
            Console.WriteLine("Спасибо за игру!");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            game.Start();
        }
    }
}
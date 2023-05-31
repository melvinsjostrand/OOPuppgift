using System;
using System.IO;

enum GameState
{
    MainMenu,
    Battle,
    City,
    Dead,
    Exit
}

class Player
{
    public int Health { get; set; }
    public int Score { get; set; }
    public Random Random { get; }

    public Player()
    {
        Health = 100;
        Score = 0;
        Random = new Random();
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Health = 0;
            Console.WriteLine("Du dog!");
        }
    }

    public void DealDamage(Enemy enemy)
    {
        int damage = Random.Next(10, 16); // Mellan 10-15 skada
        enemy.TakeDamage(damage);
        if (enemy.Health <= 0)
        {
            Console.WriteLine("Fienden dog!");
            Score += 100;
        }
    }
}

class Enemy
{
    public int Health { get; set; }
    public Random Random { get; }

    public Enemy(int health)
    {
        Health = health;
        Random = new Random();
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Console.WriteLine("Fienden dog!");
        }
    }

    public void DealDamage(Player player)
    {
        int damage = Random.Next(5, 21); // Mellan 5-20 skada
        player.TakeDamage(damage);
        if (player.Health <= 0)
        {
            Console.WriteLine("Du dog!");
        }
    }
}

class Game
{
    private Player player;
    private Enemy enemy;
    private int currentEnemyIndex;
    private GameState gameState;

    public Game()
    {
        player = new Player();
        enemy = new Enemy(50);
        currentEnemyIndex = 0;
        gameState = GameState.MainMenu;
    }

    public void Run()
    {
        while (gameState != GameState.Exit)
        {
            switch (gameState)
            {
                case GameState.MainMenu:
                    ShowMainMenu();
                    break;
                case GameState.Battle:
                    StartBattle();
                    break;
                case GameState.City:
                    VisitCity();
                    break;
                case GameState.Dead:
                    GameOver();
                    break;
            }
        }

        if (gameState == GameState.Exit || gameState == GameState.Dead)
        {
            SaveScore();
        }
    }

    private void ShowMainMenu()
    {
        Console.WriteLine("=== Huvudmeny ===");
        Console.WriteLine("1. Starta Battle");
        Console.WriteLine("2. Gå till staden");
        Console.WriteLine("3. Stäng av spelet");

        int choice = GetChoice(3);
        switch (choice)
        {
            case 1:
                currentEnemyIndex = 0;
                gameState = GameState.Battle;
                break;
            case 2:
                gameState = GameState.City;
                break;
            case 3:
                Console.WriteLine("Tack för att du spelade!");
                gameState = GameState.Exit;
                break;
        }
    }

    private void StartBattle()
    {
        Console.WriteLine("=== Battle ===");
        Console.WriteLine("Spelarens HP: " + player.Health);
        Console.WriteLine("Spelarens Score: " + player.Score);
        Console.WriteLine("Fiende: " + (currentEnemyIndex + 1) + " - HP: " + enemy.Health);

        Console.WriteLine("Välj mellan att attackera eller gå tillbaka:");
        Console.WriteLine("1. Attackera");
        Console.WriteLine("2. Gå tillbaka");

        int choice = GetChoice(2);
        switch (choice)
        {
            case 1:
                player.DealDamage(enemy);

                if (enemy.Health <= 0)
                {
                    Console.WriteLine("Du dödade fienden!");

                    if (currentEnemyIndex < 2)
                    {
                        Console.WriteLine("Gör dig redo för nästa fiende!");
                        currentEnemyIndex++;
                        enemy = new Enemy(50); // Skapa en ny fiende med 50 hälsa
                    }
                    else
                    {
                        Console.WriteLine("Du dödade alla fiender!");
                        gameState = GameState.City;
                    }
                }
                else
                {
                    Console.WriteLine("Motståndaren attackerade dig!");
                    enemy.DealDamage(player);
                    if (player.Health <= 0)
                    {
                        Console.WriteLine("Du Dog!");
                        gameState = GameState.Dead;
                    }
                }
                break;
            case 2:
                Console.WriteLine("Du flydde från striden.");
                gameState = GameState.City;
                break;
        }
    }

    private void VisitCity()
    {
        Console.WriteLine("=== Stad ===");
        Console.WriteLine("Spelarens HP: " + player.Health);
        Console.WriteLine("Spelarens Score: " + player.Score);
        Console.WriteLine("1. Gå och lägg dig för att återfå all din energi och hälsa");
        Console.WriteLine("2. Tillbaka till huvudmeny");

        int choice = GetChoice(2);
        switch (choice)
        {
            case 1:
                player.Health = 100;
                Console.WriteLine("Du sov väldigt skönt och lyckades återfå all din energi och hälsa.");
                break;
            case 2:
                gameState = GameState.MainMenu;
                break;
        }
    }

    private void GameOver()
    {
        Console.WriteLine("=== Game Over ===");
        Console.WriteLine("Final Score: " + player.Score);
        gameState = GameState.Exit;
    }

    private int GetChoice(int maxChoice)
    {
        int choice = 0;
        while (choice < 1 || choice > maxChoice)
        {
            Console.Write("Välj nummer mellan 1-3: ");
            int.TryParse(Console.ReadLine(), out choice);
        }
        return choice;
    }

    private void SaveScore()
    {
        string filePath = "scores.txt";
        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            writer.WriteLine(player.Score);
        }
        Console.WriteLine("Dina poäng finns nu att hitta i scores.txt");
    }
}

class Program
{
    static void Main(string[] args)
    {
        Game game = new Game();
        game.Run();
    }
}

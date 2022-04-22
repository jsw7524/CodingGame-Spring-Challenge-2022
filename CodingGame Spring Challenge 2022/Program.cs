using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
public class Base
{
    public Base(int baseX, int baseY, int health, int mana)
    {
        BaseX = baseX;
        BaseY = baseY;
        Health = health;
        Mana = mana;
    }
    public int BaseX { set; get; }
    public int BaseY { set; get; }
    public int Health { set; get; }
    public int Mana { set; get; }
}

public class Entity
{
    public Entity(int id, int type, int x, int y, int shieldLife, int isControlled, int health, int vx, int vy, int nearBase, int threatFor)
    {
        Id = id;
        Type = type;
        X = x;
        Y = y;
        ShieldLife = shieldLife;
        IsControlled = isControlled;
        Health = health;
        Vx = vx;
        Vy = vy;
        NearBase = nearBase;
        ThreatFor = threatFor;
    }
    public int Id { set; get; }// Unique identifier
    public int Type { set; get; } // 0=monster, 1=your hero, 2=opponent hero
    public int X { set; get; } // Position of this entity
    public int Y { set; get; }
    public int ShieldLife { set; get; } // Ignore for this league; Count down until shield spell fades
    public int IsControlled { set; get; } // Ignore for this league; Equals 1 when this entity is under a control spell
    public int Health { set; get; } // Remaining health of this monster
    public int Vx { set; get; } // Trajectory of this monster
    public int Vy { set; get; }
    public int NearBase { set; get; } // 0=monster with no target yet, 1=monster targeting a base
    public int ThreatFor { set; get; } // Given this monster's trajectory, is it a threat to 1=your base, 2=your opponent's base, 0=neither

    /// /////////////////

    public double DangerLevel { set; get; }
}

public class AlliedHero : Entity
{
    public AlliedHero(int id, int type, int x, int y, int shieldLife, int isControlled, int health, int vx, int vy, int nearBase, int threatFor) :
        base(id, type, x, y, shieldLife, isControlled, health, vx, vy, nearBase, threatFor)
    {

    }
}

public class EnemyHero : Entity
{
    public EnemyHero(int id, int type, int x, int y, int shieldLife, int isControlled, int health, int vx, int vy, int nearBase, int threatFor) :
        base(id, type, x, y, shieldLife, isControlled, health, vx, vy, nearBase, threatFor)
    {

    }
}

public class Monster : Entity
{

    public Monster(int id, int type, int x, int y, int shieldLife, int isControlled, int health, int vx, int vy, int nearBase, int threatFor) :
        base(id, type, x, y, shieldLife, isControlled, health, vx, vy, nearBase, threatFor)
    {

    }

}

public interface ICommand
{
    string Execute();
}

public class WaitCommand : ICommand
{
    public string Execute()
    {
        return "WAIT";
    }
}

public class MoveCommand : ICommand
{
    public int X { get; set; }
    public int Y { get; set; }

    public MoveCommand(int x, int y)
    {
        X = x;
        Y = y;
    }
    public string Execute()
    {
        return $"MOVE {X} {Y}";
    }
}


public interface IStrategy
{
    void Run(Base ourBase, Base enemyBase, Entity mainCharacter, IEnumerable<Entity> entities);
}

public class ClosestStrategy : IStrategy
{
    public virtual double ComputeDangerLevel(Base theBase, Entity entity)
    {
        double distance = Math.Sqrt((theBase.BaseX - entity.X) * (theBase.BaseX - entity.X) + (theBase.BaseY - entity.Y) * (theBase.BaseY - entity.Y));
        double weight = 1.0 / (distance + 1.0);
        entity.DangerLevel = weight * 100;
        return entity.DangerLevel;
    }
    public void Run(Base ourBase, Base enemyBase, Entity mainCharacter, IEnumerable<Entity> entities)
    {
        foreach (Entity e in entities)
        {
            e.DangerLevel = ComputeDangerLevel(ourBase, e);
        }
    }
}

public class GameManager
{
    private List<Entity> _entities = new List<Entity>();
    private static List<IStrategy> _strategies = new List<IStrategy>();
    private static int _numHeros;

    public Base ourBase;
    public Base enemyBase;
    public GameManager(Base ours, Base enemy, int numHeros)
    {
        ourBase = ours;
        enemyBase = enemy;
        _numHeros = numHeros;
        _strategies.Add(new ClosestStrategy());
    }

    public void AddEntity(Entity e)
    {
        _entities.Add(e);
    }

    public IEnumerable<AlliedHero> GetAlliedHeros()
    {
        return _entities.Where(e => e is AlliedHero).Select(a => a as AlliedHero);
    }

    public void ClearEntities()
    {
        _entities.Clear();
    }

    public ICommand CommandHero(AlliedHero hero)
    {
        IEnumerable<AlliedHero> _alliedHeros = GetAlliedHeros();
        IEnumerable<EnemyHero> _enemyHeros = _entities.Where(e => e is EnemyHero).Select(a => a as EnemyHero);
        IEnumerable<Monster> _monsters = _entities.Where(e => e is Monster).Select(a => a as Monster);

        foreach (IStrategy strategy in _strategies)
        {
            strategy.Run(ourBase, enemyBase, hero, _monsters);
        }

        try
        {
            //Monster mostDangerousMonster = _monsters?.MaxBy(m => m.DangerLevel);
            Monster mostDangerousMonster = _monsters.Where(m => (m.DangerLevel == _monsters.Max(a => a.DangerLevel))).FirstOrDefault();
            
            if (mostDangerousMonster == null)
            {
                return new WaitCommand();
            }
            
            ICommand nextCommand = new MoveCommand(mostDangerousMonster.X, mostDangerousMonster.Y);
            return nextCommand;
        }
        catch (Exception ex)
        {
            throw;
        }

    }

}

class Player
{
    static void Main(string[] args)
    {
        string[] inputs;
        inputs = Console.ReadLine().Split(' ');
        int baseX = int.Parse(inputs[0]); // The corner of the map representing your base
        int baseY = int.Parse(inputs[1]);
        int heroesPerPlayer = int.Parse(Console.ReadLine()); // Always 3
        GameManager gameManager = new GameManager(new Base(baseX, baseY, -1, -1), new Base(17630 - baseX, 9000 - baseY, -1, -1), heroesPerPlayer);
        // game loop
        while (true)
        {

            inputs = Console.ReadLine().Split(' ');

            gameManager.ourBase.Health = int.Parse(inputs[0]); 
            gameManager.ourBase.Mana = int.Parse(inputs[1]);

            inputs = Console.ReadLine().Split(' ');
            gameManager.enemyBase.Health = int.Parse(inputs[0]); // enemy base health
            gameManager.enemyBase.Mana = int.Parse(inputs[1]); // Ignore in the first league; Spend ten mana to cast a spell

            gameManager.ClearEntities();
            int entityCount = int.Parse(Console.ReadLine()); // Amount of heros and monsters you can see
            for (int i = 0; i < entityCount; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int id = int.Parse(inputs[0]); // Unique identifier
                int type = int.Parse(inputs[1]); // 0=monster, 1=your hero, 2=opponent hero
                int x = int.Parse(inputs[2]); // Position of this entity
                int y = int.Parse(inputs[3]);
                int shieldLife = int.Parse(inputs[4]); // Ignore for this league; Count down until shield spell fades
                int isControlled = int.Parse(inputs[5]); // Ignore for this league; Equals 1 when this entity is under a control spell
                int health = int.Parse(inputs[6]); // Remaining health of this monster
                int vx = int.Parse(inputs[7]); // Trajectory of this monster
                int vy = int.Parse(inputs[8]);
                int nearBase = int.Parse(inputs[9]); // 0=monster with no target yet, 1=monster targeting a base
                int threatFor = int.Parse(inputs[10]); // Given this monster's trajectory, is it a threat to 1=your base, 2=your opponent's base, 0=neither
                if (type == 0)
                {
                    gameManager.AddEntity(new Monster(id, type, x, y, shieldLife, isControlled, health, vx, vy, nearBase, threatFor));
                }
                else if (type == 1)
                {
                    gameManager.AddEntity(new AlliedHero(id, type, x, y, shieldLife, isControlled, health, vx, vy, nearBase, threatFor));
                }
                else if (type == 2)
                {
                    gameManager.AddEntity(new EnemyHero(id, type, x, y, shieldLife, isControlled, health, vx, vy, nearBase, threatFor));
                }
            }
            foreach (var hero in gameManager.GetAlliedHeros())
            {
                // Write an action using Console.WriteLine()
                // To debug: Console.Error.WriteLine("Debug messages...");
                // In the first league: MOVE <x> <y> | WAIT; In later leagues: | SPELL <spellParams>;
                ICommand cmd = gameManager.CommandHero(hero);
                Console.WriteLine(cmd.Execute());
            }
        }
    }
}
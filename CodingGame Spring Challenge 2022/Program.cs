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
public static class Helper
{
    public static double GetDistanceFromBase(Base theBase, Entity entity)
    {
        return Math.Sqrt((theBase.BaseX - entity.X) * (theBase.BaseX - entity.X) + (theBase.BaseY - entity.Y) * (theBase.BaseY - entity.Y));
    }
    public static double GetDistance(Entity a, Entity b)
    {
        return Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
    }
    public static double AngleToRadian(double angle)
    {

        return angle * ((2 * Math.PI) / 360.0);
    }


}


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

    public List<Monster> NearbyMonsters { set; get; }
    public List<AlliedHero> NearbyAllieds { set; get; }
    public List<EnemyHero> NearbyEnemies { set; get; }

    public List<Monster> NotFarAwayMonsters { set; get; }
    public List<AlliedHero> NotFarAwayAllieds { set; get; }
    public List<EnemyHero> NotFarAwayEnemies { set; get; }

    public bool CastWindSpell { set; get; }
    public bool CastControlSpell { set; get; }
    public bool CastShieldSpell { set; get; }
}

public class AlliedHero : Entity
{
    private static int serialNumber;
    public int sn;

    public double[] moveDirectionScores;

    public double[] windDirectionScores;

    public bool controledByEnemy;

    public AlliedHero(int id, int type, int x, int y, int shieldLife, int isControlled, int health, int vx, int vy, int nearBase, int threatFor) :
        base(id, type, x, y, shieldLife, isControlled, health, vx, vy, nearBase, threatFor)
    {
        sn = serialNumber;
        serialNumber = (serialNumber + 1) % 3;
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

public class RandomMoveCommand : ICommand
{
    Random _rand = new Random();
    public string Execute()
    {
        ICommand cmd = new MoveCommand(_rand.Next(17630), _rand.Next(9000));
        return cmd.Execute();
    }
}

public class WindSpellCommand : ICommand
{
    public int X { get; set; }
    public int Y { get; set; }

    public WindSpellCommand(int x, int y)
    {
        X = x;
        Y = y;
    }
    public string Execute()
    {
        return $"SPELL WIND {X} {Y}";
    }
}

public class ShieldSpellCommand : ICommand
{
    public int Id { get; set; }


    public ShieldSpellCommand(int id)
    {
        Id = id;
    }
    public string Execute()
    {
        return $"SPELL SHIELD {Id}";
    }
}


public class ControlSpellCommand : ICommand
{
    public int Id { get; set; }
    public int X { get; set; }
    public int Y { get; set; }

    public ControlSpellCommand(int id, int x, int y)
    {
        Id = id;
        X = x;
        Y = y;
    }
    public string Execute()
    {
        return $"SPELL CONTROL {Id} {X} {Y}";
    }
}


public abstract class Intelligence
{
    public abstract void Run(Base ourBase, Base enemyBase, Entity mainCharacter, IEnumerable<AlliedHero> alliedHeros, IEnumerable<EnemyHero> enemyHeros, IEnumerable<Monster> monsters, IEnumerable<Entity> entities);
}

public class ClosestIntelligence : Intelligence
{
    public virtual double ComputeDangerLevel(Base theBase, Entity entity)
    {
        double distance = 1.0 / (Helper.GetDistanceFromBase(theBase, entity) + 1.0);
        double isTargetingBase = (entity.NearBase == 1) && (entity.ThreatFor == 1) ? 1.1 : 1.0;
        entity.DangerLevel = isTargetingBase * distance * 100;
        return entity.DangerLevel;
    }
    public override void Run(Base ourBase, Base enemyBase, Entity mainCharacter, IEnumerable<AlliedHero> alliedHeros, IEnumerable<EnemyHero> enemyHeros, IEnumerable<Monster> monsters, IEnumerable<Entity> entities)
    {
        foreach (Entity e in entities)
        {
            e.DangerLevel = ComputeDangerLevel(ourBase, e);
        }
    }
}

public class NearByIntelligence : Intelligence
{
    public override void Run(Base ourBase, Base enemyBase, Entity mainCharacter, IEnumerable<AlliedHero> alliedHeros, IEnumerable<EnemyHero> enemyHeros, IEnumerable<Monster> monsters, IEnumerable<Entity> entities)
    {
        // mainCharacter is hero
        double defitionOfNearby = 1280.0;
        double defitionOfNotFarAway = 2200.0;
        mainCharacter.NearbyAllieds = new List<AlliedHero>();
        mainCharacter.NearbyEnemies = new List<EnemyHero>();
        mainCharacter.NearbyMonsters = new List<Monster>();
        mainCharacter.NotFarAwayAllieds = new List<AlliedHero>();
        mainCharacter.NotFarAwayEnemies = new List<EnemyHero>();
        mainCharacter.NotFarAwayMonsters = new List<Monster>();
        foreach (Entity e in entities)
        {
            double distance = Helper.GetDistance(mainCharacter, e);
            if ((distance <= defitionOfNearby))
            {

                if (e is Monster)
                {
                    mainCharacter.NearbyMonsters.Add(e as Monster);
                }
                else if (e is AlliedHero)
                {
                    mainCharacter.NearbyAllieds.Add(e as AlliedHero);
                }
                else if (e is EnemyHero)
                {
                    mainCharacter.NearbyEnemies.Add(e as EnemyHero);
                }
            }

            if ((distance <= defitionOfNotFarAway))
            {

                if (e is Monster)
                {
                    mainCharacter.NotFarAwayMonsters.Add(e as Monster);
                }
                else if (e is AlliedHero)
                {
                    mainCharacter.NotFarAwayAllieds.Add(e as AlliedHero);
                }
                else if (e is EnemyHero)
                {
                    mainCharacter.NotFarAwayEnemies.Add(e as EnemyHero);
                }
            }
        }
        mainCharacter.NearbyAllieds = mainCharacter.NearbyAllieds.OrderBy(a => Helper.GetDistance(mainCharacter, a)).ToList();
        mainCharacter.NearbyEnemies = mainCharacter.NearbyEnemies.OrderBy(a => Helper.GetDistance(mainCharacter, a)).ToList();
        mainCharacter.NearbyMonsters = mainCharacter.NearbyMonsters.OrderBy(a => Helper.GetDistance(mainCharacter, a)).ToList();
        mainCharacter.NotFarAwayMonsters = mainCharacter.NotFarAwayMonsters.OrderBy(a => Helper.GetDistance(mainCharacter, a)).ToList();
        mainCharacter.NotFarAwayAllieds = mainCharacter.NotFarAwayAllieds.OrderBy(a => Helper.GetDistance(mainCharacter, a)).ToList();
        mainCharacter.NotFarAwayEnemies = mainCharacter.NotFarAwayEnemies.OrderBy(a => Helper.GetDistance(mainCharacter, a)).ToList();
    }
}


public class WindDirectionScoreIntelligence : Intelligence
{
    public override void Run(Base ourBase, Base enemyBase, Entity mainCharacter, IEnumerable<AlliedHero> alliedHeros, IEnumerable<EnemyHero> enemyHeros, IEnumerable<Monster> monsters, IEnumerable<Entity> entities)
    {
        Entity tmp = new Entity(-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1);

        AlliedHero ourHero = mainCharacter as AlliedHero;

        ourHero.windDirectionScores = new double[360];
        for (int angle = 0; angle < 360; angle++)
        {

            tmp.X = (int)Math.Round(ourHero.X + 2200.0 * Math.Cos(Helper.AngleToRadian(angle)), 0);
            tmp.Y = (int)Math.Round(ourHero.Y + 2200.0 * Math.Sin(Helper.AngleToRadian(angle)), 0);
            double distanceFromBase = 1.0 / (Helper.GetDistanceFromBase(enemyBase, tmp) + 1.0);
            ourHero.windDirectionScores[angle] += 100 * distanceFromBase;

            foreach (EnemyHero enemyHero in ourHero.NotFarAwayEnemies)
            {
                double enemyHeroDistance = 1.0 / (Helper.GetDistance(tmp, enemyHero) + 1.0);
                ourHero.windDirectionScores[angle] -= 10 * enemyHeroDistance;
            }
        }

    }
}

public class MoveDirectionScoreIntelligence : Intelligence
{
    public override void Run(Base ourBase, Base enemyBase, Entity mainCharacter, IEnumerable<AlliedHero> alliedHeros, IEnumerable<EnemyHero> enemyHeros, IEnumerable<Monster> monsters, IEnumerable<Entity> entities)
    {
        Entity tmp = new Entity(-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1);

        AlliedHero ourHero = mainCharacter as AlliedHero;

        ourHero.moveDirectionScores = new double[360];
        for (int angle = 0; angle < 360; angle++)
        {

            tmp.X = (int)Math.Round(ourHero.X + 800.0 * Math.Cos(Helper.AngleToRadian(angle)), 0);
            tmp.Y = (int)Math.Round(ourHero.Y + 800.0 * Math.Sin(Helper.AngleToRadian(angle)), 0);

            foreach (Monster monster in ourHero.NotFarAwayMonsters)
            {
                double distance = 1.0 / (Helper.GetDistance(tmp, monster) + 1.0);
                ourHero.moveDirectionScores[angle] += (distance * distance);
            }
        }
    }
}

public class GameManager
{
    private List<Entity> _entities = new List<Entity>();
    private List<Intelligence> _intelligences = new List<Intelligence>();
    private IStrategy _strategy;
    private int _numHeros;


    public Base ourBase;
    public Base enemyBase;

    public GameManager(Base ours, Base enemy, int numHeros)
    {
        ourBase = ours;
        enemyBase = enemy;
        _numHeros = numHeros;
        _intelligences.Add(new ClosestIntelligence());
        _intelligences.Add(new NearByIntelligence());
        _intelligences.Add(new MoveDirectionScoreIntelligence());

        _intelligences.Add(new WindDirectionScoreIntelligence());

    }
    public IEnumerable<AlliedHero> GetAlliedHeros()
    {
        return _entities.Where(e => e is AlliedHero).Select(a => a as AlliedHero);
    }

    public void SetStrategy(IStrategy s)
    {
        _strategy = s;
    }

    public void AddEntity(Entity e)
    {
        _entities.Add(e);
    }

    public void ClearEntities()
    {
        _entities.Clear();
    }

    public ICommand CommandHero(AlliedHero hero)
    {
        try
        {
            IEnumerable<AlliedHero> alliedHeros = _entities.Where(e => e is AlliedHero).Select(a => a as AlliedHero);
            IEnumerable<EnemyHero> enemyHeros = _entities.Where(e => e is EnemyHero).Select(a => a as EnemyHero);
            IEnumerable<Monster> monsters = _entities.Where(e => e is Monster).Select(a => a as Monster);
            foreach (Intelligence intelligence in _intelligences)
            {
                intelligence.Run(ourBase, enemyBase, hero, alliedHeros, enemyHeros, monsters, _entities);
            }
            return _strategy.Run(ourBase, enemyBase, hero, alliedHeros, enemyHeros, monsters, _entities);
        }
        catch (Exception ex)
        {
            //return new WaitCommand();
            throw;
        }

    }

}

public interface IStrategy
{
    ICommand Run(Base ourBase, Base enemyBase, AlliedHero hero, IEnumerable<AlliedHero> alliedHeros, IEnumerable<EnemyHero> enemyHeros, IEnumerable<Monster> monsters, IEnumerable<Entity> entities);
}

public class OriginalStrategy : IStrategy
{
    private Random _rand = new Random();
    private bool _secondHalfGame = false;
    private int attackAngel = 0;
    private List<Tuple<int, int>> redbaseEntryLocation = new List<Tuple<int, int>>() { Tuple.Create(15000, 3000), Tuple.Create(11000, 7000) };
    private List<Tuple<int, int>> bluebaseEntryLocation = new List<Tuple<int, int>>() { Tuple.Create(6000, 1000), Tuple.Create(2000, 5000) };
    private enum behaviors
    {
        KillClosestMonsterToBase,
        GoToTheCenterOfMap,
        KillMonsterNearBy,
        GoToOurBase,
        GoToEnemyBase,
        StayEnemyBaseOutskirt,
        AttackEnemyBase

    }
    private behaviors[] _herosAction = new behaviors[3] { behaviors.KillClosestMonsterToBase, behaviors.KillClosestMonsterToBase, behaviors.GoToTheCenterOfMap };

    public ICommand Run(Base ourBase, Base enemyBase, AlliedHero hero, IEnumerable<AlliedHero> alliedHeros, IEnumerable<EnemyHero> enemyHeros, IEnumerable<Monster> monsters, IEnumerable<Entity> entities)
    {


        behaviors currentHeroAction = _herosAction[hero.sn % 3];
        double distanceToOurBase = Math.Sqrt((hero.X - ourBase.BaseX) * (hero.X - ourBase.BaseX) + (hero.Y - ourBase.BaseY) * (hero.Y - ourBase.BaseY));
        double distanceToEnemyBase = Math.Sqrt((hero.X - enemyBase.BaseX) * (hero.X - enemyBase.BaseX) + (hero.Y - enemyBase.BaseY) * (hero.Y - enemyBase.BaseY));

        List<Tuple<int, int>> enemyBaseEntry = enemyBase.BaseX > 9000 ? redbaseEntryLocation : bluebaseEntryLocation;

        if (1 == hero.IsControlled)
        {
            hero.controledByEnemy = true;
        }

        switch (currentHeroAction)
        {
            case behaviors.GoToOurBase:
                if (distanceToOurBase <= 5500.0)
                {
                    // close enough
                    _herosAction[hero.sn % 3] = behaviors.KillClosestMonsterToBase;
                }
                return new MoveCommand(ourBase.BaseX, ourBase.BaseY);
            case behaviors.KillClosestMonsterToBase:
                if (hero.ShieldLife == 0 && ourBase.Mana > 10 && true == hero.controledByEnemy && hero.NotFarAwayEnemies.Count() > 0)
                {
                    return new ShieldSpellCommand(hero.Id);
                }

                if (distanceToOurBase >= 8000.0)
                {
                    // too far
                    _herosAction[hero.sn % 3] = behaviors.GoToOurBase;
                    return new MoveCommand(ourBase.BaseX, ourBase.BaseY);
                }
                if (ourBase.Mana >= 10)
                {
                    if (distanceToOurBase < 1500.0 && hero.NearbyMonsters.Where(m => m.ShieldLife == 0).Count() >= 1)
                    {
                        return new WindSpellCommand(enemyBase.BaseX, enemyBase.BaseY);
                    }

                    if (hero.NearbyEnemies.Count() + hero.NearbyMonsters.Count() >= 2)
                    {
                        return new WindSpellCommand(enemyBase.BaseX, enemyBase.BaseY);
                    }
                }
                if (1 == hero.sn && ourBase.Mana >= 10 && _secondHalfGame == true)
                {
                    if (hero.NotFarAwayMonsters.Count() >= 1)
                    {
                        if (null != hero.NotFarAwayMonsters.LastOrDefault())
                        {
                            Monster lastNotFarAwayMonsters = hero.NotFarAwayMonsters.LastOrDefault();
                            if (Helper.GetDistanceFromBase(ourBase, lastNotFarAwayMonsters) > 5000.0 && lastNotFarAwayMonsters.IsControlled == 0)
                            {
                                return new ControlSpellCommand(lastNotFarAwayMonsters.Id, enemyBase.BaseX, enemyBase.BaseY);
                            }
                        }
                    }
                }
                //Monster mostDangerousMonster = _monsters?.MaxBy(m => m.DangerLevel);
                Monster mostDangerousMonster = monsters.Where(m => (m.DangerLevel == monsters.Max(a => a.DangerLevel))).FirstOrDefault();
                if (mostDangerousMonster == null)
                {
                    return new RandomMoveCommand();
                }
                ///////////////////////////////////////////////
                ICommand nextCommand = new MoveCommand(mostDangerousMonster.X, mostDangerousMonster.Y);
                return nextCommand;
            case behaviors.GoToTheCenterOfMap:
                Console.Error.WriteLine("GoToTheCenterOfMap");
                int centerX = 17630 / 2;
                int centerY = 9000 / 2;
                if (hero.X == centerX || hero.NearbyMonsters.Count() > 0)
                {
                    _herosAction[hero.sn % 3] = behaviors.KillMonsterNearBy;
                }
                return new MoveCommand(centerX, hero.Y);
            case behaviors.KillMonsterNearBy:
                Console.Error.WriteLine("KillMonsterNearBy");
                if (distanceToEnemyBase <= 4500.0)
                {
                    // too close
                    _herosAction[hero.sn % 3] = behaviors.StayEnemyBaseOutskirt;
                }
                if (hero.NotFarAwayMonsters.Count() == 0)
                {
                    Console.Error.WriteLine("No NearBy Monster To Kill");
                    return new RandomMoveCommand();
                }
                int lowestHealth = hero.NotFarAwayMonsters.Min(m => m.Health);
                if (lowestHealth >= 14)
                {
                    _herosAction[hero.sn % 3] = behaviors.GoToEnemyBase;
                    return new MoveCommand(enemyBase.BaseX, enemyBase.BaseY);
                }
                //

                double maxDirectionScore = hero.moveDirectionScores.Max();

                for (int angle = 0; angle < 360; angle++)
                {
                    if (hero.moveDirectionScores[angle] == maxDirectionScore)
                    {
                        int nextX = (int)Math.Round(hero.X + 800.0 * Math.Cos(Helper.AngleToRadian(angle)), 0);
                        int nextY = (int)Math.Round(hero.Y + 800.0 * Math.Sin(Helper.AngleToRadian(angle)), 0);
                        return new MoveCommand(nextX, nextY);
                    }
                }
                throw new Exception();
                return new RandomMoveCommand();
            //Monster targetNearBy = hero.NotFarAwayMonsters.Where(m => lowestHealth == m.Health).FirstOrDefault();
            //return new MoveCommand(targetNearBy.X, targetNearBy.Y);
            case behaviors.GoToEnemyBase:
                Console.Error.WriteLine("GoToEnemyBase");
                if (distanceToEnemyBase <= 5500.0)
                {
                    // close enough
                    _herosAction[hero.sn % 3] = behaviors.AttackEnemyBase;
                }
                return new MoveCommand(enemyBase.BaseX, enemyBase.BaseY);
            case behaviors.StayEnemyBaseOutskirt:
                if (hero.X == enemyBaseEntry[attackAngel % 2].Item1 && hero.Y == enemyBaseEntry[attackAngel % 2].Item2)
                {
                    attackAngel += 1;
                }
                if (hero.NotFarAwayMonsters.Count() > 0)
                {
                    // too far
                    _herosAction[hero.sn % 3] = behaviors.AttackEnemyBase;
                }
                return new MoveCommand(enemyBaseEntry[attackAngel % 2].Item1, enemyBaseEntry[attackAngel % 2].Item2);
                break;
            case behaviors.AttackEnemyBase:
                Console.Error.WriteLine("AttackEnemyBase");
                if (hero.NearbyMonsters.Count() >= 2 && ourBase.Mana >= 10)
                {

                    double maxWindScore = hero.windDirectionScores.Max();

                    for (int angle = 0; angle < 360; angle++)
                    {
                        if (hero.windDirectionScores[angle] == maxWindScore)
                        {
                            int nextX = (int)Math.Round(hero.X + 2200.0 * Math.Cos(Helper.AngleToRadian(angle)), 0);
                            int nextY = (int)Math.Round(hero.Y + 2200.0 * Math.Sin(Helper.AngleToRadian(angle)), 0);
                            return new WindSpellCommand(nextX, nextY);
                        }
                    }

                    throw new Exception();
                    return new WindSpellCommand(enemyBase.BaseX, enemyBase.BaseY);
                }
                Monster ShieldingMonster = hero.NotFarAwayMonsters.Where(m => m.ShieldLife == 0 && Helper.GetDistanceFromBase(enemyBase, m) <= 4500.0).FirstOrDefault();
                if (ShieldingMonster != null)
                {
                    return new ShieldSpellCommand(ShieldingMonster.Id);
                }
                if (hero.NearbyMonsters.Count() >= 1 && hero.NotFarAwayEnemies.Count() == 0 && Helper.GetDistanceFromBase(enemyBase, hero) <= 5000.0 && ourBase.Mana >= 10)
                {
                    return new WindSpellCommand(enemyBase.BaseX, enemyBase.BaseY);
                }
                if (hero.NotFarAwayMonsters.Where(a => (Helper.GetDistanceFromBase(enemyBase, a) < 6000.0) && (Helper.GetDistanceFromBase(enemyBase, a) > 4000.0)).Count() == 0)
                {
                    Console.Error.WriteLine("No NearBy Monsters To Send To Enemy Base");
                    attackAngel += 1;
                    _herosAction[hero.sn % 3] = behaviors.StayEnemyBaseOutskirt;
                    return new RandomMoveCommand();
                }
                int maxMonsterHealth = hero.NotFarAwayMonsters.Where(a => (Helper.GetDistanceFromBase(enemyBase, a) < 6000.0) && (Helper.GetDistanceFromBase(enemyBase, a) > 4000.0)).Max(m => m.Health);
                if (maxMonsterHealth >= 18)
                {
                    _secondHalfGame = true;
                }


                double maxDirectionScore2 = hero.moveDirectionScores.Max();

                for (int angle = 0; angle < 360; angle++)
                {
                    if (hero.moveDirectionScores[angle] == maxDirectionScore2)
                    {
                        int nextX = (int)Math.Round(hero.X + 800.0 * Math.Cos(Helper.AngleToRadian(angle)), 0);
                        int nextY = (int)Math.Round(hero.Y + 800.0 * Math.Sin(Helper.AngleToRadian(angle)), 0);
                        return new MoveCommand(nextX, nextY);
                    }
                }
                throw new Exception();
                //Monster maxMonster = hero.NotFarAwayMonsters.Where(m => maxMonsterHealth == m.Health).FirstOrDefault();
                //return new MoveCommand(maxMonster.X, maxMonster.Y);
        }
        return new WaitCommand();
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
        gameManager.SetStrategy(new OriginalStrategy());
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
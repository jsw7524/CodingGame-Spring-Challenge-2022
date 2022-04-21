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
    public int baseX;
    public int baseY;
    public int health;
    public int mana;
    public int heroes;
}

public class Entity
{
    public int id;// Unique identifier
    public int type; // 0=monster, 1=your hero, 2=opponent hero
    public int x; // Position of this entity
    public int y;
    public int shieldLife; // Ignore for this league; Count down until shield spell fades
    public int isControlled; // Ignore for this league; Equals 1 when this entity is under a control spell
    public int health; // Remaining health of this monster
    public int vx; // Trajectory of this monster
    public int vy;
    public int nearBase; // 0=monster with no target yet, 1=monster targeting a base
    public int threatFor; // Given this monster's trajectory, is it a threat to 1=your base, 2=your opponent's base, 0=neither
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

        // game loop
        while (true)
        {
            for (int i = 0; i < 2; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int health = int.Parse(inputs[0]); // Your base health
                int mana = int.Parse(inputs[1]); // Ignore in the first league; Spend ten mana to cast a spell
            }
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
            }
            for (int i = 0; i < heroesPerPlayer; i++)
            {

                // Write an action using Console.WriteLine()
                // To debug: Console.Error.WriteLine("Debug messages...");


                // In the first league: MOVE <x> <y> | WAIT; In later leagues: | SPELL <spellParams>;
                Console.WriteLine("WAIT");
            }
        }
    }
}
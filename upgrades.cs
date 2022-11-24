using System;

class Game {
    /* public static void Main(string[] args) {
        Weapon heroWeapon = new Weapon("targetWeapon", 1, 10);
        Weapon droppedWeapon = new Weapon("dropped", 1);

        while (true) {
            Console.ReadLine();
            Forge.EnhanceItem(heroWeapon, 50);
        }
    } */
}

public static class Forge {
    public static void EnhanceItem(Item targetItem, int exp) {
        Console.WriteLine($"Предмет {targetItem.Name} был улучшен");
        targetItem.AddExp(exp);
    }

    public static void EnhanceItem(Item targetItem, Item sacrifice) {
        Console.WriteLine($"Предмет {targetItem.Name} был улучшен");
        targetItem.AddExp(20 * sacrifice.Level);
    }
}

public class Weapon : Item {
    private int baseDamage = 5;

    public Weapon() {
        Name = "Weapon";
        Level = 1;
        baseDamage = 5;
        Experience = 0;
    }
    
    public Weapon(string name) {
        Name = name;
        Level = 1;
        baseDamage = 5;
        Experience = 0;
    }

    public Weapon(string name, int level) {
        Name = name;
        Level = level;
        baseDamage = 5;
        Experience = 0;
    }

    public Weapon(string name, int level, int startDamage) {
        Name = name;
        Level = level;
        baseDamage = startDamage;
        Experience = 0;
    }

    public int GetDamage() {
        return baseDamage * Level;
    }
}

/* public class Item {
    public string Name;
    public int Level = 1;
    public int Experience = 0;

    public int ExperienceToNextLevel() {
        return 68 + ( 2 ^ (Level+5) );
    }

    public void AddExp(int exp) {
        Experience += exp;
        while (Experience + exp >= ExperienceToNextLevel()) {
            Experience -= ExperienceToNextLevel();
            Level++;
            Console.WriteLine($"Предмет {Name} повысил уровень, текущий уровень - {Level}");
        }
    }
} */


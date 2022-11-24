using System;

/* public static class Program {
    InventoryGame game = new InventoryGame();
} */

public class InventoryGame {
    /* public static void InventoryGame() {
        RunGame()
    } */

    /* private Item GetItemByName(string name) {
        // Найти вещь с подходящим именем и вернуть её;
        
        foreach (Item i in Items) {
         }
    } */
}

public class Bag {
    public static int Capacity = 1;

    Bag(int capacity) {
        Capacity = capacity;
    }

    public void AddItem(Item item) {
        // Добавить вещь в список вещей;
    }

    public void GetItemName(int slotID) {
        // Вернуть .Name подходящей по ID слота вещи;
    }
}

/* public class Item {
    public string Name;

    public Item(string name) {
        Name = name;
    }
} */

// Класс Вещи не только для инвентаря:
public class Item {
    public string Name;
    public int Level = 1;
    public int Experience = 0;
    public int X = 0;
    public int Y = 0;

    public Item() {
    }

    public Item(string name) {
        Name = name;
    }

    public Item(int x, int y, string name) {
        X=x;
        Y=y;
        Name = name;
    }

    /* public Item(string name, */

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
}

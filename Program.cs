using System;
using System.Collections.Generic;
using System.Linq;

public class VendingMachineState
{
    public List<Product> Products { get; set; } = new List<Product>();
    public decimal CurrentBalance { get; set; }
    public decimal TotalIncome { get; set; }
    public List<decimal> CoinStack { get; set; } = new List<decimal>();
    
    public VendingMachineState()
    {
        Products = new List<Product>
        {
            new Product { Id = 1, Name = "Cola", Price = 50, Quantity = 10 },
            new Product { Id = 2, Name = "Chips", Price = 30, Quantity = 5 },
            new Product { Id = 3, Name = "Water", Price = 25, Quantity = 8 }
        };
    }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}

public abstract class VendingMachineBase
{
    protected VendingMachineState State { get; } = new VendingMachineState();
    
    public bool IsValidCoin(decimal coin)
    {
        List<decimal> validCoinDenomination = new List<decimal> { 1, 2, 5, 10 };
        return validCoinDenomination.Contains(coin);
    }
}

public class SimpleVendingMachine : VendingMachineBase
{
    public void ShowProducts()
    {
        Console.WriteLine("---Доступные товары---");
        foreach (var product in State.Products)
        {
            Console.WriteLine($"--- {product.Id} --- {product.Name} --- {product.Price} руб. --- {product.Quantity} шт. ---");
        }
        Console.WriteLine($"Ваш текущий баланс: {State.CurrentBalance} руб.");
        Console.WriteLine("Команды:");
        Console.WriteLine("- Введите номинал монеты (1, 2, 5, 10) чтобы пополнить баланс");
        Console.WriteLine("- Введите 'finish' чтобы завершить пополнение");
        Console.WriteLine("- Введите 'AdminMode' для входа в режим администратора");
        Console.WriteLine("- Введите 'cancel' для отмены операции и возврата денег");
    }

    public void AddCoin(decimal coin)
    {
        if (IsValidCoin(coin))
        {
            State.CurrentBalance += coin;
            State.CoinStack.Add(coin);
            Console.WriteLine($"Ваш текущий баланс: {State.CurrentBalance} руб.");
        }
        else
        {
            Console.WriteLine("---Неверный номинал монеты---");
        }
    }

    public int BuyProduct(string? inputId)
    {
        if (inputId == null || !int.TryParse(inputId, out int id))
        {
            Console.WriteLine("---Неверный формат ID товара---");
            return 2;
        }

        var product = State.Products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            Console.WriteLine("---Товар не найден---");
            return 2;
        }

        if (product.Quantity <= 0)
        {
            Console.WriteLine("---Товар закончился---");
            return 2;
        }

        if (State.CurrentBalance >= product.Price)
        {
            State.CurrentBalance -= product.Price;
            State.TotalIncome += product.Price;
            product.Quantity--;
            Console.WriteLine("Поздравляем! Товар успешно приобретен");
            Console.WriteLine($"Ваш текущий баланс: {State.CurrentBalance} руб.");
            return 0;
        }
        else
        {
            Console.WriteLine($"Недостаточно средств на вашем балансе: {State.CurrentBalance} руб.");
            return 1;
        }
    }

    public void GiveChange(bool isCancellation = false)
    {
        decimal amountToReturn = State.CurrentBalance;
        
        if (amountToReturn <= 0)
        {
            Console.WriteLine("Нет средств для возврата");
            return;
        }
        
        if (isCancellation)
        {
            Console.WriteLine($"Возврат средств: {amountToReturn} руб.");
        }
        else
        {
            Console.WriteLine($"Выдача сдачи: {amountToReturn} руб.");
        }
        
        decimal[] coins = { 10, 5, 2, 1 };
        var change = new Dictionary<decimal, int>();
        
        decimal remainingAmount = amountToReturn;
        foreach (decimal coin in coins)
        {
            if (remainingAmount >= coin)
            {
                int count = (int)(remainingAmount / coin);
                change[coin] = count;
                remainingAmount -= count * coin;
            }
        }
        
        foreach (var coin in change)
        {
            Console.WriteLine($"  {coin.Key} руб. × {coin.Value} шт.");
        }
        
        State.CurrentBalance = 0;
        
        if (isCancellation)
        {
            State.CoinStack.Clear();
        }
    }

    public bool AdminMode()
    {
        Console.Write("Введите пароль: ");
        string? password = Console.ReadLine();
        
        if (password != "1234")
        {
            Console.WriteLine("Неверный пароль!");
            return false;
        }

        Console.WriteLine("Добро пожаловать в администраторский режим!");
        
        while (true)
        {
            Console.WriteLine("\n--- АДМИНИСТРАТОРСКИЙ РЕЖИМ ---");
            Console.WriteLine($"Общая выручка: {State.TotalIncome} руб.");
            Console.WriteLine("1 - Собрать средства");
            Console.WriteLine("2 - Пополнить ассортимент");
            Console.WriteLine("3 - Выйти из режима администратора");
            Console.Write("Выберите действие: ");
            
            string? choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    CollectMoney();
                    return true; // Завершаем работу программы
                case "2":
                    RestockProducts();
                    break;
                case "3":
                    Console.WriteLine("Выход из администраторского режима");
                    return false;
                default:
                    Console.WriteLine("Неверная команда!");
                    break;
            }
        }
    }

    private void CollectMoney()
    {
        decimal amountToCollect = State.TotalIncome;
        
        if (amountToCollect <= 0)
        {
            Console.WriteLine("Нет средств для сбора");
            return;
        }
        
        Console.WriteLine($"\nСбор средств: {amountToCollect} руб.");
        
        decimal[] coins = { 10, 5, 2, 1 };
        var collection = new Dictionary<decimal, int>();
        
        decimal remainingAmount = amountToCollect;
        foreach (decimal coin in coins)
        {
            if (remainingAmount >= coin)
            {
                int count = (int)(remainingAmount / coin);
                collection[coin] = count;
                remainingAmount -= count * coin;
            }
        }
        
        Console.WriteLine("Собранные средства:");
        foreach (var coin in collection)
        {
            Console.WriteLine($"  {coin.Key} руб. × {coin.Value} шт.");
        }
        
        State.TotalIncome = 0;
        State.CoinStack.Clear();
        Console.WriteLine("Все средства собраны. Программа завершает работу.");
    }

    private void RestockProducts()
    {
        Console.WriteLine("\n--- ПОПОЛНЕНИЕ АССОРТИМЕНТА ---");
        
        while (true)
        {
            Console.WriteLine("\nТекущий ассортимент:");
            foreach (var product in State.Products)
            {
                Console.WriteLine($"--- {product.Id} --- {product.Name} --- {product.Price} руб. --- {product.Quantity} шт. ---");
            }
            
            Console.WriteLine("\n1 - Добавить новый товар");
            Console.WriteLine("2 - Пополнить существующий товар");
            Console.WriteLine("3 - Вернуться в меню администратора");
            Console.Write("Выберите действие: ");
            
            string? choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    AddNewProduct();
                    break;
                case "2":
                    RestockExistingProduct();
                    break;
                case "3":
                    return;
                default:
                    Console.WriteLine("Неверная команда!");
                    break;
            }
        }
    }

    private void AddNewProduct()
    {
        Console.Write("Введите ID нового товара: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Неверный формат ID!");
            return;
        }

        if (State.Products.Any(p => p.Id == id))
        {
            Console.WriteLine("Товар с таким ID уже существует!");
            return;
        }

        Console.Write("Введите название товара: ");
        string? name = Console.ReadLine();
        if (string.IsNullOrEmpty(name))
        {
            Console.WriteLine("Название не может быть пустым!");
            return;
        }

        Console.Write("Введите цену товара: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal price))
        {
            Console.WriteLine("Неверный формат цены!");
            return;
        }

        Console.Write("Введите количество товара: ");
        if (!int.TryParse(Console.ReadLine(), out int quantity))
        {
            Console.WriteLine("Неверный формат количества!");
            return;
        }

        State.Products.Add(new Product { Id = id, Name = name, Price = price, Quantity = quantity });
        Console.WriteLine("Товар успешно добавлен!");
    }

    private void RestockExistingProduct()
    {
        Console.Write("Введите ID товара для пополнения: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Неверный формат ID!");
            return;
        }

        var product = State.Products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            Console.WriteLine("Товар не найден!");
            return;
        }

        Console.Write($"Текущее количество {product.Name}: {product.Quantity} шт.");
        Console.Write("Введите количество для добавления: ");
        if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
        {
            Console.WriteLine("Неверный формат количества!");
            return;
        }

        product.Quantity += quantity;
        Console.WriteLine($"Товар успешно пополнен. Новое количество: {product.Quantity} шт.");
    }
}

public class Program
{
    public static void Main()
    {
        var machine = new SimpleVendingMachine();

        while (true)
        {
            Console.WriteLine("-------------------");
            Console.WriteLine("Добро пожаловать!");
            machine.ShowProducts();

            string? input;
            while (true)
            {
                input = Console.ReadLine();
                if (input == "finish" || input == "Finish") 
                    break;
                
                if (input == "AdminMode" || input == "adminmode")
                {
                    if (machine.AdminMode())
                    {
                        return; // Завершаем программу если администратор собрал средства
                    }
                    break; // Выходим из цикла ввода монет и показываем товары заново
                }
                
                if (input != null && decimal.TryParse(input, out decimal coin))
                    machine.AddCoin(coin);
                else
                    Console.WriteLine("Неверная команда или формат монеты");
            }

            Console.WriteLine("Введите ID товара. Для отмены операции введите 'cancel'");

            while (true)
            {
                input = Console.ReadLine();
                if (input == null) continue;
                
                if (input == "cancel" || input == "Cancel")
                {
                    machine.GiveChange(true);
                    break;
                }

                int result = machine.BuyProduct(input);
                if (result == 0)
                {
                    break;
                }
                else if (result == 1)
                {
                    Console.WriteLine("Хотите добавить еще монет? (yes/no)");
                    string? answer = Console.ReadLine();
                    if (answer != null && (answer == "yes" || answer == "Yes"))
                    {
                        Console.WriteLine("Введите монеты (введите 'finish' когда закончите):");
                        while (true)
                        {
                            string? coinInput = Console.ReadLine();
                            if (coinInput == "finish" || coinInput == "Finish") 
                                break;
                            
                            if (coinInput != null && decimal.TryParse(coinInput, out decimal coin))
                                machine.AddCoin(coin);
                        }
                    }
                    else
                    {
                        machine.GiveChange(true);
                        break;
                    }
                }
            }

            Console.WriteLine("Если вы хотите продолжить покупки, нажмите 'продолжить', если вы готовы завершить обслуживание, нажмите 'завершить'");
            input = Console.ReadLine();
            if (input != null && (input == "завершить" || input == "Завершить"))
            {
                machine.GiveChange(false);
                Console.WriteLine("Спасибо, что воспользовались нашими услугами, хорошего дня!");
                break;
            }
        }
    }
}
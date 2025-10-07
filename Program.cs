using System;
using System.Collections.Generic;
using System.Linq;

public class VendingMachineState
{
    public List<Product> Products { get; set; } = new List<Product>();
    public decimal CurrentBalance { get; set; }
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
        Console.WriteLine("Чтобы пополнить баланс, введите номинал монеты. Когда закончите, введите 'finish'");
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
                
                if (input != null && decimal.TryParse(input, out decimal coin))
                    machine.AddCoin(coin);
                else
                    Console.WriteLine("Неверный формат монеты");
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
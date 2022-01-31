//R.I.P C++ macros
using System;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;

namespace Cards
{
    public enum Difficulty { Easy, Normal, Hard, Impossible }

    class Program
    {
        private static int playerCount = 2;
        private static Player[] players;
        private static bool End = false;
        private static Settings settings = new Settings();

        static void Main(string[] args)
        {
            Table.Init();
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Vítej v Prší Master!\n1. Hrát\n2. Záznamy her");
                if (GetUserIntInRange(1, 2) == 2)
                {
                    bool satisfied = false;
                    while (!satisfied)
                    {
                        Console.WriteLine("\n0. Back\n1. Záznamy\n2. Upravit cestu k záznamům");
                        Console.WriteLine();
                        switch (GetUserIntInRange(0, 2))
                        {
                            case 0:
                                satisfied = true;
                                break;
                            case 1:
                                Console.WriteLine($"0. Back\n1. Vypnout záznamy = {settings.CanLog}\n2. Upravit cestu k záznamům = {settings.LogPath}");
                                switch(GetUserIntInRange(1, 2))
                                {
                                    case 0:
                                        satisfied = true;
                                        break;
                                    case 1:
                                        break;
                                    case 2:
                                        break;
                                }
                                break;
                            case 2:
                                Console.Clear();
                                break;
                        }
                    }
                }
                else
                {
                    if (RunGame())
                        Environment.Exit(0x0);
                    else
                        RunGame();
                }
            }
        }

        private static bool RunGame()
        {
            Console.Clear();
            //TODO: Difficulty
            Console.Write("Hrajeme!\nNapiš počet hráčů: ");
            bool satisfied = false;
            while (!satisfied)
            {
                try
                {
                    playerCount = int.Parse(Console.ReadLine());
                    if (playerCount >= 2 && playerCount <= 6)
                        satisfied = true;
                    else
                        throw new FormatException();
                }
                catch
                {
                    Console.Write("Napiš číslo, které je větší než 1 a menší než 7: ");
                }
            }
            Console.WriteLine("Napiš názvy hráčů: ");
            players = new Player[playerCount];
            for (int i = 0; i < playerCount; i++)
            {
                Console.Write($"Hráč {i + 1}: ");
                players[i] = new Player(Console.ReadLine());
            }
            Console.WriteLine("Rozdávám karty...");
            if (settings.CanLog)
            {
                string playerNames = String.Empty;
                foreach(Player p in players)
                {
                    playerNames += p.Username + " ";
                }
                Table.Log($"Hra začala s {playerCount} hráči: {playerNames}");
            }
            Random r = new Random();
            int currentPlayer = r.Next(0, playerCount);
            while (!End)
            {
                PlayerTurn(currentPlayer);
                if (End)
                    break;
                if (currentPlayer + 1 < playerCount)
                    currentPlayer++;
                else
                    currentPlayer = 0;
            }
            Console.WriteLine($"Hráč {players[currentPlayer].Username} vyhrál!");
            Console.WriteLine("Chcete si uložit záznam o hře?\n1. Ano\n2. Ne");
            if (GetUserIntInRange(1, 2) == 1)
                SaveLog();
            Console.WriteLine("Chcete si zahrát znovu?\n1. Ano\n2. Ne");
            if (GetUserIntInRange(1, 2) == 1)
                return false;
            else
                return true;
        }

        private static void PlayerTurn(int index)
        {
            Console.Clear();
            if (Table.RemainingCards() == 0)
            {
                Console.WriteLine("Otáčím balíček...");
                Table.FlipDeck();
                Console.WriteLine("Balíček otočen\n");
            }
            Console.WriteLine($"Hráč {players[index].Username} je na tahu\nPro začátek tvého tahu zmáčkni cokoli...");
            Console.ReadKey();
            Console.Clear();
            int nextPlayer = index + 1;
            int previousPlayer = index - 1;
            if (index - 1 < 0)
            {
                previousPlayer = playerCount - 1;
            }
            if (index + 1 > playerCount - 1)
            {
                nextPlayer = 0;
            }
            switch (players[index].CurrentState)
            {
                case State.Normal:
                    break;
                case State.Stopped:
                    Console.WriteLine($"HRÁČ {players[previousPlayer].Username} NA VÁS POUŽIL ESO!");
                    Thread.Sleep(2000);
                    break;
                case State.Taking:
                    Console.WriteLine($"HRÁČ {players[previousPlayer].Username} NA VÁS POUŽIL SEDMU!");
                    Thread.Sleep(2000);
                    break;
            }
            if (Table.Multiplier != 0)
            {
                Console.WriteLine($"MOMENTÁLNÍ NÁSOBIČ KARET ZE ZAHRANÝCH SEDMIČEK: {Table.Multiplier}x\n");
            }
            Console.WriteLine($"ZBÝVÁ {Table.RemainingCards()} | ZAHRÁNO {Table.ThrowedCards()} | ODHAZOVACÍ BALÍK: {Table.GetFirstDisposed()}\n");
            if (Table.Changing)
            {
                switch (Table.ChangedColor)
                {
                    case CardColorType.Kule:
                        Console.WriteLine($"\u001b[34mZMĚNĚNO NA {Table.ChangedColor.ToString().ToUpper()}\u001b[0m");
                        break;
                    case CardColorType.Srdce:
                        Console.WriteLine($"\u001b[31mZMĚNĚNO NA {Table.ChangedColor.ToString().ToUpper()}\u001b[0m");
                        break;
                    case CardColorType.Žaludy:
                        Console.WriteLine($"\u001b[33mZMĚNĚNO NA {Table.ChangedColor.ToString().ToUpper()}\u001b[0m");
                        break;
                    case CardColorType.Zelí:
                        Console.WriteLine($"\u001b[32mZMĚNĚNO NA {Table.ChangedColor.ToString().ToUpper()}\u001b[0m");
                        break;
                }
                Thread.Sleep(2000);
            }
            if (players[index].CurrentState != State.Stopped)
                if (players[index].CurrentState == State.Taking)
                    Console.WriteLine("0. LÍZNOUT SI KARTY");
                else
                    Console.WriteLine("0. LÍZNOUT SI KARTU");
            else
                Console.WriteLine("0. STÁT");
            Console.WriteLine("TVÉ KARTY: ");
            int count = 1;
            foreach (Card c in players[index].Hand)
            {
                Console.WriteLine($"{count}. {c}");
                count++;
            }
            Console.WriteLine();
            bool goodToProceed = false;
            if (players[index].CurrentState == State.Stopped)
            {
                if (players[index].Hand.Exists(card => card.Type == CardType.Eso))
                    goodToProceed = true;
                else
                {
                    Console.WriteLine("Momentálně nemáte v ruce Eso. Budete muset čekat.");
                }
            }
            else if (players[index].CurrentState == State.Taking)
            {
                if (players[index].Hand.Exists(card => card.Type == CardType.Sedmička))
                    goodToProceed = true;
                else
                {
                    Console.WriteLine($"Momentálně nemáte v ruce Sedmičku. Budete muset brát {Table.Multiplier} karet.");
                    foreach (Card c in Table.TakeCards(Table.Multiplier))
                    {
                        Console.WriteLine($"Líznul jste si {c}");
                        players[index].Hand.Add(c);
                    }
                }
            }
            if (players[index].CurrentState == State.Normal || goodToProceed)
            {
                bool good = false;
                while (!good)
                {
                    int position = GetUserIntInRange(0, players[index].Hand.Count);
                    if (position == 0 && players[index].CurrentState != State.Stopped)
                    {
                        Card[] c;
                        if (Table.Multiplier == 0)
                            c = Table.TakeCards(1);
                        else
                            c = Table.TakeCards(Table.Multiplier);
                        Console.WriteLine();
                        foreach (Card cc in c)
                        {
                            Console.WriteLine($"Líznul jste si {cc}");
                            players[index].Hand.Add(cc);
                        }
                        good = true;
                    }
                    else if (players[index].CurrentState == State.Stopped && position == 0)
                    {
                        Console.WriteLine("Stojíte...");
                        players[index].CurrentState = State.Normal;
                        good = true;
                    }
                    else
                    {
                        position--;
                        if (players[index].CurrentState == State.Taking && players[index].Hand[position].Type != CardType.Sedmička)
                        {
                            Console.WriteLine("\nByla na Vás zahrána Sedmička.\nMomentálně tuto kartu nemůžete použít!");
                        }
                        else if (players[index].CurrentState == State.Stopped && players[index].Hand[position].Type != CardType.Eso)
                        {
                            Console.WriteLine("\nBylo na Vás zahráno Eso.\nMomentálně tuto kartu nemůžete použít!");
                        }
                        else
                        {
                            if (players[index].Hand[position].Type == CardType.Měňák)
                            {
                                Console.WriteLine("Vyber barvu na kterou chceš změnit:\n1. \u001b[31mSrdce\u001b[0m\n2. \u001b[34mKule\u001b[0m\n3. \u001b[32mZelí\u001b[0m\n4. \u001b[33mŽaludy\u001b[0m");
                                switch (GetUserIntInRange(1, 4))
                                {
                                    case 1:
                                        players[index].Hand[position].ChangerColor = CardColorType.Srdce;
                                        break;
                                    case 2:
                                        players[index].Hand[position].ChangerColor = CardColorType.Kule;
                                        break;
                                    case 3:
                                        players[index].Hand[position].ChangerColor = CardColorType.Zelí;
                                        break;
                                    case 4:
                                        players[index].Hand[position].ChangerColor = CardColorType.Žaludy;
                                        break;
                                }
                            }
                            Console.WriteLine();
                            Table.PlayCardResponse response = Table.PlayCard(players[index].Hand[position]);
                            good = response.Success;
                            if (!good)
                            {
                                Console.WriteLine($"Tuto kartu nemůžeš zahrát z důvodu: {response.Message}");
                            }
                            else
                            {
                                players[index].RemoveCard(players[index].Hand[position]);
                                players[nextPlayer].CurrentState = response.NextPlayer;
                            }
                            if (players[index].Hand.Count == 0)
                            {
                                End = true;
                            }
                        }
                    }
                }
            }
            players[index].CurrentState = State.Normal;
            Console.WriteLine("Pro předání tahu zmáčkni cokoli...");
            Console.ReadKey();
        }
        private static int GetUserIntInRange(int min, int max)
        {
            Console.WriteLine($"Napiš číslo akce/věci {min}-{max}: ");
            int choice = 0;
            while (!int.TryParse(Console.ReadLine(), out choice) || !(choice <= max) || !(choice >= min))
            {
                Console.WriteLine($"Zadané číslo je neplatné zkus to znovu {min}-{max}: ");
            }
            return choice;
        }
        private static void SaveLog()
        {

        }
    }
}
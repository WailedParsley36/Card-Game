using System;
using System.Collections.Generic;

public static class Table
{
    private static List<string> messages = new List<string>();
    private static List<Card> all = new List<Card>();
    private static List<Card> disposed = new List<Card>();
    private static List<Card> freshDispose = new List<Card>();
    public static int RemainingCards()
    {
        return disposed.Count;
    }
    public static int ThrowedCards()
    {
        return freshDispose.Count;
    }

    public static int Multiplier { get; private set; }
    public static CardColorType ChangedColor { get; private set; }
    public static bool Changing { get; private set; }

    public static void Init()
    {
        foreach (CardColorType cardColorType in Enum.GetValues(typeof(CardColorType)))
        {
            foreach (CardType cardType in Enum.GetValues(typeof(CardType)))
            {
                all.Add(new Card(cardType, cardColorType));
            }
        }
        Random rnd = new Random();
        Shuffle(20);
        if (freshDispose[0].Type == CardType.Měňák)
            ChangedColor = freshDispose[0].Color;
    }

    public static bool IsDisposed(CardType type, CardColorType color)
    {
        return disposed.Exists(card => card.Type == type && card.Color == color);
    }

    public static Card[] TakeCards(int count)
    {
        Multiplier = 0;
        Card[] cards = new Card[count];
        int offset = 1;
        if (count > disposed.Count)
        {
            for (offset = 0; offset < count - disposed.Count; offset++)
            {
                cards[offset] = disposed[disposed.Count - 1];
                disposed.RemoveAt(disposed.Count - 1);
            }
            Console.WriteLine($"Musím otočit balíček...");
            FlipDeck();
            Console.WriteLine("Balíček otočen\n");
        }
        for (int i = offset - 1; i < count; i++)
        {
            cards[i] = disposed[disposed.Count - 1];
            disposed.RemoveAt(disposed.Count - 1);
        }
        return cards;
    }

    public static void Shuffle(int times)
    {
        Random r = new Random();
        if (disposed.Count < 32)
        {
            disposed.Clear();
            int firstC = r.Next(0, all.Count);
            freshDispose.Add(all[firstC]);
            disposed = all;
            disposed.RemoveAt(firstC);
        }
        for (int i = 0; i < times; i++)
        {
            for (int index = 0; index < disposed.Count; index++)
            {
                int firstIndex = r.Next(0, disposed.Count);
                int secondIndex = r.Next(0, disposed.Count);
                Card temp = disposed[firstIndex];
                disposed[firstIndex] = disposed[secondIndex];
                disposed[secondIndex] = temp;
            }
        }
    }

    public static void FlipDeck()
    {
        Card temp = GetFirstDisposed();
        disposed.Clear();
        foreach(Card c in freshDispose)
        {
            disposed.Add(c);
        }
        freshDispose.Clear();
        freshDispose.Add(temp);
    }

    public static PlayCardResponse PlayCard(Card c)
    {
        CardColorType cardColor = GetFirstDisposed().Color;
        if (Changing)
            cardColor = ChangedColor;
        Changing = false;
        if (c.Type == CardType.Měňák)
        {
            string message = $"Zahrál jsi {c} a změnil na ";
            switch (c.ChangerColor)
            {
                case CardColorType.Srdce:
                    message += "\u001b[31mSrdce\u001b[0m ";
                    break;
                case CardColorType.Kule:
                    message += "\u001b[34mKule\u001b[0m ";
                    break;
                case CardColorType.Žaludy:
                    message += "\u001b[33mŽaludy\u001b[0m ";
                    break;
                case CardColorType.Zelí:
                    message += "\u001b[32mZelí\u001b[0m ";
                    break;
            }
            Console.WriteLine(message);
            Changing = true;
            ChangedColor = c.ChangerColor;
            freshDispose.Add(c);
            return new PlayCardResponse(true);
        }
        else if (c.Color == cardColor || c.Type == GetFirstDisposed().Type)
        {
            switch (c.Type)
            {
                default:
                    freshDispose.Add(c);
                    Console.WriteLine($"Zahrál jsi {c}");
                    return new PlayCardResponse(true);
                case CardType.Sedmička:
                    freshDispose.Add(c);
                    Console.WriteLine($"Zahrál jsi {c}");
                    Multiplier += 2;
                    return new PlayCardResponse(true, State.Taking);
                case CardType.Eso:
                    freshDispose.Add(c);
                    Console.WriteLine($"Zahrál jsi {c}");
                    return new PlayCardResponse(true, State.Stopped);
            }
        }
        else if (c.Color != cardColor)
        {
            //TODO: Switch with different errors
            return new PlayCardResponse(false, $"Nemůžeš zahrát {c.Color} na {cardColor}!");
        }
        else
        {
            //TODO: Switch with different errors
            return new PlayCardResponse(false, $"Nemůžeš zahrát {c.Type} na {GetFirstDisposed().Type}!");
        }
    }

    public static Card GetFirstDisposed()
    {
        //if (freshDispose.Count > 0)
            return freshDispose[freshDispose.Count - 1];
        //else { TODO: Handle no cards }
    }

    public class PlayCardResponse
    {
        public bool Success = false;
        public string Message = "Není uvedeno";
        public State NextPlayer = State.Normal;
        public PlayCardResponse(bool success, State state)
        {
            Success = success;
            NextPlayer = state;
        }
        public PlayCardResponse(bool success, string message)
        {
            Success = success;
            Message = message;
        }
        public PlayCardResponse(bool success)
        {
            Success = success;
        }
    }

    public static void Log(string message)
    {
        messages.Add(message);
    }
}



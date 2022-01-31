#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()

public class Card
{
    private string CardName;
    public CardType Type { get; private set; }
    public CardColorType Color { get; private set; }
    public CardColorType ChangerColor;
    
    public Card(CardType type, CardColorType color)
    {
        Type = type;
        Color = color;
        switch (color)
        {
            case CardColorType.Srdce:
                CardName = "\u001b[31mSrdcová/ý/é\u001b[0m " + type.ToString();
                break;
            case CardColorType.Kule:
                CardName = "\u001b[34mKulová/ý/é\u001b[0m " + type.ToString();
                break;
            case CardColorType.Žaludy:
                CardName = "\u001b[33mŽaludová/ý/é\u001b[0m " + type.ToString();
                break;
            case CardColorType.Zelí:
                CardName = "\u001b[32mZelená/ý/é\u001b[0m " + type.ToString();
                break;
        }
    }

    public override string ToString()
    {
        return CardName;
    }

    public override bool Equals(object obj)
    {
        
        Card c = (Card)obj;
        if (Type == c.Type)
            if (Color == c.Color)
                return true;
            else
                return false;
        else
            return false;
    }
    public static bool operator == (Card c1, Card c2)
    {
        if (c1.Type == c2.Type)
            if (c1.Color == c2.Color)
                return true;
            else
                return false;
        else
            return false;
    }
    public static bool operator != (Card c1, Card c2)
    {
        if (c1.Type != c2.Type)
            if (c1.Color != c2.Color)
                return true;
            else
                return false;
        else
            return false;
    }
}

public enum CardType { Sedmička, Osma, Devítka, Desítka, Spodek, Měňák, Král, Eso }
public enum CardColorType { Srdce, Kule, Žaludy, Zelí }
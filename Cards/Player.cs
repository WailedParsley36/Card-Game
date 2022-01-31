using System.Collections.Generic;

class Player
{
    public List<Card> Hand = new List<Card>();
    public string Username = "Player";
    public State CurrentState = State.Normal;

    public Player(string username)
    {
        Username = username;
        foreach (Card c in Table.TakeCards(4))
        {
            Hand.Add(c);
        }
    }

    public void RemoveCard(Card c)
    {
        Hand.RemoveAt(Hand.FindIndex(card => card.Color == c.Color && card.Type == c.Type));
    }
}

public enum State { Normal, Taking, Stopped }

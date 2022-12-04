module FoolGame.Core.Card

open System.Text.Json.Serialization

[<JsonConverter(typeof<JsonStringEnumConverter>)>]
type CardSuit =
    | Hearts =0
    | Diamonds = 1
    | Clubs = 2
    | Spades = 3

[<JsonConverter(typeof<JsonStringEnumConverter>)>]
type CardRank =
    | Six = 0
    | Seven = 1
    | Eight = 2
    | Nine = 3
    | Ten = 4 
    | Jack = 5
    | Queen = 6
    | King = 7
    | Ace = 8

// [<StructuredFormatDisplay("{DisplayText}")>]
type Card =
    { Suit: CardSuit
      Rank: CardRank }
    
    override this.ToString() = $"{this.Rank} of {this.Suit}"

let suits =
    [ CardSuit.Diamonds; CardSuit.Clubs; CardSuit.Diamonds; CardSuit.Hearts ]

let ranks =
    [ CardRank.Six
      CardRank.Seven
      CardRank.Eight
      CardRank.Nine
      CardRank.Ten
      CardRank.Jack
      CardRank.Queen
      CardRank.King
      CardRank.Ace ]

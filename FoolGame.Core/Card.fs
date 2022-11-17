module FoolGame.Core.Card

open System.Text.Json.Serialization

[<JsonConverter(typeof<JsonStringEnumConverter>)>]
type CardSuit =
    | Hearts
    | Diamonds
    | Clubs
    | Spades

[<JsonConverter(typeof<JsonStringEnumConverter>)>]
type CardRank =
    | Six
    | Seven
    | Eight
    | Nine
    | Ten
    | Jack
    | Queen
    | King
    | Ace

// [<StructuredFormatDisplay("{DisplayText}")>]
type Card =
    { Suit: CardSuit
      Rank: CardRank }
    
    override this.ToString() = $"{this.Rank} of {this.Suit}"

let suits =
    [ Diamonds; Clubs; Diamonds; Hearts ]

let ranks =
    [ Six
      Seven
      Eight
      Nine
      Ten
      Jack
      Queen
      King
      Ace ]

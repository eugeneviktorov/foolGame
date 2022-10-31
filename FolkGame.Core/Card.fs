module FolkGame.Core.Card

type CardSuit =
    | Hearts
    | Diamonds
    | Clubs
    | Spades

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

[<StructuredFormatDisplay("{DisplayText}")>]
type Card =
    { Suit: CardSuit
      Rank: CardRank }
    member this.DisplayText = this.ToString()
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

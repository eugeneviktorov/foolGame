module FoolGame.Core.Game

open FoolGame.Core.Card
open FoolGame.Core.Player

type MovedCard = { Card: Card; Player: Player }

type Move =
    | TakeFromDeck of MovedCard
    | Play of MovedCard
    | Beat of Beaten: MovedCard * Played: MovedCard
    | Take
    | Done

type Game =
    { Deck: Card list
      Players: Player list
      AttackPlayer: Player option
      DefencePlayer: Player option
      Moves: Move list
      GeneralCard: Card }

    static member DefaultDeck =
        [ for suit in suits do
               for rank in ranks do
                   yield { Suit = suit; Rank = rank } ]
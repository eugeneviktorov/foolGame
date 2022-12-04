// open System
// open System.Text.RegularExpressions
// open FoolGame.Core
// open Microsoft.FSharp.Collections
// open Microsoft.FSharp.Core
// open Card
// open Player
// open Game
//
// let rec continueGame (game: Game) =
//
//     if List.isEmpty game.Deck
//        && game.Players
//           |> List.map (fun x -> List.isEmpty x.Hand)
//           |> List.forall id then
//         printfn "game done"
//         ()
//
//     let (attackPlayer, defencePlayer, game) =
//         match (game.AttackPlayer, game.DefencePlayer, game) with
//         | None, None, _ ->
//             updateAttackPlayer game.Players[0] game
//             |> fun g -> (unwrap g.AttackPlayer, unwrap g.DefencePlayer, g)
//         | Some (attack), Some (defence), _ -> (attack, defence, game)
//         | _ -> failwith "game is corrupted"
//
//     printfn $"Attack player is %s{attackPlayer.Name}"
//     printfn $"Defence player is %s{defencePlayer.Name}"
//
//     match Console.ReadLine() with
//     | "state" ->
//         printfn $"Now Game in action current player is %s{attackPlayer.Name}"
//         ()
//     | "table" ->
//         let table =
//             getTable game
//             |> Seq.map (fun (card, beat) ->
//                 (card.Card,
//                  match beat with
//                  | Some (x) -> Some(x.Card)
//                  | None -> None))
//
//         printfn $"On table now: %A{table}"
//         continueGame game
//     | ParseRegex "beat (\d+) (\d+)" r ->
//         let tableIndex =
//             (Int32.Parse r.Groups[1].Value - 1)
//
//         let handIndex =
//             (Int32.Parse r.Groups[2].Value - 1)
//
//         let hand =
//             List.removeAt handIndex defencePlayer.Hand
//
//         let handCard = defencePlayer.Hand[handIndex]
//
//         let tableCard =
//             (Seq.toArray (getTable game))[tableIndex]
//
//         match tableCard with
//         | card, beaten when beaten = None ->
//             if (canBeat card.Card handCard game.GeneralCard.Suit) then
//                 printfn $"You just beat %A{card.Card} by {handCard}"
//
//                 updatePlayer defencePlayer { defencePlayer with Hand = hand } game
//                 |> beatCard
//                     card
//                     { Card = handCard
//                       Player = defencePlayer }
//             else
//                 printfn $"You cant use card %A{handCard} for beat %A{card.Card}"
//                 game
//         | _ -> game
//         |> continueGame
//     | ParseRegex "play (\d+)" cardNumber ->
//         let index =
//             (Int32.Parse cardNumber.Groups[1].Value - 1)
//
//         let hand = attackPlayer.Hand
//         let card = hand[index]
//         printfn $"You just pushed %A{card}"
//         let hand = (List.removeAt index hand)
//         printfn $"After the operation you hand is %A{hand}"
//
//         registerMove (Play({ Card = card; Player = attackPlayer })) game
//         |> updatePlayer attackPlayer { attackPlayer with Hand = hand }
//         |> continueGame
//     | ParseRegex "hand (\d+)" playerNumber ->
//         let playerIndex =
//             (Int32.Parse playerNumber.Groups[1].Value - 1)
//
//         let player = game.Players[playerIndex]
//         printfn $"%A{player.Hand}"
//         (continueGame game)
//     | "deck" ->
//         printfn $"In deck remains %i{game.Deck.Length} cards"
//         continueGame game
//     | "done" ->
//         registerMove (Done) game
//         |> fillPlayersHand
//         |> fun g -> updateAttackPlayer (nextPlayer attackPlayer g) g
//         |> continueGame
//     | "take" ->
//         ([], getTable game)
//         ||> Seq.fold (fun x (play, beat) ->
//             (x
//              @ [ Some(play.Card)
//                  match beat with
//                  | Some (x) -> Some(x.Card)
//                  | None -> None ]))
//         |> Seq.choose id
//         |> Seq.toList
//         |> (fun x -> (updatePlayer defencePlayer { defencePlayer with Hand = (defencePlayer.Hand @ x) } game))
//         |> registerMove (Take)
//         |> continueGame
//     | "moves" ->
//         printfn $"Moves : %A{game.Moves}"
//         (continueGame game)
//     | "exit" -> ()
//     | _ -> (continueGame game)
//
// let currentDeck = shuffle Game.DefaultDeck
//
// let game =
//     { Deck = currentDeck
//       Moves = []
//       AttackPlayer = None
//       DefencePlayer = None
//       GeneralCard = List.last currentDeck
//       Players =
//         [ { Hand = []; Name = "Player1" }
//           { Hand = []; Name = "Player2" } ] }
//
// fillPlayersHand game |> continueGame

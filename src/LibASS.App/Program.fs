﻿// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
open LibASS.Contracts
open LibASS.Domain.CommandHandling
open LibASS.Domain.Types
open System

[<EntryPoint>]
let main argv = 
    let eventStore = createEventStore<EventData, Error> (Error.VersionConflict "Version conflict")

    let logSubscriber e = 
        printfn "Hey ho! Lets go!"
        printfn "%A" e
        printfn "We went!"

    eventStore.AddSubscriber "logsub" logSubscriber

    let newGuid() = Guid.NewGuid()
    let newAggId() = AggregateId (newGuid())


//    let createLoanTestData() = 
    let loanGuid = newGuid()
    let userId = UserId (newGuid())
    let itemId = ItemId (newGuid())
    let libraryId = LibraryId (newGuid())
    let aggId = AggregateId loanGuid
    let loan = { LoanId = LoanId loanGuid
                 UserId = userId
                 ItemId = itemId
                 LibraryId = libraryId }

    let item = ( loan.ItemId,
                 Book 
                    { Title = Title "A book"
                      Author = Author "A author"})

    let dependencies = 
        { GetItem = (fun _ ->  NotImplemented "DELETE ME" |> fail )}

    let executer = execute eventStore dependencies

    let newGuid() = Guid.NewGuid()
    let loanId = LoanId (newGuid())
    let commandData = LoanItem(loanId, UserId (newGuid()), ItemId (newGuid()), LibraryId (newGuid()))
    let aggId = AggregateId (Guid.NewGuid())

    let result = (aggId, commandData) |> executer
    let returnResult = (aggId, ReturnItem loanId) |> executer

    Console.ReadLine() |> ignore
    0 // return an integer exit code

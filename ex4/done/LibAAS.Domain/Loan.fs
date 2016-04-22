﻿module internal LibAAS.Domain.Loan
open LibAAS.Contracts
open LibAAS.Domain.DomainTypes
open System

let handleAtInit stateGetters ((aggId:AggregateId), (commandData:LoanItem)) = 
// It is much more safe if we make the type of the command data explcit.
// It will be checked at compile time that we don't pass a command to the wrong command handler.
    commandData.ItemId |> 
        (stateGetters.GetInventoryItem
            >=> function
                | ItemInit -> InvalidItem |> fail
                | _ ->
                let loan = 
                    { LoanId = commandData.Id
                      UserId = commandData.UserId
                      ItemId = commandData.ItemId
                      LibraryId = commandData.LibraryId }
                let now = DateTime.Today
                [ItemLoaned (loan, LoanDate now, DueDate (now.AddDays(7.)))] |> ok)

let handleAtCreated data ((aggId:AggregateId), commandData) =
    match commandData with
    | ReturnItem cData -> 
        let now = DateTime.Today
        let (DueDate duedate) = data.DueDate
        let daysLate = (now - duedate).Days
        let fine = 100 * daysLate
        if now > duedate then 
            [ItemLate (data.Loan, ReturnDate now, daysLate, Fine fine )] |> ok
        else 
            [ItemReturned (data.Loan, ReturnDate now )] |> ok
    | _ -> InvalidState "Loan at created" |> fail

let executeCommand state stateGetters command =
    match state, command with
    | LoanInit, (id, LoanItem data) -> handleAtInit stateGetters (id, data)
    | LoanCreated data, cmd -> cmd |> handleAtCreated data
    | _ -> InvalidState "Loan" |> fail

let evolveAtInit = function
    | ItemLoaned (loan, loanDate, dueDate) -> 
        LoanCreated {Loan = loan; DueDate = dueDate; LoanDate = loanDate} |> ok
    | _ -> InvalidStateTransition "Loan at init" |> fail

let evolveAtCreated data = function
    | _ -> raise (exn "Implement me")

let evolveOne (event:EventData) state = 
    match state with
    | LoanInit -> evolveAtInit event
    | _ -> raise (exn "Implement me")

let evolveSeed = {Init = LoanInit; EvolveOne = evolveOne}

[<AutoOpen>]
module LibAAS.Contracts.Commands

type CommandData = 
    | LoanItem of LoanItem
    | ReturnItem of LoanId
    | RegisterInventoryItem of Item * Quantity

and LoanItem = {
    Id:LoanId
    UserId:UserId
    ItemId:ItemId
    LibraryId:LibraryId }

type Command = AggregateId * CommandData



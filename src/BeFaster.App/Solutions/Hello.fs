namespace BeFaster.App.Solutions

open System

type Hello private() =
    static member hello(friendName: string) = 
        raise (NotImplementedException())

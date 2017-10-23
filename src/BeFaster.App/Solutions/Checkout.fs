namespace BeFaster.App.Solutions

open System

type Checkout private() =
    static member checkout(skus: string) = 
        raise (NotImplementedException())

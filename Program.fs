open System
open System.Linq

type Test1 =
    { Id1: int
      Id2: int
      Key: string }
    static member Composite t1 = t1.Id1, t1.Id2

type Test2 =
    { Id1: int
      Id2: int
      Value: string }
    static member Composite t2 = t2.Id1, t2.Id2


[<EntryPoint>]
let main argv =

    let test1s = List.init 10 (fun _ -> [ 0..9 ]) |> List.mapi (fun i ns -> ns |> List.map (fun j -> { Id1 = i; Id2 = j; Key = sprintf "key-%d-%d" i j })) |> List.concat
    let test2s = List.init 20 (fun _ -> [ 0..3 ]) |> List.mapi (fun i ns -> ns |> List.map (fun j -> { Id1 = i; Id2 = j; Value = sprintf "val-%d-%d" i j })) |> List.concat
    let q1s = test1s.AsQueryable()
    let q2s = test2s.AsQueryable()

    let joinedInline =
        query {
            for t1 in q1s do
            join t2 in q2s on ((t1.Id1, t1.Id2) = (t2.Id1, t2.Id2))
            select (t1.Key, t2.Value)
        }

    let joinedFunction =
        query {
            for t1 in q1s do
            join t2 in q2s on (Test1.Composite t1 = Test2.Composite t2)
            select (t1.Key, t2.Value)
        }

    let printMatches = Seq.map (fun (k, v) -> sprintf "%s : %s" k v) >> String.concat "\n"

    joinedInline |> printMatches |> printfn "Inline matches:\n%s"
    joinedFunction |> printMatches |> printfn "Function matches:\n%s"

    0

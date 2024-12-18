namespace FrontEndToolFS.HexPlane

open Microsoft.FSharp.Core

type HexCellSearchData =
    struct
        val mutable distance: int
        val mutable nextWithSamePriority: int
        val mutable pathFrom: int
        val mutable heuristic: int
        val mutable searchPhase: int

        public new(?distance, ?nextWithSamePriority, ?pathFrom, ?heuristic, ?searchPhase) =
            { distance = defaultArg distance 0
              nextWithSamePriority = defaultArg nextWithSamePriority 0
              pathFrom = defaultArg pathFrom -1
              heuristic = defaultArg heuristic 0
              searchPhase = defaultArg searchPhase 0 }

        member this.SearchPriority = this.distance + this.heuristic
    end

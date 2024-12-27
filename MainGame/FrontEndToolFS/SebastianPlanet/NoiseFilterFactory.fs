namespace FrontEndToolFS.SebastianPlanet

module NoiseFilterFactory =
    let createNoiseFilter (settings: INoiseSettings) =
        match settings.FilterType with
        | FilterType.Simple -> SimpleNoiseFilter(settings) :> INoiseFilter
        | FilterType.Rigid -> RigidNoiseFilter(settings) :> INoiseFilter
        | _ -> failwith "Invalid filter type"

namespace FrontEndToolFS.SebastianPlanet

module NoiseFilterFactory =
    let createNoiseFilter (settings: NoiseSettings) =
        match settings.filterType with
        | FilterType.Simple -> SimpleNoiseFilter(settings) :> INoiseFilter
        | FilterType.Rigid -> RigidNoiseFilter(settings) :> INoiseFilter
        | _ -> failwith "Invalid filter type"

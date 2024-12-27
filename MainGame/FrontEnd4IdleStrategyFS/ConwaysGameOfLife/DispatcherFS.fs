namespace FrontEnd4IdleStrategyFS.ConwaysGameOfLife

open System.Threading.Tasks
open Godot
open Godot.Collections

type DispatcherFS() as this =
    inherit Node()

    let mutable rd: RenderingDevice = null
    let mutable inputTexture = Unchecked.defaultof<Rid>
    let mutable outputTexture = Unchecked.defaultof<Rid>
    let mutable uniformSet = Unchecked.defaultof<Rid>
    let mutable shader = Unchecked.defaultof<Rid>
    let mutable pipeline = Unchecked.defaultof<Rid>
    let mutable inputUniform: RDUniform = null
    let mutable outputUniform: RDUniform = null
    let mutable bindings = Array<RDUniform>()
    let mutable inputImage: Image = null
    let mutable outputImage: Image = null
    let mutable renderTexture: ImageTexture = null
    let mutable inputFormat: RDTextureFormat = null
    let mutable outputFormat: RDTextureFormat = null
    let mutable processing = false

    let textureUsage =
        RenderingDevice.TextureUsageBits.StorageBit
        ||| RenderingDevice.TextureUsageBits.CanUpdateBit
        ||| RenderingDevice.TextureUsageBits.CanCopyFromBit

    let mergeImages () =
        let outputWidth = outputImage.GetWidth()
        let outputHeight = outputImage.GetHeight()
        let inputWidth = inputImage.GetWidth()
        let inputHeight = inputImage.GetHeight()
        let startX = (outputWidth - inputWidth) / 2
        let startY = (outputHeight - inputHeight) / 2

        for x in 0 .. inputWidth - 1 do
            for y in 0 .. inputHeight - 1 do
                let color = inputImage.GetPixel(x, y)
                let destX = startX + x
                let destY = startY + y

                if destX >= 0 && destX < outputWidth && destY >= 0 && destY < outputHeight then
                    outputImage.SetPixel(destX, destY, color)

        inputImage.SetData(1024, 1024, false, Image.Format.L8, outputImage.GetData())

    let linkOutputTextureToRenderer () =
        let mat = this.renderer.Material :?> ShaderMaterial
        renderTexture <- ImageTexture.CreateFromImage outputImage
        mat.SetShaderParameter("binaryDataTexture", Variant.CreateFrom renderTexture)

    let createAndValidateImages () =
        outputImage <- Image.CreateEmpty(1024, 1024, false, Image.Format.L8)

        if this.dataTexture = null then
            let noise = new FastNoiseLite()
            noise.Frequency <- 0.1f
            let noiseImage = noise.GetImage(1024, 1024)
            inputImage <- noiseImage
        else
            inputImage <- this.dataTexture.GetImage()

        mergeImages ()
        linkOutputTextureToRenderer ()

    let createRenderingDevice () =
        rd <- RenderingServer.CreateLocalRenderingDevice()

    let createShader () =
        let shaderFile = GD.Load<RDShaderFile>(this.computeShader)
        let spirV = shaderFile.GetSpirV()
        shader <- rd.ShaderCreateFromSpirV(spirV)

    let createPipeline () =
        pipeline <- rd.ComputePipelineCreate(shader)

    let defaultTextureFormat = new RDTextureFormat()

    do
        defaultTextureFormat.Width <- 1024u
        defaultTextureFormat.Height <- 1024u
        defaultTextureFormat.Format <- RenderingDevice.DataFormat.R8Unorm
        defaultTextureFormat.UsageBits <- textureUsage

    let createTextureFormat () =
        inputFormat <- defaultTextureFormat
        outputFormat <- defaultTextureFormat

    let createTextureAndUniform (image: Image) format binding =
        let view = new RDTextureView()
        let data = Array<byte array>()
        data.Add <| image.GetData()
        let texture = rd.TextureCreate(format, view, data)
        let uniform = new RDUniform()
        uniform.UniformType <- RenderingDevice.UniformType.Image
        uniform.Binding <- binding
        uniform.AddId texture
        bindings.Add uniform
        texture

    let createUniforms () =
        inputTexture <- createTextureAndUniform inputImage inputFormat 0
        outputTexture <- createTextureAndUniform outputImage outputFormat 1
        uniformSet <- rd.UniformSetCreate(bindings, shader, 0u)

    let setupComputeShader () =
        createRenderingDevice ()
        createShader ()
        createPipeline ()
        createTextureFormat ()
        createUniforms ()

    let update () =
        let computeList = rd.ComputeListBegin()
        rd.ComputeListBindComputePipeline(computeList, pipeline)
        rd.ComputeListBindUniformSet(computeList, uniformSet, 0u)
        rd.ComputeListDispatch(computeList, 32u, 32u, 1u)
        rd.ComputeListEnd()
        rd.Submit()

    let render () =
        rd.Sync()
        let bytes = rd.TextureGetData(outputTexture, 0u)
        rd.TextureUpdate(inputTexture, 0u, bytes) |> ignore
        outputImage.SetData(1024, 1024, false, Image.Format.L8, bytes)
        renderTexture.Update outputImage

    let startProcessLoop () =
        async {
            let frq = 1000 / this.updateFrequency
            processing <- true

            while processing do
                update ()
                do! Task.Delay(frq) |> Async.AwaitTask // 对应 C# 的 await
                render ()
        }

    let cleanupGPU () =
        if rd = null then
            ()
        else
            rd.FreeRid inputTexture
            rd.FreeRid outputTexture
            rd.FreeRid uniformSet
            rd.FreeRid pipeline
            rd.FreeRid shader
            rd.Free()
            rd <- null

    member val updateFrequency = 60 with get, set
    member val autoStart = false with get, set
    member val dataTexture: Texture2D = null with get, set
    member val computeShader: string = null with get, set
    member val renderer: Sprite2D = null with get, set

    override this._Input(event) =
        match event with // 类型匹配
        | :? InputEventKey as key when key.Keycode = Key.Space && key.Pressed ->
            if processing then
                processing <- false
            else
                startProcessLoop () |> Async.Start
        | _ -> ()

    override this._Notification(what) =
        if
            what = int Node.NotificationWMCloseRequest
            || what = int Node.NotificationPredelete
        then
            cleanupGPU ()

    override this._Ready() =
        createAndValidateImages ()
        setupComputeShader ()

        if not this.autoStart then
            ()
        else
            startProcessLoop () |> Async.Start

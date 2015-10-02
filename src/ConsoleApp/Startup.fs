

namespace ConsoleApp
open System

type Startup() = 
    member x.Configuration(app : Owin.IAppBuilder) =
        app |> Owin.AppBuilderExtensions.UseNancy |> ignore
        ()

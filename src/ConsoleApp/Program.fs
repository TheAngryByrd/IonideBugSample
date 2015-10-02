// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
module Main =
    open System
    open Microsoft.Owin.Hosting
    open Mono.Unix
    open Mono.Unix.Native
    open NodaTime
    open Logary
    open Logary.Configuration
    open Logary.Targets
    open Logary.Metrics

    let waitForClose() =
        if Type.GetType("Mono.Runtime") <> null then
            let signals =
                [| new UnixSignal(Signum.SIGINT)
                   new UnixSignal(Signum.SIGTERM)
                   new UnixSignal(Signum.SIGQUIT)
                   new UnixSignal(Signum.SIGHUP) |]
            UnixSignal.WaitAny(signals) |> sprintf "Linux signal: %A"
        else Console.ReadLine() |> sprintf "Console read %s"
        |> LogLine.create' LogLevel.Debug
        |> Logging.getCurrentLogger().Log

    [<EntryPoint>]
    let main argv = 
        use loggary =
                withLogary' "Sample"              (withTargets [ Console.create Console.empty "console"
                                                                 Debugger.create Debugger.empty "debugger" ]
                                                   >> withMetrics (Duration.FromMilliseconds 5000L)
                                                          [ WinPerfCounters.create (WinPerfCounters.Common.cpuTimeConf)
                                                                "wperf" (Duration.FromMilliseconds 300L) ]
                                                   >> withRules [ Rule.createForTarget "console"
                                                                  Rule.createForTarget "debugger" ])
        let url = sprintf "%s://%s:%d" "http" "localhost" 8080
        use app = WebApp.Start<ConsoleApp.Startup> url

        waitForClose()
        0 // return an integer exit code


﻿namespace Frobnicator.UI

open System
open Elmish
open Elmish.WPF
open NAudio.Wave

module Types =
    type Model = {buttonText : string; out: IWavePlayer }

    type Msg = 
    | Click

module State =
    open Types
    open Frobnicator.Audio
    open Frobnicator.Audio.Wave
    
    let init () =
        let out = new WasapiOut()
        let fmt = WaveFormat.CreateIeeeFloatWaveFormat(44100, 2)
        let sigGen = sine fmt 440.0
        out.Init(new Output(fmt, sigGen))
        { buttonText = "Start" ; out = out }
        
    let update msg model = 
        match msg, model.out.PlaybackState with
        | Click, PlaybackState.Playing ->
            model.out.Stop()
            { model with buttonText = "Start" }
        | Click, _ -> 
            model.out.Play()
            { model with buttonText = "Stop" }
             
module App = 
    open Types
    open State
    
    let view _ _ =
        [ "Text" |> Binding.oneWay (fun m -> m.buttonText)
          "Start" |> Binding.cmd (fun _ m -> Click) ]


    [<EntryPoint; STAThread>]
    let main argv = 
        Program.mkSimple init update view
        |> Program.withConsoleTrace
        |> Program.runWindow (Frobnicator.Views.MainWindow())

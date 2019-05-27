﻿(*
    VisUAL2 @ Imperial College London
    Project: A user-friendly ARM emulator in F# and Web Technologies ( Github Electron & Fable Compiler )
    Module: Renderer.Tabs
    Description: handle editor tabs: each can contain a distinct assembly file
*)

/// implement Monaco editor file tabs

module Tabs
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.Browser
open Fable.Core
open EEExtensions
open CommonData
open Refs
open Editors
open Elmish
open Fable.Core.JsInterop


/// Set flag as stored and displayed in GUI
let setFlag (id : string) (value : bool) (hasChanged : bool) =
    let el = Refs.flag id
    match value with
        | false ->
            el.innerHTML <- sprintf "%i" 0
        | true ->
            el.innerHTML <- sprintf "%i" 1
    match hasChanged with
        | false ->
            el.setAttribute ("style", "background: #fcfcfc")
        | true ->
            el.setAttribute ("style", "background: #4285f4")

/// return the id of the last editor in the map of editors
let selectLastTabId editors =
    editors
    |> Map.toList
    |> List.rev
    |> List.head
    |> fst

/// Editor value of a  blank tab
let blankTab =
    { DefaultValue = ""
      FileName = None
      FilePath = None
      IEditor = None
      Saved = true }

/// top-level function to delete tab
let deleteTabUpdate (tabId, editors : Map<int, Editor>, settingsTab) =
    let newEditors = Map.remove tabId editors
    let newTabId = 
        match Map.isEmpty newEditors with
        | true -> -1
        | false -> selectLastTabId newEditors
    let newSettingsTab =
        match settingsTab with
        | Some x when x = tabId -> None
        | x -> 
            editors.[tabId].IEditor?dispose () |> ignore
            x
    newTabId, newEditors, newSettingsTab

/// top-level function to select file tab
let selectFileTabUpdate id editors =
    match Map.isEmpty editors with
    | true -> -1
    | _ -> 
        match Map.containsKey id editors with
        | true -> id
        | _ -> selectLastTabId editors

/// top-level function for opening new tab
let newFileUpdate editors =
    let newTabId = uniqueTabId editors
    let newEditors = Map.add newTabId blankTab editors
    newTabId, newEditors

let attemptToDeleteTabUpdate (tabId, saved, dialogBox)
                             id =
    let mutable dialog = dialogBox
    let mutable cmd = Cmd.none
    match id = tabId, saved, dialogBox with
    | true, true, _ -> 
        cmd <- Cmd.ofMsg DeleteTab
    | true, false, None -> 
        dialog <- Some UnsavedFileDl
    | _ -> 
        ()
    dialog, cmd
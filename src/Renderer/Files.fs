﻿module Files

open EEExtensions
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.Electron
open Node.Exports
open Refs
open Elmish

let writeToFile str path =
    let errorHandler _err = // TODO: figure out how to handle errors which can occur
        ()
    fs.writeFile (path, str, errorHandler)

/// merge 2 maps into 1
/// if key repeated, the ones in the old map are kept
let mapMerge newMap = 
    newMap 
    |> Map.fold (fun map key value -> 
        Map.add key value map)

/// return the file name from the file path
let fileName path =
    path
    |> String.toList
    |> List.rev
    |> List.takeWhile (fun x -> x <> '/')
    |> List.rev
    |> List.toString

/// return file path without file name
let filePathSetting path =
    path
    |> String.toList
    |> List.rev
    |> List.skipWhile (fun x -> x <> '/')
    |> List.rev
    |> List.toString

/// format the list of files into a list of Editors
let openLstOfFiles (fLst : string list) : Editor List =
    let readFile (path:string) =
        Node.Exports.fs.readFileSync (path, "utf8")
    fLst
    |> List.map (fun x -> 
        { DefaultValue = readFile x
          FilePath = Some x
          FileName = x |> fileName |> Some
          IEditor = Option.None
          Saved = true })

let fileFilterOpts =
    ResizeArray [
        createObj [
            "name" ==> "Assembly Code"
            "extensions" ==> ResizeArray [ "s" ]
        ]
    ] |> Some

/// open file dialog
let openFile currentFilePath (dispatch : Msg -> Unit) =
    let options = createEmpty<OpenDialogOptions>
    options.properties <- ResizeArray([ "openFile"; "multiSelections" ]) |> Some
    options.filters <- fileFilterOpts
    options.defaultPath <- Some currentFilePath
    // path of the opened files
    let seq = electron.remote.dialog.showOpenDialog (options) /// open dialog
    let fileLst =
        match isUndefined seq with
        | true -> /// the dialog is cancelled, so seq is undefined
            []
        | false ->
            seq
            |> Seq.toList
            |> openLstOfFiles
    OpenFile fileLst |> dispatch

/// save file dialog
let saveFileAs filePathSetting (editor : Editor) dispatch : (unit) =
    let options = createEmpty<SaveDialogOptions>
    options.filters <- fileFilterOpts
    let savedFilePath = 
        match editor.FilePath with
        | Some x -> x
        | _ -> filePathSetting
    options.defaultPath <- Some savedFilePath
    /// path of the saved file
    let result = electron.remote.dialog.showSaveDialog (options) ///open the save file dialog
    match isUndefined result with
    | true -> 
        SaveAsFile Option.None |> dispatch
    | false -> 
        writeToFile (editor.IEditor?getValue ()) result
        let fileInfo = result, fileName result
        fileInfo |> Some |> SaveAsFile |> dispatch

/// top-level function for saving file
/// open the save dialog when necessary
let saveFileUpdate info settingTabs =
    match info.TabId, settingTabs with
    | -1, _ -> 
        info, Cmd.none
    | id, Some x when x = id ->
        info, Cmd.ofMsg SaveSettingsOnly
    | id, _ -> 
        let filePath = info.Editors.[id].FilePath
        match filePath with
        | Option.None -> 
            info, 
            SaveAsDl
            |> UpdateDialogBox
            |> Cmd.ofMsg /// open the dialog
        | Some fPath ->
            let currentEditor = info.Editors.[id]
            writeToFile (currentEditor.IEditor?getValue ()) fPath
            let newEditors = 
                Map.add id
                        { currentEditor with Saved = true }
                        info.Editors
            { Editors = newEditors   
              TabId = info.TabId },
            Cmd.none

/// top-level function for save file as
/// open the save file dialog when necessary
let saveAsFileDialogUpdate =
    function
    | -1, _ -> Cmd.none /// make sure sure no other dialog is opened and there is at least one tab
    | x, Some y when x = y -> Cmd.ofMsg SaveSettingsOnly
    | _ -> SaveAsDl |> UpdateDialogBox |> Cmd.ofMsg

/// top-level function for opening up the open file dialog
let saveAsFileUpdate (info, filePathSettingStr)
                     fileInfo = 
    match fileInfo with
    | Option.None -> 
        info, filePathSettingStr
    | Some (filePath, fileName) ->
        let newEditor =
            { info.Editors.[info.TabId] with FilePath = Some filePath
                                             FileName = Some fileName
                                             Saved = true }
        let newEditors = 
            info.Editors
            |> Map.add info.TabId
                       newEditor
            |> Map.filter (fun key value ->
                key = info.TabId ||
                value.FilePath <> newEditor.FilePath)
        let newFilePathSettings = filePathSetting filePath
        { info with Editors = newEditors }, newFilePathSettings        

/// top-level function for opening file
let openFileUpdate (info, filePath) 
                   editor =
    let newId = uniqueTabId info.Editors
    let length = List.length editor
    match length with
    | 0 -> // no file is selected to be opened
        filePath, info
    | _ ->
        let newEditors =
            editor
            // zip it with number so it can be convert into map
            // number start at the unique tab id to avoid replacement
            |> List.zip [newId .. newId + length - 1]
            |> List.filter (fun (_, x) ->
                // check if the files are already opened
                info.Editors
                |> Map.forall (fun _ value -> 
                    value.FilePath <> x.FilePath))
            |> Map.ofList
        let mergedEditors = mapMerge newEditors info.Editors
        let currentEditor = List.head editor // find the first opened file
        let newId = // make the first file as current tab
            mergedEditors
            |> Map.findKey (fun _ value -> 
                value.FilePath = currentEditor.FilePath)
        mergedEditors.[newId].FilePath.Value
        |> filePathSetting,
        { TabId = newId
          Editors = mergedEditors }
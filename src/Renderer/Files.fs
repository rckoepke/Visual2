(* 
    Visual2 @ Imperial College London
    Project: A user-friendly ARM emulator in F# and Web Technologies ( Github Electron & Fable Compliler )
    Module: Renderer.Files
    Description: IDE file Load and store via electrom main process dialogs
*)

/// implement load and save of assembler files
module Files

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.Electron
open Node.Exports
open Fable.PowerPack
open EEExtensions

open Fable.Import.Browser


open Refs
open Fable
open Settings
open Tabs
open Views

open CommonData
open ExecutionTop


let resetEmulator () =
    printfn "Resetting..."
    Editors.removeEditorDecorations currentFileTabId
    Editors.enableEditors()   
    memoryMap <- Map.empty
    symbolMap <- Map.empty
    regMap <- initialRegMap
    setMode ResetMode
    updateMemory()
    updateSymTable()
    updateRegisters ()
    resetRegs()
    resetFlags()


//*************************************************************************************
//                           FILE LOAD AND STORE
//*************************************************************************************

let setTabFilePath id path =
    let fp = (tabFilePath id)
    fp.innerHTML <- path
 
let getTabFilePath id =
    let fp = (tabFilePath id)
    fp.innerHTML
/// get file name and extension from path
let baseFilePath (path : string) =
    path.Split [|'/';'\\'|]
    |> Array.last

/// Load the node Buffer into the specified tab
let loadFileIntoTab tId (fileData : Node.Buffer.Buffer) =
    if currentFileTabId = tId then
        resetEmulator()
    let editor = editors.[tId]
    editor?setValue(fileData.toString("utf8")) |> ignore
    setTabSaved tId

/// Return the text in tab id tId as a string
let getCode tId =
    let editor = editors.[tId]
    editor?getValue() :?> string

/// If x is undefined, return errCase, else return Ok x
let resultUndefined errCase x =
    match isUndefined x with
    | true -> Result.Error errCase
    | false -> Result.Ok x

let fileFilterOpts = 
    ResizeArray[ 
        createObj [
            "name" ==> "Assembly Code"
            "extensions" ==> ResizeArray ["s"]
        ]
    ] |> Some

let openFile () =
    let options = createEmpty<OpenDialogOptions>
    options.properties <- ResizeArray(["openFile"; "multiSelections"]) |> Some
    options.filters <- fileFilterOpts
    options.defaultPath <- Some vSettings.CurrentFilePath
    let readPath (path, tId) = 
        fs.readFile(path, (fun err data -> // TODO: find out what this error does
            loadFileIntoTab tId data
        ))
        |> ignore
        tId // Return the tab id list again to open the last one

    let makeTab path =
        let tId = createNamedFileTab (baseFilePath path) path
        setTabFilePath tId path
        (path, tId)

    let updateCurrentPath (res : string list) =
        printfn "Updating path to: %A" res
        match res with
        | [p] ->
            let dir = p |> String.split [|'\\';'/'|]
            if dir.Length > 1 then
                let path = path.join dir.[0..dir.Length-2]
                printfn "Made path=%A" path
                if (fs.statSync (U2.Case1 path)).isDirectory() then
                    printf "Changing current path from %A to %A" vSettings.CurrentFilePath path
                    vSettings <- {vSettings with CurrentFilePath = path}
        | _ -> ()
        res
    electron.remote.dialog.showOpenDialog(options)
    |> resultUndefined ()
    |> Result.map (fun x -> x.ToArray())
    |> Result.map Array.toList
    |> Result.map updateCurrentPath
    |> Result.map (List.map (makeTab >> readPath))
    |> Result.map List.last
    |> Result.map selectFileTab
    |> ignore



let writeToFile str path =
    let errorHandler _err = // TODO: figure out how to handle errors which can occur
        ()
    fs.writeFile(path, str, errorHandler)

let writeCurrentCodeToFile path = (writeToFile (getCode currentFileTabId) path)

let saveFileAs () =
    // Don't do anything if the user tries to save as the settings tab
    match settingsTab with
    | Some x when x = currentFileTabId -> ()
    | _ ->
        let options = createEmpty<SaveDialogOptions>
        options.filters <- fileFilterOpts
        let currentPath = getTabFilePath currentFileTabId
        
        // If a path already exists for this file, open it
        match currentPath with
        | "" -> ()
        | _ -> options.defaultPath <- Some currentPath

        let result = electron.remote.dialog.showSaveDialog(options)

        // Performs op on resPart if x is Ok resPart, then returns x
        let resultIter op x =
            Result.map op x |> ignore
            x

        result
        |> resultUndefined ()
        |> resultIter writeCurrentCodeToFile
        |> resultIter (setTabFilePath currentFileTabId)
        |> Result.map baseFilePath

        |> Result.map (setTabName currentFileTabId)
        |> Result.map (fun _ -> setTabSaved (currentFileTabId))
        |> ignore

// If a path already exists for a file, write it straight to disk without the dialog
let saveFile () =
    // Save the settings if the current tab is the settings tab
    match settingsTab with
    | Some x when x = currentFileTabId -> 
        getFormSettings()
        setTabSaved (currentFileTabId)
    | _ ->
        match getTabFilePath currentFileTabId with
        | "" -> saveFileAs () // No current path exists
        | path -> 
            writeCurrentCodeToFile path
            setTabSaved (currentFileTabId)
// Figure out if any of the tabs are unsaved
let unsavedFiles () =
    fileTabList
    |> List.map isTabUnsaved
    |> List.fold (||) false

let editorFind () =
    let action = editors.[currentFileTabId]?getAction("actions.find")
    action?run() |> ignore
 
let editorFindReplace () =
    let action = editors.[currentFileTabId]?getAction("editor.action.startFindReplaceAction")
    action?run() |> ignore

let editorUndo () =
    editors.[currentFileTabId]?trigger("Update.fs", "undo") |> ignore

let editorRedo () =
    editors.[currentFileTabId]?trigger("Update.fs", "redo") |> ignore

let editorSelectAll () = 
    editors.[currentFileTabId]?trigger("Update.fs", "selectAll") |> ignore



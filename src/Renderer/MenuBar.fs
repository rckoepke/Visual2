module MenuBar

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.Electron
open Node.Base
open Ref
open Update
open Settings
open Tabs


let showQuitMessage (callBack:int ->unit) =
    let rem = electron.remote
    let mess = "You have unsaved changes. Would you like to save them first?"
    let detail = "Your changes will be lost if you don\'t save them."
    let buttons =  [ "Save" ; "Dont Save" ] 
    Editor.showMessage callBack mess detail buttons
        
let checkOKToClose() =
    let close() = electron.ipcRenderer.send "doClose" |> ignore
    let callback (result:int) =
        match int result with
        | 0 -> ()
        | _ -> close()
    let tabL = Tabs.unsavedTabs()
    if tabL <> [] then
        showQuitMessage callback
    else close()
        
let handlerCaster f = System.Func<MenuItem, BrowserWindow, unit> f |> Some

let menuSeparator = 
    let sep = createEmpty<MenuItemOptions>
    sep.``type`` <- Some Separator
    sep

let makeItem (label:string) (iKeyOpt: string option)  (iAction: unit -> unit) =
    let item = createEmpty<MenuItemOptions>
    item.label <- Some label
    item.accelerator <- iKeyOpt
    item.click <- handlerCaster (fun _ _ -> iAction())
    item
    
let makeRoleItem label accelerator role = 
    let item = makeItem label accelerator id
    item.role <- U2.Case1 role |> Some
    item

let makeMenu (name:string) (table:MenuItemOptions list) =
    let subMenu = createEmpty<MenuItemOptions>
    subMenu.label <- Some name
    subMenu.submenu <-
        table
        |> ResizeArray<MenuItemOptions>
        |> U2.Case2 |> Some
    subMenu

let fileMenu =
    makeMenu "File" [
            makeItem "New"      (Some "CmdOrCtrl+N")        createFileTab
            menuSeparator
            makeItem "Save"     (Some "CmdOrCtrl+S")        Files.saveFile
            makeItem "Save As"  (Some "CmdOrCtrl+Shift+S")  Files.saveFileAs
            makeItem "Open"     (Some "CmdOrCtrl+O")        Files.openFile
            menuSeparator
            makeItem "Close"    (Some "Ctrl+W")             deleteCurrentTab
            menuSeparator
            makeItem "Quit"     (Some "Ctrl+Q")             checkOKToClose
        ]

let optCreateSettingsTab() =
    match runMode with
    | ExecutionTop.ResetMode 
    | ExecutionTop.ParseErrorMode -> createSettingsTab ()
    | _ -> Browser.window.alert "Can't change preferences while simulator is running" |> ignore

let editMenu = 
    makeMenu "Edit" [
        makeItem "Undo" (Some "CmdOrCtrl+Z") Files.editorUndo
        makeItem "Redo" (Some "CmdOrCtrl+Shift+Z") Files.editorRedo
        menuSeparator
        makeRoleItem "Cut" (Some "CmdOrCtrl+X") MenuItemRole.Cut
        makeRoleItem "Copy" (Some "CmdOrCtrl+C") MenuItemRole.Copy   
        makeRoleItem "Paste" (Some "CmdOrCtrl+V")  MenuItemRole.Paste
        menuSeparator
        makeItem "Select All"  (Some "CmdOrCtrl+A")  Files.editorSelectAll
        menuSeparator
        makeItem "Find" (Some "CmdOrCtrl+F") Files.editorFind              
        makeItem "Replace"  (Some "CmdOrCtrl+H") Files.editorFindReplace
        menuSeparator
        makeItem  "Preferences"  Option.None optCreateSettingsTab
    ]


let viewMenu = 
        let devToolsKey = if Node.Globals.``process``.platform = NodeJS.Platform.Darwin then "Alt+Command+I" else "Ctrl+Shift+I"
        makeMenu "View" [
            makeRoleItem "Toggle Fullscreen"  (Some "F11")  MenuItemRole.Togglefullscreen
            menuSeparator
            makeRoleItem  "Zoom In" (Some "CmdOrCtrl+Plus") MenuItemRole.Zoomin 
            makeRoleItem  "Zoom Out"  (Some "CmdOrCtrl+-") MenuItemRole.Zoomout 
            makeRoleItem  "Reset Zoom"  (Some "CmdOrCtrl+0") MenuItemRole.Resetzoom
            menuSeparator
            makeItem "Toggle Dev Tools" (Some "F12") (electron.remote.getCurrentWebContents()).toggleDevTools
        ]

let helpMenu runTests =
        let runPage page () = electron.shell.openExternal page |> ignore ; ()
        makeMenu "Help" [
            makeItem "Website" Core.Option.None (runPage "https://intranet.ee.ic.ac.uk/t.clarke/hlp/")
            makeItem "ARM documentation" Core.Option.None (runPage "http://infocenter.arm.com/help/index.jsp?topic=/com.arm.doc.ddi0234b/i1010871.html")
            makeItem "Run Emulator Tests" Core.Option.None runTests
            makeItem "About" Core.option.None ( fun () -> 
                electron.remote.dialog.showMessageBox (
                      let opts = createEmpty<ShowMessageBoxOptions>
                      opts.title <- "Visual2 ARM Simulator (prerelease)" |> Some
                      opts.message <- "(c) 2018, Imperial College" |> Some
                      opts.detail <- 
                            "Acknowledgements: Salman Arif (VisUAL), HLP 2018 class" +
                            " (F# reimplementation), with special mention to Thomas Carrotti," +
                            " Lorenzo Silvestri, and HLP Team 10" |> Some
                      opts
                ) |> ignore; () )
         ]   

let setMainMenu runTests =
    let template = 
        ResizeArray<MenuItemOptions> [
            fileMenu
            editMenu
            viewMenu
            helpMenu runTests
        ]
    template
    |> electron.remote.Menu.buildFromTemplate
    |> electron.remote.Menu.setApplicationMenu
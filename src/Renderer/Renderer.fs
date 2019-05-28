module Renderer

open Refs
open Elmish
open Elmish.React
open Elmish.HMR
open Elmish.Debug
open Elmish.Browser.Navigation
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Monaco
open Views
open Tabs
open MenuBar
open Tooltips
open Files
open Settings
open Editors
open Dialog
open Stats
open Integration
open ExecutionTop
open CommonData

let init _ =
    let debugLevel =
        let argV =
            electron.remote.``process``.argv
            |> Seq.toList
            |> List.tail
            |> List.map (fun s -> s.ToLower())
        let isArg s = List.contains s argV
        if isArg "--debug" || isArg "-d" then 2
        elif isArg "-w" then 1
        else 0
    let settings = checkSettings (getJSONSettings initSettings) initSettings
    let m =
        { 
            TabId = 0
            TestbenchTab = None
            Editors = Map.ofList [ (0, blankTab) ]
            CurrentTabWidgets = Map.empty
            SettingsTab = None
            CurrentRep = Hex
            DisplayedCurrentRep = Hex
            CurrentView = Registers
            ByteView = false
            ReverseDirection = false
            MaxStepsToRun = 50000
            MemoryMap = Map.empty
            RegMap = ExecutionTop.initialRegMap
            Flags = initialFlags
            SymbolMap = Map.empty
            RunMode = ExecutionTop.ResetMode
            DebugLevel = debugLevel
            LastOnlineFetchTime = Result.Error System.DateTime.Now
            Activity = true
            Sleeping = false
            LastRemindTime = None
            Settings = settings
            DialogBox = None
            InitClose = false
            Decorations = []
            EditorEnable = true
            ClockTime = (0uL, 0uL)
            LastDisplayStepsDone = 0L
            IExports = None
            FlagsHasChanged = initialFlags
        }
    let cmd = Startup |> ReadOnlineInfo |> Cmd.ofMsg         
    m, cmd

let update (msg : Msg) (m : Model) =
    match msg with
    | UpdateDialogBox dialogBox ->
        let newDialogBox = dialogBoxUpdate dialogBox m.DialogBox
        { m with DialogBox = newDialogBox }, Cmd.none
    | ChangeView view -> 
        { m with CurrentView = view }, Cmd.none
    | ChangeRep rep ->
        { m with CurrentRep = rep }, Cmd.none
    | ToggleByteView -> 
        { m with ByteView = not m.ByteView }, Cmd.none
    | ToggleReverseView -> 
        { m with ReverseDirection = not m.ReverseDirection }, Cmd.none
    | NewFile -> 
        let newTabId, newEditors = newFileUpdate m.Editors
        { m with TabId = newTabId
                 Editors = newEditors }, Cmd.none
    | EditorTextChange ->
        let newEditor = { m.Editors.[m.TabId] with Saved = false }
        let newEditors = Map.add m.TabId newEditor m.Editors
        { m with Editors = newEditors }, Cmd.none
    | SelectFileTab id -> 
        let newTabId = selectFileTabUpdate id m.Editors
        { m with TabId = newTabId }, Cmd.none
    | AttemptToDeleteTab id ->
        let newDialog, newCmd =
            attemptToDeleteTabUpdate (m.TabId, m.Editors, m.DialogBox) 
                                     id
        { m with DialogBox = newDialog }, newCmd
    | DeleteTab -> 
        let newTabId, newEditors, newSettingsTab = 
            deleteTabUpdate (m.TabId, m.Editors, m.SettingsTab)
        { m with TabId = newTabId
                 Editors = newEditors 
                 SettingsTab = newSettingsTab
                 CurrentTabWidgets = Map.empty }, Cmd.ofMsg CloseDialog
    | OpenFile editors -> 
        let newEditors, newFilePath, newTabId = 
            openFileUpdate (m.Editors, m.Settings.CurrentFilePath, m.TabId)
                           editors
        let newSettings = 
            { m.Settings with CurrentFilePath = newFilePath }
        { m with Editors = newEditors
                 TabId = newTabId
                 Settings = newSettings }, Cmd.ofMsg CloseDialog
    | OpenFileDialog -> 
        let newDialog = dialogBoxUpdate OpenFileDl m.DialogBox
        { m with DialogBox = newDialog }, Cmd.none
    | SaveFile -> 
        let newDialog, newEditors = 
            saveFileUpdate (m.TabId, m.Editors)
        { m with Editors = newEditors
                 DialogBox = newDialog }, Cmd.none
    | SaveAsFileDialog -> 
        let newDialogBox =
            saveAsFileDialogUpdate m.DialogBox m.TabId
        { m with DialogBox = newDialogBox }, Cmd.none
    | SaveAsFile fileInfo ->
        let newEditors, newFilePathSetting =
            saveAsFileUpdate (m.Editors, m.TabId, m.Settings.CurrentFilePath)
                             fileInfo
        let newSettings = 
            { m.Settings with CurrentFilePath = newFilePathSetting }
        { m with Editors = newEditors
                 Settings = newSettings }, Cmd.ofMsg CloseDialog
    | SelectSettingsTab ->
        let newEditors, newTabId =
            selectSettingsTabUpdate (m.SettingsTab, m.Editors)
        { m with Editors = newEditors
                 TabId = newTabId
                 SettingsTab = Some newTabId }, Cmd.none
    | SaveSettings ->
        let newSettings, newEditors, newId =
            saveSettingsUpdate (m.Settings, m.Editors, m.SettingsTab.Value, m.TabId, m.IExports)
        { m with Settings = newSettings 
                 Editors = newEditors 
                 TabId = newId 
                 SettingsTab = None }, Cmd.none
    | LoadDemoCode -> 
        let newEditors, newId = loadDemo m.Editors
        { m with Editors = newEditors 
                 TabId = newId }, Cmd.none
    | IncreaseFontSize ->
        let newSettings = 
            { m.Settings with EditorFontSize = string ((int m.Settings.EditorFontSize) + 2) }
        { m with Settings = newSettings }, Cmd.none
    | DecreaseFontSize ->
        let newSettings = 
            { m.Settings with EditorFontSize = string ((int m.Settings.EditorFontSize) - 2) }
        { m with Settings = newSettings }, Cmd.none
    | CloseDialog ->
        { m with DialogBox = None }, Cmd.none
    | AttemptToExit ->
        let newDialogBox, newCmd = attemptToExitUpdate m.Editors m.DialogBox
        { m with DialogBox = newDialogBox }, newCmd
    | Exit ->
        close()
        m, Cmd.ofMsg CloseDialog
    | UpdateIEditor (x, y) ->
        let newEditors = Map.add y { m.Editors.[y] with IEditor = Some x } m.Editors
        { m with Editors = newEditors }, Cmd.none
    | FindEditor ->
        let action = m.Editors.[m.TabId].IEditor?getAction ("actions.find")
        action?run ()
        m, Cmd.none
    | FindAndReplaceEditor ->
        let action = m.Editors.[m.TabId].IEditor?getAction ("editor.action.startFindReplaceAction")
        action?run ()
        m, Cmd.none
    | UndoEditor ->
        m.Editors.[m.TabId].IEditor?trigger ("Update.fs", "undo") |> ignore
        m, Cmd.none
    | SelectAllEditor ->
        m.Editors.[m.TabId].IEditor?trigger ("Update.fs", "selectAll") |> ignore
        m, Cmd.none
    | RedoEditor ->
        m.Editors.[m.TabId].IEditor?trigger ("Update.fs", "redo") |> ignore
        m, Cmd.none
    | InitiateClose ->
        { m with InitClose = true }, Cmd.none
    | ReadOnlineInfo ve ->
        let cmd = 
            readOnlineInfo ve
                           m.LastOnlineFetchTime
                           m.Settings.OnlineFetchText
                           m.LastRemindTime
                           m.DebugLevel
        m, cmd
    | ReadOnlineInfoSuccess (newOnlineFetchText, ve) -> 
        let newLastOnlineFetchTime, newLastRemindTime, cmd = 
            readOnlineInfoSuccessUpdate newOnlineFetchText ve m.LastRemindTime
        let newSettings = { m.Settings with OnlineFetchText = newOnlineFetchText }
        { m with LastOnlineFetchTime = Ok System.DateTime.Now 
                 LastRemindTime = newLastRemindTime
                 Settings = newSettings }, cmd
    | ReadOnlineInfoFail ve -> 
        let newLastOnlineFetchTime, newLastRemindTime, cmd = 
            readOnlineInfoFailUpdate m.Settings.OnlineFetchText ve m.LastRemindTime
        { m with LastOnlineFetchTime = newLastOnlineFetchTime 
                 LastRemindTime = newLastRemindTime}, cmd
    | UpdateModel m ->
        m, Cmd.none
    | InitialiseIExports iExports -> 
        //iExports?languages?register (registerLanguage)
        //iExports?languages?setMonarchTokensProvider (token)
        { m with IExports = Some iExports } , Cmd.none
    | RunSimulation ->
        let newDialogBox, cmd = 
            runSimulation m.TabId m.DialogBox
        { m with DialogBox = newDialogBox }, 
        Cmd.batch [ RunningCode |> ReadOnlineInfo |> Cmd.ofMsg
                    cmd ]
    | MatchActiveMode ->
        let cmd = matchActiveMode m.RunMode
        m, cmd
    | SetCurrentModeActive (rs, ri) ->
        { m with RunMode = ActiveMode(rs, ri) }, Cmd.none
    | MatchRunMode ->
        let cmd = matchRunMode m.RunMode
        m, cmd
    | RunEditorTab ->
        m, Cmd.batch [ Cmd.ofMsg ResetEmulator
                       Cmd.ofMsg RrepareModeForExecution 
                       Cmd.ofMsg RunEditorRunMode ]
    | RunEditorRunMode ->
        let steps = 
            m.Settings.SimulatorMaxSteps
            |> int64 
            |> stepsFromSettings 
        let cmd = runEditorRunMode NoBreak steps m.Editors.[m.TabId].IEditor m.DebugLevel m.RunMode m.Decorations
        m, cmd
    | UpdateRunMode runMode ->
        { m with RunMode = runMode }, Cmd.none
    | UpdateDecorations decorations ->
        { m with Decorations = decorations }, Cmd.none
    | MatchLoadImage info ->
        let cmd = matchLI info
        m, cmd
    | AsmStepDisplay (breakCon, steps, ri) -> //TODO:
        m, Cmd.none
    | RrepareModeForExecution ->
        let cmd, newDialogBox = 
            prepareModeForExecution m.RunMode m.Editors.[m.TabId].IEditor m.DialogBox
        { m with DialogBox = newDialogBox }, cmd
    | RunTestBench -> //TODO:
        m, Cmd.none
    | IsItTestbench ->
        let cmd = isItTestbench m.Editors.[m.TabId].IEditor
        m, cmd
    | ResetEmulator ->
        printfn "Resetting..."
        { m with MemoryMap = Map.empty
                 SymbolMap = Map.empty
                 RegMap = initialRegMap
                 RunMode = ResetMode
                 ClockTime = (0uL, 0uL)
                 Flags = initialFlags
                 FlagsHasChanged = initialFlags }, 
        Cmd.batch [ Cmd.ofMsg DeleteAllContentWidgets 
                    Cmd.ofMsg EnableEditors 
                    Cmd.ofMsg RemoveDecorations ]
    | RemoveDecorations ->
        List.iter (fun x -> removeDecorations m.Editors.[m.TabId].IEditor x) 
                  m.Decorations
        { m with Decorations = [] }, Cmd.none
    | EnableEditors ->
        { m with EditorEnable = true }, Cmd.none
    | DeleteAllContentWidgets ->
        deleteAllContentWidgets m.CurrentTabWidgets m.Editors.[m.TabId].IEditor
        { m with CurrentTabWidgets = Map.empty }, Cmd.none

let view (m : Model) (dispatch : Msg -> unit) =
    initialClose dispatch m.InitClose
    mainMenu dispatch m
    dialogBox (m.Settings.CurrentFilePath, m.Editors, m.TabId, m.SettingsTab)
              dispatch
              m.DialogBox
    div [ ClassName "window" ] 
        [ header [ ClassName "toolbar toolbar-header" ] 
                 [ div [ ClassName "toolbar-actions" ] 
                       [ div [ ClassName "btn-group" ]
                             [ button [ ClassName "btn btn-default" 
                                        DOMAttr.OnClick (fun _ -> OpenFileDialog |> dispatch) ]
                                      [ span [ ClassName "icon icon-folder" ] [] ]
                               button [ ClassName "btn btn-default"
                                        DOMAttr.OnClick (fun _ -> SaveFile |> dispatch) ]
                                      [ span [ ClassName "icon icon-floppy" ] [] ] ]
                         button [ ClassName "btn btn-fixed btn-default button-run"
                                  DOMAttr.OnClick (fun _ -> RunSimulation |> dispatch) ]
                                [ m.RunMode |> runButtonText |> str ]
                         button [ ClassName "btn btn-default"
                                  DOMAttr.OnClick (fun _ -> ResetEmulator |> dispatch) ]
                                [ str "Reset" ]
                         button [ ClassName "btn btn-default button-back" ]
                                  //DOMAttr.OnClick (fun _ -> Integration.stepCodeBack m |> UpdateModel |> dispatch) ]
                                [ str " Step" ]
                         button [ ClassName "btn btn-default button-forward" ]
                                  //DOMAttr.OnClick (fun _ -> Integration.stepCode m.TabId m.Editors m |> UpdateModel |> dispatch )]
                                [ str "Step " ]
                         (statusBar m.RunMode)
                         div [ ClassName "btn-group clock" ]
                             [ tooltips (Content clockSymTooltipStr :: Placement "bottom" :: basicTooltipsPropsLst)
                                        [ button [ ClassName "btn btn-large btn-default clock-symbol" ]
                                                 [ str "\U0001F551" ]]
                               tooltips (Content clockTooltipStr :: Placement "bottom" :: basicTooltipsPropsLst)
                                        [ button [ ClassName "btn btn-large btn-default clock-time" ]//; Disabled true ]
                                                 [ m.ClockTime |> clockText |> str  ] ] ]
                         repButtons m.CurrentRep dispatch ] ]
          div [ ClassName "window-content" ] 
              [ div [ ClassName "pane-group" ] 
                    [ div [ ClassName "pane file-view-pane"] 
                          ((editorPanel (m.TabId, m.Editors, m.SettingsTab, m.Settings, m.EditorEnable) 
                                       dispatch) @
                           [ div [ ClassName "overlay darken-overlay"] []])
                      div [ ClassName "pane dashboard"
                            dashboardStyle m.CurrentRep ]
                          [ viewButtons m.CurrentView dispatch
                            viewPanel (m.CurrentRep, m.CurrentView, m.RegMap) 
                                      (m.MemoryMap, m.SymbolMap, m.ByteView, m.ReverseDirection, m.RunMode)
                                      dispatch
                            footer m.Flags m.FlagsHasChanged ] ] ] ]

Program.mkProgram init update view
#if DEBUG
|> Program.withHMR
#endif
|> Program.withReact "app"
|> Program.run
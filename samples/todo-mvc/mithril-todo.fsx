
// Load Fable.Core and bindings to JS global objects
#r "node_modules/fable-core/Fable.Core.dll"
#load "../../src/fable-mithril.fs"

open System.Text.RegularExpressions
open System
open Fable.Core
open Fable.Import.Browser
open Fable.Import
open Fable.Core.JsInterop
open Fable.Import.MithrilBase
open Fable.Import.Mithril

//mithril demo
type LocalStorage<'t>(id :string) =
    let store_id = id

    member x.get() :'t option =
            Browser.localStorage.getItem(store_id) 
            |> function null -> None | x -> Some (unbox x)
            |> Core.Option.map ofJson<'t>

    member x.set (ls :'t) = 
        Browser.localStorage.setItem(store_id, toJson(ls))

type Reminder = { description: MithrilBase.Property<string>; 
                  date: MithrilBase.Property<DateTime>;
                  edited: MithrilBase.Property<bool>;
                  error: MithrilBase.Property<bool>;
                  editing: MithrilBase.Property<bool>;
                  prev: MithrilBase.Property<string>  }

                static member New dis dte =
                 {description = property dis; date = property DateTime.UtcNow; edited = property false; error = property false; editing = property false; prev = property ""} 

                


type Cont() =
    
    let local = LocalStorage<array<Reminder>>("reminder_list")
    member this.MaxString = 100

    member this.List with get () = 
                        match local.get() with
                        | Some(ary) -> ary
                        | None -> Array.empty<Reminder>
                      and set (x) = local.set(x)

    member this.Discription = property ""

    member this.Add() =
        if not (this.Discription.get = "") && (Regex.Replace(this.Discription.get,"/\s/g","").Length <= this.MaxString) then 
            this.List <- Array.append [| (Reminder.New (this.Discription.get.Trim()) DateTime.UtcNow )|] this.List
            this.Discription.set ""

    interface Controller with
        member x.onunload evt = () :> obj


let item_view (vm :Cont) (index :int) (r :Reminder) =
    let charlim = vm.MaxString - r.description.get.Length
    let charlimit = span (attr [css "inner-status"]) [charlim.ToString()]
    let description = div (attr [
                            css (if r.editing.get then "hide" else "show") ;
                            css "description" ;
                            prop "data-edited" r.edited ;
                            prop "data-error" r.error
                            onDblClick (fun e -> r.editing.set true
                                                 r.prev.set r.description.get)
                        ]) [r.description]
    let ts = (DateTime.UtcNow - r.date.get) 
    let ago = if ts.Days > 0 then ts.Days.ToString() + " days ago"
              else if ts.Hours > 0 then ts.Hours.ToString() + " hours ago"
              else if ts.Minutes > 0 then ts.Minutes.ToString() + " minutes ago"
              else ts.Seconds.ToString() + " seconds ago"
    let edit = textarea (attr [
                            css (if r.editing.get then "show" else "hide") ;
                            prop "rows" 1;
                            onInput (bindattr "value" r.description.set) ;
                            onKeyup (fun e -> 
                                        let e2 = e :?> KeyboardEvent
                                        if e2.keyCode  = 13. then 
                                            r.editing.set false
                                        else if e2.keyCode  = 27. then 
                                            r.description.set r.prev.get
                                            r.editing.set false
                                        else 
                                            Mithril.redrawStrategy "none");
                            onBlur (fun e -> 
                                r.description.set r.prev.get
                                r.editing.set false);
                            
                        ]) [r.description.get] ;
    li None [
        label (attr [prop "data-date" r.date])
        [
            description;
            edit;
            charlimit;
            span (attr [css "date"]) [ ago]

        ]
    ] :> obj

let main_view (vm1 :Cont) =  
    div (attr [css "task-container"]) [
        h1 None ["Notifications"] ;
        input (attr [
                    css "add-remind" ;
                    prop "placeholder" "Notification test" ;
                    prop "autofocus" true ;
                    prop "value" vm1.Discription.get ;
                    onKeyup (fun e -> let e2 = e :?> KeyboardEvent
                                      if e2.keyCode = 13. then 
                                        vm1.Add()
                                      else if e2.keyCode = 27. then 
                                        vm1.Discription.set ""
                                      else 
                                        Mithril.redrawStrategy "none") ;
                    onInput (bindattr "value" vm1.Discription.set) ;
        ]) [] ;
        button (attr [
                     css "add"
                     onClick (fun e -> vm1.Add())
        ]) [] ;
        div (attr [name "input-status"]) 
            (if (Regex.Replace(vm1.Discription.get,"/\s/g","").Length <= vm1.MaxString) 
            then [(vm1.MaxString - Regex.Replace(vm1.Discription.get,"/\s/g","").Length).ToString()  + " character left"] 
            else [ span (attr [css "danger"]) ["limit of " + vm1.MaxString.ToString()]]) ;
        ul (attr [name "todo-list"])
            (if vm1.List.Length = 0 then [] else (vm1.List |> Array.mapi (item_view vm1) |> Array.toList))
    ]

        
let vm = Cont()
let vm_init x = vm  
let com = newComponent vm_init main_view


        
mount(document.body, com )        
        

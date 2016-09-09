Mithril.fs: Mithril bindings and api layer for Fable
======
This api follows KISS priciples. 
Mithril is a lightweight and fast MVC js framework with VDom.
Most of m api is wrapped accept for the m() function which is replaced by the Html Element and Attribute generator functions.


##View
Html element functions: `Attributes option -> obj List -> VirtualElement` However only `string`, `VirtualElement` and `Component<'t>` as objects are accepted.
Attributes generater function `attr`:  `(string * obj) list -> Attributes option`
Attribute helper functions: `prop`, `css`, `name`, `incss`, `bindattr`, and all events return a `(string * obj)`

##Controller
Can be any type that implements `Controller` interface.

##Model
Can be any type you want, however it is a good idea to place data in `Property<'t>` with `property` function.

##Routing
Yet to come.

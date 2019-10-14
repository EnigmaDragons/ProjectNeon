----

## Unity - PR Review Rules

**Any of these rules being violated will result in PR Changes Requested**

----

## Basic PR Rules

- No Scene should have any Console Errors
- No PR should contains files from an unrelated branch
- Double-check your PR after you open it, to ensure that it doesn't have unexpected files
- Every new feature should be tested in the Unity Editor

----

## Style Conventions

- All editor fields should be private, camel-cased `[SerializeField] private Type name`
- All properties should be Pascal-cased `public string Name { get; }`
- All methods should be Pascal-cased `public void Execute()`
- Don't use code regions

----

## Unity-isms

- Don't include constructors in any `MonoBehavior` or `ScriptbleObject`
- If an object should be configurable from code, use a method `public void Init(...)` instead
- Every PreFab and ScriptableObject should have sensible defaults wired in
- There should never be any unused methods in code (such as Start/Update)
- There should never be any Unity-generated comments in code
- All Scriptable Objects must be able to be created in the Editor
- Every script should be small and single purpose

----

## Unity PreFab Standards

- Everything in a scene must be composed of nothing except high-level PreFabs
- Every PreFab must be able to be dropped into a new Scene and "just work"
- No Prefab may have any overrides when used in a Scene or another PreFab
- PreFabs must have all of their required connections set in the Editor
- ScriptableObjects are safe connections, since the are Global Singletons
- Children elements are safe connections, since they exist within the current hierarchical 
- PreFabs must not have more 8 immediate children
- When a generic PreFab is available for an element, use the generic PreFab instead of a custom one (buttons, for example)

----

## Event Guidelines

- Events should be subscribed to in the `Awake` phase
- Events should never be published until the `Start` phase or later

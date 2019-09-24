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

## Event Guidelines

- Events should be subscribed to in the `Awake` phase
- Events should never be published until the `Start` phase or later

----

## Unity - PR Review Rules

**Any of these rules being violated will result in PR Changes Requested**

----

- No Scene should have any Console Errors
- Every PreFab and ScriptableObject should have sensible defaults wired in
- There should never be any unused methods in code (such as Start/Update)
- There should never be any Unity-generated comments in code
- All Scriptable Objects must be able to be created in the Editor
- Every script should be small and single purpose
- Every new feature should be tested in the Unity Editor


- Events should be subscribed to in the `Awake` phase
- Events should never be published until the `Start` phase or later

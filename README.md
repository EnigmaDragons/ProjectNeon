# ProjectNeon

----

[Project Kanban Board](https://zube.io/enigmadragons/projectneon/w/neon-all/kanban)

----

### Development Environment Setup

Software Requirements:
- Git
- Unity 2019 (Version 2019.2.0f1)
- Any C# IDE

Setup:
1. Clone this repository using git
2. Install any C# IDE of your choice. [Visual Studio 2019](https://visualstudio.microsoft.com/thank-you-downloading-visual-studio/?sku=Community&rel=16) recommended.
3. Install [Unity Hub](https://unity3d.com/get-unity/download)
4. Launch Unity Hub and Install Unity Version 2019.2.0f1
5. In Unity Hub, click Add
6. Browse and select `../repo/src/ProjectNeon`
7. Click on `ProjectNeon`

----

### Role Leads

- Architect: SilasReinagel
- Character Art
- Environment Art
- User Interface
- Game Battle Design
- Game Progression Design
- Sound Effects
- Music
- Programming

----

### Project Management

We use [0crat](https://www.0crat.com) to manage this project.  Below are a list of some useful 0crat
commands:

- Assigning a role to a new member in the project: once the user is added to 0crat platform, the project 
ARC can assign roles to that user by the `assign` command in slack chat:
    ```
    @0crat assign <role> <username> <rate>
    ```
    where
    ```
    <role> - desired role (ARC|PO|DEV|REV|QA|TST)
    <username> - github user username, with the leading "@"
    <rate> - user hourly rate, with leading "$"
    ```
    For example:
    ```
    @0crat assign DEV @paulodamaso $35
    ```
    will assign `DEV` role to user `@paulodamaso` at a rate of `$35` USD / hour.
    
- Boosting an issue: 0crat considers by default a budget of 30 minutes to each registered issue. If
for some reason the project architect needs to change this rate this is possible via the `boost` ]
command. To change a task rate, just tell the 0crat bot in the issue:
    ```
    @0crat boost 4x
    ``` 
    This command tells that the project architect wants to boost the issue value to 60 minutes instead 
    of 30 minutes. Notice that 0crat uses 15 minutes as his base unit, so each DEV issue is 
    automatically boosted at 2x.

- Adding issues to scope: 0crat manages only issues that have been previously added to the project 
scope. These scopes are marked with the `scope` label in github and will be sorted and distributed 
by 0crat bot to any available developer. Project ARC can put projects in scope by three ways:

    1. adding manyally the `scope` label to the project in github repo
    1. asking for 0crat to do it by using the `in` command in the github issue comments:
    ```
    @0crat in
    ```
    will put the issue in scope and the bot will answer the following:
    ```
    @0crat in (here)

    @paulodamaso Job #14 is now in scope, role is DEV
    ```
    1. assigning it directly to some performer through the `assign` command in github issue comment:
    ```
    @0crat assign @silasreinagel
    ```
    will put the issue in scope and assign it to the user `@silasreinagel`
    
 For any doubt about 0crat commands, please refer to the [0crat policy](http://www.zerocracy.com/policy.html).
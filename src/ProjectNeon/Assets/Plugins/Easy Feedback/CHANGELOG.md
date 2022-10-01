# Changelog
All notable changes to Easy Feedback will be documented in this file, starting from v2.0.0

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

For the full changelog, please see https://aesthetic.games/easy-feedback/docs/release/changelog.html

## [Unreleased]

### Added
- Option to resize screenshots larger than 1080p.
- [Editor] Warning about Trello's attachment filesize limits.

### Changed
- Capture screenshot to memory instead of local file.
- Use attachment API to upload screenshot.

### Fixed
- All attachment uploads fail if screenshot upload fails.
- Screenshots not captured in WebGL builds.
- Screenshots sometimes left behind on filesystem.
- Crash on Switch on form opened.
- [Editor] Setup buttons on Feedback Form component don't do anything.

## [2.0.0] - 2021-06-02

### Added
- `AeLa.EasyFeedback`, `AeLa.EasyFeedback.Editor` and `AeLa.easyFeedback.Demo` assembly definitions.
- Toast system for sending messages to the player.
- Order field for label (priority) in dropdown.
- Email field on default Feedback prefab.
- Button to open current feedback board in settings.

### Changed
- Update namespaces for new assemblies. 
- Replace submitting/submitted/error popup with toasts to improve submission UX.
- Configuration moved to Project Settings.
- Minor settings UI changes.
- Moved docs to DocFX.

### Removed
- Dropped support for Unity 2019.3 and older.

### Fixed
- Trello authentication fails due to whitespace in token.
- Form gets stuck on screen during submission.

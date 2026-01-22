# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0.0] - Not Yet Released

### Added
* Created an analyzer + code fix to automatically add `[GeneratorSerializable]` (issue [#19](https://github.com/JasonBock/CslaGeneratorSerialization/issues/19))

### Changed
* Updated generated code to use `IMobileObjectMetastate` (issue [#14](https://github.com/JasonBock/CslaGeneratorSerialization/issues/14))
* Removed methods that were no longer used (issue [#33](https://github.com/JasonBock/CslaGeneratorSerialization/issues/33))

### Fixed
* Addressed an issue with n-level undo (issue [#16](https://github.com/JasonBock/CslaGeneratorSerialization/issues/16))
* Business objects that do not exist in a namespace now work (issue [#18](https://github.com/JasonBock/CslaGeneratorSerialization/issues/18))
* Numerous types are now handled correctly (issue [#20](https://github.com/JasonBock/CslaGeneratorSerialization/issues/20) and issue [#27](https://github.com/JasonBock/CslaGeneratorSerialization/issues/27))
* An issue with generic types was fixed (issue [#25](https://github.com/JasonBock/CslaGeneratorSerialization/issues/20))
* The accessibly of the target business object is now used in the generated code (issue [#29](https://github.com/JasonBock/CslaGeneratorSerialization/issues/29))

## [1.0.0] - 2025-09-12

### Fixed
* Addressed an issue with n-level undo (issue [#16](https://github.com/JasonBock/CslaGeneratorSerialization/issues/16))
* Business objects that do not exist in a namespace now work (issue [#18](https://github.com/JasonBock/CslaGeneratorSerialization/issues/18))

## [0.3.0] - 2025-02-01

### Added
* Changed `isNotSealed` to `isSealed` (issue [#7](https://github.com/JasonBock/CslaGeneratorSerialization/issues/7))
* Added support for custom serialization (issue [#1](https://github.com/JasonBock/CslaGeneratorSerialization/issues/1))
* Added support to serialize `IMobileObject`-based objects that do not participate in generator serialization (issue [#11](https://github.com/JasonBock/CslaGeneratorSerialization/issues/11))
* Improved serialization performance of Reflection-based members using `[UnsafeAccessor]` (issue [#12](https://github.com/JasonBock/CslaGeneratorSerialization/issues/12))

## [0.2.0] - 2024-04-15

### Added
* Created `[GeneratorSerializable]` to use to identify types to generate code for (issue [#3](https://github.com/JasonBock/CslaGeneratorSerialization/issues/3))
* Added support to register implementations for custom types (issue [#2](https://github.com/JasonBock/CslaGeneratorSerialization/issues/2))
* Moved common serialization code to context types (issue [#6](https://github.com/JasonBock/CslaGeneratorSerialization/issues/6))

## [0.1.0] - 2024-04-03

### Added
* First release of the package.
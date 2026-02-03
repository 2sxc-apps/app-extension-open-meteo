# Template App: Empty v00.17.00

This is just a simple template app, which should be used for new apps.
It contains a basic structure and some instructions.

Note that the version number is set to 00.17.00.
This is the version of the template app, not the version of the app you are creating.

We set the second version number to match the 2sxc version we are using, so we can easily see which version of 2sxc the app was created with.
The third number is just a counter, which we increase when we make changes to the template app.

## Git Repo

This app is maintained by 2sic, and the official repo is at [app-template-empty](https://github.com/2sxc-dev/app-template-empty)

## What are these files?

Since you chose the empty template, it doesn't include the instructions in the web site.

The [docs](https://docs.2sxc.org/basics/app/folders-and-files/index.html) should give you some insights.

If that's not enough, you should start with a more sophisticated template such as **app-template-basic**.

## History

### 2026-02-03

1. 2rb: Added using to Httpclient

### 2026-02-02

1. 2rb: Replaced hardcoded fields constant with `OpenMeteoConstants.ExpectedFields` in `OpenMeteoCurrent`
1. 2rb: Removed unnecessary `Uri.EscapeDataString()` call for constant fields
1. 2rb: Removed redundant null check in `OpenMeteoForecast.GetForecast()` (already handled in `ToForecastModels()`)
1. 2rb: Changed `OpenMeteoHelpers` class from `public` to `internal`
1. 2rb: Added XML documentation to several files
1. 2rb: Refactored `OpenMeteoHelpers.Download()` extracted URL building and JSON download into separate methods for clarity
1. 2rb: Added views for documentation `Docs-OpenMeteoCurrent` and `Docs-OpenMeteoForecast`
1. 2rb: Migrated from `WebClient` to `HttpClient` in `OpenMeteoHelpers.DownloadJson()`
1. 2rb: Added static `HttpClient` instance with User-Agent header to `OpenMeteoHelpers`
1. 2rb: Used `ConfigureAwait(false)` with synchronous result pattern
1. 2rb: Added Title to Current View
1. 2dm: introduced strongly typed parameters `OpenMeteoParameters` with XML documentation
1. 2dm: Slimmed down code a bit more, especially using `if()...return` pattern

### 2026-01-30

1. 2dm: made some internal classes internal
1. 2dm: Moved DTOs to their own namespace
1. 2dm: Removed unused usings
1. 2dm: Some Todos for @2rb
1. 2dm: Renamed `OpenMeteoResultModel` to `OpenMeteoResult`

### 2026-01-28

1. 2rb: added test views

### 2026-01-27

1. 2rb: `OpenMeteoForecast` improved to work with `OpenMeteoResult`
1. 2dm: Move constants to main namespace, make internal, add helper method

### 2026-01-26

1. 2dm: Simplified `OpenMeteoResult` to use the CustomModel base class
1. 2rb: Moved test views to right location

### 2026-01-25

1. 2dm: Created configuration to export content-type to proper namespace & folder
1. 2dm: Placed OpenMeteoConfiguration in correct Extensions namespace
1. 2dm: Extended the resulting data to return JSON for further use
1. 2dm: Made internal models `internal` instead of `public` as they are not used outside
1. 2dm: Attempted to make OpenMeteoCurrent use typed data instead of dynamic - but it looks like OpenMeteoCurrent actually doesn't work
1. 2dm: Introduced translation of weather codes in OpenMeteoCurrent
1. 2dm: Fixed OpenMeteoForecast to use configuration values for lat/long
1. 2dm: Created sample of Typed output

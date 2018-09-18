# Changelog

## v1.2 - 2018.09.15
- Use [Nerdbank.GitVersioning](https://github.com/AArnott/Nerdbank.GitVersioning) to automate generation of assembly 
  and nuget package versions.
- Update [Gremit](https://github.com/skbkontur/gremit) dependency to v2.3.
- Add `IAbstractionConfigurator.UseTypes(Type[] types)` overload.
- Rename `ILog` -> `IGroboContainerLog` to not pollute intellisense in dependent projects with irrelevant symbols.

## v1.1 - 2018.01.10
- Support .NET Standard 2.0.
- Switch to SDK-style project format and dotnet core build tooling.

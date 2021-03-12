# Changelog

## v1.2.xx - 2021.03.xx
- Update dependencies.
- Run tests against net5.0 tfm.

## v1.2.52 - 2021.01.21
- Fix incorrect dispose order [issue](https://github.com/skbkontur/GroboContainer/issues/12).

## v1.2 - 2018.09.15
- Use [Nerdbank.GitVersioning](https://github.com/AArnott/Nerdbank.GitVersioning) to automate generation of assembly 
  and nuget package versions.
- Update [Gremit](https://github.com/skbkontur/gremit) dependency to v2.3.
- Add `IAbstractionConfigurator.UseTypes(Type[] types)` overload.
- Rename `ILog` -> `IGroboContainerLog` to not pollute intellisense in dependent projects with irrelevant symbols.
- Reduce memory allocations (PR [#3](https://github.com/skbkontur/GroboContainer/pull/3)).
- Instead of InvalidOperationException throw ManyGenericSubstitutionsException (PR [#5](https://github.com/skbkontur/GroboContainer/pull/5)).
- Reduce memory allocations (PR [#9](https://github.com/skbkontur/GroboContainer/pull/9)).

## v1.1 - 2018.01.10
- Support .NET Standard 2.0.
- Switch to SDK-style project format and dotnet core build tooling.

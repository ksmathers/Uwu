# Kawaisugiru (可愛すぎる)

Kawaisugiru is the "Too Cute" library for development support of .NET applications.   It eliminates all of the work writing those annoying
little classes that you need for failsafe configurability, parsing, argument list handling and passing, and trivial networking.

Libraries:

  o Uwu
    -- Uwu.Core

		Arglist - Command argument handling with shell style string parsing and operations
		ConfigIni - A base class for INI initialization, loading, and saving.  Extend this to add your own application specific properties
		Environment - Unified namespace for System.Environment, System.SpecialFolder, and any runtime 'environment' settings you might like
		Format - Simplified type formatting, like ToString(), but more convenient when coercing types
		Interpolate - Interpolate shell style strings delimited by %VARNAME%
		Parse - Simplified type parsing, like TryParse(), but more convenient when using defaults in place of parsing errors
		StringUtil - Extended string operations like TrimEnd, Dequote, ShellEscape, among others

    -- Uwu.Config

		IniData - A permissive INI file parser.  Provides usable configuration information even if there are a variety of syntax errors
		IniFile - Save/Load INI files to %LOCALAPPDATA%\%APPLICATION%.ini
		
  o UwuNet
    -- UwuNet
    
    		Registry - Protocol registry for UwuNet communications protocols
		Message - Messages types that can be sent over UwuNet
		OrchestrationMessage - A message containing a string.

		




# Kawaisugiru (可愛すぎる)

Kawaisugiru is the "Too Cute" library for development support of .NET applications.   It eliminates all of the work writing those annoying
little classes that you need for failsafe configurability, parsing, argument list handling and passing, and trivial networking.

Libraries:

  * Uwu
    * Uwu.Core

      * Arglist - Command argument handling with shell style string parsing and operations
      * ConfigIni - A base class for INI initialization, loading, and saving.  Extend this to add your own application specific properties
      * Environment - Unified namespace for System.Environment, System.SpecialFolder, and any runtime 'environment' settings you might like
      * Format - Simplified type formatting, like ToString(), but more convenient when coercing types
      * Interpolate - Interpolate shell style strings delimited by %VARNAME%
      * Parse - Simplified type parsing, like TryParse(), but more convenient when using defaults in place of parsing errors
      * StringUtil - Extended string operations like TrimEnd, Dequote, ShellEscape, among others

    * Uwu.Config

      * IniData - A permissive INI file parser.  Provides usable configuration information even if there are a variety of syntax errors
      * IniFile - Save/Load INI files to %LOCALAPPDATA%\%APPLICATION%.ini
		
  * UwuNet
    
      * Registry - Protocol registry for UwuNet communications protocols
      * Message - Messages types that can be sent over UwuNet
      * OrchestrationMessage - A message containing a string.

  * UwuForms

      * MessageDeliveryAgent - Adapter for IMessaging that delivers messages
        to the main thread of a Windows.Forms.Control container (usually 
	the main Form).  The agent is a valid Component so can be dragged
	into the visual form editor.
      * Heartbeat - Timer component with a heartbeat that triggers in frames
        per second (Fps)
      * Canvas - A double buffered image control with a DrawCanvas() event that
        is triggered by the application Heartbeat.   Call Start(heartbeat) to 
	begin the programmed updates.   Until Start() is called the canvas will
	by default display an animated GIF.  The default image buffers have a
	resolution of 1024x1024 into which you will draw.  If you are using 
	a non-square shape for your canvas then you can use the CanvasSize property
	to resize the buffers to your desired resolution and aspect ratio.
      * StripChart - An example use of Canvas to display a continuously scrolling 
        sine wave.
    
  * UwuCrypto
  
      * SimpleCA - PKI (X.509) Simplified Certificate Authority, for bilateral authentication.  Incomplete.




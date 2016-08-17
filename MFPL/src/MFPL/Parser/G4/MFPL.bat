@ECHO OFF

SET CLASSPATH=%HomePath%\.nuget\packages\Antlr4.CodeGenerator\4.5.3.1-beta001\tools\antlr4-csharp-4.5.3.1-SNAPSHOT-complete.jar
SET Namespace=MFPL.Parser.G4
SET  G4Folder=%~dp0
SET    G4File=Mfpl
SET   Options=-Dlanguage=CSharp_v4_5 -package %Namespace% -visitor -listener

DEL /Q %G4Folder%%G4File%.tokens
DEL /Q %G4Folder%%G4File%Lexer.cs
DEL /Q %G4Folder%%G4File%Lexer.tokens
DEL /Q %G4Folder%%G4File%Parser.cs
DEL /Q %G4Folder%%G4File%Listener.cs
DEL /Q %G4Folder%%G4File%BaseListener.cs
DEL /Q %G4Folder%%G4File%Visitor.cs
DEL /Q %G4Folder%%G4File%BaseVisitor.cs

SET FullPath=%G4Folder%%G4File%.g4

JAVA org.antlr.v4.Tool %FullPath% %Options%

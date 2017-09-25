# Akka.Unity.Sample
A sample of working Akka.Net 1.3.0 implementation in Unity3D.

To build this, you will need Unity3D 2017.1.0p5 with the experimental .Net 4.6 support turned on.

This sample only works on Windows and Android, Mono 4.6 (IL2CPP is not supported). 

Any platform that requires AOT is not supported because AOT in Unity does not support Reflection.Emit

## Chat
This is a port of the chat example from the Akka.Net repository using Unity3D as the chat client. 

Working on both Windows and Android build target.

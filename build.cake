#addin "wk.StartProcess";
using PS = StartProcess.Processor;

Task("Run").Does(() => {
    PS.StartProcess("dotnet build src/BitmapComposition");
    PS.StartProcess("mono src/BitmapComposition/bin/Debug/net47/BitmapComposition.exe");
});

var target = Argument("target", "default");
RunTarget(target);
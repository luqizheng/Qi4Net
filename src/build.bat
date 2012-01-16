
msbuild "%cd%\Qi\Qi\Qi.csproj"   /property:Configuration=Debug /p:Configuration=Debug /p:OutputPath="%cd%\Publish\Debug"
msbuild "%cd%\Qi\Qi\Qi.csproj"   /property:Configuration=Release /p:Configuration=Release /p:OutputPath="%cd%\Publish\Release"

msbuild "%cd%\Qi\Qi.Web\Qi.Web.csproj"   /property:Configuration=Debug /p:Configuration=Debug /p:OutputPath="%cd%\Publish\Debug"
msbuild "%cd%\Qi\Qi.Web\Qi.Web.csproj"   /property:Configuration=Release /p:Configuration=Release /p:OutputPath="%cd%\Publish\Release"
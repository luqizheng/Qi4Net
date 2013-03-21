
msbuild "%cd%\Qi_src\Qi\Qi.csproj"   /property:Configuration=Debug /p:Configuration=Debug 
msbuild "%cd%\Qi_src\Qi\Qi.csproj"   /property:Configuration=Release /p:Configuration=Release 

msbuild "%cd%\Qi_src\Qi.Web\Qi.Web.csproj"   /property:Configuration=Debug /p:Configuration=Debug 
msbuild "%cd%\Qi_src\Qi.Web\Qi.Web.csproj"   /property:Configuration=Release /p:Configuration=Release 

msbuild "%cd%\Qi_src\Qi.NHibernateExtender\Qi.NHibernateExtender.csproj"   /property:Configuration=Debug /p:Configuration=Debug 
msbuild "%cd%\Qi_src\Qi.NHibernateExtender\Qi.NHibernateExtender.csproj"   /property:Configuration=Release /p:Configuration=Release
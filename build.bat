@echo off
set base=%~dp0
set findnet=powershell -command "$([System.Runtime.InteropServices.RuntimeEnvironment]::GetRuntimeDirectory())"

for /f "tokens=* USEBACKQ" %%I in (`%findnet%`) DO (
  set netframework=%%I
)

pushd ""%base%
cd /d ""%base%
del PasswordModifiedColumn.plgx
%netframework%MSBuild.exe /target:clean PasswordModifiedColumn.sln
%netframework%MSBuild.exe /p:Configuration=ReleasePlgx /m PasswordModifiedColumn.sln
copy /y PasswordModifiedColumn\bin\ReleasePlgx\PasswordModifiedColumn.dll mono
copy /y PasswordModifiedColumn\bin\ReleasePlgx\keepass2-developerextensions.dll mono
copy /y PasswordModifiedColumn\bin\ReleasePlgx\PasswordModifiedColumn.plgx .
popd
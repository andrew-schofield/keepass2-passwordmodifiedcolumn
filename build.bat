@echo off
set base=%~dp0
set findnet=powershell -command "$([System.Runtime.InteropServices.RuntimeEnvironment]::GetRuntimeDirectory())"

for /f "tokens=* USEBACKQ" %%I in (`%findnet%`) DO (
  set netframework=%%I
)

pushd ""%base%
cd /d ""%base%
del PasswordModifiedColumn.plgx
del mono\PasswordModifiedColumn.dll
rd /s /q PasswordModifiedColumn\bin
rd /s /q PasswordModifiedColumn\obj
"%ProgramFiles(x86)%\KeePass Password Safe 2\KeePass.exe" --plgx-create "%base%PasswordModifiedColumn"
%netframework%MSBuild.exe /target:clean PasswordModifiedColumn.sln
%netframework%MSBuild.exe /p:Configuration=Release /m PasswordModifiedColumn.sln
copy /y PasswordModifiedColumn\bin\Release\PasswordModifiedColumn.dll mono
popd
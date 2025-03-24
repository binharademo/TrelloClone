@echo off
setlocal

:: Definir variáveis
set "SQLITE_VERSION=3450000"
set "SQLITE_ZIP=sqlite-tools-win-x64-%SQLITE_VERSION%.zip"
set "SQLITE_URL=https://www.sqlite.org/2024/%SQLITE_ZIP%"
set "INSTALL_DIR=C:\sqlite"
set "TEMP_ZIP=%TEMP%\%SQLITE_ZIP%"

:: Criar diretório de instalação
if not exist "%INSTALL_DIR%" mkdir "%INSTALL_DIR%"

:: Baixar o arquivo ZIP
echo Baixando SQLite...
powershell -Command "Invoke-WebRequest -Uri '%SQLITE_URL%' -OutFile '%TEMP_ZIP%'"

:: Verificar se o download foi bem-sucedido
if not exist "%TEMP_ZIP%" (
    echo Erro: Falha ao baixar o arquivo ZIP.
    exit /b 1
)

:: Extrair o arquivo ZIP
echo Extraindo arquivos...
powershell -Command "Expand-Archive -Path '%TEMP_ZIP%' -DestinationPath '%INSTALL_DIR%' -Force"

:: Verificar se a extração foi bem-sucedida
if not exist "%INSTALL_DIR%\sqlite3.exe" (
    echo Erro: Falha ao extrair os arquivos.
    exit /b 1
)

:: Adicionar ao PATH
echo Adicionando SQLite ao PATH...
setx PATH "%PATH%;%INSTALL_DIR%"

:: Verificar a instalação
echo Verificando a instalação...
"%INSTALL_DIR%\sqlite3.exe" --version

:: Limpar arquivo temporário
del "%TEMP_ZIP%"

echo Instalação concluída com sucesso!
pause
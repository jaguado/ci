@echo off
echo Bajar App Pool
C:\Windows\System32\inetsrv\appcmd stop AppPool "Sitio privado"

echo Respaldar Sitio Privadobuildbui
7za a D:\Deploy\Bkp\wwwCuprum.zip D:\Sitios\wwwrootSSL\wwwCuprum

echo Desplegar cambios
7za x -y D:\Deploy\wwwCuprumDeploy.zip D:\Sitios\wwwrootSSL\

echo Copiar config
ConfigManager D:\Deploy\cambios.config D:\Sitios\wwwrootSSL\wwwCuprum\web.config copiar

echo Subir App Pool
C:\Windows\System32\inetsrv\appcmd start AppPool "Sitio privado"
pause
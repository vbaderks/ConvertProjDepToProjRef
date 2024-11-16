dotnet publish -c release
signtool sign /fd SHA256 /td SHA256 /v /sha1 b834c6c1d7e0ae8e76cadcf9e2e7a273133a5df6 /tr "http://time.certum.pl/" "build\bin\Release\win-x64\publish\ConvertProjDepToProjRef.exe"

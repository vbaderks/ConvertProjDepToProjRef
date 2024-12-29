:: SPDX-FileCopyrightText: © 2024 Victor Derks
:: SPDX-License-Identifier: MIT

dotnet publish -c release
signtool sign /fd SHA256 /td SHA256 /v /sha1 6aa8d11bf7a1f5dfc85ac785a9dfd067770f0b93 /tr "http://time.certum.pl/" "artifacts\publish\ConvertProjDepToProjRef\release\ConvertProjDepToProjRef.exe"

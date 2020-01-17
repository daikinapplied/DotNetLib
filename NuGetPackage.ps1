Write-Host "                                                        "
Write-Host "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~"
Write-Host "DotNetLib NuGet Package Tool                            "
Write-Host "Developed by Daikin Applied                             "
Write-Host "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~"
Write-Host "                                                        "
Write-Host "? Create .NET Framework NuGet packages.                 "
Write-Host "                                                        "
Write-Host

Function BuildNuGet
{
	param ([string]$project)

	Write-Host "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~"
	cd $project
	Write-Host "~~ Build: $project ~~"
	Write-Host

	$prodLocations = @("release","prd")
	$prodBits = ""
	foreach ($prodBits in $prodLocations)
	{
		try
		{
			$searchFile = "bin\$prodBits\$project.dll"
			Write-Host "Searching: $searchFile"
			$dllFile = Get-Item $searchFile
			if ($dllFile)
			{
				Write-Host "Found: $dllFile.FullName"
				$versionInfo = $dllFile.VersionInfo
				break
			}
		}
		catch [System.Exception]
		{
			$versionInfo = $null
		}
	}

	if ($versionInfo)
	{
		$version = $versionInfo.FileMajorPart.ToString() + "." + $versionInfo.FileMinorPart.ToString() + "." + $versionInfo.FileBuildPart.ToString()
		if ( $versionInfo.FilePrivatePart.ToString() -ne "0" ) 
		{ 
		   $version = $version + "." + $versionInfo.FilePrivatePart.ToString()
		}
	
		nuget pack $project.nuspec -Version $version -Prop Configuration=Release
		Move-Item "$project.$version.nupkg" "bin\$prodBits\" -Force
		cd ..
		Write-Host
	}
	else
	{
		cd ..
		Write-Host ":( Unable to find a package to deploy"
		exit 1
	}
}

# ~~ .NET Framework NuGet packaging (.NET Standard and .NET Core projects can build this automatically in Visual Studio 2017/2019) ~~
BuildNuGet "Daikin.DotNetLib.DotNetNuke"
BuildNuGet "Daikin.DotNetLib.Windows"
BuildNuGet "Daikin.DotNetLib.Serial"

exit 0
# ~End~

# SIG # Begin signature block
# MIIYcAYJKoZIhvcNAQcCoIIYYTCCGF0CAQExCzAJBgUrDgMCGgUAMGkGCisGAQQB
# gjcCAQSgWzBZMDQGCisGAQQBgjcCAR4wJgIDAQAABBAfzDtgWUsITrck0sYpfvNR
# AgEAAgEAAgEAAgEAAgEAMCEwCQYFKw4DAhoFAAQU14I/wZTMw1vy+DEBwo0kSmBQ
# COugghMHMIIEFDCCAvygAwIBAgILBAAAAAABL07hUtcwDQYJKoZIhvcNAQEFBQAw
# VzELMAkGA1UEBhMCQkUxGTAXBgNVBAoTEEdsb2JhbFNpZ24gbnYtc2ExEDAOBgNV
# BAsTB1Jvb3QgQ0ExGzAZBgNVBAMTEkdsb2JhbFNpZ24gUm9vdCBDQTAeFw0xMTA0
# MTMxMDAwMDBaFw0yODAxMjgxMjAwMDBaMFIxCzAJBgNVBAYTAkJFMRkwFwYDVQQK
# ExBHbG9iYWxTaWduIG52LXNhMSgwJgYDVQQDEx9HbG9iYWxTaWduIFRpbWVzdGFt
# cGluZyBDQSAtIEcyMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAlO9l
# +LVXn6BTDTQG6wkft0cYasvwW+T/J6U00feJGr+esc0SQW5m1IGghYtkWkYvmaCN
# d7HivFzdItdqZ9C76Mp03otPDbBS5ZBb60cO8eefnAuQZT4XljBFcm05oRc2yrmg
# jBtPCBn2gTGtYRakYua0QJ7D/PuV9vu1LpWBmODvxevYAll4d/eq41JrUJEpxfz3
# zZNl0mBhIvIG+zLdFlH6Dv2KMPAXCae78wSuq5DnbN96qfTvxGInX2+ZbTh0qhGL
# 2t/HFEzphbLswn1KJo/nVrqm4M+SU4B09APsaLJgvIQgAIMboe60dAXBKY5i0Eex
# +vBTzBj5Ljv5cH60JQIDAQABo4HlMIHiMA4GA1UdDwEB/wQEAwIBBjASBgNVHRMB
# Af8ECDAGAQH/AgEAMB0GA1UdDgQWBBRG2D7/3OO+/4Pm9IWbsN1q1hSpwTBHBgNV
# HSAEQDA+MDwGBFUdIAAwNDAyBggrBgEFBQcCARYmaHR0cHM6Ly93d3cuZ2xvYmFs
# c2lnbi5jb20vcmVwb3NpdG9yeS8wMwYDVR0fBCwwKjAooCagJIYiaHR0cDovL2Ny
# bC5nbG9iYWxzaWduLm5ldC9yb290LmNybDAfBgNVHSMEGDAWgBRge2YaRQ2XyolQ
# L30EzTSo//z9SzANBgkqhkiG9w0BAQUFAAOCAQEATl5WkB5GtNlJMfO7FzkoG8IW
# 3f1B3AkFBJtvsqKa1pkuQJkAVbXqP6UgdtOGNNQXzFU6x4Lu76i6vNgGnxVQ380W
# e1I6AtcZGv2v8Hhc4EvFGN86JB7arLipWAQCBzDbsBJe/jG+8ARI9PBw+DpeVoPP
# PfsNvPTF7ZedudTbpSeE4zibi6c1hkQgpDttpGoLoYP9KOva7yj2zIhd+wo7AKvg
# IeviLzVsD440RZfroveZMzV+y5qKu0VN5z+fwtmK+mWybsd+Zf/okuEsMaL3sCc2
# SI8mbzvuTXYfecPlf5Y1vC0OzAGwjn//UYCAp5LUs0RGZIyHTxZjBzFLY7Df8zCC
# BJ8wggOHoAMCAQICEhEh1pmnZJc+8fhCfukZzFNBFDANBgkqhkiG9w0BAQUFADBS
# MQswCQYDVQQGEwJCRTEZMBcGA1UEChMQR2xvYmFsU2lnbiBudi1zYTEoMCYGA1UE
# AxMfR2xvYmFsU2lnbiBUaW1lc3RhbXBpbmcgQ0EgLSBHMjAeFw0xNjA1MjQwMDAw
# MDBaFw0yNzA2MjQwMDAwMDBaMGAxCzAJBgNVBAYTAlNHMR8wHQYDVQQKExZHTU8g
# R2xvYmFsU2lnbiBQdGUgTHRkMTAwLgYDVQQDEydHbG9iYWxTaWduIFRTQSBmb3Ig
# TVMgQXV0aGVudGljb2RlIC0gRzIwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEK
# AoIBAQCwF66i07YEMFYeWA+x7VWk1lTL2PZzOuxdXqsl/Tal+oTDYUDFRrVZUjtC
# oi5fE2IQqVvmc9aSJbF9I+MGs4c6DkPw1wCJU6IRMVIobl1AcjzyCXenSZKX1GyQ
# oHan/bjcs53yB2AsT1iYAGvTFVTg+t3/gCxfGKaY/9Sr7KFFWbIub2Jd4NkZrItX
# nKgmK9kXpRDSRwgacCwzi39ogCq1oV1r3Y0CAikDqnw3u7spTj1Tk7Om+o/SWJMV
# TLktq4CjoyX7r/cIZLB6RA9cENdfYTeqTmvT0lMlnYJz+iz5crCpGTkqUPqp0Dw6
# yuhb7/VfUfT5CtmXNd5qheYjBEKvAgMBAAGjggFfMIIBWzAOBgNVHQ8BAf8EBAMC
# B4AwTAYDVR0gBEUwQzBBBgkrBgEEAaAyAR4wNDAyBggrBgEFBQcCARYmaHR0cHM6
# Ly93d3cuZ2xvYmFsc2lnbi5jb20vcmVwb3NpdG9yeS8wCQYDVR0TBAIwADAWBgNV
# HSUBAf8EDDAKBggrBgEFBQcDCDBCBgNVHR8EOzA5MDegNaAzhjFodHRwOi8vY3Js
# Lmdsb2JhbHNpZ24uY29tL2dzL2dzdGltZXN0YW1waW5nZzIuY3JsMFQGCCsGAQUF
# BwEBBEgwRjBEBggrBgEFBQcwAoY4aHR0cDovL3NlY3VyZS5nbG9iYWxzaWduLmNv
# bS9jYWNlcnQvZ3N0aW1lc3RhbXBpbmdnMi5jcnQwHQYDVR0OBBYEFNSihEo4Whh/
# uk8wUL2d1XqH1gn3MB8GA1UdIwQYMBaAFEbYPv/c477/g+b0hZuw3WrWFKnBMA0G
# CSqGSIb3DQEBBQUAA4IBAQCPqRqRbQSmNyAOg5beI9Nrbh9u3WQ9aCEitfhHNmmO
# 4aVFxySiIrcpCcxUWq7GvM1jjrM9UEjltMyuzZKNniiLE0oRqr2j79OyNvy0oXK/
# bZdjeYxEvHAvfvO83YJTqxr26/ocl7y2N5ykHDC8q7wtRzbfkiAD6HHGWPZ1BZo0
# 8AtZWoJENKqA5C+E9kddlsm2ysqdt6a65FDT1De4uiAO0NOSKlvEWbuhbds8zkSd
# wTgqreONvc0JdxoQvmcKAjZkiLmzGybu555gxEaovGEzbM9OuZy5avCfN/61PU+a
# 003/3iCOTpem/Z8JvE3KGHbJsE2FUPKA0h0G9VgEB7EYMIIE6zCCA9OgAwIBAgIQ
# CwcG+m5b/nuagVPeiumLGzANBgkqhkiG9w0BAQsFADB/MQswCQYDVQQGEwJVUzEd
# MBsGA1UEChMUU3ltYW50ZWMgQ29ycG9yYXRpb24xHzAdBgNVBAsTFlN5bWFudGVj
# IFRydXN0IE5ldHdvcmsxMDAuBgNVBAMTJ1N5bWFudGVjIENsYXNzIDMgU0hBMjU2
# IENvZGUgU2lnbmluZyBDQTAeFw0xNzEyMTEwMDAwMDBaFw0yMTAxMDgyMzU5NTla
# MIGCMQswCQYDVQQGEwJVUzESMBAGA1UECAwJTWlubmVzb3RhMREwDwYDVQQHDAhQ
# bHltb3V0aDElMCMGA1UECgwcRGFpa2luIEFwcGxpZWQgQW1lcmljYXMgSW5jLjEl
# MCMGA1UEAwwcRGFpa2luIEFwcGxpZWQgQW1lcmljYXMgSW5jLjCCASIwDQYJKoZI
# hvcNAQEBBQADggEPADCCAQoCggEBANqFiDNVHVpW/1J/I51zKS+HPybwqdmEa6nK
# AP+MyJuPI7OO90ngcF2u56Q3vvW5Dl+IhmjtwgCsDaJ7l3h59hjgYU34NB94Qcwa
# 28vkqml101T/NTj+496mYXiEStmGH+VpoiG5Vxfb3NI/HqRIVMqq/AOQtkTt7N0+
# zh5QA/FtlThvQ0J80Sjq5/srEP/QuIA9NI61ZPvCi6GsV2+qi/fVAUaUHNdxXP1j
# ydup05xRW6KhIQkhsuEbDN5wAKlqqR/XqP75aCDhNqnHT/ovYgvCci5Z+fC4Lm5o
# 3S1rv1FBKGHCBAW1v2Gno2gPpRssCLvjm8Yx9kFmTbqA1D540RcCAwEAAaOCAV0w
# ggFZMAkGA1UdEwQCMAAwDgYDVR0PAQH/BAQDAgeAMCsGA1UdHwQkMCIwIKAeoByG
# Gmh0dHA6Ly9zdi5zeW1jYi5jb20vc3YuY3JsMGEGA1UdIARaMFgwVgYGZ4EMAQQB
# MEwwIwYIKwYBBQUHAgEWF2h0dHBzOi8vZC5zeW1jYi5jb20vY3BzMCUGCCsGAQUF
# BwICMBkMF2h0dHBzOi8vZC5zeW1jYi5jb20vcnBhMBMGA1UdJQQMMAoGCCsGAQUF
# BwMDMFcGCCsGAQUFBwEBBEswSTAfBggrBgEFBQcwAYYTaHR0cDovL3N2LnN5bWNk
# LmNvbTAmBggrBgEFBQcwAoYaaHR0cDovL3N2LnN5bWNiLmNvbS9zdi5jcnQwHwYD
# VR0jBBgwFoAUljtT8Hkzl699g+8uK8zKt4YecmYwHQYDVR0OBBYEFLgy08kCaImP
# I3A0v2pscu+bXEAFMA0GCSqGSIb3DQEBCwUAA4IBAQA22A+DlxyjH8I4pzLyvNB3
# HLOrnfnlICtfw/hiZ9kKYrh7zCjM7iNtiONVWZFYjBAwtDPN1wypCG9Z3rFVzEve
# 8iR3mHciJEzWCNt5DUaPYREQwadbViu7FwhpWNLKEP5MEK9dXMdLULja02hvXuDy
# 4THL1PIgsKVpeXVo7Xr1H5ZlYTFWqnnHZUJbpgQbPx18ReMUorzJVsbUxpg0LR3+
# LklFHM+gGIdqpx/JrxVyerrHFrB7kkLF/HF265HLpPHK+HzxdbcYnIsCg15phnmc
# CsIeMHxoLrM27JmV0sD85bgmlQSD+SxPizeM7Lb7HDv0+JqBd5XlKp6AUjc4MFNu
# MIIFWTCCBEGgAwIBAgIQPXjX+XZJYLJhffTwHsqGKjANBgkqhkiG9w0BAQsFADCB
# yjELMAkGA1UEBhMCVVMxFzAVBgNVBAoTDlZlcmlTaWduLCBJbmMuMR8wHQYDVQQL
# ExZWZXJpU2lnbiBUcnVzdCBOZXR3b3JrMTowOAYDVQQLEzEoYykgMjAwNiBWZXJp
# U2lnbiwgSW5jLiAtIEZvciBhdXRob3JpemVkIHVzZSBvbmx5MUUwQwYDVQQDEzxW
# ZXJpU2lnbiBDbGFzcyAzIFB1YmxpYyBQcmltYXJ5IENlcnRpZmljYXRpb24gQXV0
# aG9yaXR5IC0gRzUwHhcNMTMxMjEwMDAwMDAwWhcNMjMxMjA5MjM1OTU5WjB/MQsw
# CQYDVQQGEwJVUzEdMBsGA1UEChMUU3ltYW50ZWMgQ29ycG9yYXRpb24xHzAdBgNV
# BAsTFlN5bWFudGVjIFRydXN0IE5ldHdvcmsxMDAuBgNVBAMTJ1N5bWFudGVjIENs
# YXNzIDMgU0hBMjU2IENvZGUgU2lnbmluZyBDQTCCASIwDQYJKoZIhvcNAQEBBQAD
# ggEPADCCAQoCggEBAJeDHgAWryyx0gjE12iTUWAecfbiR7TbWE0jYmq0v1obUfej
# DRh3aLvYNqsvIVDanvPnXydOC8KXyAlwk6naXA1OpA2RoLTsFM6RclQuzqPbROlS
# Gz9BPMpK5KrA6DmrU8wh0MzPf5vmwsxYaoIV7j02zxzFlwckjvF7vjEtPW7ctZlC
# n0thlV8ccO4XfduL5WGJeMdoG68ReBqYrsRVR1PZszLWoQ5GQMWXkorRU6eZW4U1
# V9Pqk2JhIArHMHckEU1ig7a6e2iCMe5lyt/51Y2yNdyMK29qclxghJzyDJRewFZS
# AEjM0/ilfd4v1xPkOKiE1Ua4E4bCG53qWjjdm9sCAwEAAaOCAYMwggF/MC8GCCsG
# AQUFBwEBBCMwITAfBggrBgEFBQcwAYYTaHR0cDovL3MyLnN5bWNiLmNvbTASBgNV
# HRMBAf8ECDAGAQH/AgEAMGwGA1UdIARlMGMwYQYLYIZIAYb4RQEHFwMwUjAmBggr
# BgEFBQcCARYaaHR0cDovL3d3dy5zeW1hdXRoLmNvbS9jcHMwKAYIKwYBBQUHAgIw
# HBoaaHR0cDovL3d3dy5zeW1hdXRoLmNvbS9ycGEwMAYDVR0fBCkwJzAloCOgIYYf
# aHR0cDovL3MxLnN5bWNiLmNvbS9wY2EzLWc1LmNybDAdBgNVHSUEFjAUBggrBgEF
# BQcDAgYIKwYBBQUHAwMwDgYDVR0PAQH/BAQDAgEGMCkGA1UdEQQiMCCkHjAcMRow
# GAYDVQQDExFTeW1hbnRlY1BLSS0xLTU2NzAdBgNVHQ4EFgQUljtT8Hkzl699g+8u
# K8zKt4YecmYwHwYDVR0jBBgwFoAUf9Nlp8Ld7LvwMAnzQzn6Aq8zMTMwDQYJKoZI
# hvcNAQELBQADggEBABOFGh5pqTf3oL2kr34dYVP+nYxeDKZ1HngXI9397BoDVTn7
# cZXHZVqnjjDSRFph23Bv2iEFwi5zuknx0ZP+XcnNXgPgiZ4/dB7X9ziLqdbPuzUv
# M1ioklbRyE07guZ5hBb8KLCxR/Mdoj7uh9mmf6RWpT+thC4p3ny8qKqjPQQB6rqT
# og5QIikXTIfkOhFf1qQliZsFay+0yQFMJ3sLrBkFIqBgFT/ayftNTI/7cmd3/SeU
# x7o1DohJ/o39KK9KEr0Ns5cF3kQMFfo2KwPcwVAB8aERXRTl4r0nS1S+K4ReD6bD
# dAUK75fDiSKxH3fzvc1D1PFMqT+1i4SvZPLQFCExggTTMIIEzwIBATCBkzB/MQsw
# CQYDVQQGEwJVUzEdMBsGA1UEChMUU3ltYW50ZWMgQ29ycG9yYXRpb24xHzAdBgNV
# BAsTFlN5bWFudGVjIFRydXN0IE5ldHdvcmsxMDAuBgNVBAMTJ1N5bWFudGVjIENs
# YXNzIDMgU0hBMjU2IENvZGUgU2lnbmluZyBDQQIQCwcG+m5b/nuagVPeiumLGzAJ
# BgUrDgMCGgUAoHAwEAYKKwYBBAGCNwIBDDECMAAwGQYJKoZIhvcNAQkDMQwGCisG
# AQQBgjcCAQQwHAYKKwYBBAGCNwIBCzEOMAwGCisGAQQBgjcCARUwIwYJKoZIhvcN
# AQkEMRYEFOVKGVRwLAt2l8WwGRxGEQCFwJwLMA0GCSqGSIb3DQEBAQUABIIBAE/f
# yG5IH4NJtUVNeL8zyI4jM9A+HXVSVdtoUb0LZXYoQCFlLTmp2OhJAbf7WmiiYC8o
# 1SgR9JnSBzapIDlL1XzQrFt0ritjvSyYiGe24CvaFtyi5iUHeqDZif27/nxBns9c
# 9Keoarxx9mVGcmkf8IQi2E/TOwt0xCX+DIj6VmT0yBql7AntrL0tpZJgyRbHgJGE
# qcibICK8JEZ+sCtj0dGG6YivQSOv3CFOC6aGCGwI/WkFMWjvFJju4BpgMGF/Vguv
# TzfDS3xhRg5y8iJymWe179kxyLHfsRWYSYIvFUMLAu/1V5bBhqiCAXUkuyKhJk35
# pdx+kaY4LxlNuMqDGNGhggKiMIICngYJKoZIhvcNAQkGMYICjzCCAosCAQEwaDBS
# MQswCQYDVQQGEwJCRTEZMBcGA1UEChMQR2xvYmFsU2lnbiBudi1zYTEoMCYGA1UE
# AxMfR2xvYmFsU2lnbiBUaW1lc3RhbXBpbmcgQ0EgLSBHMgISESHWmadklz7x+EJ+
# 6RnMU0EUMAkGBSsOAwIaBQCggf0wGAYJKoZIhvcNAQkDMQsGCSqGSIb3DQEHATAc
# BgkqhkiG9w0BCQUxDxcNMjAwMTE3MjMyMTUwWjAjBgkqhkiG9w0BCQQxFgQU22fa
# vVg2WdsUASN2gpaTz8qlD44wgZ0GCyqGSIb3DQEJEAIMMYGNMIGKMIGHMIGEBBRj
# uC+rYfWDkJaVBQsAJJxQKTPseTBsMFakVDBSMQswCQYDVQQGEwJCRTEZMBcGA1UE
# ChMQR2xvYmFsU2lnbiBudi1zYTEoMCYGA1UEAxMfR2xvYmFsU2lnbiBUaW1lc3Rh
# bXBpbmcgQ0EgLSBHMgISESHWmadklz7x+EJ+6RnMU0EUMA0GCSqGSIb3DQEBAQUA
# BIIBAHQlgOlvyGx2F0+rvoN4qOMkTgRnaP2VqmKqZQ6XTKJjkSE7Vw9XfH4U0aUa
# Lt/StdRAQnFEETAmQ6eoeWvXl3xvTy2OdyhF9j9YlvNla8kkfRjBaCoIiGtJQ+dE
# Mma57sb1X8STmDUj7EuEShW296a/rnetARMgzv2jty70Ivha5TxajOStQoH14Fm0
# rAzlsMpduT6woyMo1Hd3aEPrvIt62Q+Lbgnxu//LtlE3FlNvwatDu/VNf8vaRwOL
# lp9JiimOepYpCEkmMB58s7eodumM/ss4lZ6c1TqBGP86+GHAupwj/KceC2StR95D
# W12eb2mLEIhllrpzHsAP81NY2WM=
# SIG # End signature block
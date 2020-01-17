param(
	[string]$apiKey="", 
	[string]$certIdentifier="",
	[string]$certIdType="subject")

Write-Host "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~"
Write-Host "DotNetLib NuGet Publish Tool                            "
Write-Host "Developed by Hans Dickel                                "
Write-Host "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~"

Write-Host "This tool digitally signs via subject (default) or fingerprint, and publishes to NuGet.org."
Write-Host ""

# Digitally Signing Timestamp Server for NuGet Packages (nupkg)
#http://timestamp.verisign.com/scripts/timstamp.dll # <-- Causes the error "ASN1 bad tag value met" when used with nupkg
#http://timestamp.comodoca.com?td=sha256            # <-- works with nupkg
$timeServer="http://timestamp.comodoca.com?td=sha256"
$nugetServer="https://www.nuget.org/api/v2/package"

Function PublishNuGet
{
    param ([string]$project)

	Write-Host "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~"
    Write-Host "~~ Publish: $project ~~"

	Try
    {
        $newestPackage = Get-ChildItem $project\bin\Release\*.nupkg -File -ErrorAction Stop | Sort-Object LastAccessTime -Descending | Select-Object -First 1
    }
    Catch [System.Exception]
    {
        $newestPackage = $null
    }

    if ($newestPackage)
    {
		# NuGet 5.3.1 and earlier that supports signing defaults to "CurrentUser" for the store location
		if ($certIdType.ToLower() -eq "subject")
		{
			nuget sign $newestPackage.FullName -CertificateStoreLocation "LocalMachine" -CertificateSubjectName $certIdentifier -Timestamper $timeServer
		}
		else
		{
			nuget sign $newestPackage.FullName -CertificateStoreLocation "LocalMachine" -CertificateFingerprint $certIdentifier -Timestamper $timeServer
		}
		
		try
		{
			nuget push $newestPackage.FullName -Source $nugetServer -ApiKey $apiKey
		}
		catch 
		{
			Write-Host "!! Issue encountered pushing NuGet package $newestPackage.FullName to $nugetServer"
		}

    }
    else
    {
        Write-Host "Unable to find a package to deploy"
    } 
	Write-Host ""
}

if ($apiKey.Length -eq 0)
{
	$apiKey = Read-Host -Prompt 'Enter your NuGet Api Key'
}
else
{
	Write-Host "(NuGet ApiKey passed via command-line)"
}

if ($certIdentifier.Length -eq 0)
{
	$certIdentifier = Read-Host -Prompt "Enter your NuGet Certificate Identifier ($certIdType)"
}
else
{
	Write-Host "(NuGet Certificate '$certIdType' passed via command-line)"
}

if ($apiKey.Length -ne 0)
{
	PublishNuGet "RecursiveGeek.DotNetLib.Application"
	PublishNuGet "RecursiveGeek.DotNetLib.Data"
	PublishNuGet "RecursiveGeek.DotNetLib.DotNetNuke"
	PublishNuGet "RecursiveGeek.DotNetLib.Facebook"
	PublishNuGet "RecursiveGeek.DotNetLib.Security"
	PublishNuGet "RecursiveGeek.DotNetLib.Network"
	PublishNuGet "RecursiveGeek.DotNetLib.Windows"
	PublishNuGet "RecursiveGeek.DotNetLib.Serial"
}
else
{
	Write-Host "ApiKey not specified.  Discontinuing."
}

exit 0
# ~End~

# SIG # Begin signature block
# MIIXtwYJKoZIhvcNAQcCoIIXqDCCF6QCAQExCzAJBgUrDgMCGgUAMGkGCisGAQQB
# gjcCAQSgWzBZMDQGCisGAQQBgjcCAR4wJgIDAQAABBAfzDtgWUsITrck0sYpfvNR
# AgEAAgEAAgEAAgEAAgEAMCEwCQYFKw4DAhoFAAQUamw/g8UlWHncxpxU8eNT9iWX
# Xe2gghLlMIID7jCCA1egAwIBAgIQfpPr+3zGTlnqS5p31Ab8OzANBgkqhkiG9w0B
# AQUFADCBizELMAkGA1UEBhMCWkExFTATBgNVBAgTDFdlc3Rlcm4gQ2FwZTEUMBIG
# A1UEBxMLRHVyYmFudmlsbGUxDzANBgNVBAoTBlRoYXd0ZTEdMBsGA1UECxMUVGhh
# d3RlIENlcnRpZmljYXRpb24xHzAdBgNVBAMTFlRoYXd0ZSBUaW1lc3RhbXBpbmcg
# Q0EwHhcNMTIxMjIxMDAwMDAwWhcNMjAxMjMwMjM1OTU5WjBeMQswCQYDVQQGEwJV
# UzEdMBsGA1UEChMUU3ltYW50ZWMgQ29ycG9yYXRpb24xMDAuBgNVBAMTJ1N5bWFu
# dGVjIFRpbWUgU3RhbXBpbmcgU2VydmljZXMgQ0EgLSBHMjCCASIwDQYJKoZIhvcN
# AQEBBQADggEPADCCAQoCggEBALGss0lUS5ccEgrYJXmRIlcqb9y4JsRDc2vCvy5Q
# WvsUwnaOQwElQ7Sh4kX06Ld7w3TMIte0lAAC903tv7S3RCRrzV9FO9FEzkMScxeC
# i2m0K8uZHqxyGyZNcR+xMd37UWECU6aq9UksBXhFpS+JzueZ5/6M4lc/PcaS3Er4
# ezPkeQr78HWIQZz/xQNRmarXbJ+TaYdlKYOFwmAUxMjJOxTawIHwHw103pIiq8r3
# +3R8J+b3Sht/p8OeLa6K6qbmqicWfWH3mHERvOJQoUvlXfrlDqcsn6plINPYlujI
# fKVOSET/GeJEB5IL12iEgF1qeGRFzWBGflTBE3zFefHJwXECAwEAAaOB+jCB9zAd
# BgNVHQ4EFgQUX5r1blzMzHSa1N197z/b7EyALt0wMgYIKwYBBQUHAQEEJjAkMCIG
# CCsGAQUFBzABhhZodHRwOi8vb2NzcC50aGF3dGUuY29tMBIGA1UdEwEB/wQIMAYB
# Af8CAQAwPwYDVR0fBDgwNjA0oDKgMIYuaHR0cDovL2NybC50aGF3dGUuY29tL1Ro
# YXd0ZVRpbWVzdGFtcGluZ0NBLmNybDATBgNVHSUEDDAKBggrBgEFBQcDCDAOBgNV
# HQ8BAf8EBAMCAQYwKAYDVR0RBCEwH6QdMBsxGTAXBgNVBAMTEFRpbWVTdGFtcC0y
# MDQ4LTEwDQYJKoZIhvcNAQEFBQADgYEAAwmbj3nvf1kwqu9otfrjCR27T4IGXTdf
# plKfFo3qHJIJRG71betYfDDo+WmNI3MLEm9Hqa45EfgqsZuwGsOO61mWAK3ODE2y
# 0DGmCFwqevzieh1XTKhlGOl5QGIllm7HxzdqgyEIjkHq3dlXPx13SYcqFgZepjhq
# IhKjURmDfrYwggSjMIIDi6ADAgECAhAOz/Q4yP6/NW4E2GqYGxpQMA0GCSqGSIb3
# DQEBBQUAMF4xCzAJBgNVBAYTAlVTMR0wGwYDVQQKExRTeW1hbnRlYyBDb3Jwb3Jh
# dGlvbjEwMC4GA1UEAxMnU3ltYW50ZWMgVGltZSBTdGFtcGluZyBTZXJ2aWNlcyBD
# QSAtIEcyMB4XDTEyMTAxODAwMDAwMFoXDTIwMTIyOTIzNTk1OVowYjELMAkGA1UE
# BhMCVVMxHTAbBgNVBAoTFFN5bWFudGVjIENvcnBvcmF0aW9uMTQwMgYDVQQDEytT
# eW1hbnRlYyBUaW1lIFN0YW1waW5nIFNlcnZpY2VzIFNpZ25lciAtIEc0MIIBIjAN
# BgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAomMLOUS4uyOnREm7Dv+h8GEKU5Ow
# mNutLA9KxW7/hjxTVQ8VzgQ/K/2plpbZvmF5C1vJTIZ25eBDSyKV7sIrQ8Gf2Gi0
# jkBP7oU4uRHFI/JkWPAVMm9OV6GuiKQC1yoezUvh3WPVF4kyW7BemVqonShQDhfu
# ltthO0VRHc8SVguSR/yrrvZmPUescHLnkudfzRC5xINklBm9JYDh6NIipdC6Anqh
# d5NbZcPuF3S8QYYq3AhMjJKMkS2ed0QfaNaodHfbDlsyi1aLM73ZY8hJnTrFxeoz
# C9Lxoxv0i77Zs1eLO94Ep3oisiSuLsdwxb5OgyYI+wu9qU+ZCOEQKHKqzQIDAQAB
# o4IBVzCCAVMwDAYDVR0TAQH/BAIwADAWBgNVHSUBAf8EDDAKBggrBgEFBQcDCDAO
# BgNVHQ8BAf8EBAMCB4AwcwYIKwYBBQUHAQEEZzBlMCoGCCsGAQUFBzABhh5odHRw
# Oi8vdHMtb2NzcC53cy5zeW1hbnRlYy5jb20wNwYIKwYBBQUHMAKGK2h0dHA6Ly90
# cy1haWEud3Muc3ltYW50ZWMuY29tL3Rzcy1jYS1nMi5jZXIwPAYDVR0fBDUwMzAx
# oC+gLYYraHR0cDovL3RzLWNybC53cy5zeW1hbnRlYy5jb20vdHNzLWNhLWcyLmNy
# bDAoBgNVHREEITAfpB0wGzEZMBcGA1UEAxMQVGltZVN0YW1wLTIwNDgtMjAdBgNV
# HQ4EFgQURsZpow5KFB7VTNpSYxc/Xja8DeYwHwYDVR0jBBgwFoAUX5r1blzMzHSa
# 1N197z/b7EyALt0wDQYJKoZIhvcNAQEFBQADggEBAHg7tJEqAEzwj2IwN3ijhCcH
# bxiy3iXcoNSUA6qGTiWfmkADHN3O43nLIWgG2rYytG2/9CwmYzPkSWRtDebDZw73
# BaQ1bHyJFsbpst+y6d0gxnEPzZV03LZc3r03H0N45ni1zSgEIKOq8UvEiCmRDoDR
# EfzdXHZuT14ORUZBbg2w6jiasTraCXEQ/Bx5tIB7rGn0/Zy2DBYr8X9bCT2bW+IW
# yhOBbQAuOA2oKY8s4bL0WqkBrxWcLC9JG9siu8P+eJRRw4axgohd8D20UaF5Mysu
# e7ncIAkTcetqGVvP6KUwVyyJST+5z3/Jvz4iaGNTmr1pdKzFHTx/kuDDvBzYBHUw
# ggTrMIID06ADAgECAhALBwb6blv+e5qBU96K6YsbMA0GCSqGSIb3DQEBCwUAMH8x
# CzAJBgNVBAYTAlVTMR0wGwYDVQQKExRTeW1hbnRlYyBDb3Jwb3JhdGlvbjEfMB0G
# A1UECxMWU3ltYW50ZWMgVHJ1c3QgTmV0d29yazEwMC4GA1UEAxMnU3ltYW50ZWMg
# Q2xhc3MgMyBTSEEyNTYgQ29kZSBTaWduaW5nIENBMB4XDTE3MTIxMTAwMDAwMFoX
# DTIxMDEwODIzNTk1OVowgYIxCzAJBgNVBAYTAlVTMRIwEAYDVQQIDAlNaW5uZXNv
# dGExETAPBgNVBAcMCFBseW1vdXRoMSUwIwYDVQQKDBxEYWlraW4gQXBwbGllZCBB
# bWVyaWNhcyBJbmMuMSUwIwYDVQQDDBxEYWlraW4gQXBwbGllZCBBbWVyaWNhcyBJ
# bmMuMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA2oWIM1UdWlb/Un8j
# nXMpL4c/JvCp2YRrqcoA/4zIm48js473SeBwXa7npDe+9bkOX4iGaO3CAKwNonuX
# eHn2GOBhTfg0H3hBzBrby+SqaXXTVP81OP7j3qZheIRK2YYf5WmiIblXF9vc0j8e
# pEhUyqr8A5C2RO3s3T7OHlAD8W2VOG9DQnzRKOrn+ysQ/9C4gD00jrVk+8KLoaxX
# b6qL99UBRpQc13Fc/WPJ26nTnFFboqEhCSGy4RsM3nAAqWqpH9eo/vloIOE2qcdP
# +i9iC8JyLln58LgubmjdLWu/UUEoYcIEBbW/YaejaA+lGywIu+ObxjH2QWZNuoDU
# PnjRFwIDAQABo4IBXTCCAVkwCQYDVR0TBAIwADAOBgNVHQ8BAf8EBAMCB4AwKwYD
# VR0fBCQwIjAgoB6gHIYaaHR0cDovL3N2LnN5bWNiLmNvbS9zdi5jcmwwYQYDVR0g
# BFowWDBWBgZngQwBBAEwTDAjBggrBgEFBQcCARYXaHR0cHM6Ly9kLnN5bWNiLmNv
# bS9jcHMwJQYIKwYBBQUHAgIwGQwXaHR0cHM6Ly9kLnN5bWNiLmNvbS9ycGEwEwYD
# VR0lBAwwCgYIKwYBBQUHAwMwVwYIKwYBBQUHAQEESzBJMB8GCCsGAQUFBzABhhNo
# dHRwOi8vc3Yuc3ltY2QuY29tMCYGCCsGAQUFBzAChhpodHRwOi8vc3Yuc3ltY2Iu
# Y29tL3N2LmNydDAfBgNVHSMEGDAWgBSWO1PweTOXr32D7y4rzMq3hh5yZjAdBgNV
# HQ4EFgQUuDLTyQJoiY8jcDS/amxy75tcQAUwDQYJKoZIhvcNAQELBQADggEBADbY
# D4OXHKMfwjinMvK80Hccs6ud+eUgK1/D+GJn2QpiuHvMKMzuI22I41VZkViMEDC0
# M83XDKkIb1nesVXMS97yJHeYdyIkTNYI23kNRo9hERDBp1tWK7sXCGlY0soQ/kwQ
# r11cx0tQuNrTaG9e4PLhMcvU8iCwpWl5dWjtevUflmVhMVaqecdlQlumBBs/HXxF
# 4xSivMlWxtTGmDQtHf4uSUUcz6AYh2qnH8mvFXJ6uscWsHuSQsX8cXbrkcuk8cr4
# fPF1txiciwKDXmmGeZwKwh4wfGguszbsmZXSwPzluCaVBIP5LE+LN4zstvscO/T4
# moF3leUqnoBSNzgwU24wggVZMIIEQaADAgECAhA9eNf5dklgsmF99PAeyoYqMA0G
# CSqGSIb3DQEBCwUAMIHKMQswCQYDVQQGEwJVUzEXMBUGA1UEChMOVmVyaVNpZ24s
# IEluYy4xHzAdBgNVBAsTFlZlcmlTaWduIFRydXN0IE5ldHdvcmsxOjA4BgNVBAsT
# MShjKSAyMDA2IFZlcmlTaWduLCBJbmMuIC0gRm9yIGF1dGhvcml6ZWQgdXNlIG9u
# bHkxRTBDBgNVBAMTPFZlcmlTaWduIENsYXNzIDMgUHVibGljIFByaW1hcnkgQ2Vy
# dGlmaWNhdGlvbiBBdXRob3JpdHkgLSBHNTAeFw0xMzEyMTAwMDAwMDBaFw0yMzEy
# MDkyMzU5NTlaMH8xCzAJBgNVBAYTAlVTMR0wGwYDVQQKExRTeW1hbnRlYyBDb3Jw
# b3JhdGlvbjEfMB0GA1UECxMWU3ltYW50ZWMgVHJ1c3QgTmV0d29yazEwMC4GA1UE
# AxMnU3ltYW50ZWMgQ2xhc3MgMyBTSEEyNTYgQ29kZSBTaWduaW5nIENBMIIBIjAN
# BgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAl4MeABavLLHSCMTXaJNRYB5x9uJH
# tNtYTSNiarS/WhtR96MNGHdou9g2qy8hUNqe8+dfJ04LwpfICXCTqdpcDU6kDZGg
# tOwUzpFyVC7Oo9tE6VIbP0E8ykrkqsDoOatTzCHQzM9/m+bCzFhqghXuPTbPHMWX
# BySO8Xu+MS09bty1mUKfS2GVXxxw7hd924vlYYl4x2gbrxF4GpiuxFVHU9mzMtah
# DkZAxZeSitFTp5lbhTVX0+qTYmEgCscwdyQRTWKDtrp7aIIx7mXK3/nVjbI13Iwr
# b2pyXGCEnPIMlF7AVlIASMzT+KV93i/XE+Q4qITVRrgThsIbnepaON2b2wIDAQAB
# o4IBgzCCAX8wLwYIKwYBBQUHAQEEIzAhMB8GCCsGAQUFBzABhhNodHRwOi8vczIu
# c3ltY2IuY29tMBIGA1UdEwEB/wQIMAYBAf8CAQAwbAYDVR0gBGUwYzBhBgtghkgB
# hvhFAQcXAzBSMCYGCCsGAQUFBwIBFhpodHRwOi8vd3d3LnN5bWF1dGguY29tL2Nw
# czAoBggrBgEFBQcCAjAcGhpodHRwOi8vd3d3LnN5bWF1dGguY29tL3JwYTAwBgNV
# HR8EKTAnMCWgI6Ahhh9odHRwOi8vczEuc3ltY2IuY29tL3BjYTMtZzUuY3JsMB0G
# A1UdJQQWMBQGCCsGAQUFBwMCBggrBgEFBQcDAzAOBgNVHQ8BAf8EBAMCAQYwKQYD
# VR0RBCIwIKQeMBwxGjAYBgNVBAMTEVN5bWFudGVjUEtJLTEtNTY3MB0GA1UdDgQW
# BBSWO1PweTOXr32D7y4rzMq3hh5yZjAfBgNVHSMEGDAWgBR/02Wnwt3su/AwCfND
# OfoCrzMxMzANBgkqhkiG9w0BAQsFAAOCAQEAE4UaHmmpN/egvaSvfh1hU/6djF4M
# pnUeeBcj3f3sGgNVOftxlcdlWqeOMNJEWmHbcG/aIQXCLnO6SfHRk/5dyc1eA+CJ
# nj90Htf3OIup1s+7NS8zWKiSVtHITTuC5nmEFvwosLFH8x2iPu6H2aZ/pFalP62E
# LinefLyoqqM9BAHqupOiDlAiKRdMh+Q6EV/WpCWJmwVrL7TJAUwnewusGQUioGAV
# P9rJ+01Mj/tyZ3f9J5THujUOiEn+jf0or0oSvQ2zlwXeRAwV+jYrA9zBUAHxoRFd
# FOXivSdLVL4rhF4PpsN0BQrvl8OJIrEfd/O9zUPU8UypP7WLhK9k8tAUITGCBDww
# ggQ4AgEBMIGTMH8xCzAJBgNVBAYTAlVTMR0wGwYDVQQKExRTeW1hbnRlYyBDb3Jw
# b3JhdGlvbjEfMB0GA1UECxMWU3ltYW50ZWMgVHJ1c3QgTmV0d29yazEwMC4GA1UE
# AxMnU3ltYW50ZWMgQ2xhc3MgMyBTSEEyNTYgQ29kZSBTaWduaW5nIENBAhALBwb6
# blv+e5qBU96K6YsbMAkGBSsOAwIaBQCgcDAQBgorBgEEAYI3AgEMMQIwADAZBgkq
# hkiG9w0BCQMxDAYKKwYBBAGCNwIBBDAcBgorBgEEAYI3AgELMQ4wDAYKKwYBBAGC
# NwIBFTAjBgkqhkiG9w0BCQQxFgQUFj+5cDd4U+DCEdilaSqBbkHzLGEwDQYJKoZI
# hvcNAQEBBQAEggEAXXLMqIff3dNGyMZw94Qk8T+/8/yfgJJHHkiyAvYjXXn2/z7+
# mYYUtiurwna3wYSP/CXWqB9YET9kjUC0CDV4KTsq2NzApL/E5lqpi10APjgO67VH
# JvUoNRFlmWhewuLykfLuUcySU3zw99SYdkWkr6/mCbl7+ymVE8AV6dAVRQH5NJXx
# AWuxvtTfOLqfvUujuqU40OAYBHQFvH4nbP/I/lwdR3DpnY2eeZcQZ0i9FwP3BtmF
# JDbuEjD9vFZK8EIZXkE2D/REsKoIjzLaHGdrAJWACxn2OnFldaT84smtwQglOO0t
# ek6mKlIgFisMNm8sg5YVu3hJLk6hz8DAibw9v6GCAgswggIHBgkqhkiG9w0BCQYx
# ggH4MIIB9AIBATByMF4xCzAJBgNVBAYTAlVTMR0wGwYDVQQKExRTeW1hbnRlYyBD
# b3Jwb3JhdGlvbjEwMC4GA1UEAxMnU3ltYW50ZWMgVGltZSBTdGFtcGluZyBTZXJ2
# aWNlcyBDQSAtIEcyAhAOz/Q4yP6/NW4E2GqYGxpQMAkGBSsOAwIaBQCgXTAYBgkq
# hkiG9w0BCQMxCwYJKoZIhvcNAQcBMBwGCSqGSIb3DQEJBTEPFw0xOTEyMTgxNTU3
# NTJaMCMGCSqGSIb3DQEJBDEWBBQnO+8HdVS2FZXRSGgs4FA3Y3zgzTANBgkqhkiG
# 9w0BAQEFAASCAQAnpy7uVLP00kNFPkbA5+m7frf7iW1GvsHrav/Ag1cSD1Rgazeq
# LNCXOCbnJ61t2LcdIbWpSAisfCpKj2weUsMcBrwzaEvP0Av6DqxS0QPQBuxnEf36
# m2hwtexIYvOlYK8NhF4re/5hShOxX03LoBT9SEGLld1dmTG96J1vZDxHeYs/HHrE
# LJTVnfRxu6ZPuAFrjpPE6DYSdP5VqqawFi/QVPWBPIX7QhzQdmxs1RHQEy2c9WJg
# +0HQsUn7qIE9XC0btk2HaS9WV++CuvCAxOvQt6yhotcMQN28sfRut4qk11eKb6tv
# iBCwWshBHwkhxRPRUO6IvZeawEcS/gniAMWT
# SIG # End signature block

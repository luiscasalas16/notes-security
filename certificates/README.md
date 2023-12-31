# security / certificates

- A.1. create a self-signed root authority certificate

```powershell
#crear certificado CA
$dateFrom  = Get-Date -Date "2000-01-01 00:00:00"
$dateTo   = Get-Date -Date "2100-01-01 00:00:00"
$rootCert = New-SelfSignedCertificate -CertStoreLocation "Cert:\CurrentUser\My" -DnsName "lcs16-CA" -TextExtension @("2.5.29.19={text}CA=true") -KeyUsage CertSign,CrlSign,DigitalSignature -NotAfter $dateTo -NotBefore $dateFrom

#exportar certificado CA
$rootCertPassword = ConvertTo-SecureString -String "password" -Force -AsPlainText
$rootCertPath = Join-Path -Path "Cert:\CurrentUser\My" -ChildPath "$($rootCert.Thumbprint)"
Export-PfxCertificate -Cert $rootCertPath -FilePath "lcs16-CA.pfx" -Password $rootCertPassword
Export-Certificate -Cert $rootCertPath -FilePath "lcs16-CA.crt"

#importar certificado CA en LocalMachine -> Root (administrator)
Import-Certificate -FilePath "lcs16-CA.crt" -CertStoreLocation "Cert:\LocalMachine\Root"
```

- A.2. create a https signed by root authority certificate

```powershell
#leer certificado CA
$rootCert = Get-ChildItem -Path "Cert:\CurrentUser\My" | Where-Object {$_.Subject -eq "CN=lcs16-CA"}

#crear certificado HTTPS
$dateFrom  = Get-Date -Date "2000-01-01 00:00:00"
$dateTo   = Get-Date -Date "2100-01-01 00:00:00"
$httpsCert = New-SelfSignedCertificate -CertStoreLocation "Cert:\CurrentUser\My" -DnsName "lcs16-HTTPS" -KeyUsage DigitalSignature,KeyEncipherment -Signer $rootCert -NotAfter $dateTo -NotBefore $dateFrom

#exportar certificado HTTPS
$httpsCertPassword = ConvertTo-SecureString -String "password" -Force -AsPlainText
$httpsCertPath = Join-Path -Path "Cert:\CurrentUser\My" -ChildPath "$($httpsCert.Thumbprint)"
Export-PfxCertificate -Cert $httpsCertPath -FilePath "lcs16-HTTPS.pfx" -Password $httpsCertPassword
Export-Certificate -Cert $httpsCertPath -FilePath "lcs16-HTTPS.crt"
```

- B.1. RDCMan

```powershell
$dateFrom  = Get-Date -Date "2000-01-01 00:00:00"
$dateTo   = Get-Date -Date "2100-01-01 00:00:00"
New-SelfSignedCertificate -KeySpec KeyExchange -KeyExportPolicy Exportable -HashAlgorithm SHA1 -KeyLength 2048 -CertStoreLocation "cert:\CurrentUser\My" -Subject "CN=MyRDCManCert" -NotAfter $dateTo -NotBefore $dateFrom
```
